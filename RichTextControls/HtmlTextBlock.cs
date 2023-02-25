using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Toolkit.Uwp.UI.Controls.Markdown.Render;
using Microsoft.Toolkit.Uwp.UI.Helpers;
using RichTextControls.Generators;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace RichTextControls
{
    public partial class HtmlTextBlock : Control, Generators.ILinkRegister
    {
        private Border _rootElement;

        // Used to attach the URL to hyperlinks.
        private static readonly DependencyProperty HyperlinkUrlProperty =
            DependencyProperty.RegisterAttached("HyperlinkUrl", typeof(string), typeof(MarkdownTextBlock), new PropertyMetadata(null));

        // Checkes if clicked image is a hyperlink or not.
        private static readonly DependencyProperty IsHyperlinkProperty =
            DependencyProperty.RegisterAttached("IsHyperLink", typeof(string), typeof(MarkdownTextBlock), new PropertyMetadata(null));
        private bool multiClickDetectionTriggered;
        private readonly List<object> _listeningHyperlinks = new List<object>();
        /// Gets the interface that is used to register hyperlinks.
        /// </summary>

        /// <summary>
        /// Gets or sets the style for the blockquote Border
        /// </summary>
        public Style BlockquoteBorderStyle
        {
            get { return (Style)GetValue(BlockquoteBorderStyleProperty); }
            set { SetValue(BlockquoteBorderStyleProperty, value); }
        }

        /// <summary>
        /// Gets the dependency property for <see cref="BlockquoteBorderStyle"/>.
        /// </summary>
        public static readonly DependencyProperty BlockquoteBorderStyleProperty = DependencyProperty.Register(
            nameof(BlockquoteBorderStyle),
            typeof(Style),
            typeof(HtmlTextBlock),
            new PropertyMetadata(null, OnRenderingPropertyChanged)
        );

        /// <summary>
        /// Gets or sets the style for the preformatted block Border
        /// </summary>
        public Style PreformattedBorderStyle
        {
            get { return (Style)GetValue(PreformattedBorderStyleProperty); }
            set { SetValue(PreformattedBorderStyleProperty, value); }
        }

        /// <summary>
        /// Gets the dependency property for <see cref="PreformattedBorderStyle"/>.
        /// </summary>
        public static readonly DependencyProperty PreformattedBorderStyleProperty = DependencyProperty.Register(
            nameof(PreformattedBorderStyle),
            typeof(Style),
            typeof(HtmlTextBlock),
            new PropertyMetadata(null, OnRenderingPropertyChanged)
        );

        /// <summary>
        /// Gets or sets the html to be rendered.
        /// </summary>
        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }

        /// <summary>
        /// Gets the dependency property for <see cref="Html"/>.
        /// </summary>
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.Register(
            nameof(Html),
            typeof(string),
            typeof(HtmlTextBlock),
            new PropertyMetadata("", OnRenderingPropertyChanged)
        );

        /// <summary>
        /// Gets or sets a custom Html to Xaml generator.
        /// </summary>
        public IHtmlXamlGenerator CustomGenerator
        {
            get { return (IHtmlXamlGenerator)GetValue(CustomGeneratorProperty); }
            set { SetValue(CustomGeneratorProperty, value); }
        }

        /// <summary>
        /// Gets the dependency property for <see cref="CustomGenerator"/>.
        /// </summary>
        public static readonly DependencyProperty CustomGeneratorProperty = DependencyProperty.Register(
            nameof(CustomGenerator),
            typeof(Dictionary<string, Func<string, Inline>>),
            typeof(HtmlTextBlock),
            new PropertyMetadata(null, OnRenderingPropertyChanged)
        );

        /// <summary>
        /// The root child containing the parsed and rendered HTML.
        /// </summary>
        public UIElement Child
        {
            get
            {
                return _rootElement;
            }
        }

        /// <summary>
        /// Re-renders the HTML document when the property changes
        /// </summary>
        private static void OnRenderingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as HtmlTextBlock;
            if (control == null)
                return;
            
            control.RenderDocument();
        }

        public HtmlTextBlock
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            

































































            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            ()
        {
            DefaultStyleKey = typeof(HtmlTextBlock);
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _rootElement = GetTemplateChild("RootElement") as Border;

            RenderDocument();
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            HookListeners();

            // Register for property callbacks that are owned by our parent class.

        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {

        }

    }
}
