using OpenAI_API;
using System.Text;
using System.Configuration;
using Bogus_CV_Gen;

class MainPage
{

    private static string docsFolderPath = ConfigSettings.DocsFolderPath;
    private static string tempFolderPath = ConfigSettings.TempFolderPath;
    private static string apiKey = ConfigSettings.ApiKey;


    static async Task Main()
    {
        CreateFoldersIfNotExist(docsFolderPath);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("______  _____ _____ _   _ _____   _____ _   _ \r\n| ___ \\|  _  |  __ \\ | | /  ___| /  __ \\ | | |\r\n| |_/ /| | | | |  \\/ | | \\ `--.  | /  \\/ | | |\r\n| ___ \\| | | | | __| | | |`--. \\ | |   | | | |\r\n| |_/ /\\ \\_/ / |_\\ \\ |_| /\\__/ / | \\__/\\ \\_/ /\r\n\\____/  \\___/ \\____/\\___/\\____/   \\____/\\___/ \r\n                                            ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Connecting to OpenAI API...");
        var api = await ConnectToOpenApiAsync(apiKey);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Connected to OpenAI API!");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Welcome to the Bogus CV Generator!");
        string customUser = MainPage.PromptUserForCustomInformation();
        int numCvs = PromptUserForNumCvs();
        int tempNum = PromptUserForTemplate();
     

        Console.WriteLine($"Generating {numCvs} CVs...");
        await GenerateCvsAsync(numCvs, api, tempNum, customUser);

        Console.WriteLine("All tasks completed.");
    }

    private static void CreateFoldersIfNotExist(string docsFolderPath)
    {

        if (!Directory.Exists(docsFolderPath))
            Directory.CreateDirectory(docsFolderPath);
    }

    private static async Task<OpenAIAPI> ConnectToOpenApiAsync(string apiKey)
    {
        try
        {
            var api = new OpenAIAPI(apiKey);
            return api;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to connect to OpenAI API: {ex.Message}");
            throw;
        }
    }

    private static int PromptUserForNumCvs()
    {
        int numCvs;
        do
        {
            Console.WriteLine("How many CVs do you want to generate (Maximum is 20)?");
            string input = Console.ReadLine();
            if (!int.TryParse(input, out numCvs) || numCvs < 1 || numCvs > 20)
            {
                Console.WriteLine("Please enter a valid number between 1 and 20.");
            }
        } while (numCvs < 1 || numCvs > 20);

        return numCvs;
    }

    private static int PromptUserForTemplate()
    {
        // Replace "parentFolder" with the actual name of the parent folder
        int totalChildren = Directory.GetDirectories(tempFolderPath).Length;

        int numTemplate;
        do
        {
            Console.WriteLine("What template would you like to select? (Enter a number between 1 and {0}).Recommended template is 1", totalChildren);
            string input = Console.ReadLine();
            if (!int.TryParse(input, out numTemplate) || numTemplate < 1 || numTemplate > totalChildren)
            {
                Console.WriteLine("Please enter a valid number between 1 and {0}.", totalChildren);
            }
        } while (numTemplate < 1 || numTemplate > totalChildren);

        return numTemplate;
    }

    public static string PromptUserForCustomInformation()
    {
        Console.WriteLine("Would you like to provide custom information? Y/N");
        string input = Console.ReadLine();
        if (input.ToUpper() == "Y")
        {
            Console.WriteLine("Use an * to give a command. eg: *Use a uk phonenumber");
            List<string> fields = new List<string> { "FullName", "Email", "Phone","Full Address","Company(companies)", "Job Title(s)", "Job Area(s)"}; 
            StringBuilder result = new StringBuilder();
            foreach (string field in fields)
            {
                Console.WriteLine("Please enter {0}:", field);
                string fieldValue = Console.ReadLine();
                result.AppendFormat("{0}: {1}\n", field, fieldValue);
            }
            return result.ToString();
        }
        else if (input.ToUpper() == "N")
        {
            return null;
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter Y or N.");
            return PromptUserForCustomInformation();
        }

    }

    private static async Task GenerateCvsAsync(int numCvs, OpenAIAPI api, int tempNum, string customUser)
    {
        var createCVTasks = new List<Task>();
        for (int i = 0; i < numCvs; i++)
        {
            createCVTasks.Add(Task.Run(() => Program.CreateCV.ProcessCVAsync(api, tempNum, customUser)));
        }
        await Task.WhenAll(createCVTasks);
    }
}

