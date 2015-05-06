using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ZappChat.Core
{
    static class Support
    {
        public static string DateTimeToString(DateTime date)
        {
            var dateString = date.ToString("u");
            return dateString.Remove(dateString.Length - 1);
        }
        public static string Sha1WithString(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            var sha1 = SHA1.Create();
            var hashBytes = sha1.ComputeHash(bytes);

            return HexStringFromBytes(hashBytes);
        }
        private static string HexStringFromBytes(IEnumerable<byte> bytes)
        {
            var sb = new StringBuilder();
            foreach (var hex in bytes.Select(b => b.ToString("x2")))
            {
                sb.Append(hex);
            }
            return sb.ToString();
        }

        public static string Base64FromString(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            var convertLine = Convert.ToBase64String(bytes);
            return convertLine;
        }
        public static string StringFromBase64(string s)
        {
            var bytes = Convert.FromBase64String(s);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string XorEncoder(string clearText)
        {
            var sait = "EFjQJKLmpuSQ6Bld";
            var keyword = "zappchat_app";
            var gamma = "";
            while (gamma.Length < clearText.Length)
            {
                var concatString = new StringBuilder(keyword);
                concatString.Append(gamma);
                concatString.Append(sait);
                var growingGamma = new StringBuilder(gamma);
                growingGamma.Append(Sha1WithString(concatString.ToString()).Substring(0, 2));
                gamma = growingGamma.ToString();
            }
            return EncryptOrDecrypt(clearText, gamma);

        }
        private static string EncryptOrDecrypt(string text, string key)
        {
            var result = new StringBuilder();

            for (int c = 0; c < text.Length; c++)
                result.Append((char)(text[c] ^ (uint)key[c % key.Length]));

            return result.ToString();
        }
    }

    [TestFixture]
    class SomethingTester
    {
        [Test]
        public static void TestDateTimeToString()
        {
            var date = new DateTime(2015, 2, 1, 0, 0, 0);
            Assert.AreEqual("2015-02-01 00:00:00",
                Support.DateTimeToString(date));
        }

        [Test]
        public static void TestConvertors()
        {
            Assert.AreEqual(Support.StringFromBase64(Support.Base64FromString("Hello")), "Hello");
        }

        [Test]
        public static void TestEncription()
        {
            Assert.AreEqual(Support.XorEncoder(Support.XorEncoder("Hello")),"Hello");
        }

        [Test]
        public static void TestSpecialEncription()
        {
            Assert.AreEqual(Support.Base64FromString(Support.XorEncoder("VyyJ5aKunzgsNcViedzZ")), "b0gYKFRQeUBfGVNBd1VvC1VUSm0=");
        }
    }
}
