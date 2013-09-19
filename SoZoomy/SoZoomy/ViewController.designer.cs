// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SoZoomy
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton memeButton { get; set; }

		[Outlet]
		SoZoomy.PreviewView previewView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISlider slider { get; set; }

		[Action ("meme:")]
		partial void meme (MonoTouch.Foundation.NSObject sender);

		[Action ("sliderChanged:")]
		partial void sliderChanged (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (memeButton != null) {
				memeButton.Dispose ();
				memeButton = null;
			}

			if (previewView != null) {
				previewView.Dispose ();
				previewView = null;
			}

			if (slider != null) {
				slider.Dispose ();
				slider = null;
			}
		}
	}
}
