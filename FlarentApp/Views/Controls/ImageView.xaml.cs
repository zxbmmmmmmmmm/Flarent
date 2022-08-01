using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace FlarentApp.Views.Controls
{
    public sealed partial class ImageView : UserControl
    {
        private Popup Popup;
        public double WindowWidth;
        public double WindowHeight;
        public ImageView()
        {
            this.InitializeComponent();
            Window.Current.SizeChanged += WindowSizeChanged;
            Popup = new Popup();
            Popup.Child = this;
        }
        public void Show(string image)
        {
            var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("ForwardConnectedAnimation");
            if (anim != null)
            {
                anim.TryStart(MainImage);
            }
            MainImage.Source = new BitmapImage(new Uri(image));
            LinkTextBlock.Text = image;
            Width = Window.Current.Bounds.Width;
            Height = Window.Current.Bounds.Height;
            this.Popup.IsOpen = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            MainImage.Visibility = Visibility.Collapsed;
            Window.Current.SizeChanged -= WindowSizeChanged;
            this.Popup.IsOpen = false;
            UnloadObject(this);
        }
        private void WindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            WindowWidth = e.Size.Width;
            WindowHeight = e.Size.Height;
            Width = e.Size.Width;
            Height = e.Size.Height;
        }

        private void ZoomInBtn_Click(object sender, RoutedEventArgs e)
        {
            var Height = MainScrollViewer.ExtentHeight;
            var Width = MainScrollViewer.ExtentWidth;
            MainScrollViewer.ChangeView(Width / 2, Height / 2, (float?)(MainScrollViewer.ZoomFactor + 0.5));
        }

        private void ZoomOutBtn_Click(object sender, RoutedEventArgs e)
        {
            var Height = MainScrollViewer.ExtentHeight;
            var Width = MainScrollViewer.ExtentWidth;
            MainScrollViewer.ChangeView(Width / 2, Height / 2, (float?)(MainScrollViewer.ZoomFactor - 0.5));
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            var rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(MainImage);

            var saveFile = new FileSavePicker();
            saveFile.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            saveFile.FileTypeChoices.Add("JPEG files", new List<string>() { ".jpg" });
            saveFile.SuggestedFileName = "111";
            StorageFile sFile = await saveFile.PickSaveFileAsync();
            if (sFile == null)
                return;

            var pixels = await rtb.GetPixelsAsync();
            using (IRandomAccessStream stream = await sFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await
                BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                byte[] bytes = pixels.ToArray();
                encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                        BitmapAlphaMode.Ignore,
                                        (uint)rtb.PixelWidth,
                                        (uint)rtb.PixelHeight,
                                        200,
                                        200,
                                        bytes);

                await encoder.FlushAsync();
            }
        }

        private async void CopyBtn_Click(object sender, RoutedEventArgs e)
        {
            //use RenderTargetBitmap to render UIElement to image byte data array
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(MainImage);//any xmal element
            var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();
            var pixels = pixelBuffer.ToArray();
            //Create MemoryStream to write image byte data to
            var stream = new InMemoryRandomAccessStream();
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                        BitmapAlphaMode.Ignore,
                        (uint)renderTargetBitmap.PixelWidth,
                        (uint)renderTargetBitmap.PixelHeight,
                        DisplayInformation.GetForCurrentView().RawDpiX,
                        DisplayInformation.GetForCurrentView().RawDpiY,
                        pixels);
            await encoder.FlushAsync();
            //Set stream data into DataPackage object
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));
            //Set DataPackage object into Clipboard
            Clipboard.SetContent(dataPackage);
        }

        private void CopyLinkBtn_Click(object sender, RoutedEventArgs e)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText("");
            Clipboard.SetContent(dataPackage);
        }
    }
}
