using System;
using System.IO;

namespace ZappChatLauncher
{
    internal static class FileDispetcher
    {
        private static string appDataDirectory;
        private static string rootDirectory;
        private const string ZappChatDirectoryName = "ZappChat";
        private const string UpdateDirectoryName = "Update";
        public static string FullPathToUpdateFolder { get; private set; }

        public static void InitializeFileDispetcher()
        {
            appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (appDataDirectory == null) throw new Exception("Не возможно определить расположение ApplicationsData");

            rootDirectory = Path.Combine(appDataDirectory, ZappChatDirectoryName);
            FullPathToUpdateFolder = Path.Combine(rootDirectory, UpdateDirectoryName);
            CheckExistsFiles();
        }

        public static void CheckExistsFiles()
        {
            if (!Directory.Exists(rootDirectory)) Directory.CreateDirectory(rootDirectory);
            if (!Directory.Exists(FullPathToUpdateFolder)) Directory.CreateDirectory(FullPathToUpdateFolder);
        }
    }
}
