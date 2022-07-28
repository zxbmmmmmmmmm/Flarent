using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace FlarentApp.Helpers
{
    public class LayoutHelper
    {
        public static LayoutValues Values = new LayoutValues();
    }
    /// <summary>
    /// 用于存放一些布局值
    /// </summary>
    public class LayoutValues : INotifyPropertyChanged
    {
        public int TitleTextBlockMaxWidth
        {
            get => Flarent.Settings.OpenPaneLegnth - 105;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); });
        }
    }

}
