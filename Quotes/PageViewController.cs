using System;
using CoreGraphics;

using Foundation;
using UIKit;

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

		partial void ParagraphSelected (UIKit.UILongPressGestureRecognizer sender)
		{
			pageView.SelectParagraphAtPosition (sender.LocationInView (pageView), 
			                                    sender.State == UIKit.UIGestureRecognizerState.Ended);
		}
		
		partial void DrawingModeToggled (UIKit.UISwipeGestureRecognizer sender)
		{
			pageView.UnstyledDrawing = !pageView.UnstyledDrawing;
			pageView.UpdatePage ();
		}
		
		partial void MenuDismissed (UIKit.UITapGestureRecognizer sender)
		{
			pageView.SelectParagraphAtPosition (new CoreGraphics.CGPoint (-100.0f, -100.0f), false);
		}
	
		partial void LineHeightChanged (UIKit.UISlider sender)
		{
			pageView.SetLineHeight (sender.Value);
		}
	}
}