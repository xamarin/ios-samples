using System;
using UIKit;
using MonoTouch.Dialog;
using System.Collections.Generic;

namespace PaintCode
{
	public class NewsDialogViewController : DialogViewController
	{
		List<Tuple<DateTime,string>> newsItems = new List<Tuple<DateTime, string>> {
			new Tuple<DateTime, string> (new DateTime(2012,07,04), "Fireworks"),
			new Tuple<DateTime, string> (new DateTime(2012,05,01), "S-O-S"),
			new Tuple<DateTime, string> (new DateTime(2012,02,29), "Is it a leap year?")
		};

		public NewsDialogViewController () : base (UITableViewStyle.Plain, null)
		{
			View.BackgroundColor = UIColor.White;
			TableView.BackgroundColor = UIColor.White;

			var section = new Section ();
			// creates the rows using MT.Dialog
			
			foreach (var item in newsItems) {
				var published = item.Item1;
				var image = CustomBadgeElement.MakeCalendarBadge (
					                                  published.ToString ("MMM").ToUpper ()
													, published.ToString ("dd"));
				var badgeRow = new BadgeElement (image, item.Item2);

				badgeRow.Tapped += () => {
					var dv = new DrawingViewController ();
					NavigationController.PushViewController (dv, true);
				};
			 	section.Add (badgeRow);
			}
			Root = new RootElement ("PaintCode examples") { section };
		}
	}
}