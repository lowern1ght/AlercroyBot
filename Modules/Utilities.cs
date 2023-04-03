using System.Configuration;

namespace AlercroyBot.Modules;

public static class Utilities {
    public static class ConfigAlercroyBot {
        public static async Task GetTelegramToken() {
            if (!ConfigFileIsExists()) {
                var token = ConfigurationManager.AppSettings["Token"];
            }
        }

        private static Boolean ConfigFileIsExists() {
            return false;
        }
    }
}
