using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using NUnit.Framework;
using ZappChat.Properties;

namespace ZappChat.Core
{
    internal static class FileDispetcher
    {
        private static string appDataDirectory;
        private static string rootDirectory;
        private const string ZappChatDirectoryName = "ZappChat";
        private const string SettingFile = "ZappSetting";
        private const string DialoguesInformationFile = "DialogueInformation";

        public static string FullPathToSettingFile { get; private set; }
        public static string FullPasthToDialogueInformation { get; private set; }

        public static void InitializeFileDispetcher()
        {
            appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (appDataDirectory == null) throw new Exception("Не возможно определить расположение ApplicationsData");

            rootDirectory = Path.Combine(appDataDirectory, ZappChatDirectoryName);
            FullPathToSettingFile = Path.Combine(rootDirectory, SettingFile);
            FullPasthToDialogueInformation = Path.Combine(rootDirectory, DialoguesInformationFile);
            CheckExistsFiles();
        }

        public static void CheckExistsFiles()
        {
            if (!Directory.Exists(rootDirectory)) Directory.CreateDirectory(rootDirectory);
//            if (!File.Exists(FullPathToSettingFile)) File.Create(FullPathToSettingFile);
//            if (!File.Exists(FullPasthToDialogueInformation)) File.Create(FullPasthToDialogueInformation);
            if (!File.Exists(FullPathToSettingFile)) File.WriteAllText(FullPathToSettingFile,"");
            if (!File.Exists(FullPasthToDialogueInformation)) File.WriteAllText(FullPasthToDialogueInformation, "");
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
                    RewriteFile(FullPathToSettingFile, allSettings);
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
                if (currentLine != null) allLineFile.Remove(currentLine);
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
            return File.ReadAllText(path).Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private static void RewriteFile(string path, IEnumerable<string> allLine)
        {
            if (File.Exists(path)) File.Delete(path);
            File.WriteAllLines(path, allLine);

        }

        public static void WriteAllCollection(string path, Dictionary<ulong, string> statuses)
        {
            var fileDataBuilder = new StringBuilder();
            foreach (var statuse in statuses)
            {
                var line = string.Concat(statuse.Key, ":", statuse.Value, ";");
                fileDataBuilder.Append(line);
            }
            File.WriteAllText(path, fileDataBuilder.ToString());
        }

        public static Dictionary<ulong, string> ReadAllCollection(string path)
        {
            return File.ReadAllText(path)
                .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                .ToDictionary(x => ulong.Parse(x.Split(':')[0]), x => x.Split(':')[1]);
        }

        public static void RewriteFileCollection(string path, Dictionary<ulong, string> statuses)
        {
            if (File.Exists(path)) File.Delete(path);
            WriteAllCollection(path, statuses);
        }

        public static void SynchronizeDialogueStatuses(Dictionary<ulong, string> statuses)
        {
            var dictionaryInFile = ReadAllCollection(FullPasthToDialogueInformation);
            var currentDifferents = statuses.Except(dictionaryInFile);
            foreach (var status in currentDifferents)
            {
                if (dictionaryInFile.ContainsKey(status.Key))
                    dictionaryInFile[status.Key] = status.Value;
                else
                    dictionaryInFile.Add(status.Key, status.Value);

            }
            var fileDifferents = dictionaryInFile.Except(statuses);
            for (var i = 0; i < fileDifferents.Count(); i++)
                dictionaryInFile.Remove(fileDifferents.ToList()[i].Key);
            RewriteFileCollection(FullPasthToDialogueInformation, dictionaryInFile);
        }
    }


    [TestFixture]
    internal class FileReadWriteTester
    {
        private List<string> _settingInformation = new List<string>();

        [SetUp]
        public void SaveSettingWithoutFile()
        {
            FileDispetcher.InitializeFileDispetcher();
            _settingInformation = File.ReadAllText(FileDispetcher.FullPathToSettingFile)
                .Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)
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

            Assert.AreEqual(testLineInFile, string.Concat(testField, ":", testInfo));
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

    [TestFixture]
    internal class TestCollectionClass
    {
        private Dictionary<ulong, string> _collectionInformation;

        [SetUp]
        public void SaveSetting()
        {
            FileDispetcher.InitializeFileDispetcher();
            _collectionInformation = FileDispetcher.ReadAllCollection(FileDispetcher.FullPasthToDialogueInformation);
//            if(File.Exists(FileDispetcher.FullPasthToDialogueInformation)) File.Delete(FileDispetcher.FullPasthToDialogueInformation);
//            File.Create(FileDispetcher.FullPasthToDialogueInformation);
        }

        [TearDown]
        public void LoadSetting()
        {
            FileDispetcher.RewriteFileCollection(FileDispetcher.FullPasthToDialogueInformation,_collectionInformation);
        }


        [Test]
        public void WriteDialogueStatuses()
        {
            var dic = new Dictionary<ulong, string>
            {
                {1, "d"},
                {2, "d"},
                {3, "2"},
                {4, "125"}
            };
            var testSuccess = "1:d;2:d;3:2;4:125;";

            FileDispetcher.WriteAllCollection(FileDispetcher.FullPasthToDialogueInformation, dic);

            Assert.AreEqual(File.ReadAllText(FileDispetcher.FullPasthToDialogueInformation), testSuccess);
        }

        [Test]
        public void ReadDialogueStatuses()
        {
            var dicSuccess = new Dictionary<ulong, string>
            {
                {1, "d"},
                {2, "d"},
                {3, "2"},
                {4, "125"}
            };
            FileDispetcher.WriteAllCollection(FileDispetcher.FullPasthToDialogueInformation, dicSuccess);

            var dic = FileDispetcher.ReadAllCollection(FileDispetcher.FullPasthToDialogueInformation);

            Assert.AreEqual(dic[1], dicSuccess[1]);
        }

        [Test]
        public void SyncAddTest()
        {
            var startDic = new Dictionary<ulong, string>
            {
                {1, "d"},
                {2, "d"},
                {3, "2"},
                {4, "125"}
            };
            FileDispetcher.WriteAllCollection(FileDispetcher.FullPasthToDialogueInformation,startDic);
            var endDic = new Dictionary<ulong, string>
            {
                {1, "d"},
                {2, "d"},
                {3, "2"},
                {4, "125"},
                {5, "1"}
            };

            FileDispetcher.SynchronizeDialogueStatuses(endDic);

            Assert.AreEqual(FileDispetcher.ReadAllCollection(FileDispetcher.FullPasthToDialogueInformation)[5], endDic[5]);

        }
        [Test]
        public void SyncChangeTest()
        {
            var startDic = new Dictionary<ulong, string>
            {
                {1, "d"},
                {2, "d"},
                {3, "2"},
                {4, "125"}
            };
            FileDispetcher.WriteAllCollection(FileDispetcher.FullPasthToDialogueInformation, startDic);
            var endDic = new Dictionary<ulong, string>
            {
                {1, "d"},
                {2, "d"},
                {3, "d"},
                {4, "125"}
            };

            FileDispetcher.SynchronizeDialogueStatuses(endDic);

            Assert.AreEqual(FileDispetcher.ReadAllCollection(FileDispetcher.FullPasthToDialogueInformation)[3], endDic[3]);
        }
        [Test]
        public void SyncDeleteTest()
        {
            var startDic = new Dictionary<ulong, string>
            {
                {1, "d"},
                {2, "d"},
                {3, "2"},
                {4, "125"}
            };
            FileDispetcher.WriteAllCollection(FileDispetcher.FullPasthToDialogueInformation, startDic);
            var endDic = new Dictionary<ulong, string>
            {
                {1, "d"},
                {2, "d"},
                {4, "125"}
            };

            FileDispetcher.SynchronizeDialogueStatuses(endDic);

            Assert.AreEqual(FileDispetcher.ReadAllCollection(FileDispetcher.FullPasthToDialogueInformation).Count, endDic.Count);
        }
    }
}
