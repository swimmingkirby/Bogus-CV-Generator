using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenAI_API;
using System.Configuration;
using System.Xml;
using System.Xml.Linq;

namespace Program
{
    class CreateCV
    {
        private static int templateNumber;
        private static string templateDocumentPath;
        private static string outputDirectoryPath = ConfigurationManager.AppSettings["outputDirectory"];
        private static string inputDirectoryPath = ConfigurationManager.AppSettings["templateDirectory"];
        private static readonly object _xmlLock = new object();




        // Method that processes the CV asynchronously using OpenAI API
        public static async Task ProcessCVAsync(OpenAIAPI apikey, int templateNumber,string customUser)
        {
            string userString;
            // Generate a user string to use in the API prompt
            var generateUser = new GenerateUser();
            
            if (customUser == null) {
                userString = generateUser.User();
                
            } else
            {
                userString = customUser;
            }

            // Set template number and file paths
            CreateCV.templateNumber = templateNumber;
            CreateCV.templateDocumentPath = Path.Combine(inputDirectoryPath, $"Template {templateNumber}\\template.docx");

            //Call open Ai to return xml
            await OpenAIConnection.OpenAI(apikey, userString, templateNumber);
        }








        // Method that generates a Word document based on XML data
        public static void CreateWordDocument(string xmlData)
        {
          // Console.WriteLine("Setting up paths...");

            // Set up output file name and path
            int index = 1;
            string outputFileName;
            string outputDocumentPath;
            lock (_xmlLock)
            {

                do
                {
                    outputFileName = $"GeneratedCV_{index}.docx";
                    outputDocumentPath = Path.Combine(Path.GetDirectoryName(outputDirectoryPath), outputFileName);

                    index++;
                } while (File.Exists(outputDocumentPath));

                // Copy template document to output path and replace custom XML parts with new XML data

                using var sourceDoc = WordprocessingDocument.Open(templateDocumentPath, false);
                File.Copy(templateDocumentPath, outputDocumentPath);
                using var newDoc = WordprocessingDocument.Open(outputDocumentPath, true);
                newDoc.MainDocumentPart.DocumentSettingsPart.Settings = (DocumentFormat.OpenXml.Wordprocessing.Settings)sourceDoc.MainDocumentPart.DocumentSettingsPart.Settings.Clone();
                newDoc.MainDocumentPart.StyleDefinitionsPart.Styles = (DocumentFormat.OpenXml.Wordprocessing.Styles)sourceDoc.MainDocumentPart.StyleDefinitionsPart.Styles.Clone();
                newDoc.MainDocumentPart.StyleDefinitionsPart.Styles.Save();

                var mainPart = newDoc.MainDocumentPart;
                mainPart.DeleteParts<CustomXmlPart>(mainPart.CustomXmlParts);

                // Write the XML data to a file
                string xmlFilePath = Path.Combine(Path.GetDirectoryName(outputDocumentPath), "CustomXml.xml");
                File.WriteAllText(xmlFilePath, xmlData);

                // load the XML file into an XDocument
                XDocument xmldoc = XDocument.Load(xmlFilePath);

                // remove leading whitespace before the first tag
                XNode firstNode = xmldoc.FirstNode;
                if (firstNode is XText && string.IsNullOrWhiteSpace(firstNode.ToString()))
                {
                    firstNode.Remove();
                }

                // save the modified XML back to the original file without the XML declaration
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                using (XmlWriter writer = XmlWriter.Create(xmlFilePath, settings))
                {
                    xmldoc.Save(writer);
                }

                var customXmlPart = mainPart.AddCustomXmlPart(CustomXmlPartType.CustomXml, "rId1");
                using (var stream = new FileStream(xmlFilePath, FileMode.Open))
                {
                    customXmlPart.FeedData(stream);

                }

                newDoc.Save();
                newDoc.Close();
                removeContentControls(outputDocumentPath);
            }


        }








        public static void removeContentControls(string documentPath)
        {

            using var doc = WordprocessingDocument.Open(documentPath, true);
            var mainPart = doc.MainDocumentPart;

            // Get the XML string from the main part and trim the white spaces
            string xmlString = mainPart.Document.InnerXml;

            // Load the trimmed XML string into an XmlDocument
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlString);

            var namespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
            namespaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");




            foreach (var control in mainPart.RootElement.Descendants<SdtElement>().ToList())
            {
                var placeholdertext = control.Descendants<Text>().FirstOrDefault()?.Text;
                var text = "";
                //Console.WriteLine($"Content control before processing: {control.OuterXml}");

                var dataBinding = control.Descendants<DataBinding>().FirstOrDefault();
                if (dataBinding != null)
                {
                    var xpath = dataBinding.XPath.Value;
                  //  Console.WriteLine($"Data binding XPath: {xpath}");
                    var node = xmlDocument.SelectSingleNode(xpath, namespaceManager);
                    var value = node?.InnerText;
                    text = node?.InnerText;
                  // Console.WriteLine($"Found content control with text: {value}");
                   // Console.WriteLine($"Value found for XPath '{xpath}': {value}");
                }
                else
                {
                   // Console.WriteLine("Content control has no data binding.");
                }
                foreach (var t in control.Descendants<Text>())
                {
                    t.Text = text;
                }
            }

           // Console.WriteLine($"Document after removing content controls: {mainPart.Document.OuterXml}");

            doc.Save();
            doc.Close();

            Console.WriteLine("CV generated!");
        }

        
    }
}