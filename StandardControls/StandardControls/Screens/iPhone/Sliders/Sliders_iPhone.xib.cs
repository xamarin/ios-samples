
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace Example_StandardControls.Screens.iPhone.Sliders
{
	public partial class Sliders_iPhone : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public Sliders_iPhone (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public Sliders_iPhone (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public Sliders_iPhone () : base("Sliders_iPhone", null)
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
			
			this.Title = "Sliders";
			
			this.sldrWithImages.SetThumbImage(UIImage.FromFile("Images/Icons/29_icon.png"), UIControlState.Normal);

		}
		
	}
}

