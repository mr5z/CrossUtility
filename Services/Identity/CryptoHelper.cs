using System;
using System.Security.Cryptography;
using System.Text;

namespace CrossUtility.Services.Identity
{
    static class CryptoHelper
    {
        private static readonly Random random = new();
        private const string DefaultAllowedCharacters = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";

        public static string ToSha256(string input)
        {
            var hash = Sha256(input, Encoding.UTF8);
            return Base64Encode(hash);
        }

        public static string ToSha256(string input, Encoding encoding)
        {
            var hash = Sha256(input, encoding);
            return Base64Encode(hash);
        }

        public static byte[] Sha256(string input, Encoding encoding)
        {
            using var sha = SHA256.Create();
            var bytes = encoding.GetBytes(input);
            return sha.ComputeHash(bytes);
        }

        // Copied from https://github.com/IdentityModel/IdentityModel/blob/main/src/Base64Url.cs
        public static string Base64Encode(byte[] arg)
        {
            var s = Convert.ToBase64String(arg); // Standard base64 encoder

            s = s.Split('=')[0]; // Remove any trailing '='s
            s = s.Replace('+', '-'); // 62nd char of encoding
            s = s.Replace('/', '_'); // 63rd char of encoding

            return s;
        }

        // Copied from https://github.com/IdentityModel/IdentityModel/blob/main/src/Base64Url.cs
        public static byte[] Base64Decode(string arg)
        {
            var s = arg;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding

            switch (s.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: s += "=="; break; // Two pad chars
                case 3: s += "="; break; // One pad char
                default: throw new Exception("Illegal base64url string!");
            }

            return Convert.FromBase64String(s); // Standard base64 decoder
        }

        public static string RandomString(int min, int max, string allowedCharacters = DefaultAllowedCharacters)
        {
            var chars = new char[max];
            var length = random.Next(min, max + 1);

            for (int i = 0; i < length; ++i)
            {
                chars[i] = allowedCharacters[random.Next(allowedCharacters.Length)];
            }

            return new string(chars, 0, length);
        }
    }
}
