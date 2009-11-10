using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System;
using System.Drawing;

[Register]
public class DetailViewController : UIViewController {

	[Connect]
	public UILabel itemTitle { 
		get {
			return (UILabel) GetNativeField ("itemTitle");
		}
		set {
			SetNativeField ("itemTitle", value);
		}
	}
	
	[Connect]
	public UIImageView checkedImage { 
		get {
			return (UIImageView) GetNativeField ("checkedImage");
		}
		set {
			SetNativeField ("checkedImage", value);
		}
	}
	
	public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation interfaceOrientation) {
		return true; 
	}
	
	public DetailViewController(string nibName, NSBundle bundle) : base(nibName, bundle) {
		
	}
	
	public DetailViewController (IntPtr handle) : base (handle) {
		
	}
	
}