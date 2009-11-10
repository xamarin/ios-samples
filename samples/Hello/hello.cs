using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

[Register]
public class AppController : UIApplicationDelegate {
	UIWindow window;
	UITextField text, text2;
	UILabel label;
	UIImageView  image;

	//
	// Our ViewController exists to get the touch events
	//
	[Register]
	public class ViewController : UIViewController {
		AppController app;
		
		public ViewController (AppController app)
		{
			this.app = app;
		}
		
		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			app.text.ResignFirstResponder ();
			app.text.Text = "Tapped";
		}
	}

	//
	// A dummy empty function, that hooks up to the "onTextChange", seems
	// like this is the only way of being able to get the endediting event
	// on the UITextField
	//
	[Export ("onTextChange")]
	public void OnTextChange (NSObject sender)
	{
		Console.WriteLine ("ON TEXT CHANGE");
		
	}
	
	public override bool FinishedLaunching (UIApplication app, NSDictionary options)
	{
		UIApplication.SharedApplication.StatusBarHidden = true;
		
		image = new UIImageView (UIScreen.MainScreen.Bounds) {
			Image = UIImage.FromFile ("Background.png")
		};
		text = new UITextField (new RectangleF (44, 32, 232, 31)) {
			BorderStyle = UITextBorderStyle.RoundedRect,
			TextColor = UIColor.Black,
			BackgroundColor = UIColor.Black,
			ClearButtonMode = UITextFieldViewMode.WhileEditing,
			Placeholder = "Hello world",
		};
		text.ShouldReturn = delegate (UITextField theTextfield) {
			text.ResignFirstResponder ();
			
			label.Text = text.Text;
			return true;
		};

		label = new UILabel (new RectangleF (20, 120, 280, 44)){
			TextColor = UIColor.Gray,
			BackgroundColor = UIColor.Black,
			Text = text.Placeholder
		};

		var vc = new ViewController (this) { image, text, label };
			
		window = new UIWindow (UIScreen.MainScreen.Bounds){ vc.View };

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
		UIApplication.Main (args, null, "AppController");
		Console.WriteLine ("Returning from Main, this sucks");
	}		
}
