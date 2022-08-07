using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FlarentApp.Helpers
{
    public class WebViewExtensions
    {
        public static readonly DependencyProperty HtmlSourceProperty =
               DependencyProperty.RegisterAttached("HtmlSource", typeof(string), typeof(WebViewExtensions), new PropertyMetadata("", OnHtmlSourceChanged));
        public static string GetHtmlSource(DependencyObject obj) { return (string)obj.GetValue(HtmlSourceProperty); }
        public static void SetHtmlSource(DependencyObject obj, string value) { obj.SetValue(HtmlSourceProperty, value); }
        private static void OnHtmlSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WebView wv = d as WebView;
            if (wv != null)
            {
                string ansStr = GenarateHtml((string)e.NewValue);
                wv.NavigateToString((string)ansStr);
                wv.UpdateLayout();
            }
        }
        private static string GenarateHtml(string div)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html>");
            sb.Append("<head>");
            string js = "<script>function getHeight() {var body = document.body," +
                            "html = document.documentElement; var height = document.getElementsByTagName('html')[0].offsetHeight;" +
            "window.external.notify(\"height:\"+ height);}" +
            "function preventBehavior(e) {e.preventDefault();};document.addEventListener(\"touchmove\", preventBehavior, {passive: false});" +
            "</script>";

            sb.Append(js);

            sb.Append("</head>");

            sb.Append(div);
            sb.Append("</body>");
            sb.Append("</html>");
            return sb.ToString();
        }
    }
}
