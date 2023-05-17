using System.Configuration;
namespace Bogus_CV_Gen
{
    public static class ConfigSettings
    {
        public static string DocsFolderPath { get; } = ConfigurationManager.AppSettings["outputDirectory"];
        public static string TempFolderPath { get; } = ConfigurationManager.AppSettings["templateDirectory"];
        public static string ApiKey { get; } = ConfigurationManager.AppSettings["apikey"];
        public static string Testmode { get; } = ConfigurationManager.AppSettings["testmode"];
        public static string GPTModel { get; } = ConfigurationManager.AppSettings["gptmodel"];

    }
}
