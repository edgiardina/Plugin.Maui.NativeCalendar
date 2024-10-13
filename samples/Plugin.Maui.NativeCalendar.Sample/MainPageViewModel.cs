using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Maui.NativeCalendar.Sample
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private DateTime maximumDate;

        [ObservableProperty]
        private DateTime minimumDate;

        public MainPageViewModel()
        {
            maximumDate = DateTime.Now.AddYears(1);
            minimumDate = DateTime.Now.AddYears(-1);


        }

    }
}
