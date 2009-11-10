using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

[Register]
public class MyViewController : UIViewController {
	HelloWorldAppDelegate app;
	
	[Connect]
	public UIView view {
		get { return (UIView) GetNativeField ("view"); }
		set { SetNativeField ("view", value); }
	}

	[Connect]
	public UILabel label {
		get { return (UILabel) GetNativeField ("label"); }
		set { SetNativeField ("label", value); }
	}
	
	[Connect]
	public UITextField textField {
		get { return (UITextField) GetNativeField ("textField"); }
		set { SetNativeField ("textField", value); }
	}
		
	public MyViewController (string nibName, NSBundle bundle, HelloWorldAppDelegate app) : base (nibName, bundle)
	{
		this.app = app;
	}
	
	public override void ViewDidLoad ()
	{
		textField.ClearButtonMode = UITextFieldViewMode.WhileEditing;
		label.Text = textField.Placeholder;
		textField.ShouldReturn += delegate {
			textField.ResignFirstResponder ();
			label.Text = textField.Text;
			return true;
		};
	}
	
	public override void TouchesBegan (NSSet touches, UIEvent evt)
	{
		textField.ResignFirstResponder ();
		textField.Text = "Tapped";
		base.TouchesBegan (touches, evt);
	}
}

[Register]
public class HelloWorldAppDelegate : UIApplicationDelegate {
        [Connect]                                                                                                                            
        public UIWindow window {
                get { return (UIWindow) GetNativeField ("window"); }
                set { SetNativeField ("window", value); }
        }

	public override bool FinishedLaunching (UIApplication app, NSDictionary options)
	{
		var window = (UIWindow) GetNativeField ("window");
		
		MyViewController controller = new MyViewController ("HelloWorld", NSBundle.MainBundle, this);
		UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.BlackOpaque;
		window.AddSubview (controller.View);
		window.MakeKeyAndVisible ();

		return true;
	}

	public override void OnActivated (UIApplication application)
	{
		Console.WriteLine ("OnActivated");
	}
	
}

class Demo {
	static void Main (string [] args)
	{
		Console.WriteLine ("Launching");
		UIApplication.Main (args);
		Console.WriteLine ("Returning from Main, this sucks");
	}		
}
