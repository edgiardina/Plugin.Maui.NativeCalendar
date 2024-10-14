using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Maui.NativeCalendar
{
    public class NativeCalendarView : View
    {
        public static readonly BindableProperty EventsProperty = BindableProperty.Create(
            propertyName: nameof(Events),
            returnType: typeof(List<NativeCalendarEvent>),
            declaringType: typeof(NativeCalendarView),
            defaultValue: new List<NativeCalendarEvent>(),
            defaultBindingMode: BindingMode.TwoWay
        );
        public List<NativeCalendarEvent> Events
        {
            get => (List<NativeCalendarEvent>)GetValue(EventsProperty);
            set => SetValue(EventsProperty, value);
        }

        // Selected Date Bindable Property
        public static readonly BindableProperty SelectedDateProperty = BindableProperty.Create(
            propertyName: nameof(SelectedDate),
            returnType: typeof(DateTime),
            declaringType: typeof(NativeCalendarView),
            defaultValue: DateTime.Now,
            defaultBindingMode: BindingMode.TwoWay
        );

        public DateTime SelectedDate
        {
            get => (DateTime)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        // Maximum Date Bindable Property
        public static readonly BindableProperty MaximumDateProperty = BindableProperty.Create(
            propertyName: nameof(MaximumDate),
            returnType: typeof(DateTime),
            declaringType: typeof(NativeCalendarView),
            defaultValue: DateTime.MaxValue,
            defaultBindingMode: BindingMode.TwoWay
        );

        public DateTime MaximumDate
        {
            get => (DateTime)GetValue(MaximumDateProperty);
            set => SetValue(MaximumDateProperty, value);
        }

        // Minimum Date Bindable Property
        public static readonly BindableProperty MinimumDateProperty = BindableProperty.Create(
            propertyName: nameof(MinimumDate),
            returnType: typeof(DateTime),
            declaringType: typeof(NativeCalendarView),
            defaultValue: DateTime.MinValue,
            defaultBindingMode: BindingMode.TwoWay
        );
        public DateTime MinimumDate
        {
            get => (DateTime)GetValue(MinimumDateProperty);
            set => SetValue(MinimumDateProperty, value);
        }



        public event EventHandler<DateChangedEventArgs> DateChanged;
    }
}
