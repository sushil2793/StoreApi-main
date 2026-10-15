using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SmartMeterAdmin.Infrastructure.Helpers
{
    public class CryptoUtility
    {
        readonly byte[] key = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
        readonly byte[] iv = { 65, 110, 68, 26, 69, 178, 200, 219 };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public byte[] Encrypt(string plainText)
        {
            UTF8Encoding utf8encoder = new();
            byte[] inputInBytes = utf8encoder.GetBytes(plainText);

            using var tdesProvider = TripleDES.Create();
            tdesProvider.Key = key;
            tdesProvider.IV = iv;
            ICryptoTransform cryptoTransform = tdesProvider.CreateEncryptor(key, iv);

            MemoryStream encryptedStream = new();
            CryptoStream cryptStream = new(encryptedStream, cryptoTransform, CryptoStreamMode.Write);

            cryptStream.Write(inputInBytes, 0, inputInBytes.Length);
            cryptStream.FlushFinalBlock();
            encryptedStream.Position = 0;

            byte[] result = new byte[encryptedStream.Length];

            encryptedStream.Read(result, 0, (int)encryptedStream.Length);
            cryptStream.Close();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputInBytes"></param>
        /// <returns></returns>
        public string Decrypt(byte[] inputInBytes)
        {
            using var tdesProvider = TripleDES.Create();

            ICryptoTransform cryptoTransform = tdesProvider.CreateDecryptor(key, iv);

            MemoryStream decryptedStream = new();
            CryptoStream cryptStream = new(decryptedStream, cryptoTransform, CryptoStreamMode.Write);
            cryptStream.Write(inputInBytes, 0, inputInBytes.Length);
            cryptStream.FlushFinalBlock();
            decryptedStream.Position = 0;

            byte[] result = new byte[decryptedStream.Length];
            decryptedStream.Read(result, 0, (int)decryptedStream.Length);
            cryptStream.Close();
            UTF8Encoding myutf = new();

            return myutf.GetString(result);

        }
    }
}
