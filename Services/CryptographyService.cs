using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BlazorAuthAPI.Models;

namespace BlazorAuthAPI.Services
{
    public class CryptographyService
    {
        // Chave de 32 bytes para AES-256
        private readonly byte[] key = Encoding.UTF8.GetBytes("0123456789ABCDEF0123456789ABCDEF");

        // Vetor de inicialização (IV) de 16 bytes para AES
        private readonly byte[] iv = Encoding.UTF8.GetBytes("ABCDEF0123456789");

        public string Encrypt(UserState userState)
        {
            if (userState == null)
                throw new ArgumentNullException(nameof(userState));

            // Serializa o objeto para JSON
            var json = JsonSerializer.Serialize(userState);
            byte[] encryptedBytes;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(json);
                    sw.Flush();
                    cs.FlushFinalBlock();
                    encryptedBytes = ms.ToArray();
                }
            }

            return Base64UrlEncode(encryptedBytes);
        }

        public UserState Decrypt(string encryptedString)
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

        private string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        private byte[] Base64UrlDecode(string input)
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
    }
}
