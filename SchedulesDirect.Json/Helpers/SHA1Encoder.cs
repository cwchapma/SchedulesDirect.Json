using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SchedulesDirect.Json.Helpers
{
    static class eSHA1Encoder
    {
        static SHA1 sha1 = SHA1.Create();

        public static string Encode(string s)
        {
            var hash = sha1.ComputeHash(Encoding.Default.GetBytes(s));
            return ConvertToHexString(hash);
        }

        private static string ConvertToHexString(byte[] byteArray)
        {
            StringBuilder hex = new StringBuilder(byteArray.Length * 2);
            foreach (byte b in byteArray)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
