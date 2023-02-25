using Microsoft.Toolkit.Uwp.UI.Controls;
using RichTextControls.EventsArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using LinkClickedEventArgs = RichTextControls.EventsArgs.LinkClickedEventArgs;

namespace RichTextControls
{
    public partial class HtmlTextBlock
    {
        private void Hyperlink_Click(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            LinkHandled((string)sender.GetValue(HyperlinkUrlProperty), true);
        }
        private void HyperlinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs args)
        {
            LinkHandled(((HyperlinkButton)sender).GetValue(HyperlinkUrlProperty).ToString(), true);
        }
        /// <summary>
        /// Fired when a user taps one of the image elements
        /// </summary>
        private void NewImagelink_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            string hyperLink = (string)(sender as ImageEx).GetValue(HyperlinkUrlProperty);
            bool isHyperLink = (bool)(sender as ImageEx).GetValue(IsHyperlinkProperty);
            LinkHandled(hyperLink, isHyperLink,(ImageEx)sender);
        }

        /// <summary>
        /// Fired when a link element in the markdown was tapped.
        /// </summary>
        public event EventHandler<LinkClickedEventArgs> LinkClicked;

        /// <summary>
        /// Fired when an image element in the markdown was tapped.
        /// </summary>
        public event EventHandler<LinkClickedEventArgs> ImageClicked;
    }
}
