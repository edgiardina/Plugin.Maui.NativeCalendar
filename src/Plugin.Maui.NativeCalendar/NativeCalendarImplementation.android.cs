using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Util;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.ResourceInspection.Annotation;
using Com.Applandeo.Materialcalendarview;
using Java.Util;
using Microsoft.Maui.Platform;
using System.Xml;
using MaterialCalendar = Com.Applandeo.Materialcalendarview;

namespace Plugin.Maui.NativeCalendar
{
    public class NativeCalendarImplementation : FrameLayout
    {
        private MaterialCalendar.CalendarView calendarView;
        private readonly NativeCalendarView nativeCalendarView;

        public NativeCalendarImplementation(Context context, NativeCalendarView nativeCalendarView) : base(context)
        {
            this.nativeCalendarView = nativeCalendarView;

            SetupCalendarView();

            Application.Current.RequestedThemeChanged += (s, a) =>
            {
                SetupCalendarView();
            };
        }

        public void UpdateTintColor(NativeCalendarView nativeCalendarView)
        {
            // TODO: no-op? or something else
            // set selection background color to tintColor
            //calendarView.SetSelectionBackground(nativeCalendarView.TintColor.ToPlatform());
        }

        public void UpdateTitleTextColor(NativeCalendarView nativeCalendarView)
        {
            calendarView.SetHeaderLabelColor(nativeCalendarView.TitleTextColor.ToPlatform());
            // set forward button image to be a fontimage of a greater than symbol colored to match the title text color
            calendarView.SetForwardButtonImage(CreateArrowDrawable(">"));
            calendarView.SetPreviousButtonImage(CreateArrowDrawable("<"));
        }

        public void UpdateHeaderColor(NativeCalendarView nativeCalendarView)
        {
            calendarView.SetHeaderColor(nativeCalendarView.HeaderColor.ToPlatform());
            //TODO: set left and right marker colors to be the same as the header color
            //calendarView.(nativeCalendarView.HeaderColor.ToPlatform());
        }

        public void UpdateSelectedDate(NativeCalendarView nativeCalendarView)
        {
            Calendar date = Calendar.Instance;
            date.Set(nativeCalendarView.SelectedDate.Year, nativeCalendarView.SelectedDate.Month - 1, nativeCalendarView.SelectedDate.Day);
            calendarView.SetDate(date);
        }

        public void UpdateMaximumDate(NativeCalendarView nativeCalendarView)
        {
            Calendar maxDate = Calendar.Instance;
            maxDate.Set(nativeCalendarView.MaximumDate.Year, nativeCalendarView.MaximumDate.Month - 1, nativeCalendarView.MaximumDate.Day);
            calendarView.SetMinimumDate(maxDate);
        }

        public void UpdateMinimumDate(NativeCalendarView nativeCalendarView)
        {
            // calendarView.SetMinimumDate();// = new DateTimeOffset(nativeCalendarView.MinimumDate).ToUnixTimeMilliseconds(); 
            Calendar minDate = Calendar.Instance;
            minDate.Set(nativeCalendarView.MinimumDate.Year, nativeCalendarView.MinimumDate.Month - 1, nativeCalendarView.MinimumDate.Day);
            calendarView.SetMinimumDate(minDate);
        }

        public void UpdateEvents(NativeCalendarView nativeCalendarView)
        {
            // Create a ShapeDrawable to represent a dot
            var dotDrawable = new Android.Graphics.Drawables.ShapeDrawable(new OvalShape());
            dotDrawable.SetIntrinsicHeight(16);
            dotDrawable.SetIntrinsicWidth(16);
            dotDrawable.Paint.Color = nativeCalendarView.EventIndicatorColor.ToPlatform(); // Custom color for the dot

            // Update the events
            List<CalendarDay> calendarDays = new List<CalendarDay>();

            foreach (var date in nativeCalendarView.Events)
            {
                // Convert DateTime to Calendar
                Calendar calendar = Calendar.Instance;
                calendar.Set(date.StartDate.Year, date.StartDate.Month - 1, date.StartDate.Day);  // Month is 0-based in Calendar

                // Create CalendarDay
                CalendarDay calendarDay = new CalendarDay(calendar);
                //calendarDay.s // Set a custom label (optional)
                calendarDay.ImageDrawable = dotDrawable;  // Set a custom indicator (optional)

                // set calendar day background color
                //calendarDay.SelectedBackgroundDrawable = new Android.Graphics.Drawables.ColorDrawable(Android.Graphics.Color.Red);
                //calendarDay.BackgroundDrawable = new Android.Graphics.Drawables.ColorDrawable(Android.Graphics.Color.Pink);

                calendarDays.Add(calendarDay);
            }

            // Set the calendar days to the CalendarView
            calendarView.SetCalendarDays(calendarDays);            
        }

        private void SetupCalendarView()
        {
            var attributes = GetAttributeSet();

            calendarView = new MaterialCalendar.CalendarView(this.Context, attributes);

            // Set layout parameters, e.g., match parent in both dimensions
            var layoutParams = new CoordinatorLayout.LayoutParams(
                LayoutParams.MatchParent,
                LayoutParams.MatchParent
            );

            // Add the CalendarView to the CoordinatorLayout
            calendarView.LayoutParameters = layoutParams;
            AddView(calendarView);

            // Set event handler when date is changed
            calendarView.SetOnCalendarDayClickListener(new OnCalendarDayClickListener(calendarView, nativeCalendarView));


            calendarView.SetHeaderColor(nativeCalendarView.HeaderColor?.ToPlatform() ?? Android.Graphics.Color.Green);
            calendarView.SetBackgroundColor(nativeCalendarView.BackgroundColor?.ToPlatform() ?? Android.Graphics.Color.Transparent);
            calendarView.SetHeaderLabelColor(nativeCalendarView.TitleTextColor?.ToPlatform() ?? Android.Graphics.Color.Black);
            //calendarView.SetSelectionBackground(nativeCalendarView.TintColor?.ToPlatform() ?? Android.Graphics.Color.Blue);

            UpdateEvents(nativeCalendarView);
        }

        private IAttributeSet GetAttributeSet()
        {
            XmlReader xmlResource;
            // set to single selection
            // unfortunately, MaterialCalendarView's selection mode is not available in code, you have to declare it as an XML property
            // https://github.com/dotnet/runtime/issues/102300

            // if dark mode, load dark layout, otherwise load light layout
            if (Application.Current.RequestedTheme == AppTheme.Dark)
            {
                xmlResource = this.Resources.GetXml(Resource.Layout.dark);
            }
            else
            {
                xmlResource = this.Resources.GetXml(Resource.Layout.light);
            }

            // TODO: Change these values in the XML element to match the NativeCalendarView properties
            //    app: pagesColor = "#FFC0CB"
            //app: abbreviationsBarColor = "#FF4455"
            //app: selectionColor = "#FF6699"
            //app: todayLabelColor = "@android:color/holo_purple"

            xmlResource.Read();
            return Xml.AsAttributeSet(xmlResource);
        }

        private Drawable CreateArrowDrawable(string symbol)
        {
            // Create a bitmap to draw the symbol
            Bitmap bitmap = Bitmap.CreateBitmap(100, 100, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);

            // Set up the paint for drawing text
            Android.Graphics.Paint paint = new Android.Graphics.Paint
            {
                AntiAlias = true,
                Color = nativeCalendarView.TitleTextColor.ToPlatform(),  // Customize the color if needed                
                TextSize = 150         // Adjust text size based on your preference
            };

            // Measure the text size to center it in the bitmap
            Android.Graphics.Rect textBounds = new Android.Graphics.Rect();
            paint.GetTextBounds(symbol, 0, symbol.Length, textBounds);
            float x = (bitmap.Width - textBounds.Width()) / 2f;
            float y = (bitmap.Height + textBounds.Height()) / 2f;

            // Draw the symbol to the canvas
            canvas.DrawText(symbol, x, y, paint);

            // Create a drawable from the bitmap
            return new BitmapDrawable(bitmap);
        }

        private class OnCalendarDayClickListener : Java.Lang.Object, MaterialCalendar.Listeners.IOnCalendarDayClickListener
        {
            private readonly MaterialCalendar.CalendarView calendarView;
            private readonly NativeCalendarView nativeCalendarView;

            public OnCalendarDayClickListener(MaterialCalendar.CalendarView calendarView, NativeCalendarView nativeCalendarView)
            {
                this.calendarView = calendarView;
                this.nativeCalendarView = nativeCalendarView;
            }

            public void OnClick(CalendarDay calendarDay)
            {
                Calendar clickedDate = calendarDay.Calendar;

                // Get the day, month, and year
                int day = clickedDate.Get(CalendarField.DayOfMonth);
                int month = clickedDate.Get(CalendarField.Month) + 1; // Month is 0-based
                int year = clickedDate.Get(CalendarField.Year);

                // Your custom logic here
                Console.WriteLine($"User clicked on date: {year}-{month}-{day}");

                nativeCalendarView.SelectedDate = new DateTime(year, month, day);
            }

        }

    }
}
