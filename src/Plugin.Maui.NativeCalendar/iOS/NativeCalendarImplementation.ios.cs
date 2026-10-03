using CoreGraphics;
using Foundation;
using Microsoft.Maui.Platform;
using Plugin.Maui.NativeCalendar.iOS;
using UIKit;

namespace Plugin.Maui.NativeCalendar
{
    public class NativeCalendarImplementation : UIView
    {
        private readonly UICalendarView calendarView;

        private NSDate MaxDate { get; set; } = NSDate.DistantFuture;
        private NSDate MinDate { get; set; } = NSDate.DistantPast;

        private NativeCalendarView nativeCalendarView;

        private readonly CalendarSelectionSingleDateDelegate calendarSelectionSingleDateDelegate;

        public NativeCalendarImplementation(NativeCalendarView nativeCalendarView)
        {
            // only add a calendar on iOS 16.0 or later
            if (UIDevice.CurrentDevice.CheckSystemVersion(16, 0))
            {
                calendarView = new UICalendarView();
                calendarView.Calendar = new NSCalendar(NSCalendarType.Gregorian);

                calendarSelectionSingleDateDelegate = new CalendarSelectionSingleDateDelegate(nativeCalendarView);

                calendarView.SelectionBehavior = new UICalendarSelectionSingleDate(calendarSelectionSingleDateDelegate);

                // Enable Auto Layout
                calendarView.TranslatesAutoresizingMaskIntoConstraints = false;

                AddSubview(calendarView);

                // Set constraints to fill the parent view
                NSLayoutConstraint.ActivateConstraints(new[]
                {
                    calendarView.LeadingAnchor.ConstraintEqualTo(this.LeadingAnchor),
                    calendarView.TrailingAnchor.ConstraintEqualTo(this.TrailingAnchor),
                    calendarView.TopAnchor.ConstraintEqualTo(this.TopAnchor),
                    calendarView.BottomAnchor.ConstraintEqualTo(this.BottomAnchor)
                });

                // TODO: is this needed? it seems in iOS the background color bleeds through
                if(nativeCalendarView.BackgroundColor != null)  
                    calendarView.BackgroundColor = nativeCalendarView.BackgroundColor.ToPlatform();

                // Set the delegate for calendarView
                calendarView.Delegate = new CalendarViewDelegate(nativeCalendarView.Events, GetEventIndicatorColor(nativeCalendarView));

            }
            else
            {
                throw new PlatformNotSupportedException("iOS 16.0 or later is required to use the NativeCalendarView");
            }

            this.nativeCalendarView = nativeCalendarView;
        }

        // Report the calendar's natural size so MAUI can auto-size the control. Without this the
        // wrapping UIView has no intrinsic content size, so the control collapses unless the caller
        // sets an explicit HeightRequest. The height depends on the width, so honor the constraint.
        public override CGSize SizeThatFits(CGSize size)
        {
            if (calendarView is not null)
                return calendarView.SizeThatFits(size);

            return base.SizeThatFits(size);
        }

        public void UpdateTintColor(NativeCalendarView nativeCalendarView)
        {
            // A null TintColor gives the calendar the tint of its parent view again.
            calendarView.TintColor = nativeCalendarView.TintColor?.ToPlatform();

            // The event indicator uses the tint when it has no color of its own.
            if (nativeCalendarView.EventIndicatorColor is null)
                UpdateEvents(nativeCalendarView);
        }

        private UIColor GetEventIndicatorColor(NativeCalendarView nativeCalendarView)
        {
            return nativeCalendarView.EventIndicatorColor?.ToPlatform()
                ?? nativeCalendarView.TintColor?.ToPlatform()
                ?? calendarView.TintColor
                ?? UIColor.SystemBlue;
        }

        public void UpdateSelectedDate(NativeCalendarView nativeCalendarView)
        {
            // Create NSDateComponents from the DateTime
            var dateTime = nativeCalendarView.SelectedDate;

            NSDateComponents dateComponents = new NSDateComponents
            {
                Year = dateTime.Year,
                Month = dateTime.Month,
                Day = dateTime.Day
            };

            // Cast to UICalendarSelectionSingleDate and set the SelectedDate
            if (calendarView.SelectionBehavior is UICalendarSelectionSingleDate singleDateSelection)
            {
                singleDateSelection.SelectedDate = dateComponents;
            }
        }

        public void UpdateMaximumDate(NativeCalendarView nativeCalendarView)
        {
            UpdateAvailableDateRange(nativeCalendarView);
        }

        public void UpdateMinimumDate(NativeCalendarView nativeCalendarView)
        {
            UpdateAvailableDateRange(nativeCalendarView);
        }

        private void UpdateAvailableDateRange(NativeCalendarView nativeCalendarView)
        {
            var minimumDate = nativeCalendarView.MinimumDate.Date;
            var maximumDate = nativeCalendarView.MaximumDate.Date;

            var hasMinimum = minimumDate > DateTime.MinValue.Date;
            var hasMaximum = maximumDate < DateTime.MaxValue.Date;

            // NSDateInterval throws when the start is after the end. An inverted range has no
            // valid day, so show the calendar with no limits.
            if (hasMinimum && hasMaximum && minimumDate > maximumDate)
                hasMinimum = hasMaximum = false;

            // The range runs from the first second of the minimum day to the last second of the
            // maximum day, so that the two days are in the range.
            MinDate = hasMinimum ? ToNSDate(minimumDate) : NSDate.DistantPast;
            MaxDate = hasMaximum ? ToNSDate(maximumDate.AddDays(1).AddSeconds(-1)) : NSDate.DistantFuture;

            calendarView.AvailableDateRange = new Foundation.NSDateInterval(MinDate, MaxDate);
        }

        // The cast to NSDate throws for a DateTime with no kind. The dates of the calendar view
        // are local days.
        private static NSDate ToNSDate(DateTime dateTime)
        {
            return (NSDate)DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
        }

        public void UpdateEvents(NativeCalendarView nativeCalendarView)
        {
            // Remove and reset the delegate to force a decoration re-evaluation
            calendarView.Delegate = null;

            // TODO: is this enough?
            calendarView.Delegate = new CalendarViewDelegate(nativeCalendarView.Events, GetEventIndicatorColor(nativeCalendarView));

            // Trigger a layout update to redraw the decorations
            calendarView.SetNeedsLayout();
            calendarView.LayoutIfNeeded();
        }

    }
}
