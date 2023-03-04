using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace Tasks.Helpers
{
    public sealed class HtmlHelper
    {
        public static string GetFirstImage(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var nodes = doc.DocumentNode.SelectNodes("//img");

            if (nodes != null)
                return nodes[0].Attributes["src"].Value;
            return null;
        }
    }
}
