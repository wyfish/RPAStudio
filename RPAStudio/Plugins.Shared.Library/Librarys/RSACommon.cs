using CSharp_easy_RSA_PEM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Plugins.Shared.Library.Librarys
{
    public class RSACommon
    {
        public static string EncryptString(string inputString, byte[] publicKey)
        {
            string loadedPublicKey = System.Text.Encoding.UTF8.GetString(publicKey);
            RSACryptoServiceProvider publickey = Crypto.DecodeRsaPublicKey(loadedPublicKey);
            string secretString = Crypto.EncryptString(inputString, publickey);

            return secretString;
        }

        public static string DecryptString(string secretString, byte[] privateKey, string password = "")
        {
            string loadedRSA = System.Text.Encoding.UTF8.GetString(privateKey);
            RSACryptoServiceProvider privateRSAkey = Crypto.DecodeRsaPrivateKey(loadedRSA, password);
            string inputString = Crypto.DecryptString(secretString, privateRSAkey);

            return inputString;
        }

        // 对消息进行签名，返回BASE64编码格式
        public static string SignMessage(byte[] privateKey, string importantMessage, object halg = null)
        {
            if (halg == null)
            {
                halg = typeof(SHA256);
            }

            string privateRSA = System.Text.Encoding.UTF8.GetString(privateKey);

            RSACryptoServiceProvider privateRSAkey = Crypto.DecodeRsaPrivateKey(privateRSA);

            var importantMessageBytes = System.Text.Encoding.UTF8.GetBytes(importantMessage);
            var bytes = privateRSAkey.SignData(importantMessageBytes, halg);
            var signature = Convert.ToBase64String(bytes);

            return signature;
        }

        public static bool VerifyMessage(byte[] publicKey, string signature, string importantMessage, object halg = null)
        {
            if (halg == null)
            {
                halg = typeof(SHA256);
            }

            var publicRSA = System.Text.Encoding.UTF8.GetString(publicKey);

            RSACryptoServiceProvider publicRSAkey = Crypto.DecodeRsaPublicKey(publicRSA);

            var signatureBytes = Convert.FromBase64String(signature);
            var importantMessageBytes = System.Text.Encoding.UTF8.GetBytes(importantMessage);
            var isSignatureOkay = publicRSAkey.VerifyData(importantMessageBytes, halg, signatureBytes);
            return isSignatureOkay;
        }

        public static void CreateRsaKeys(int keyLength = 4096)
        {
            RSACryptoServiceProvider newKey = Crypto.CreateRsaKeys(keyLength);
            File.WriteAllText("new.private.rsa.pem", Crypto.ExportPrivateKeyToRSAPEM(newKey));
            File.WriteAllText("new.public.rsa.pem", Crypto.ExportPublicKeyToRSAPEM(newKey));
        }

    }
}