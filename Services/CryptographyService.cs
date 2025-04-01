using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BlazorAuthAPI.Models;

namespace BlazorAuthAPI.Services
{
    public class CryptographyService
    {
        // Chave de 32 bytes para AES-256 (substitua por um valor seguro)
        private readonly byte[] key = Encoding.UTF8.GetBytes("0123456789ABCDEF0123456789ABCDEF");
        // Vetor de inicialização (IV) de 16 bytes para AES (substitua por um valor seguro)
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
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(json);
                        }
                        encryptedBytes = ms.ToArray();
                    }
                }
            }

            // Converte os bytes criptografados para Base64 e faz URL Encode para transmissão via query string
            string base64 = Convert.ToBase64String(encryptedBytes);
            return WebUtility.UrlEncode(base64);
        }

        public UserState Decrypt(string encryptedString)
        {
            if (string.IsNullOrEmpty(encryptedString))
                throw new ArgumentNullException(nameof(encryptedString));

            // Faz URL Decode e converte de Base64 para byte array
            string base64 = WebUtility.UrlDecode(encryptedString);
            byte[] cipherBytes = Convert.FromBase64String(base64);
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

            // Desserializa o JSON para o objeto UserState
            return JsonSerializer.Deserialize<UserState>(json)!;
        }
    }
}
