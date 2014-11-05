using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using Example_Touch.Code;
using ObjCRuntime;

namespace Example_Touch.Screens.iPhone.GestureRecognizers
{
	public partial class CustomCheckmarkGestureRecognizer_iPhone : UIViewController
	{
		CheckmarkGestureRecognizer checkmarkGesture;
		protected bool isChecked = false;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public CustomCheckmarkGestureRecognizer_iPhone (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public CustomCheckmarkGestureRecognizer_iPhone (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public CustomCheckmarkGestureRecognizer_iPhone () : base("CustomCheckmarkGestureRecognizer_iPhone", null)
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
			
			imgCheckmark.Image = UIImage.FromBundle ("Images/CheckBox_Start.png");
			
			WireUpCheckmarkGestureRecognizer();
		}
		
		protected void WireUpCheckmarkGestureRecognizer()
		{
			// create the recognizer
			checkmarkGesture = new CheckmarkGestureRecognizer();
			// wire up the event handler
			checkmarkGesture.AddTarget ( () => {
				if (checkmarkGesture.State == (UIGestureRecognizerState.Recognized | UIGestureRecognizerState.Ended)) {

					if (isChecked)
						imgCheckmark.Image = UIImage.FromBundle ("Images/CheckBox_Unchecked.png");
					else
						imgCheckmark.Image = UIImage.FromBundle ("Images/CheckBox_Checked.png");
					isChecked = !isChecked;
				}
			});
			// add the gesture recognizer to the view
			View.AddGestureRecognizer (checkmarkGesture);
		}
	}
}

