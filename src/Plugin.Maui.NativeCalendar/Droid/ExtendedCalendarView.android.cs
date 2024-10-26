using Android.Graphics;
using Android.Widget;
using Plugin.Maui.NativeCalendar.Extensions;

namespace Plugin.Maui.NativeCalendar.Droid
{
    public class ExtendedCalendarView : CalendarView
    {
        public IEnumerable<NativeCalendarEvent> Events
        {
            get => events;
            set
            {
                events = value;
                Invalidate();
            }
        }

        public Android.Graphics.Color EventIndicatorColor
        {
            get => eventPaint.Color;
            set
            {
                eventPaint.Color = value;
                Invalidate();
            }
        }

        private Android.Graphics.Paint eventPaint;
        private IEnumerable<NativeCalendarEvent> events = Enumerable.Empty<NativeCalendarEvent>();

        private const int NUM_COLUMNS = 7;
        private const int NUM_ROWS = 6;

        public ExtendedCalendarView(Android.Content.Context context) : base(context)
        {

            // Set default value for the event indicator color
            eventPaint = new Android.Graphics.Paint
            {
                Color = Android.Graphics.Color.Red,
                AntiAlias = true
            };

            eventPaint.SetStyle(Android.Graphics.Paint.Style.Fill);

        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            int width = Width;
            int height = Height;
            int cellWidth = width / NUM_COLUMNS;
            int cellHeight = height / NUM_ROWS;

            // Custom drawing for event indicators
            foreach (var date in events)
            {
                if (date.StartDate.Date.ToLongInteger() == this.Date)
                {
                    int day = date.StartDate.Day;
                    int column = (day - 1) % NUM_COLUMNS;
                    int row = (day - 1) / NUM_COLUMNS;

                    float x = (column * cellWidth) + (cellWidth / 2f);
                    float y = (row * cellHeight) + (cellHeight * 0.8f);

                    canvas.DrawCircle(x, y, 10, eventPaint);
                }
            }
        }
    }
}
