using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Task3
{
    class KeyAndHMACGenerator
    {
        public static string KeyGenerator()
        {
            var random = RandomNumberGenerator.Create();
            var bytes = new byte[16]; // 16 bytes
            random.GetNonZeroBytes(bytes);
            string result = Convert.ToString(BitConverter.ToInt32(bytes));
            return result;
        }
        public static string HMACGenerator(string sKey, string sMessage)
        {
            var key = StringEncode(sKey);
            var message = StringEncode(sMessage);
            var hash = new HMACSHA256(key);
            var result = HashEncode(hash.ComputeHash(message));
            return result;
        }
        private static byte[] StringEncode(string text)
        {
            var encoding = new ASCIIEncoding();
            return encoding.GetBytes(text);
        }
        private static string HashEncode(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
