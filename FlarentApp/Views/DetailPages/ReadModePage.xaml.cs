using FlarentApp.ViewModels.Dialogs;
using FlarumApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FlarentApp.Views.DetailPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ReadModePage : Page
    {
        public ReadModePageVM ViewModel = new();
        public ReadModePage()
        {
            this.InitializeComponent();           
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.Discussion = e.Parameter as Discussion;
            ViewModel.LoadMoreCommand.ExecuteAsync(null);
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var height = Window.Current.Bounds.Height*0.8 + MainScrollViewer.ExtentHeight * 0.1;
            if (MainScrollViewer.VerticalOffset + height >= MainScrollViewer.ExtentHeight&&!ViewModel.LoadMoreCommand.IsRunning)
            {
                ViewModel.LoadMoreCommand.ExecuteAsync(null);
            }
        }
    }
}
