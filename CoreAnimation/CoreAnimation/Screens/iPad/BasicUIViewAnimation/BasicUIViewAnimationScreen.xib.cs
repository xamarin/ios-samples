
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_CoreAnimation.Screens.iPad.BasicUIViewAnimation
{
	public partial class BasicUIViewAnimationScreen : UIViewController, IDetailView
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public BasicUIViewAnimationScreen (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public BasicUIViewAnimationScreen (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public BasicUIViewAnimationScreen () : base("BasicUIViewAnimationScreen", null)
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
			
			btnClickMe.TouchUpInside += (s, e) => {
				
				
				//==== OLD WAY
//				// begin our animation block. the name allows us to refer to it later
//				UIView.BeginAnimations("ImageMove");
//				
//				// move the image one way or the other
//				if(imgToAnimate.Frame.Y == 64)
//				{
//					imgToAnimate.Frame = new System.Drawing.RectangleF(
//						imgToAnimate.Frame.X, imgToAnimate.Frame.Y + 400,
//						imgToAnimate.Frame.Size.Width, imgToAnimate.Frame.Size.Height);
//				}
//				else
//				{
//					imgToAnimate.Frame = new System.Drawing.RectangleF(
//						imgToAnimate.Frame.X, imgToAnimate.Frame.Y - 400,
//						imgToAnimate.Frame.Size.Width, imgToAnimate.Frame.Size.Height);
//				}
//				
//				// finish our animation block
//				UIView.CommitAnimations();
				
				
				//===== NEW WAY
				
				// basic prototypes:
				//UIView.Animate(0.2, () => { /* code to animate */ });
				//UIView.Animate(0.2, delegate() { /* code to animate */ });
				UIView.Animate(0.2, () => {
					// move the image one way or the other
					if(imgToAnimate.Frame.Y == 64) {
						imgToAnimate.Frame = new System.Drawing.RectangleF (
							imgToAnimate.Frame.X, imgToAnimate.Frame.Y + 400,
							imgToAnimate.Frame.Size.Width, imgToAnimate.Frame.Size.Height);
					}
					else {
						imgToAnimate.Frame = new System.Drawing.RectangleF (
							imgToAnimate.Frame.X, imgToAnimate.Frame.Y - 400,
							imgToAnimate.Frame.Size.Width, imgToAnimate.Frame.Size.Height);
					}					
				});
			};
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		public void AddContentsButton (UIBarButtonItem button)
		{
			button.Title = "Contents";
			tlbrMain.SetItems(new UIBarButtonItem[] { button }, false );
		}
		
		public void RemoveContentsButton ()
		{
			tlbrMain.SetItems(new UIBarButtonItem[0], false);
		}

	}
}

