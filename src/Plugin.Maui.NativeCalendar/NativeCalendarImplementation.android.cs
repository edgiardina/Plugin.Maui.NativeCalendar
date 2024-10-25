using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Util;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Java.Util;
using Microsoft.Maui.Platform;
using System.Xml;
using static Android.Widget.CalendarView;

namespace Plugin.Maui.NativeCalendar
{
    public class NativeCalendarImplementation : FrameLayout
    {
        private CalendarView calendarView;
        private readonly NativeCalendarView nativeCalendarView;

        public NativeCalendarImplementation(Context context, NativeCalendarView nativeCalendarView) : base(context)
        {
            this.nativeCalendarView = nativeCalendarView;

            calendarView = new CalendarView(context);
            calendarView.SetOnDateChangeListener(new DateChangeListener(nativeCalendarView));

            var layoutParams = new CoordinatorLayout.LayoutParams(
                LayoutParams.MatchParent,
                LayoutParams.MatchParent
            );


            // Add the CalendarView to the CoordinatorLayout
            calendarView.LayoutParameters = layoutParams;
            AddView(calendarView);
        }

        public void UpdateTintColor(NativeCalendarView nativeCalendarView)
        {
            // TODO: no-op? or something else
            // set selection background color to tintColor         
        }

        public void UpdateSelectedDate(NativeCalendarView nativeCalendarView)
        {
            calendarView.Date = ConvertDateTimeToLong(nativeCalendarView.SelectedDate);
        }

        public void UpdateMaximumDate(NativeCalendarView nativeCalendarView)
        {
            calendarView.MaxDate = ConvertDateTimeToLong(nativeCalendarView.MaximumDate);
        }

        public void UpdateMinimumDate(NativeCalendarView nativeCalendarView)
        {
            calendarView.MinDate = ConvertDateTimeToLong(nativeCalendarView.MinimumDate);
        }

        public void UpdateEvents(NativeCalendarView nativeCalendarView)
        {
            // TODO represent Events on Calendar
        }

        private long ConvertDateTimeToLong(DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
        }

        private class DateChangeListener : Java.Lang.Object, CalendarView.IOnDateChangeListener
        {
            private readonly NativeCalendarView nativeCalendarView;

            public DateChangeListener(NativeCalendarView nativeCalendarView)
            {
                this.nativeCalendarView = nativeCalendarView;
            }

            public void OnSelectedDayChange(CalendarView view, int year, int month, int dayOfMonth)
            {
                // Handle the date change
                this.nativeCalendarView.SelectedDate = new DateTime(year, month + 1, dayOfMonth);
            }
        }

    }
}
