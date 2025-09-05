using System.Configuration;

namespace TweakAppClient
{
    public static class Configuration
    {
        // Default API base URL - can be overridden via app.config
        private static readonly string DefaultApiBaseUrl = "http://localhost:5062";
        
        public static string ApiBaseUrl
        {
            get
            {
                try
                {
                    // Try to read from app.config first
                    var configValue = ConfigurationManager.AppSettings["ApiBaseUrl"];
                    return !string.IsNullOrEmpty(configValue) ? configValue : DefaultApiBaseUrl;
                }
                catch
                {
                    // Fallback to default if configuration fails
                    return DefaultApiBaseUrl;
                }
            }
        }
        
        public static string WebBaseUrl
        {
            get
            {
                try
                {
                    var configValue = ConfigurationManager.AppSettings["WebBaseUrl"];
                    return !string.IsNullOrEmpty(configValue) ? configValue : "https://localhost:7223";
                }
                catch
                {
                    return "https://localhost:7223";
                }
            }
        }
    }
}