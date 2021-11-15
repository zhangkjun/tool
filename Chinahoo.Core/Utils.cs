using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinahoo.Core
{
    public class Utils
    {
        /// <summary>
        /// 转简体
        /// </summary>
        /// <param name="s_souce"></param>
        /// <returns></returns>
        public static string TraditionalToSimplified(string s_souce)
        { 
            return ChineseConverter.Convert(s_souce, ChineseConversionDirection.TraditionalToSimplified);//转简体
        }
        /// <summary>
        /// 转繁体
        /// </summary>
        /// <param name="s_souce"></param>
        /// <returns></returns>
        public static string SimplifiedToTraditional(string s_souce)
        {
            return ChineseConverter.Convert(s_souce, ChineseConversionDirection.SimplifiedToTraditional);//转繁体
        }

       

        public static string GetError(string txt, string title, string weui_icon = "weui_icon_warn")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<!DOCTYPE html>");
            sb.Append(" <html>");
            sb.Append("<head>");
            sb.Append("<title>" + title + "</title>");
            sb.Append("<meta charset=\"utf-8\">");
            sb.Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1, user-scalable=0\">");
            sb.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/weui.css\">");
            sb.Append("</head>");
            sb.Append("<body>");
            sb.Append(" <div class=\"weui_msg\">");
            sb.Append("  <div class=\"weui_icon_area\"><i class=\"" + weui_icon + " weui_icon_msg\"></i></div>");
            sb.Append(" <div class=\"weui_text_area\">");
            sb.Append("<h4 class=\"weui_msg_title\">" + txt + "</h4>");
            sb.Append(" </div>");
            sb.Append("  </div>");
            sb.Append(" </body>");
            sb.Append("</html>");
            return sb.ToString();
        }
        #region 普通二维码
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">存储内容</param>
        /// <param name="pixel">像素大小</param>
        /// <returns></returns>
        public static Bitmap GetPTQRCode(string url, int pixel)
        {
            QRCodeGenerator generator = new QRCodeGenerator();
            QRCodeData codeData = generator.CreateQrCode(url, QRCodeGenerator.ECCLevel.M, true);
            QRCoder.QRCode qrcode = new QRCoder.QRCode(codeData);
            Bitmap qrImage = qrcode.GetGraphic(pixel, Color.Black, Color.White, true);
            return qrImage;
        }
        #endregion
        /// <summary>
        /// 全角转换为半角
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }
        public static bool IsValidIP(string ip)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(ip, "[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}"))
            {
                string[] ips = ip.Split('.');
                if (ips.Length == 4 || ips.Length == 6)
                {
                    if (System.Int32.Parse(ips[0]) < 256 && System.Int32.Parse(ips[1]) < 256 & System.Int32.Parse(ips[2]) < 256 & System.Int32.Parse(ips[3]) < 256)
                        return true;
                    else
                        return false;
                }
                else
                    return false;

            }
            else
                return false;
        }
    }
}
