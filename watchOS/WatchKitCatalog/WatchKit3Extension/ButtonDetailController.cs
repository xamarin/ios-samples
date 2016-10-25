/*
 * This controller displays buttons and shows use of groups within buttons. 
 * This also demonstrates how to hide and show UI elements at runtime.
*/

using System;

using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	public partial class ButtonDetailController : WKInterfaceController
	{
		bool hidden;
		bool alpha;

		public ButtonDetailController ()
		{
			hidden = false;
			alpha = true;
		}

		public override void WillActivate ()
		{
			// This method is called when the controller is about to be visible to the wearer.
			Console.WriteLine ("{0} will activate", this);
		}

		public override void DidDeactivate ()
		{
			// This method is called when the controller is no longer visible.
			Console.WriteLine ("{0} did deactivate", this);
		}

		partial void HideAndShow (NSObject obj)
		{
			placeholderButton.SetHidden (!hidden);
			hidden = !hidden;
		}

		partial void ChangeAlpha (NSObject obj)
		{
			placeholderButton.SetAlpha (alpha ? 0f : 1f);
			alpha = !alpha;
		}
	}
}