using FlarentApp.Helpers;
using FlarentApp.Services;
using FlarentApp.Views.Controls;
using FlarentApp.Views.DetailPages;
using HtmlAgilityPack;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace FlarentApp.HTMLParser
{
    public class HtmlProperties : DependencyObject
    {
        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.RegisterAttached(
                "Html",
                typeof(string),
                typeof(HtmlProperties),
                new PropertyMetadata(null, HtmlChanged));

        public static void SetHtml(DependencyObject obj, string value)
            => obj.SetValue(HtmlProperty, value);

        public static string GetHtml(DependencyObject obj)
            => (string)obj.GetValue(HtmlProperty);

        public static Func<Span> H1SpanFactory { get; set; }
        public static Func<Span> H2SpanFactory { get; set; }
        public static Func<Span> H3SpanFactory { get; set; }

        public static Action<object, TappedRoutedEventArgs> OnImageTapped { get; set; }

        public static double ImageMaxPixelWidth { get; set; } = 800.0;
        public static double ImageMaxPixelHeight { get; set; } = 600.0;

        private static RichTextBlock _currentObject;

        private static void HtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richText = d as RichTextBlock;
            if (richText == null) return;

            _currentObject = richText;

            //Generate blocks
            var xhtml = e.NewValue as string;
            var blocks = GenerateBlocksForHtml(xhtml);

            _currentObject = null;

            //Add the blocks to the RichTextBlock
            richText.Blocks.Clear();
            foreach (var b in blocks)
                richText.Blocks.Add(b);
        }

        private static List<Block> GenerateBlocksForHtml(string xhtml)
        {
            var blocks = new List<Block>();

            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(xhtml);

                var block = GenerateParagraph(doc.DocumentNode);
                blocks.Add(block);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return blocks;
        }

        // TODO this method seams to be removing necessary spaces in #text nodes
        private static string CleanText(string input)
        {
            var clean = Windows.Data.Html.HtmlUtilities.ConvertToText(input);
            //clean = System.Net.WebUtility.HtmlEncode(clean);
            if (clean == "\0")
                clean = "\n";
            return clean;
        }

        private static Block GenerateBlockForTopNode(HtmlNode node)
            => GenerateParagraph(node);


        private static void AddChildren(Paragraph p, HtmlNode node)
        {
            var added = false;
            foreach (var child in node.ChildNodes)
            {
                try
                {
                    var i = GenerateBlockForNode(child);
                    if (i != null)
                    {
                        p.Inlines.Add(i);
                        added = true;
                    }
                }
                catch
                {
                    p.Inlines.Add(new Run { Text = node.InnerText});
                    added = true;
                }
            }
            if (!added)
            {
                p.Inlines.Add(new Run { Text = CleanText(node.InnerText) });
            }
        }

        private static void AddChildren(Span s, HtmlNode node)
        {
            var added = false;

            foreach (var child in node.ChildNodes)
            {
                var i = GenerateBlockForNode(child);
                if (i != null)
                {
                    s.Inlines.Add(i);
                    added = true;
                }
            }
            if (!added)
            {
                s.Inlines.Add(new Run { Text = CleanText(node.InnerText) });
            }
        }

        private static Inline GenerateBlockForNode(HtmlNode node)
        {
            switch (node.Name)
            {
                case "div":
                    return GenerateSpan(node);
                case "p":
                case "P":
                    return GenerateInnerParagraph(node);
                case "img":
                case "IMG":
                    return GenerateImage(node);
                case "a":
                case "A":
                    return node.ChildNodes.Count >= 1 && (node.FirstChild.Name == "img" || node.FirstChild.Name == "IMG")
                        ? GenerateImage(node.FirstChild)
                        : GenerateHyperLink(node);
                case "li":
                case "LI":
                    return GenerateLi(node);
                case "b":
                case "B":
                case "strong":
                case "STRONG":
                    return GenerateBold(node);
                case "i":
                case "I":
                case "em":
                case "EM":
                    return GenerateItalic(node);
                case "u":
                case "U":
                    return GenerateUnderline(node);
                case "br":
                case "BR":
                    return new LineBreak();
                case "span":
                case "Span":
                    return GenerateSpan(node);
                case "iframe":
                case "Iframe":
                    return GenerateIFrame(node);
                case "video":
                case "Video":
                    return GenterateVideo(node);
                case "#text":
                    if (!string.IsNullOrWhiteSpace(node.InnerText))
                        //return new Run { Text = CleanText(node.InnerText) }; // CleanText is removing white spaces in this case
                        return new Run { Text = node.InnerText };
                    break;
                case "h1":
                case "H1":
                    return GenerateH1(node);
                case "h2":
                case "H2":
                    return GenerateH2(node);
                case "h3":
                case "H3":
                    return GenerateH3(node);
                case "ul":
                case "UL":
                    return GenerateUl(node);
                case "hr":
                case "HR":
                    return GenerateHr(node);
                default:
                    return GenerateSpanWNewLine(node);
            }
            return null;
        }

        private static Inline GenerateHr(HtmlNode node)
        {
            var span = new Span();
            span.Inlines.Add(new LineBreak());
            var inlineUiContainer = new InlineUIContainer();
            var border = new Border
            {
                BorderThickness = new Thickness(0,1,0,0),
                Width = 2048,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                BorderBrush = new SolidColorBrush(Color.FromArgb(50,128,128,128)),
            };

            inlineUiContainer.Child = border;
            span.Inlines.Add(inlineUiContainer);
            span.Inlines.Add(new LineBreak());
            return span;
        }

        private static Inline GenerateLi(HtmlNode node)
        {
            var span = new Span();
            var inlineUiContainer = new InlineUIContainer();
            var ellipse = new Ellipse
            {
                Fill = _currentObject.Foreground ?? new SolidColorBrush(Colors.Black),
                Width = 6,
                Height = 6,
                Margin = new Thickness(-30, 0, 0, 1)
            };
            inlineUiContainer.Child = ellipse;
            span.Inlines.Add(inlineUiContainer);
            AddChildren(span, node);
            span.Inlines.Add(new LineBreak());
            return span;
        }

        private static Inline GenerateImage(HtmlNode node)
        {
            var span = new Span();
            try
            {
                var inlineUiContainer = new InlineUIContainer();
                var sourceUri = WebUtility.HtmlDecode(node.Attributes["src"].Value);
                var sourceWidth = WebUtility.HtmlDecode(node.Attributes["width"]?.Value);
                var sourceHeight = WebUtility.HtmlDecode(node.Attributes["height"]?.Value);

                var image = new Image
                {
                    Stretch = Stretch.Uniform,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                };

                if (sourceWidth != null || sourceHeight != null)
                {
                    if (sourceWidth != null)
                    {
                        image.MaxWidth = double.Parse(sourceWidth);
                        image.Width = double.Parse(sourceWidth);
                    }

                    if (sourceHeight != null)
                    {
                        image.MaxHeight = double.Parse(sourceHeight);
                        image.Height = double.Parse(sourceHeight);
                    }
                }
                else
                {
                    image.ImageOpened += ImageOpened;
                }

                image.ImageFailed += ImageFailed;
                image.Tapped += ImageOnTapped;

                image.Source = new BitmapImage(new Uri(sourceUri, UriKind.Absolute));

                inlineUiContainer.Child = image;

                span.Inlines.Add(inlineUiContainer);
                span.Inlines.Add(new LineBreak());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return span;
        }

        private static void ImageOnTapped(object sender, TappedRoutedEventArgs tappedRoutedEventArgs)
        {
            var image = sender as Image;
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ForwardConnectedAnimation", (UIElement)sender);
            var source = image.Source as BitmapImage;
            new ImageView().Show(source.UriSource.ToString());
            OnImageTapped?.Invoke(sender, tappedRoutedEventArgs);
        }

        private static void ImageFailed(object sender, ExceptionRoutedEventArgs e)
            => Debug.WriteLine("image failed to load");

        private static void ImageOpened(object sender, RoutedEventArgs e)
        {
            var img = sender as Image;
            if (img == null) return;

            var bimg = img.Source as BitmapImage;
            if (bimg != null && (bimg.PixelWidth > ImageMaxPixelWidth || bimg.PixelHeight > ImageMaxPixelHeight))
            {
                img.Width = ImageMaxPixelWidth;
                img.Height = ImageMaxPixelHeight;

                if (bimg.PixelWidth > ImageMaxPixelWidth)
                {
                    img.Width = ImageMaxPixelWidth;
                    img.Height = (ImageMaxPixelWidth / bimg.PixelWidth) * bimg.PixelHeight;
                }
                if (img.Height > ImageMaxPixelHeight)
                {
                    img.Height = ImageMaxPixelHeight;
                    img.Width = (ImageMaxPixelHeight / img.Height) * img.Width;
                }
            }
            else
            {
                if (bimg == null) return;

                img.Height = bimg.PixelHeight;
                img.Width = bimg.PixelWidth;
            }
        }

        private static Inline GenerateHyperLink(HtmlNode node)
        {
            var span = new Span();
            var inlineUiContainer = new InlineUIContainer();
            var link = node.Attributes["href"].Value;
            if (node.Attributes["class"] != null)
            {
                if (node.Attributes["class"].Value == "PostMention")
                {
                    var btn = new HyperlinkButton
                    {
                        Margin = new Thickness(0, 0, 0, -10),
                        Content = new StackPanel { Spacing = 8,Orientation = Orientation.Horizontal},
                        
                        Tag = node.Attributes["data-id"].Value
                        //Content = CleanText(node.InnerText)
                    };
                    ToolTipService.SetToolTip(btn, link);
                    btn.Click += ReplyBtn_Click;
                    var content = btn.Content as StackPanel;
                    content.Children.Add(new FontIcon { Glyph = "\uE97A" ,FontSize = 14});
                    content.Children.Add(new TextBlock { Text = CleanText(node.InnerText) });
                    inlineUiContainer.Child = btn;
                    span.Inlines.Add(inlineUiContainer);
                    return span;
                }
                else if(node.Attributes["class"].Value == "UserMention")
                {
                    var btn = new HyperlinkButton
                    {
                        Margin = new Thickness(0, 0, 0, -10),
                        Content = CleanText(node.InnerText),
                        Tag = node.Attributes["href"].Value,
                    };
                    ToolTipService.SetToolTip(btn, link);
                    btn.Click += UserMentionBtn_Click;
                    inlineUiContainer.Child = btn;
                    span.Inlines.Add(inlineUiContainer);
                    return span;
                }
                else
                {                 
                    var btn = new HyperlinkButton
                    {
                        Margin = new Thickness(0, 0, 0, -10),
                        Content = CleanText(node.InnerText),
                        NavigateUri = new Uri(node.Attributes["href"].Value, UriKind.Absolute),
                    };
                    ToolTipService.SetToolTip(btn, link);
                    inlineUiContainer.Child = btn;
                    span.Inlines.Add(inlineUiContainer);
                    return span;
                }
            }

            if (link.Contains($"{Flarent.Settings.Forum.ToLower()}/d/"))
            {
                var btn = new HyperlinkButton
                {
                    Margin = new Thickness(0, 0, 0, -10),
                    Content = new StackPanel { Spacing = 8, Orientation = Orientation.Horizontal },
                    Tag = link
                    //Content = CleanText(node.InnerText)
                };
                ToolTipService.SetToolTip(btn, link);
                btn.Click += DiscussionViewBtn_Click; ;
                var content = btn.Content as StackPanel;
                content.Children.Add(new FontIcon { Glyph = "\uE130", FontSize = 14 });
                content.Children.Add(new TextBlock { Text = CleanText(node.InnerText) });
                inlineUiContainer.Child = btn;
                span.Inlines.Add(inlineUiContainer);
                return span;
            }

            var hyperlinkButton = new HyperlinkButton
            {
                NavigateUri = new Uri(node.Attributes["href"].Value, UriKind.Absolute),
                Content = CleanText(node.InnerText),               
            };
            hyperlinkButton.Style = (Style)Application.Current.Resources["TextBlockButtonStyle"];
            ToolTipService.SetToolTip(hyperlinkButton, link);

            hyperlinkButton.Margin = new Thickness(0, 0, 0, -10);
            inlineUiContainer.Child = hyperlinkButton;
            span.Inlines.Add(inlineUiContainer);
            return span;
        }

        private static void DiscussionViewBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as HyperlinkButton;
            var link = btn.Tag.ToString();
            var split = link.Split("/");
            var id = split[split.Count() - 1];
            if (split.Count() > 5)
                NavigationService.OpenInRightPane(typeof(PostDetailPage), link);
            else
            {
                if (id.Contains("-"))//防止链接后面有字符存在
                {
                    var index = id.IndexOf('-');
                    id = id.Remove(index);
                }
                NavigationService.Navigate<DiscussionDetailPage>(int.Parse(id));
            }
        }

        private static void UserMentionBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as HyperlinkButton;
            var link = btn.Tag.ToString();
            var split = link.Split("/");
            try
            {
                var uid = int.Parse(split[split.Count() - 1]);

                if (Flarent.Settings.ViewUsersInPane)
                    NavigationService.OpenInRightPane(typeof(UserDetailPage), uid);
                else
                    NavigationService.Navigate<UserDetailPage>(uid);
            }
            catch
            {
                var username = split[split.Count() - 1];
                if (Flarent.Settings.ViewUsersInPane)
                    NavigationService.OpenInRightPane(typeof(UserDetailPage), username);
                else
                    NavigationService.Navigate<UserDetailPage>(username);
            }
        }

        private static void ReplyBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as HyperlinkButton;
            NavigationService.OpenInRightPane(typeof(PostDetailPage), int.Parse(btn.Tag.ToString()));
        }

        private static Inline GenterateVideo(HtmlNode node)
        {
            var span = new Span();
            span.Inlines.Add(new LineBreak());
            var inlineUiContainer = new InlineUIContainer();
            var video = new MediaPlayerElement
            {
                Source = MediaSource.CreateFromUri(new Uri(node.Attributes["src"].Value, UriKind.Absolute)),
                AreTransportControlsEnabled = true,
                //Source = (IMediaPlaybackSource)new Uri(node.Attributes["src"].Value, UriKind.Absolute),
            };

            inlineUiContainer.Child = video;
            span.Inlines.Add(inlineUiContainer);
            span.Inlines.Add(new LineBreak());
            return span;
        }
        private static Inline GenerateIFrame(HtmlNode node)
        {
            try
            {
                var span = new Span();
                span.Inlines.Add(new LineBreak());
                var inlineUiContainer = new InlineUIContainer();
                var webView = new WebView
                {
                    Source = new Uri("about: blank"),
                    Width = 800,
                    Height = 480,
                    //Width = int.Parse(node.Attributes["width"].Value),
                    //Height = int.Parse(node.Attributes["height"].Value)
                };
                //webView.CacheMode = 
                if (node.Attributes["src"] != null)
                {
                    var src = node.Attributes["src"].Value;
                    if (src.StartsWith("//"))
                    {
                        src =  "https:" + src;
                    }
                    if(src.Contains("music.163.com"))
                    {
                        webView.Source = new Uri(src.Replace("amp;", ""));
                        webView.Height = 88;
                    }

                    webView.Source = new Uri(src.Replace("amp;", ""));                                        
                }
                if (node.Attributes["width"] != null)
                    webView.Width = int.Parse(node.Attributes["width"].Value);
                if (node.Attributes["height"] != null)
                    webView.Height = int.Parse(node.Attributes["height"].Value);
                webView.Unloaded += WebView_Unloaded;
                inlineUiContainer.Child = webView;
                span.Inlines.Add(inlineUiContainer);
                span.Inlines.Add(new LineBreak());
                return span;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private static async void WebView_Unloaded(object sender, RoutedEventArgs e)
        {
            var wv = sender as WebView;
            //await Windows.UI.Xaml.Controls.WebView.ClearTemporaryWebDataAsync();
            for (var i = 0; i < 3; i++)
            {
                wv.Navigate(new Uri("about:blank"));
                await Task.Delay(100);
            }
        }

        private static Block GenerateTopIFrame(HtmlNode node)
        {
            try
            {
                var paragraph = new Paragraph();
                var inlineUiContainer = new InlineUIContainer();
                var webView = new WebView
                {
                    Source = new Uri(node.Attributes["src"].Value, UriKind.Absolute),
                    Width = int.Parse(node.Attributes["width"].Value),
                    Height = int.Parse(node.Attributes["height"].Value)
                };
                inlineUiContainer.Child = webView;
                paragraph.Inlines.Add(inlineUiContainer);
                return paragraph;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private static Inline GenerateBold(HtmlNode node)
        {
            var bold = new Bold();
            AddChildren(bold, node);
            return bold;
        }

        private static Inline GenerateUnderline(HtmlNode node)
        {
            var underline = new Underline();
            AddChildren(underline, node);
            return underline;
        }

        private static Inline GenerateItalic(HtmlNode node)
        {
            var italic = new Italic();
            AddChildren(italic, node);
            return italic;
        }

        private static Block GenerateParagraph(HtmlNode node)
        {
            var paragraph = new Paragraph();
            AddChildren(paragraph, node);
            return paragraph;
        }

        private static Inline GenerateUl(HtmlNode node)
        {
            var span = new Span();
            span.Inlines.Add(new LineBreak());
            AddChildren(span, node);
            return span;
        }

        private static Inline GenerateInnerParagraph(HtmlNode node)
        {
            var span = new Span();
            span.Inlines.Add(new LineBreak());
            AddChildren(span, node);
            span.Inlines.Add(new LineBreak());
            return span;
        }

        private static Inline GenerateSpan(HtmlNode node)
        {
            var span = new Span();
            AddChildren(span, node);
            return span;
        }

        private static Inline GenerateSpanWNewLine(HtmlNode node)
        {
            var span = new Span();
            AddChildren(span, node);
            if (span.Inlines.Count > 0)
                span.Inlines.Add(new LineBreak());
            return span;
        }

        private static Span GenerateH3(HtmlNode node)
        {
            var span = H3SpanFactory?.Invoke() ?? new Span();
            span.Inlines.Add(new LineBreak());
            var bold = new Bold();
            var run = new Run { Text = CleanText(node.InnerText) };
            bold.Inlines.Add(run);
            span.Inlines.Add(bold);
            span.Inlines.Add(new LineBreak());
            return span;
        }

        private static Inline GenerateH2(HtmlNode node)
        {
            var span = H2SpanFactory?.Invoke() ?? new Span { FontSize = 24 };
            span.Inlines.Add(new LineBreak());
            var run = new Run { Text = CleanText(node.InnerText) };
            span.Inlines.Add(run);
            span.Inlines.Add(new LineBreak());
            return span;
        }

        private static Inline GenerateH1(HtmlNode node)
        {
            var span = H1SpanFactory?.Invoke() ?? new Span { FontSize = 30 };
            span.Inlines.Add(new LineBreak());
            var run = new Run { Text = CleanText(node.InnerText) };
            span.Inlines.Add(run);
            span.Inlines.Add(new LineBreak());
            return span;
        }
    }

}
