using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using NUnit.Framework;
using ZappChat.Properties;

namespace ZappChat.Core
{
    static class FileDispetcher
    {
        private static string appDataDirectory;
        private static string rootDirectory;
        private const string ZappChatDirectoryName = "ZappChat";
        private const string SettingFile = "ZappSetting";

        public static string FullPathToSettingFile { get; private set; }

        public static void InitializeFileDispetcher()
        {
            appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (appDataDirectory == null) throw new Exception("Не возможно определить расположение ApplicationsData");

            rootDirectory = Path.Combine(appDataDirectory, ZappChatDirectoryName);
            FullPathToSettingFile = Path.Combine(rootDirectory,SettingFile);

            CheckExistsFiles();

        }

        public static void CheckExistsFiles()
        {
            if (!Directory.Exists(rootDirectory)) Directory.CreateDirectory(rootDirectory);
            if (!File.Exists(FullPathToSettingFile)) File.Create(FullPathToSettingFile);
        }
        public static string GetToken()
        {
            return GetSetting("token");
        }
        public static string FindFieldInfoLineInFile(string path, string field)
        {
            if (!File.Exists(path)) File.Create(path);
            return GetAllLineInFile(path).FirstOrDefault(str => str.StartsWith(field));
        }

        public static bool SaveSettings(string field, string setting)
        {
            return SaveInformationToFile(FullPathToSettingFile, field, setting);
        }

        public static string GetSetting(string field)
        {
            return GetFieldInfoInFile(FullPathToSettingFile, field);
        }

        public static void DeleteSetting(string field)
        {

            try
            {
                var allSettings = GetAllLineInFile(FullPathToSettingFile);
                var currentLine = FindFieldInfoLineInFile(FullPathToSettingFile, field);
                if (currentLine != null && allSettings.Contains(currentLine))
                {
                    allSettings.Remove(currentLine);
                    RewriteFile(FullPathToSettingFile,allSettings);
                }
                else
                    throw new Exception();
            }
            catch
            {
                // ignored
            }
        }
        private static string GetFieldInfoInFile(string path, string field)
        {
            var lineInfo = GetAllLineInFile(path)
                .FirstOrDefault(str => str.StartsWith(field));
            return lineInfo == null
                ? null
                : lineInfo.Split(':')[1];
        }

        private static bool SaveInformationToFile(string path, string field, string info)
        {
            try
            {
                var allLineFile = GetAllLineInFile(FullPathToSettingFile);
                var currentLine = allLineFile.FirstOrDefault(str => str.StartsWith(field));
                if(currentLine != null) allLineFile.Remove(currentLine);
                allLineFile.Add(string.Concat(field, ":", info));
                RewriteFile(path, allLineFile);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка при сохранении настроек!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private static List<string> GetAllLineInFile(string path)
        {
            if (!File.Exists(path)) File.Create(path);
            Thread.Sleep(1000);
            return File.ReadAllText(path).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private static void RewriteFile(string path, IEnumerable<string> allLine)
        {
            if(File.Exists(path)) File.Delete(path);
            File.WriteAllLines(path, allLine);

        }
    }


    [TestFixture]
    class FileReadWriteTester
    {
        private List<string> _settingInformation = new List<string>();

        [SetUp]
        public void SaveSettingWithoutFile()
        {
            FileDispetcher.InitializeFileDispetcher();
            _settingInformation = File.ReadAllText(FileDispetcher.FullPathToSettingFile)
                .Split(new []{'\r','\n'}, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }

        [TearDown]
        public void LoadSettingToFile()
        {
            Console.WriteLine(Resources.FileReadWriteTester_SaveSettingWithoutFile_Setting_file_conteinted_);
            Console.WriteLine(File.ReadAllText(FileDispetcher.FullPathToSettingFile));
            File.Delete(FileDispetcher.FullPathToSettingFile);
            File.WriteAllLines(FileDispetcher.FullPathToSettingFile, _settingInformation);
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

        [Test]
        public void DeleteSettingTest()
        {
            FileDispetcher.SaveSettings("test", "this must be deleted");
            FileDispetcher.DeleteSetting("test");
            var testLine = FileDispetcher.FindFieldInfoLineInFile(FileDispetcher.FullPathToSettingFile, "test");
            Assert.AreEqual(testLine, null);
        }
    }
}
