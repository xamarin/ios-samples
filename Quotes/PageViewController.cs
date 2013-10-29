using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Quotes
{
	public partial class PageViewController : UIViewController
	{
		public Page controllerPage { get; set; }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			pageView.UnstyledDrawing = !NSUserDefaults.StandardUserDefaults.BoolForKey ("DrawWithStyle");

			pageView.SetPage (controllerPage);

			if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0)) { 
				this.EdgesForExtendedLayout = UIRectEdge.None;
			}
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			NSUserDefaults.StandardUserDefaults.SetBool (!pageView.UnstyledDrawing, "DrawWithStyle");
		}

		public override string Title {
			get {
				return controllerPage.Title;
			}
			set {
				base.Title = value;
			}
		}

		partial void ParagraphSelected (MonoTouch.UIKit.UILongPressGestureRecognizer sender)
		{
			pageView.SelectParagraphAtPosition (sender.LocationInView (pageView), 
			                                    sender.State == MonoTouch.UIKit.UIGestureRecognizerState.Ended);
		}
		
		partial void DrawingModeToggled (MonoTouch.UIKit.UISwipeGestureRecognizer sender)
		{
			pageView.UnstyledDrawing = !pageView.UnstyledDrawing;
			pageView.UpdatePage ();
		}
		
		partial void MenuDismissed (MonoTouch.UIKit.UITapGestureRecognizer sender)
		{
			pageView.SelectParagraphAtPosition (new System.Drawing.PointF (-100.0f, -100.0f), false);
		}
	
		partial void LineHeightChanged (MonoTouch.UIKit.UISlider sender)
		{
			pageView.SetLineHeight (sender.Value);
		}
	}
}