using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.UI.Xaml.Controls;
using RichTextControls.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using LinkClickedEventArgs = RichTextControls.EventsArgs.LinkClickedEventArgs;

namespace RichTextControls
{
    public partial class HtmlTextBlock
    {
        private void RenderDocument()
        {
            if (_rootElement == null || String.IsNullOrEmpty(Html))
                return;
            UnhookListeners();
            try
            {
                var generator = CustomGenerator ?? new HtmlXamlGenerator(Html,this);

                generator.BlockquoteBorderStyle = BlockquoteBorderStyle;
                generator.PreformattedBorderStyle = PreformattedBorderStyle;

                var parsedHtml = generator.Generate();

                _rootElement.Child = parsedHtml;
            }
            catch (Exception ex)
            {
                var panel = new StackPanel { Spacing = 8};
                
                var infobar = new InfoBar
                {
                    IsClosable = false,
                    Title = "错误",
                    Severity = InfoBarSeverity.Error,
                    Message = $"HTML转换失败，将显示源HTML\n{ex.Message}",
                    IsOpen = true,                    
                    HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch,
                };
                panel.Children.Add(infobar);
                panel.Children.Add(new TextBlock() { Text = Html ,TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap});
                _rootElement.Child = panel;
            }
        }
        /// <summary>
        /// Called when the render has a link we need to listen to.
        /// </summary>
        public void RegisterNewHyperLink(Hyperlink newHyperlink, string linkUrl)
        {
            // Setup a listener for clicks.
            newHyperlink.Click += Hyperlink_Click;

            // Associate the URL with the hyperlink.
            newHyperlink.SetValue(HyperlinkUrlProperty, linkUrl);

            // Add it to our list
            _listeningHyperlinks.Add(newHyperlink);
        }
        public void RegisterNewHyperLink(HyperlinkButton newHyperlink, string linkUrl)
        {
            // Setup a listener for clicks.
            newHyperlink.Click += HyperlinkButton_Click;

            // Associate the URL with the hyperlink.
            newHyperlink.SetValue(HyperlinkUrlProperty, linkUrl);

            // Add it to our list
            _listeningHyperlinks.Add(newHyperlink);
        }
        /// <summary>
        /// Called when the render has a link we need to listen to.
        /// </summary>
        public void RegisterNewHyperLink(Image newImagelink, string linkUrl, bool isHyperLink)
        {
            // Setup a listener for clicks.
            newImagelink.Tapped += NewImagelink_Tapped;

            // Associate the URL with the hyperlink.
            newImagelink.SetValue(HyperlinkUrlProperty, linkUrl);

            // Set if the Image is HyperLink or not
            newImagelink.SetValue(IsHyperlinkProperty, isHyperLink);

            // Add it to our list
            _listeningHyperlinks.Add(newImagelink);
        }        /// <summary>
                 /// Called when the render has a link we need to listen to.
                 /// </summary>
        public void RegisterNewHyperLink(ImageEx newImagelink, string linkUrl, bool isHyperLink)
        {
            // Setup a listener for clicks.
            newImagelink.Tapped += NewImagelink_Tapped;

            // Associate the URL with the hyperlink.
            newImagelink.SetValue(HyperlinkUrlProperty, linkUrl);

            // Set if the Image is HyperLink or not
            newImagelink.SetValue(IsHyperlinkProperty, isHyperLink);

            // Add it to our list
            _listeningHyperlinks.Add(newImagelink);
        }

        private void HookListeners()
        {
            // Re-hook all hyper link events we currently have
            foreach (object link in _listeningHyperlinks)
            {
                if (link is Hyperlink hyperlink)
                {
                    hyperlink.Click -= Hyperlink_Click;
                    hyperlink.Click += Hyperlink_Click;
                }
                else if (link is ImageEx image)
                {
                    image.Tapped -= NewImagelink_Tapped;
                    image.Tapped += NewImagelink_Tapped;
                }
            }
        }

        private void UnhookListeners()
        {
            // Unhook any hyper link events if we have any
            foreach (object link in _listeningHyperlinks)
            {
                if (link is Hyperlink hyperlink)
                {
                    hyperlink.Click -= Hyperlink_Click;
                }
                else if (link is ImageEx image)
                {
                    image.Tapped -= NewImagelink_Tapped;
                }
            }
        }
        internal async void LinkHandled(string url, bool isHyperlink,ImageEx imageEx = null)
        {
            // Links that are nested within superscript elements cause the Click event to fire multiple times.
            // e.g. this markdown "[^bot](http://www.reddit.com/r/youtubefactsbot/wiki/index)"
            // Therefore we detect and ignore multiple clicks.
            if (multiClickDetectionTriggered)
            {
                return;
            }

            multiClickDetectionTriggered = true;
            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            await dispatcherQueue.EnqueueAsync(() => multiClickDetectionTriggered = false, DispatcherQueuePriority.High);

            // Get the hyperlink URL.
            if (url == null)
            {
                return;
            }

            // Fire off the event.

            var eventArgs = new LinkClickedEventArgs(url);
            if (imageEx != null)
                eventArgs = new LinkClickedEventArgs(url,imageEx);
            if (isHyperlink)
            {
                LinkClicked?.Invoke(this, eventArgs);
            }
            else
            {
                ImageClicked?.Invoke(this, eventArgs);
            }
        }
    }

}
