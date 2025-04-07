using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BlazorAuthAPI.Models;

namespace BlazorAuthAPI.Services
{
    public class CryptographyService
    {
        private readonly byte[] key = Encoding.UTF8.GetBytes("0123456789ABCDEF0123456789ABCDEF");

        private readonly byte[] iv = Encoding.UTF8.GetBytes("ABCDEF0123456789");

        public string Encrypt(UserState userState)
        {
            ArgumentNullException.ThrowIfNull(userState);

            string json = JsonSerializer.Serialize(userState);
            byte[] encryptedBytes;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using MemoryStream ms = new();
                using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write);
                using StreamWriter sw = new(cs);
                sw.Write(json);
                sw.Flush();
                cs.FlushFinalBlock();
                encryptedBytes = ms.ToArray();
            }

            return Base64UrlEncode(encryptedBytes);
        }

        private UserState Decrypt(string encryptedString)
        {
            if (string.IsNullOrEmpty(encryptedString))
                throw new ArgumentNullException(nameof(encryptedString));

            byte[] cipherBytes = Base64UrlDecode(encryptedString);
            string json;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using MemoryStream ms = new(cipherBytes);
                using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
                using StreamReader sr = new(cs);
                json = sr.ReadToEnd();
            }

            return JsonSerializer.Deserialize<UserState>(json)!;
        }

        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        private static byte[] Base64UrlDecode(string input)
        {
            string base64 = input
                .Replace("-", "+")
                .Replace("_", "/");

            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
                case 1: throw new FormatException("Base64 string inválida.");
            }

            return Convert.FromBase64String(base64);
        }

        public bool TryDecrypt(string encryptedString, out UserState? userState)
        {
            userState = null;

            try
            {
                userState = Decrypt(encryptedString);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
