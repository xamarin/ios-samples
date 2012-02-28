
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace Example_StandardControls.Screens.iPhone.TextFields
{
	public partial class TextFields_iPhone : UIViewController
	{
		UITextField textField;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public TextFields_iPhone (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public TextFields_iPhone (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public TextFields_iPhone () : base("TextFields_iPhone", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			this.Title = "UITextField";
			
			textField = new UITextField (new RectangleF (20, 150, 280, 33));
			textField.Font = UIFont.FromName ("Helvetica-Bold", 20);
			textField.BorderStyle = UITextBorderStyle.Bezel;
			textField.Placeholder = "edit me!";
			
			this.View.AddSubview (textField);
		}
		
	}
}

