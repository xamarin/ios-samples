
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace Example_StandardControls.Screens.iPhone.PagerControl
{
	public partial class PagerControl_iPhone : UIViewController
	{
		/// <summary>
		/// A list of all our controllers that hold the views for our pages
		/// </summary>
		List<UIViewController> controllers = new List<UIViewController>();
		
		#region Constructors
	
		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code
		public PagerControl_iPhone (IntPtr handle) : base(handle) {}
	
		[Export("initWithCoder:")]
		public PagerControl_iPhone (NSCoder coder) : base(coder) {}
	
		public PagerControl_iPhone () : base("PagerControl_iPhone", null) {}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// set our title
			Title = "Pager Control";
			
			// wire up our pager and scroll view event handlers
			pgrMain.ValueChanged += HandlePgrMainValueChanged;
			scrlMain.Scrolled += HandleScrlMainScrolled;
			
			// load our controllers (we'll use one per page)
			LoadControllers ();
			
		}
	
		/// <summary>
		/// Runs when the scroll view is scrolled. Updates the pager control so that it's 
		/// current, based on the page
		/// </summary>
		protected void HandleScrlMainScrolled (object sender, EventArgs e)
		{
			// calculate the page number
			int pageNumber = (int)(Math.Floor ((scrlMain.ContentOffset.X - scrlMain.Frame.Width / 2) / scrlMain.Frame.Width) + 1);
		
			// if it's a valid page
			if (pageNumber >= 0 && pageNumber < controllers.Count)
			{
				// Set the current page on the pager control
				pgrMain.CurrentPage = pageNumber;
			}
		}
		
		/// <summary>
		/// Runs when a dot on the pager is clicked. Scrolls the scroll view to the appropriate
		/// page, based on which one was clicked
		/// </summary>
		protected void HandlePgrMainValueChanged (object sender, EventArgs e)
		{
			// it moves page by page. we scroll right to the next controller
			scrlMain.ScrollRectToVisible (controllers[(sender as UIPageControl).CurrentPage].View.Frame, true);
		}
			
		/// <summary>
		/// Loads our controllers that we'll use for each page. Also sets the content size
		/// of the scroll view based on the number of controllers we add
		/// </summary>
		protected void LoadControllers ()
		{
			// instantiate and add the controllers to our list
			controllers.Add (new Controller_1 ());
			controllers.Add (new Controller_2 ());
			controllers.Add (new Controller_3 ());
			
			// loop through each one
			for (int i = 0; i < controllers.Count; i++)
			{
				// set their location and size, each one is moved to the 
				// right by the width of the previous
				RectangleF viewFrame = new RectangleF (
					scrlMain.Frame.Width * i,
					scrlMain.Frame.Y,
					scrlMain.Frame.Width,
					scrlMain.Frame.Height);
				controllers[i].View.Frame = viewFrame;
		
				// add the view to the scrollview
				scrlMain.AddSubview (controllers[i].View);
			}
			
			// set our scroll view content size (width = number of pages * scroll view width)
			scrlMain.ContentSize = new SizeF (
				scrlMain.Frame.Width * controllers.Count, scrlMain.Frame.Height);
		}
			
	}
}

