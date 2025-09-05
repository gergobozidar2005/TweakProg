using System;
using System.IO;

namespace TweakAppClient
{
    public static class ActivationService
    {
        // We create a unique path in the user's local app data folder
        private static readonly string AppFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TweakApp");
        private static readonly string KeyFilePath = Path.Combine(AppFolderPath, "activation.key");

        public static void SaveActivationKey(string key)
        {
            // Ensure the directory exists
            Directory.CreateDirectory(AppFolderPath);
            // Save the key to the file
            File.WriteAllText(KeyFilePath, key);
        }

        public static bool IsActivated()
        {
            // Check if the key file exists
            return File.Exists(KeyFilePath);
        }
    }
}