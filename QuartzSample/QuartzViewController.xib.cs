using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

using CoreGraphics;

namespace QuartzSample
{
	public abstract class QuartzView : UIView 
	{
		public abstract void DrawInContext (CGContext context);
					       
		public override void Draw (CGRect rect)
		{
			using (var ctxt = UIGraphics.GetCurrentContext ()) {
				DrawInContext ( ctxt );
			}
		}
	}

	
	public partial class QuartzViewController : UIViewController
	{
		#region Constructors
		
		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code
		
		public QuartzViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		[Export ("initWithCoder:")]
		public QuartzViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		[Export ("initWithNibName:bundle:")]
		public QuartzViewController (string nib, NSBundle bundle) : base (nib, bundle)
		{
		}
			

		public QuartzViewController () : base ("QuartzViewController", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		public QuartzViewController (CreateView creator, string title, string info) : this (creator, "QuartzViewController", title, info)
		{
		}
		
		public QuartzViewController (CreateView creator, string nib, string title, string info) : this (nib, null)
		{
			this.create = creator;
			DemoTitle = title;
			DemoInfo = info;
		}
		
		#endregion
		
		public delegate QuartzView CreateView ();
		CreateView create;
	
		public string DemoTitle;
		public string DemoInfo;
	
		public QuartzView quartzView;
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			quartzView = create ();
			scrollView.AddSubview (quartzView);
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			
			// Animated this property, use the SetStatusBarStyle instead of the property.
			UIApplication.SharedApplication.SetStatusBarStyle (UIStatusBarStyle.BlackOpaque, true);
	
			// Reset scroll view to 1.0 zoom
			scrollView.ZoomScale = 1.0f;
	
			quartzView.Frame = scrollView.Bounds;
			scrollView.ContentSize = new CGSize (scrollView.Bounds.Width, scrollView.Bounds.Height);
		}
		
		[Export ("viewForZoomingInScrollView:")]
		public UIView ViewForZoomingInScrollView (UIScrollView v)
		{
			return quartzView;
		}

	}
}

