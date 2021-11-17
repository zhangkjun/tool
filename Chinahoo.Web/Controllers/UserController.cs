using FineUICore;
using Jsbeautifier;
using Microsoft.AspNetCore.Mvc;
using NETCore.Encrypt;
using System.Net;
using System.Text;
using System.Web;

namespace Chinahoo.Web.Controllers
{
    public class UserController : BaseController
    {
        public IActionResult Main()
        {
            
            var ip = Chinahoo.Extensions.Context.Request.GetIP();
            if (Chinahoo.Core.Utils.IsValidIP(ip))
            {
                var address = Chinahoo.Core.IPSearch.GetAddress(ip);
                ViewBag.Ip = "您的iP地址是:[" + ip + "]来自：" + address.country + address.area;
            }
            else
            {
               
                ViewBag.Ip = "您的iP地址是:[" + ip + "]";
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchIp_Click(IFormCollection values)
        {
            try
            {
                var ip = values["ip"].ToString();
                if (string.IsNullOrEmpty(ip))
                {
                    Alert.Show("ip或者域名不能为空");
                }
                else
                {
                    if (Chinahoo.Core.Utils.IsValidIP(ip))
                    {
                        var address = Chinahoo.Core.IPSearch.GetAddress(ip);
                        UIHelper.Label("Label2").Text("您查询的iP地址是:[" + ip + "]来自：" + address.country + address.area);
                        ShowNotify("查询成功！", MessageBoxIcon.Success);

                    }
                    else
                    {
                        try
                        {
                            var url = new Uri("http://"+(ip.Replace("http://", "").Replace("https://", "")));
                            IPAddress[] IPs = Dns.GetHostAddresses(url.Host);
                            if (IPs.Count() > 0)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.Append("" + url.Host + "服务器ip：\n");
                                foreach (IPAddress ips in IPs)
                                {
                                    var address = Chinahoo.Core.IPSearch.GetAddress(ips.ToString());
                                    sb.Append("当前解析:[" + ips + "]  " + address.country + address.area + ",");
                                }
                                UIHelper.TextBox("ip").Text(url.Host);
                                UIHelper.Label("Label2").Text(sb.ToString().TrimEnd(',').Replace(",", "\n"));
                            }
                            else
                            {
                                Alert.Show("当前查询异常");
                            }
                        }
                        catch
                        {
                            Alert.Show("当前网址输入错误");
                        }

                    }
                }
               
            }
            catch
            {
                ShowNotify("系统异常稍后重试", MessageBoxIcon.Error);
            }
            return UIHelper.Result();
        }
        #region aes
        /// <summary>
        /// AES
        /// </summary>
        /// <returns></returns>
        public IActionResult AES()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAES_Click()
        {
            try
            {
                var aseKey = EncryptProvider.CreateAesKey();
                var Key = UIHelper.TextBox("Key");
                var IV = UIHelper.TextBox("IV");
                Key.Text(aseKey.Key);
                IV.Text(aseKey.IV);
                ShowNotify("生成成功", MessageBoxIcon.Success);
            }
            catch
            {
                ShowNotify("系统异常稍后重试", MessageBoxIcon.Error);
            }
            return UIHelper.Result();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EncryptionAES_Click(IFormCollection values)
        {
            try
            {
                var Key = values["Key"];
                var IV = values["IV"];
                var original = values["original"];
                if (string.IsNullOrEmpty(original))
                {
                    Alert.Show("原文不能为空");
                }
                else
                {
                    var mode = Chinahoo.Extensions.Context.CommonHelper.ToInt(values["mode"]);
                    var ciphertext = UIHelper.TextArea("ciphertext");
                    if (mode == 0)
                    {
                        ciphertext.Text(EncryptProvider.AESEncrypt(original, Key, IV));
                    }
                    else
                    {
                        ciphertext.Text(EncryptProvider.AESEncrypt(original, Key));
                    }
                    ShowNotify("加密成功！", MessageBoxIcon.Success);
                }
            }
            catch
            {
                ShowNotify("系统异常稍后重试", MessageBoxIcon.Error);
            }
            return UIHelper.Result();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DecryptAES_Click(IFormCollection values)
        {
            try
            {
                var Key = values["Key"];
                var IV = values["IV"];
                var ciphertext = values["ciphertext"];
                if (string.IsNullOrEmpty(ciphertext))
                {
                    Alert.Show("密文不能为空");
                }
                else
                {
                    var mode = Chinahoo.Extensions.Context.CommonHelper.ToInt(values["mode"]);
                    var original = UIHelper.TextArea("original");
                    if (mode == 0)
                    {
                        original.Text(EncryptProvider.AESDecrypt(ciphertext, Key, IV));
                    }
                    else
                    {
                        original.Text(EncryptProvider.AESDecrypt(ciphertext, Key));
                    }
                    ShowNotify("解密成功！", MessageBoxIcon.Success);
                }
            }
            catch
            {
                ShowNotify("系统异常稍后重试", MessageBoxIcon.Error);
            }
            return UIHelper.Result();
        }
        #endregion
        #region des
        /// <summary>
        /// AES
        /// </summary>
        /// <returns></returns>
        public IActionResult DES()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateDES_Click()
        {
            try
            {
                var desKey = UIHelper.TextBox("desKey");
                desKey.Text(EncryptProvider.CreateDesKey());
                ShowNotify("生成成功", MessageBoxIcon.Success);
            }
            catch
            {
                ShowNotify("系统异常稍后重试", MessageBoxIcon.Error);
            }
            return UIHelper.Result();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EncryptionDES_Click(IFormCollection values)
        {
            try
            {
                var desKey = values["desKey"];
                var original = values["original"];
                if (string.IsNullOrEmpty(original))
                {
                    Alert.Show("原文不能为空");
                }
                else
                {
                    var ciphertext = UIHelper.TextArea("ciphertext");
                    ciphertext.Text(EncryptProvider.DESEncrypt(original, desKey));
                    ShowNotify("加密成功！", MessageBoxIcon.Success);
                }
            }
            catch
            {
                ShowNotify("系统异常稍后重试", MessageBoxIcon.Error);
            }
            return UIHelper.Result();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DecryptDES_Click(IFormCollection values)
        {
            try
            {
                var desKey = values["desKey"];
                var ciphertext = values["ciphertext"];
                if (string.IsNullOrEmpty(ciphertext))
                {
                    Alert.Show("密文不能为空");
                }
                else
                {
                    var original = UIHelper.TextArea("original");

                    original.Text(EncryptProvider.DESDecrypt(ciphertext, desKey));

                    ShowNotify("解密成功！", MessageBoxIcon.Success);
                }
            }
            catch
            {
                ShowNotify("系统异常稍后重试", MessageBoxIcon.Error);
            }
            return UIHelper.Result();
        }
        #endregion
        #region MD5
        public IActionResult MD5()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EncryptionMd5_Click(IFormCollection values)
        {
            try
            {
                var original = values["original"];
                var mode = Chinahoo.Extensions.Context.CommonHelper.ToInt(values["mode"]);
                var ciphertext = UIHelper.TextArea("ciphertext");
                if (mode == 0)
                {
                    ciphertext.Text(EncryptProvider.Md5(original, MD5Length.L32));
                }
                else
                {
                    ciphertext.Text(EncryptProvider.Md5(original, MD5Length.L16));
                }
                ShowNotify("加密成功！", MessageBoxIcon.Success);
            }
            catch
            {
                ShowNotify("系统异常稍后重试", MessageBoxIcon.Error);
            }
            return UIHelper.Result();
        }
        #endregion

        #region SHA
        public IActionResult SHA()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EncryptionSHA_Click(IFormCollection values)
        {
            try
            {
                var original = values["original"];
                var mode =values["mode"];
                var ciphertext = UIHelper.TextArea("ciphertext");
                if (mode == "Sha256")
                {
                    ciphertext.Text(EncryptProvider.Sha256(original));
                }
                else if (mode == "Sha384")
                {
                    ciphertext.Text(EncryptProvider.Sha384(original));
                }
                else if (mode == "Sha512")
                {
                    ciphertext.Text(EncryptProvider.Sha512(original));
                }
                else
                {
                    ciphertext.Text(EncryptProvider.Sha1(original));
                }
                ShowNotify("加密成功！", MessageBoxIcon.Success);
            }
            catch
            {
                ShowNotify("系统异常稍后重试", MessageBoxIcon.Error);
            }
            return UIHelper.Result();
        }
        #endregion
        #region HMAC
        public IActionResult HMAC()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EncryptionHMAC_Click(IFormCollection values)
        {
            try
            {
                var original = values["original"];
                var mode = values["mode"];
                var key = values["key"];
                var ciphertext = UIHelper.TextArea("ciphertext");
                if (mode == "HMACSHA1")
                {
                    ciphertext.Text(EncryptProvider.HMACSHA1(original,key));
                }
                else if (mode == "HMACSHA256")
                {
                    ciphertext.Text(EncryptProvider.HMACSHA256(original,key));
                }
                else if (mode == "HMACSHA384")
                {
                    ciphertext.Text(EncryptProvider.HMACSHA384(original,key));
                }
                else if (mode == "HMACSHA512")
                {
                    ciphertext.Text(EncryptProvider.HMACSHA512(original,key));
                }
                else
                {
                    ciphertext.Text(EncryptProvider.HMACMD5(original,key));
                }
                ShowNotify("加密成功！", MessageBoxIcon.Success);
            }
            catch
            {
                ShowNotify("系统异常稍后重试", MessageBoxIcon.Error);
            }
            return UIHelper.Result();
        }
        #endregion
        #region Base64
        public IActionResult Base64()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EncryptionBase64_Click(IFormCollection values)
        {
            try
            {
                var original = values["original"];
                if (string.IsNullOrEmpty(original))
                {
                    Alert.Show("明文不能为空");
                }
                else
                {
                    var ciphertext = UIHelper.TextArea("ciphertext");
                    ciphertext.Text(EncryptProvider.Base64Encrypt(original));
                    ShowNotify("加密成功！", MessageBoxIcon.Success);
                }
            }
            catch
            {
                ShowNotify("系统异常稍后重试", MessageBoxIcon.Error);
            }
            return UIHelper.Result();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DecryptBase64_Click(IFormCollection values)
        {
            try
            {
                var ciphertext = values["ciphertext"];
                if (string.IsNullOrEmpty(ciphertext))
                {
                    Alert.Show("密文不能为空");
                }
                else
                {
                    var original = UIHelper.TextArea("original");
                    original.Text(EncryptProvider.Base64Decrypt(ciphertext));
                    ShowNotify("解密成功！", MessageBoxIcon.Success);
                }
            }
            catch
            {
                ShowNotify("系统异常稍后重试", MessageBoxIcon.Error);
            }
            return UIHelper.Result();
        }

        #endregion
        #region RSA
        public IActionResult RSA()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateRSA_Click(IFormCollection values)
        {
            try
            {
                var pkcs1KeyTuple = EncryptProvider.RSAToPem(true);
                var ciphertext = UIHelper.TextArea("ciphertext");
                var mode = values["mode"];
                if (mode == "3072")
                {
                    pkcs1KeyTuple = EncryptProvider.RSAToPem(true, 3072);
                }
                else if (mode == "4096")
                {
                    pkcs1KeyTuple = EncryptProvider.RSAToPem(true, 4096);
                }
                var publicPem = UIHelper.TextArea("publicPem");
                var privatePem = UIHelper.TextArea("privatePem");
                publicPem.Text(pkcs1KeyTuple.publicPem);
                privatePem.Text(pkcs1KeyTuple.privatePem);
                ShowNotify("生成成功", MessageBoxIcon.Success);
            }
            catch
            {
                ShowNotify("系统异常稍后重试", MessageBoxIcon.Error);
            }
            return UIHelper.Result();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateRSA1_Click(IFormCollection values)
        {
            try
            {
                var pkcs1KeyTuple = EncryptProvider.RSAToPem(false);
                var mode = values["mode"];
                if (mode == "3072")
                {
                    pkcs1KeyTuple = EncryptProvider.RSAToPem(true, 3072);
                }
                else if (mode == "4096")
                {
                    pkcs1KeyTuple = EncryptProvider.RSAToPem(true, 4096);
                }
                var publicPem = UIHelper.TextArea("publicPem");
                var privatePem = UIHelper.TextArea("privatePem");
                publicPem.Text(pkcs1KeyTuple.publicPem);
                privatePem.Text(pkcs1KeyTuple.privatePem);
                ShowNotify("生成成功", MessageBoxIcon.Success);
            }
            catch
            {
                ShowNotify("系统异常稍后重试", MessageBoxIcon.Error);
            }
            return UIHelper.Result();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EncryptionRSA_Click(IFormCollection values)
        {
            try
            {
                var publicPem = values["publicPem"];
                var privatePem = values["privatePem"];
                var original = values["original"];
                if (string.IsNullOrEmpty(publicPem))
                {
                    Alert.Show("公钥不能为空");
                }
                else if (string.IsNullOrEmpty(privatePem))
                {
                    Alert.Show("私钥不能为空");
                }
                else if (string.IsNullOrEmpty(original))
                {
                    Alert.Show("加密原文不能为空");
                }
                else
                {
                    UIHelper.TextArea("ciphertext").Text(Chinahoo.Core.Encrypt.RSAEncrypt(publicPem, original));
                    ShowNotify("加密成功", MessageBoxIcon.Success);
                }
               
               
            }
            catch
            {
                ShowNotify("系统异常稍后重试", MessageBoxIcon.Error);
            }
            return UIHelper.Result();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DecryptRSA_Click(IFormCollection values)
        {
            try
            {
                var publicPem = values["publicPem"];
                var privatePem = values["privatePem"];
                var ciphertext = values["ciphertext"];
                if (string.IsNullOrEmpty(publicPem))
                {
                    Alert.Show("公钥不能为空");
                }
                else if (string.IsNullOrEmpty(privatePem))
                {
                    Alert.Show("私钥不能为空");
                }
                else if (string.IsNullOrEmpty(ciphertext))
                {
                    Alert.Show("密文不能为空");
                }
                else
                {
                    UIHelper.TextArea("original").Text(Chinahoo.Core.Encrypt.RSADecrypt(privatePem, ciphertext));
                    ShowNotify("解密成功", MessageBoxIcon.Success);
                }

                
            }
            catch
            {
                ShowNotify("系统异常稍后重试", MessageBoxIcon.Error);
            }
            return UIHelper.Result();
        }
        //
        #endregion
        #region RSA
        public IActionResult QRCode()
        {
            return View();
        }
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateQRCode_Click(IFormCollection values)
        {
            try
            {
                var Key = values["Key"];
                if (string.IsNullOrEmpty(Key))
                {
                    Alert.Show("文字内容不能为空");
                }
                else
                {
                    UIHelper.Image("Image3").ImageUrl("/QRCode?url=" + HttpUtility.UrlEncode(Key));
                    ShowNotify("生成成功", MessageBoxIcon.Success);
                }

              
            }
            catch
            {
                ShowNotify("系统异常稍后重试", MessageBoxIcon.Error);
            }
            return UIHelper.Result();
        }
        #endregion
        #region 繁简转换
        public IActionResult ChineseTrans()
        {
            return View();
        }
        /// <summary>
        /// 转换繁体
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EncryptionChineseTrans_Click(IFormCollection values)
        {
            try
            {
                var original = values["original"];
                if (string.IsNullOrEmpty(original))
                {
                    Alert.Show("简体中文内容不能为空");
                }
                else
                {
                    UIHelper.TextArea("ciphertext").Text(Chinahoo.Core.Utils.SimplifiedToTraditional(original));
                    ShowNotify("转换成功", MessageBoxIcon.Success);
                }

               
            }
            catch
            {
                ShowNotify("系统异常稍后重试", MessageBoxIcon.Error);
            }
            return UIHelper.Result();
        }
        /// <summary>
        /// 转换简体
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DecryptChineseTrans_Click(IFormCollection values)
        {
            try
            {
                var ciphertext = values["ciphertext"];
                if (string.IsNullOrEmpty(ciphertext))
                {
                    Alert.Show("繁体中文内容不能为空");
                }
                else
                {
                    UIHelper.TextArea("original").Text(Chinahoo.Core.Utils.TraditionalToSimplified(ciphertext));
                    ShowNotify("转换成功", MessageBoxIcon.Success);
                }

               
            }
            catch
            {
                ShowNotify("系统异常稍后重试", MessageBoxIcon.Error);
            }
            return UIHelper.Result();
        }
        //
        #endregion
        #region js格式化
        private Beautifier beautifier = new Beautifier();
        public IActionResult Js()
        {
            
           
            return  View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Encryptionjs_Click(IFormCollection values)
        {
            try
            {
                BeautifierOptions bo = beautifier.Opts;
                bo.IndentWithTabs = true;
                var original = values["original"];
                if (string.IsNullOrEmpty(original))
                {
                    Alert.Show("内容不能为空");
                }
                else
                {
                    UIHelper.TextArea("original").Text(beautifier.Beautify(original));
                    ShowNotify("格式化成功", MessageBoxIcon.Success);
                }


            }
            catch
            {
                ShowNotify("系统异常稍后重试", MessageBoxIcon.Error);
            }
            return UIHelper.Result();
        }
        //
        #endregion

    }
}
