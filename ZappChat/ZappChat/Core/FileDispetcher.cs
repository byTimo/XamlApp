using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using NUnit.Framework;

namespace ZappChat.Core
{
    static class FileDispetcher
    {
        private static readonly string appDataDirectory;
        private static readonly string rootDirectory;
        private const string zappChatDirectoryName = "ZappChat";
        private const string settingFile = "ZappSetting";

        public static string FullPathToSettingFile { get; private set; }

        static FileDispetcher()
        {
            appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (appDataDirectory == null) throw new Exception("Не возможно определить расположение ApplicationsData");

            rootDirectory = Path.Combine(appDataDirectory, zappChatDirectoryName);
            if (!Directory.Exists(rootDirectory)) Directory.CreateDirectory(rootDirectory);

            FullPathToSettingFile = Path.Combine(rootDirectory,settingFile);
            if (!File.Exists(FullPathToSettingFile)) File.Create(FullPathToSettingFile);
        }

        public static bool SaveSettings(string field, string setting)
        {
            return SaveInformationToFile(FullPathToSettingFile, field, setting);
        }

        public static string GetSetting(string field)
        {
            return GetFieldInfoInFile(FullPathToSettingFile, field);
        }

        private static string GetFieldInfoInFile(string path, string field)
        {
            var lineInfo = File.ReadAllText(path).Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault(str => str.StartsWith(field));
            return lineInfo == null
                ? null
                : lineInfo.Split(':')[1];
        }

        public static string FindFieldInfoLineInFile(string path, string field)
        {
            return File.ReadAllText(path).Split('\r','\n').First(str => str.StartsWith(field));
        }

        private static bool SaveInformationToFile(string path, string field, string info)
        {
            try
            {
                var allFileLine = File.ReadAllText(path).Split(new []{'\r','\n'},StringSplitOptions.RemoveEmptyEntries).ToList();
                var currentLine = allFileLine.FirstOrDefault(str => str.StartsWith(field));
                if(currentLine != null) allFileLine.Remove(currentLine);
                allFileLine.Add(string.Concat(field, ":", info));
                File.Delete(path);
                File.WriteAllLines(path,allFileLine);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка при сохранении настроек!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }


    [TestFixture]
    class FileReadWriteTester
    {
        private List<string> settingInformation = new List<string>();

        [SetUp]
        public void SaveSettingWithoutFile()
        {
            Thread.Sleep(1000);
            settingInformation = File.ReadAllText(FileDispetcher.FullPathToSettingFile)
                .Split(new []{'\r','\n'}, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }

        [TearDown]
        public void LoadSettingToFile()
        {
            Console.WriteLine("Setting file conteinted:\n");
            Console.WriteLine(File.ReadAllText(FileDispetcher.FullPathToSettingFile));
            File.Delete(FileDispetcher.FullPathToSettingFile);
            File.WriteAllLines(FileDispetcher.FullPathToSettingFile, settingInformation);
        }

        [Test]
        public void SaveInfoTest()
        {
            var testField = "test";
            var testInfo = "RErretqtewwwqwe";
            FileDispetcher.SaveSettings(testField, testInfo);
            var testLineInFile = FileDispetcher.FindFieldInfoLineInFile(FileDispetcher.FullPathToSettingFile, testField);
            Console.WriteLine(testLineInFile);

            Assert.AreEqual(testLineInFile,string.Concat(testField, ":", testInfo));
        }

        [Test]
        public void GetInfoReturnNull()
        {
            var testField = "nullTest";
            var testLine = FileDispetcher.GetSetting(testField);
            Console.WriteLine(testLine);
            Assert.AreEqual(testLine, null);
        }

        [Test]
        public void GetInfoReturnInfo()
        {
            var testField = "InfoTest";
            var testInfo = "This informaciton will be retuned!";
            FileDispetcher.SaveSettings(testField, testInfo);
            var gettedLine = FileDispetcher.GetSetting(testField);
            Console.WriteLine(gettedLine);
            Assert.AreEqual(gettedLine, testInfo);
        }

        [Test]
        public void GetInfoReturnInfoWithThreeLine()
        {
            var testField = "SecondLine";
            var testInfo = "Second line info.";
            FileDispetcher.SaveSettings("FirstLine", "First line info.");
            FileDispetcher.SaveSettings(testField, testInfo);
            FileDispetcher.SaveSettings("ThirdLine", "Third line info.");

            var gettdLine = FileDispetcher.GetSetting(testField);
            Assert.AreEqual(gettdLine, testInfo);
        }
    }
}
