using Chinahoo.Core;
using Chinahoo.Web.Models;
using FineUICore;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Web;
using System.Xml;

namespace Chinahoo.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// 二维码
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pixel"></param>
        /// <returns></returns>
        [Route("QRCode")]//重写地址
        public IActionResult GetPTQRCode(string url, int pixel = 5)
        {
            try
            {
                url = HttpUtility.UrlDecode(url);
                Response.ContentType = "image/png";

                var bitmap = Utils.GetPTQRCode(url, pixel);
                MemoryStream ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Png);
                return File(ms.ToArray(), "image/png");
            }
            catch 
            {
               
                return File(System.Text.Encoding.UTF8.GetBytes(Chinahoo.Core.Utils.GetError("系统错误", "抱歉，出错了")), "text/html");
            }
        }

        public IActionResult Index()
        {
            LoadData();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        // GET: Themes
        public IActionResult Themes()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #region LoadData

        private string _cookieMenuStyle = "tree";
        private bool _cookieShowOnlyBase = false;
        private string _cookieDisplayMode = "normal";
        private string _cookieMainTabs = "multi";
        private string _cookieLang = "zh_CN";
        private string _cookieSearchText = "";
        // 示例数
        private int _examplesCount = 0;

        private void LoadData()
        {
            string cookie = String.Empty;
            // 从Cookie中读取 - 是否仅显示基础版示例
            cookie = Request.Cookies["ShowOnlyBase"];
            if (cookie != null)
            {
                _cookieShowOnlyBase = Convert.ToBoolean(cookie);
            }


            // 如果是FineUICore（基础版），或者用户选择仅显示基础版示例
            if (_cookieShowOnlyBase || Constants.IS_BASE)
            {
                // 基础版不支持紧凑模式，大字体模式，大间距模式
                //_cookieMenuStyle = "plaintree";
                _cookieDisplayMode = "normal";
            }
            else
            {
                // 从Cookie中读取 - 显示模式
                cookie = Request.Cookies["DisplayMode"];
                if (!String.IsNullOrEmpty(cookie))
                {
                    _cookieDisplayMode = cookie;
                }
            }

            // 从Cookie中读取 - 左侧菜单类型
            cookie = Request.Cookies["MenuStyle"];
            if (!String.IsNullOrEmpty(cookie))
            {
                _cookieMenuStyle = cookie;
            }


            // 从Cookie中读取 - 语言
            cookie = Request.Cookies["Language"];
            if (!String.IsNullOrEmpty(cookie))
            {
                _cookieLang = cookie;
            }


            // 从Cookie中读取 - 搜索文本
            cookie = Request.Cookies["SearchText"];
            if (!String.IsNullOrEmpty(cookie))
            {
                _cookieSearchText = HttpUtility.UrlDecode(cookie);
            }

            // 从Cookie中读取 - 主选项卡标签
            cookie = Request.Cookies["MainTabs"];
            if (!String.IsNullOrEmpty(cookie))
            {
                _cookieMainTabs = cookie;
            }

            LoadTreeMenuData();

            ViewBag.CookieMenuStyle = _cookieMenuStyle;
            ViewBag.CookieShowOnlyBase = _cookieShowOnlyBase;
            ViewBag.CookieIsBase = Constants.IS_BASE;
            ViewBag.CookieDisplayMode = _cookieDisplayMode;
            ViewBag.CookieMainTabs = _cookieMainTabs;
            ViewBag.CookieLang = _cookieLang;
            ViewBag.CookieSearchText = _cookieSearchText;

            ViewBag.ProductVersion = GlobalConfig.ProductVersion;
            ViewBag.ExamplesCount = _examplesCount.ToString();
        }

        private void LoadTreeMenuData()
        {
            string key ="Chinahoo.web.LoadTreeMenuData";
            IList<TreeNode> nodes=WebCache.Get(key, () =>
            {
                string xmlPath = PageContext.MapWebPath("~/res/menu.xml");

                string xmlContent = String.Empty;
                using (StreamReader sr = new StreamReader(xmlPath))
                {
                    xmlContent = sr.ReadToEnd();
                }

                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(xmlContent);

                IList<TreeNode> nodes = new List<TreeNode>();
                ResolveXmlNodeList(nodes, xdoc.DocumentElement.ChildNodes);
                return nodes;
            });
           
            // 视图数据
            ViewBag.TreeMenuNodes = nodes.ToArray();
        }

        private int ResolveXmlNodeList(IList<TreeNode> nodes, XmlNodeList xmlNodes)
        {
            // nodes 中渲染到页面上的节点个数
            int nodeVisibleCount = 0;

            foreach (XmlNode xmlNode in xmlNodes)
            {
                if (xmlNode.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                TreeNode node = new TreeNode();

                // 是否叶子节点
                bool isLeaf = xmlNode.ChildNodes.Count == 0;

                bool currentNodeIsVisible = true;

                string nodeText = "";
                bool nodeIsCorp = false;

                XmlAttribute textAttr = xmlNode.Attributes["Text"];
                if (textAttr != null)
                {
                    nodeText = textAttr.Value;
                }

                // 是否企业版
                XmlAttribute isCorpAttr = xmlNode.Attributes["IsCorp"];
                if (isCorpAttr != null)
                {
                    nodeIsCorp = isCorpAttr.Value.ToLower() == "true";
                }


                int childVisibleCount = 0;
                if (isLeaf)
                {
                    // 仅显示基础版示例
                    if (_cookieShowOnlyBase && nodeIsCorp)
                    {
                        currentNodeIsVisible = false;
                    }

                    // 存在搜索文本
                    if (!String.IsNullOrEmpty(_cookieSearchText))
                    {
                        if (!nodeText.Contains(_cookieSearchText))
                        {
                            currentNodeIsVisible = false;
                        }
                    }
                }
                else
                {
                    // 递归
                    childVisibleCount = ResolveXmlNodeList(node.Nodes, xmlNode.ChildNodes);

                    if (childVisibleCount == 0)
                    {
                        currentNodeIsVisible = false;
                    }
                    else
                    {
                        // 存在搜索文本
                        if (!String.IsNullOrEmpty(_cookieSearchText))
                        {
                            // 展开节点
                            node.Expanded = true;
                        }
                    }
                }

                if (currentNodeIsVisible)
                {
                    foreach (XmlAttribute attribute in xmlNode.Attributes)
                    {
                        string name = attribute.Name;
                        string value = attribute.Value;

                        if (name == "Text")
                        {
                            // Text需要特殊处理
                            if (isLeaf)
                            {
                                // 设置节点的提示信息
                                node.ToolTip = nodeText;
                            }

                            // 存在 IsCorp=True 属性，则改变 Text 的值
                            if (nodeIsCorp)
                            {
                                node.IconFont = IconFont._Enterprise;
                                //nodeText += "&nbsp;<span class=\"iscorp\">Corp.</span>";
                            }

                            node.Text = nodeText;
                        }
                        else
                        {
                            node.SetPropertyValue(name, value);
                        }
                    }

                    nodes.Add(node);

                    // 本子节点显示
                    nodeVisibleCount++;

                    // 示例数只计算叶子节点
                    if (isLeaf)
                    {
                        _examplesCount++;
                    }

                }

            }

            return nodeVisibleCount;
        }


        #endregion

    }
}