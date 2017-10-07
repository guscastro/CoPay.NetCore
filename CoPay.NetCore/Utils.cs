using NBitcoin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CoPay
{
    public class Utils
    {
        public static String encryptMessage(String message, String password)
        {
            //default AES 128
            return Encrypt(message, password);
        }

        //args.name, args.xPubKey, args.requestPubKey
        public static String getCopayerHash(String name, String xPubKey, String requestPubKey)
        {
            //return [name, xPubKey, requestPubKey].join('|');
            return String.Format("{0}|{1}|{2}", name, xPubKey, requestPubKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="privKey">Assume hex</param>
        /// <returns></returns>
        public static String signMessage(String text, String privKey)
        {
            //Assume private key is in hex
            Byte[] keyAsBytes = NBitcoin.DataEncoders.Encoders.Hex.DecodeData(privKey);
            Key key = new Key(keyAsBytes);

            return Utils.signMessage(text, key);
        }

        public static String signMessage(String text, Key key) {
            var hashString = Utils.hashMessage(text);
            var hash = new uint256(hashString);
            return NBitcoin.DataEncoders.Encoders.Hex.EncodeData(key.Sign(hash).ToDER());
        }

        public static string SignRequest(string method, string url, object args, string key)
        {
            var parsedKey = Key.Parse(key);
            return Utils.signRequest(method, url, args, parsedKey);
        }

        public static string signRequest(String method, String url, object args, Key key)
        {
            String json = JsonConvert.SerializeObject(args);
            String message = String.Format("{0}|{1}|{2}", method.ToLower(), url, json);

            return Utils.signMessage(message, key);
        }

        private static byte[] hashMessage(String test)
        {
            Byte[] data = NBitcoin.DataEncoders.Encoders.ASCII.DecodeData(test);
            return NBitcoin.Crypto.Hashes.SHA256(NBitcoin.Crypto.Hashes.SHA256(data));
        }

        public static string Encrypt(string clearText, string key)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        // cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new
                    Rfc2898DeriveBytes(EncryptionKey, new byte[]
                    { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        // cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public static string XPubKeyToCopayerId(string xPubKey) {
            var s = SHA256.Create();
            var hash = s.ComputeHash(Encoding.ASCII.GetBytes(xPubKey));
            return BitConverter.ToString(hash);
        }

        public static string PrivateKeyToAESKey(string privKey) {
            var pk = NBitcoin.Key.Parse(privKey);
            var hash = NBitcoin.Crypto.Hashes.SHA256(pk.ToBytes()).Take(16).ToArray();
            return NBitcoin.DataEncoders.Encoders.Base64.EncodeData(hash);
        }
    }
}
