using System;

using Foundation;
using ClockKit;
using UIKit;

namespace iOSApp.WatchAppExtension
{
	[Register("ComplicationController")]
	public class ComplicationController : CLKComplicationDataSource
	{
		public ComplicationController()
		{
		}

		public override void GetCurrentTimelineEntry(CLKComplication complication, Action<CLKComplicationTimelineEntry> handler)
		{
			// Call the handler with the current timeline entry
			Console.WriteLine("GetCurrentTimelineEntry");

			CLKComplicationTimelineEntry entry = null;
			try
			{
				var minutesPastHour = DateTime.Now.Minute.ToString();

				if (complication.Family == CLKComplicationFamily.ModularSmall)
				{
					var textTemplate1 = new CLKComplicationTemplateModularSmallSimpleText();
					textTemplate1.TextProvider = CLKSimpleTextProvider.FromText(minutesPastHour);
					entry = CLKComplicationTimelineEntry.Create(NSDate.Now, textTemplate1);

				}
				else if (complication.Family == CLKComplicationFamily.ModularLarge)
				{
					var textTemplate = new CLKComplicationTemplateModularLargeStandardBody();
					textTemplate.HeaderTextProvider = CLKSimpleTextProvider.FromText("Set " + minutesPastHour + " mins past", "XH", "```");

					textTemplate.Body1TextProvider = CLKSimpleTextProvider.FromText("Body text", "X1", "~~~");
					textTemplate.Body2TextProvider = CLKSimpleTextProvider.FromText("Body 2", "X2", "---");

					// if the user typed something in the app, show that in complication
					var c = NSUserDefaults.StandardUserDefaults["complication"];
					if (c != null) 
					{ 
						textTemplate.Body1TextProvider = CLKSimpleTextProvider.FromText("You typed:", "You:", "~~~");
						textTemplate.Body2TextProvider = CLKSimpleTextProvider.FromText(c.ToString(), c.ToString(), "---");
					}


					entry = CLKComplicationTimelineEntry.Create(NSDate.Now, textTemplate);

				}
				else if (complication.Family == CLKComplicationFamily.UtilitarianSmall)
				{
					var textTemplate = new CLKComplicationTemplateUtilitarianSmallFlat();
					textTemplate.TextProvider = CLKSimpleTextProvider.FromText(minutesPastHour + "m");
					entry = CLKComplicationTimelineEntry.Create(NSDate.Now, textTemplate);

				}
				else if (complication.Family == CLKComplicationFamily.UtilitarianLarge)
				{
					var textTemplate = new CLKComplicationTemplateUtilitarianLargeFlat();
					textTemplate.TextProvider = CLKSimpleTextProvider.FromText(minutesPastHour + " past hour");
					entry = CLKComplicationTimelineEntry.Create(NSDate.Now, textTemplate);

				}
				else if (complication.Family == CLKComplicationFamily.CircularSmall)
				{
					var ringTemplate = new CLKComplicationTemplateCircularSmallRingText();
					ringTemplate.TextProvider = CLKSimpleTextProvider.FromText(minutesPastHour);
					ringTemplate.FillFraction = DateTime.Now.Minute / 60f;
					entry = CLKComplicationTimelineEntry.Create(NSDate.Now, ringTemplate);
				}
				else
				{
					Console.WriteLine("GetCurrentTimelineEntry: Complication not supported (" + complication.Family + ")");
				}
			}
			catch (Exception x)
			{
				Console.WriteLine("Exception " + x);
			}
			handler(entry);
		}

		public override void GetPlaceholderTemplate(CLKComplication complication, Action<CLKComplicationTemplate> handler)
		{
			// This method will be called once per supported complication, and the results will be cached
			Console.WriteLine("GetPlaceholderTemplate for " + complication);

			CLKComplicationTemplate template = null;

			if (complication.Family == CLKComplicationFamily.ModularSmall)
			{
				var textTemplate = new CLKComplicationTemplateModularSmallSimpleText();
				textTemplate.TextProvider = CLKSimpleTextProvider.FromText("X1");
				template = textTemplate;

			}
			else if (complication.Family == CLKComplicationFamily.ModularLarge)
			{
				var textTemplate = new CLKComplicationTemplateModularLargeStandardBody();
				textTemplate.HeaderTextProvider = CLKSimpleTextProvider.FromText("Header A", "A 2", "~~~");
				textTemplate.Body1TextProvider = CLKSimpleTextProvider.FromText("Body B", "B 2", "~~~");
				template = textTemplate;

			}
			else if (complication.Family == CLKComplicationFamily.UtilitarianSmall)
			{
				var textTemplate = new CLKComplicationTemplateUtilitarianSmallFlat();
				textTemplate.TextProvider = CLKSimpleTextProvider.FromText("XAM2");
				template = textTemplate;

			}
			else if (complication.Family == CLKComplicationFamily.UtilitarianLarge)
			{
				var textTemplate = new CLKComplicationTemplateUtilitarianLargeFlat();
				textTemplate.TextProvider = CLKSimpleTextProvider.FromText("XAM3");
				template = textTemplate;

			}
			else if (complication.Family == CLKComplicationFamily.CircularSmall)
			{
				var textTemplate = new CLKComplicationTemplateCircularSmallRingText();
				textTemplate.TextProvider = CLKSimpleTextProvider.FromText("s");
				template = textTemplate;
			}
			else
			{
				Console.WriteLine("GetPlaceholderTemplate: Complication not supported (" + complication + ")");
			}

			handler(template);
		}

		public override void GetSupportedTimeTravelDirections(CLKComplication complication, Action<CLKComplicationTimeTravelDirections> handler)
		{
			// Retrieves the time travel directions supported by your complication
			Console.WriteLine("GetSupportedTimeTravelDirections");
			handler(CLKComplicationTimeTravelDirections.None);
		}
	}
}

