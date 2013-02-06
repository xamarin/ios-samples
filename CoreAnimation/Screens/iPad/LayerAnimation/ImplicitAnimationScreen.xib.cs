
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreAnimation;
using System.Drawing;

namespace Example_CoreAnimation.Screens.iPad.LayerAnimation
{
	public partial class ImplicitAnimationScreen : UIViewController, IDetailView
	{
		protected CALayer imgLayer;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public ImplicitAnimationScreen (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public ImplicitAnimationScreen (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public ImplicitAnimationScreen () : base("ImplicitAnimationScreen", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
			Console.WriteLine("Creating Layer");
			
			// create our layer and set it's frame
			imgLayer = this.CreateLayerFromImage();
			imgLayer.Frame = new RectangleF (200, 70, 114, 114);
			
			// add the layer to the layer tree so that it's visible
			this.View.Layer.AddSublayer (imgLayer);
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			
			// anonymous delegate that runs when the btnAnimate button is clicked
			this.btnAnimate.TouchUpInside += (s, e) => {
			
				// if you want to override the animation duration, you can:
				//this.imgToAnimate.Layer.Duration = 1.0;
			
				if(imgLayer.Frame.Y == 70)
				{
					 imgLayer.Frame = new RectangleF (new PointF (200, 270), imgLayer.Frame.Size);
					 imgLayer.Opacity = 0.2f;
				}
				else
				{
					 imgLayer.Frame = new RectangleF (new PointF (200, 70), imgLayer.Frame.Size);
					 imgLayer.Opacity = 1.0f;
				}
			};
		}
		
		
		//==== Ways to create a CALayer
		
		//==== Method 1: create a layer from an image
		protected CALayer CreateLayerFromImage ()
		{
			CALayer layer = new CALayer ();
			layer.Contents = UIImage.FromBundle ("icon-114.png").CGImage;
			return layer;			
		}
		
		//==== Method 2: create a layer and assign a custom delegate that performs the drawing
		protected CALayer CreateLayerWithDelegate ()
		{
			CALayer layer = new CALayer ();
			layer.Delegate = new LayerDelegate ();
			return layer;
		}
		
		public class LayerDelegate : CALayerDelegate
		{
			public override void DrawLayer (CALayer layer, MonoTouch.CoreGraphics.CGContext context)
			{
				// implement your drawing
			}
		}
		
		//===== Method 3: Create a custom CALayer and override the appropriate methods
		public class MyCustomLayer : CALayer
		{
			public override void DrawInContext (MonoTouch.CoreGraphics.CGContext ctx)
			{
				base.DrawInContext (ctx);
				// implement your drawing
			}			
		}
		
		/// <summary>
		/// 
		/// </summary>
		public void AddContentsButton (UIBarButtonItem button)
		{
			button.Title = "Contents";
			this.tlbrMain.SetItems (new UIBarButtonItem[] { button }, false );
		}
		
		public void RemoveContentsButton ()
		{
			this.tlbrMain.SetItems (new UIBarButtonItem[0], false);
		}

	}
}

