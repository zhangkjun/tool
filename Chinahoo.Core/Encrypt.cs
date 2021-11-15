using NETCore.Encrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Chinahoo.Core
{
    /// <summary>
    /// 加密相关
    /// </summary>
    public class Encrypt
    {
        public static string MD5(string srcString)
        {
            return EncryptProvider.Md5(srcString);
        }
        /// <summary>
        /// SHA256
        /// </summary>
        /// <param name="srcString"></param>
        /// <returns></returns>
        public static string SHA256(string srcString)
        {
            return EncryptProvider.Sha256(srcString);
        }
        /// <summary>
        /// Sha384
        /// </summary>
        /// <param name="srcString"></param>
        /// <returns></returns>
        public static string Sha384(string srcString)
        {
            return EncryptProvider.Sha384(srcString);
        }
        /// <summary>
        /// Sha512
        /// </summary>
        /// <param name="srcString"></param>
        /// <returns></returns>
        public static string Sha512(string srcString)
        {
            return EncryptProvider.Sha512(srcString);
        }
        /// <summary>
        /// HMACMD5
        /// </summary>
        /// <param name="srcString"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string HMACMD5(string srcString, string key)
        {
            return EncryptProvider.HMACMD5(srcString, key);
        }
        /// <summary>
        /// HMACSHA1
        /// </summary>
        /// <param name="srcString"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string HMACSHA1(string srcString, string key)
        {
            return EncryptProvider.HMACSHA1(srcString, key);
        }
        /// <summary>
        /// HMACSHA256
        /// </summary>
        /// <param name="srcString"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string HMACSHA256(string srcString, string key)
        {
            return EncryptProvider.HMACSHA256(srcString, key);
        }
        /// <summary>
        /// HMACSHA384
        /// </summary>
        /// <param name="srcString"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string HMACSHA384(string srcString, string key)
        {
            return EncryptProvider.HMACSHA384(srcString, key);
        }
        /// <summary>
        /// HMACSHA512
        /// </summary>
        /// <param name="srcString"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string HMACSHA512(string srcString, string key)
        {
            return EncryptProvider.HMACSHA512(srcString, key);
        }
        //        BASE64 操作
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="srcString"></param>
        /// <returns></returns>
        public static string Base64Encrypt(string srcString)
        {
            return EncryptProvider.Base64Encrypt(srcString, Encoding.UTF8);   //default encoding is UTF-8
        }

        /// <summary>
        ///    Base64解密
        /// </summary>
        /// <param name="encryptedStr"></param>
        /// <returns></returns>
        public static string Base64Decrypt(string encryptedStr)
        {
            return EncryptProvider.Base64Decrypt(encryptedStr, Encoding.UTF8);   //default encoding is UTF-8
        }
        /// <summary>
        /// rsa加密
        /// </summary>
        /// <param name="pemPublicKey"></param>
        /// <param name="srcString"></param>
        /// <returns></returns>
        public static string RSAEncrypt(string pemPublicKey, string srcString)
        {
            return Convert.ToBase64String(HexStringToBytes(EncryptProvider.RSAEncryptWithPem(pemPublicKey, srcString)));
        }
        /// <summary>
        /// rsa解密
        /// </summary>
        /// <param name="pemPrivateKey"></param>
        /// <param name="encryptedStr"></param>
        /// <returns></returns>
        public static string RSADecrypt(string pemPrivateKey, string encryptedStr)
        {
            return EncryptProvider.RSADecryptWithPem(pemPrivateKey, BytesTohexString(Convert.FromBase64String(encryptedStr)));
        }

        public static string BytesTohexString(byte[] bytes)
        {
            if (bytes == null || bytes.Count() < 1)
            {
                return string.Empty;
            }

            var count = bytes.Count();

            var cache = new StringBuilder();
            //cache.Append("0x");
            for (int ii = 0; ii < count; ++ii)
            {
                var tempHex = Convert.ToString(bytes[ii], 16).ToUpper();
                cache.Append(tempHex.Length == 1 ? "0" + tempHex : tempHex);
            }

            return cache.ToString();
        }

        public static byte[] HexStringToBytes(string hexStr)
        {
            if (string.IsNullOrEmpty(hexStr))
            {
                return new byte[0];
            }

            if (hexStr.StartsWith("0x"))
            {
                hexStr = hexStr.Remove(0, 2);
            }

            var count = hexStr.Length;

            if (count % 2 == 1)
            {
                throw new ArgumentException("Invalid length of bytes:" + count);
            }

            var byteCount = count / 2;
            var result = new byte[byteCount];
            for (int ii = 0; ii < byteCount; ++ii)
            {
                var tempBytes = Byte.Parse(hexStr.Substring(2 * ii, 2), System.Globalization.NumberStyles.HexNumber);
                result[ii] = tempBytes;
            }

            return result;
        }

    }
}
