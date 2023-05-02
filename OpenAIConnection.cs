using OpenAI_API;
using System.Configuration;
using OpenAI_API.Chat;
using OpenAI_API.Models;
public class OpenAIConnection
{
    private static string inputDirectoryPath = ConfigurationManager.AppSettings["templateDirectory"];
    private static string testmode = ConfigurationManager.AppSettings["testmode"];
    private static string gptmodel = ConfigurationManager.AppSettings["gptmodel"];
    public static async Task OpenAI (OpenAIAPI api, string userString, int templateNumber)
	{
        var promptText = File.ReadAllText(Path.Combine(inputDirectoryPath, $"Template {templateNumber}\\prompt.txt"));
        var xmlPromptText = File.ReadAllText(Path.Combine(inputDirectoryPath, $"Template {templateNumber}\\template.xml"));
        if (testmode == "false")
        {
            try
            {
                //Console.WriteLine("Waiting for response from OpenAI API...");

                if (gptmodel == "3")
                {
                    //GPT 3 VERSION!
                    var completions = await api.Completions.CreateCompletionAsync(
                       prompt: promptText + userString + xmlPromptText,
                       model: "text-davinci-003",
                       max_tokens: 1800,
                       temperature: 0.8f
                   ).ConfigureAwait(false);

                    // Get the XML data from the completion
                    var xmlData = completions.Completions[0].Text;

                    // Generate a new Word document based on the XML data
                    Console.WriteLine("Generating CV...");
                    Program.CreateCV.CreateWordDocument(xmlData);
                } else if (gptmodel == "3.5") { 
               
                var completions = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
                {
                    Model = Model.ChatGPTTurbo,
                    Temperature = 0.9,
                    MaxTokens = 1200,
                    Messages = new ChatMessage[]
                          {
               new ChatMessage(ChatMessageRole.User, promptText + userString + xmlPromptText)
                          }
                }).ConfigureAwait(false);

                Console.WriteLine("Received response from OpenAI API!");

                var xmlData = completions;

                Console.WriteLine("Generating CV...");
                Program.CreateCV.CreateWordDocument(xmlData.ToString());
                
                //Console.WriteLine("Completed processing completions.");
                Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
        else {
            Console.WriteLine("Generating CV In test mode...");
            Program.CreateCV.CreateWordDocument("<CV><Contact><FirstName>Shanel</FirstName><LastName>Altenwerth</LastName><Title>Dynamic Quality Technician</Title><Address>804 Kozey Port, North Myrtis, Belize</Address><City>North Myrtis</City><State>Belize</State><Zip>0000</Zip><Phone>461.975.1421 x76336</Phone><Email>Joshuah_Kassulke80@gmail.com</Email><LinkedIn></LinkedIn></Contact><Profile>\r\n    Experienced Dynamic Quality Technician with a background in marketing and extensive experience in analyzing and problem-solving. Proven track record of developing innovative solutions to improve operational efficiency and customer service. Skilled in data analysis, process optimization, and team management. Highly organized and detail-oriented.\r\n  </Profile><KeySkills><Skill>Marketing</Skill><Skill>Analyzing</Skill><Skill>Problem-Solving</Skill><Skill>Data Analysis</Skill><Skill>Process Optimization</Skill></KeySkills><ActivitiesAndInterests><Interest>Gardening</Interest><Interest>Camping</Interest><Interest>Hiking</Interest><Interest>Photography</Interest><Interest>Reading</Interest><Interest>Cooking</Interest></ActivitiesAndInterests><employment><position>Dynamic Quality Technician</position><employer>Ortiz LLC</employer><city>North Myrtis</city><state>Belize</state><startdate>January 2019</startdate><enddate>Present</enddate><description>\r\n      Responsible for analyzing data to identify trends and areas for process improvement, developing innovative solutions to improve operational efficiency and customer service, and managing teams of technicians to ensure quality standards are met.\r\n    </description></employment><employment><position>Marketing Analyst</position><employer>Hodkiewicz Group</employer><city>North Myrtis</city><state>Belize</state><startdate>January 2015</startdate><enddate>December 2018</enddate><description>\r\n      Responsible for managing and optimizing marketing campaigns, analyzing data to identify trends and areas for improvement, and providing reports and recommendations to senior management.\r\n    </description></employment><employment><position>Customer Service Representative</position><employer>Heller, Kassulke and Dietrich</employer><city>North Myrtis</city><state>Belize</state><startdate>May 2013</startdate><enddate>December 2014</enddate><description>\r\n      Responsible for responding to customer inquiries and complaints, resolving customer issues in a timely and efficient manner, and providing excellent customer service.\r\n    </description></employment><education><degree>Bachelor of Arts</degree><institution>University of Belize</institution><city>North Myrtis</city><state>Belize</state><grad_date>May 2013</grad_date><additional_info>Major: Communications</additional_info><additional_info>Minor: Psychology</additional_info></education><education><degree>Associate's Degree</degree><institution>Belize Technical College</institution><city>North Myrtis</city><state>Belize</state><grad_date>June 2010</grad_date></education></CV>");

        }

    }
}