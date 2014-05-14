using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using CoreAnimation;
using CoreGraphics;

namespace Example_CoreAnimation.Screens.iPad.LayerAnimation
{
	public partial class ImplicitAnimationScreen : UIViewController, IDetailView
	{
		public event EventHandler ContentsButtonClicked;

		protected CALayer imgLayer;

		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need
		// to be able to be created from a xib rather than from managed code
		public ImplicitAnimationScreen (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		[Export ("initWithCoder:")]
		public ImplicitAnimationScreen (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		public ImplicitAnimationScreen () : base ("ImplicitAnimationScreen", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
			Console.WriteLine ("Creating Layer");
			
			// create our layer and set it's frame
			imgLayer = CreateLayerFromImage ();
			imgLayer.Frame = new CGRect (200, 70, 114, 114);
			
			// add the layer to the layer tree so that it's visible
			View.Layer.AddSublayer (imgLayer);
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			btnContents.TouchUpInside += (sender, e) => {
				if (ContentsButtonClicked != null)
					ContentsButtonClicked (sender, e);
			};

			// anonymous delegate that runs when the btnAnimate button is clicked
			btnAnimate.TouchUpInside += (s, e) => {
				if (imgLayer.Frame.Y == 70) {
					imgLayer.Frame = new CGRect (new CGPoint (200, 270), imgLayer.Frame.Size);
					imgLayer.Opacity = 0.2f;
				} else {
					imgLayer.Frame = new CGRect (new CGPoint (200, 70), imgLayer.Frame.Size);
					imgLayer.Opacity = 1.0f;
				}
			};
		}

		// Ways to create a CALayer
		// Method 1: create a layer from an image
		protected CALayer CreateLayerFromImage ()
		{
			var layer = new CALayer ();
			layer.Contents = UIImage.FromBundle ("icon-114.png").CGImage;
			return layer;			
		}

		// Method 2: create a layer and assign a custom delegate that performs the drawing
		protected CALayer CreateLayerWithDelegate ()
		{
			var layer = new CALayer ();
			layer.Delegate = new LayerDelegate ();
			return layer;
		}

		public class LayerDelegate : CALayerDelegate
		{
			public override void DrawLayer (CALayer layer, CoreGraphics.CGContext context)
			{
				// implement your drawing
			}
		}
		// Method 3: Create a custom CALayer and override the appropriate methods
		public class MyCustomLayer : CALayer
		{
			public override void DrawInContext (CoreGraphics.CGContext ctx)
			{
				base.DrawInContext (ctx);
				// implement your drawing
			}
		}
	}
}

