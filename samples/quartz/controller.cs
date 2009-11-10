//
// Quartz Demos C#
//
// This class tries to mimic the pattern in the original sample, I would
// have made this different, but I tried to keep the spirit of the original
// program intact for reference.   In particular the creation on demand
// of the view seems like a strange choice, but for the sake of showing
// how this would work, I have kept it here.
//

using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;
using MonoTouch.CoreGraphics;

//
// A helper base class that fetches the context, and passes that to
// the various views.    
//
[Register]
public abstract class QuartzView : UIView {
	public abstract void DrawInContext (CGContext context);
				       
	public override void Draw (RectangleF rect)
	{
		DrawInContext (UIGraphics.GetCurrentContext ());
	}
}

[Register]
public partial class QuartzViewController : UIViewController {
	public delegate QuartzView CreateView ();
	CreateView create;

	public string DemoTitle;
	public string DemoInfo;

	public QuartzView quartzView;

	//
	// Required to allow derived classes
	//
	[Export ("initWithNibName:bundle:")]
	public QuartzViewController (string nib, NSBundle bundle) : base (nib, bundle)
	{
	}
		
	public QuartzViewController (CreateView creator, string title, string info) : this (creator, "DemoView", title, info)
	{
	}
	
	public QuartzViewController (CreateView creator, string nib, string title, string info) : this (nib, null)
	{
		this.create = creator;
		DemoTitle = title;
		DemoInfo = info;
	}

	//
	// The view loaded: create our QuartzView
	//
	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();
		quartzView = create ();
		scrollView.AddSubview (quartzView);
	}

	public override void ViewWillAppear (bool animated)
	{
		// Animated this property, use the SetStatusBarStyle instead of the property.
		UIApplication.SharedApplication.SetStatusBarStyle (UIStatusBarStyle.BlackOpaque, true);

		// Reset scroll view to 1.0 zoom
		scrollView.ZoomScale = 1.0f;

		quartzView.Frame = scrollView.Bounds;
		scrollView.ContentSize = new SizeF (scrollView.Bounds.Width, scrollView.Bounds.Height);
	}

	//
	// This is the only method from the UIScrollView delegate defined in DemoView.xib that
	// we implement.
	//
	[Export ("viewForZoomingInScrollView:")]
	public UIView ViewForZoomingInScrollView (UIScrollView v)
	{
		return quartzView;
	}
		
}

