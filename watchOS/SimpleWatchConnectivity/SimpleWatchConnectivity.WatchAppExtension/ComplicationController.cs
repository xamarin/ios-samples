
// The complication simply supports the Modular Large (tall body) family and
// shows a random number for the current timeline entry.
// You can make the complication current by following these steps:
// 1. Choose a Modular watch face on your watch.
// 2. Deep press to get to the customization screen, tap the Customize button,
//    then swipe right to get to the complications configuration screen and tap the tall body area.
// 3. Rotate the digital crown to choose the SimpleWatchConnectivity complication.
// 4. Press the digital crown and tap the screen to go back to the watch face.

namespace SimpleWatchConnectivity.WatchAppExtension
{
    using ClockKit;
    using Foundation;
    using System;

    /// <summary>
    /// The complication controller class for the complication.
    /// </summary>
    public class ComplicationController : CLKComplicationDataSource
    {
        #region Timeline Configuration.

        public override void GetSupportedTimeTravelDirections(CLKComplication complication, Action<CLKComplicationTimeTravelDirections> handler)
        {
            handler(CLKComplicationTimeTravelDirections.Forward | CLKComplicationTimeTravelDirections.Backward);
        }

        public override void GetTimelineStartDate(CLKComplication complication, Action<NSDate> handler)
        {
            handler(null);
        }

        public override void GetTimelineEndDate(CLKComplication complication, Action<NSDate> handler)
        {
            handler(null);
        }

        public override void GetPrivacyBehavior(CLKComplication complication, Action<CLKComplicationPrivacyBehavior> handler)
        {
            handler(CLKComplicationPrivacyBehavior.ShowOnLockScreen);
        }

        #endregion

        #region Timeline Population

        public override void GetCurrentTimelineEntry(CLKComplication complication, Action<CLKComplicationTimelineEntry> handler)
        {
            // Only support .modularLarge currently.
            if (complication.Family == CLKComplicationFamily.ModularLarge)
            {
                var random = new Random();

                // Display a random number string on the body.
                var tallBody = new CLKComplicationTemplateModularLargeTallBody
                {
                    HeaderTextProvider = CLKSimpleTextProvider.FromText("SimpleWC"),
                    BodyTextProvider = CLKSimpleTextProvider.FromText($"{random.Next(400)}")
                };

                // Pass the entry to ClockKit.
                var entry = CLKComplicationTimelineEntry.Create(new NSDate(), tallBody);
                handler(entry);
            }
            else
            {
                handler(null);
            }
        }

        public override void GetTimelineEntriesBeforeDate(CLKComplication complication, NSDate beforeDate, nuint limit, Action<CLKComplicationTimelineEntry[]> handler)
        {
            // Call the handler with the timeline entries prior to the given date.
            handler(null);
        }

        public override void GetTimelineEntriesAfterDate(CLKComplication complication, NSDate afterDate, nuint limit, Action<CLKComplicationTimelineEntry[]> handler)
        {
            // Call the handler with the timeline entries after to the given date.
            handler(null);
        }

        #endregion

        #region Placeholder Templates.

        public override void GetLocalizableSampleTemplate(CLKComplication complication, Action<CLKComplicationTemplate> handler)
        {
            // This method will be called once per supported complication, and the results will be cached.
            handler(null);
        }

        public override void GetPlaceholderTemplate(CLKComplication complication, Action<CLKComplicationTemplate> handler)
        {
            // Only support .modularLarge currently.
            if (complication.Family == CLKComplicationFamily.ModularLarge)
            {
                // Display a random number string on the body.
                var tallBody = new CLKComplicationTemplateModularLargeTallBody
                {
                    HeaderTextProvider = CLKSimpleTextProvider.FromText("SimpleWC"),
                    BodyTextProvider = CLKSimpleTextProvider.FromText("Random")
                };

                // Pass the template to ClockKit.
                handler(tallBody);
            }
            else
            {
                handler(null);
            }
        }

        #endregion
    }
}
