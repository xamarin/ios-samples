using System;
using UIKit;
using CoreGraphics;


namespace Example_Drawing.Screens.iPad.Transformations
{
	public class Controller : UIViewController
	{
		UIImageView imageView;
		
		UIButton btnUp;
		UIButton btnRight;
		UIButton btnDown;
		UIButton btnLeft;
		UIButton btnReset;
		UIButton btnRotateLeft;
		UIButton btnRotateRight;
		UIButton btnScaleUp;
		UIButton btnScaleDown;
		
		float currentScale, initialScale = 1.0f;
		CGPoint currentLocation, initialLocation = new CGPoint(380, 500);
		float currentRotation , initialRotation = 0;
		float movementIncrement = 20;
		float rotationIncrement = (float)(Math.PI * 2 / 16);
		float scaleIncrement = 1.5f;


		
		#region -= constructors =-

		public Controller () : base()
		{
			currentScale = initialScale;
			currentLocation = initialLocation;
			currentRotation = initialRotation;
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// set the background color of the view to white
			View.BackgroundColor = UIColor.White;
			
			// instantiate a new image view that takes up the whole screen and add it to 
			// the view hierarchy
			CGRect imageViewFrame = new CGRect (0, -NavigationController.NavigationBar.Frame.Height, View.Frame.Width, View.Frame.Height);
			imageView = new UIImageView (imageViewFrame);
			View.AddSubview (imageView);
			
			// add all of our buttons
			InitializeButtons ();
			
			DrawScreen ();
		}
		
		protected void DrawScreen ()
		{
			
			// create our offscreen bitmap context
			// size
			CGSize bitmapSize = new CGSize (imageView.Frame.Size);
			using (CGBitmapContext context = new CGBitmapContext (IntPtr.Zero, (int)bitmapSize.Width, (int)bitmapSize.Height, 8, (int)(4 * bitmapSize.Width), CGColorSpace.CreateDeviceRGB (), CGImageAlphaInfo.PremultipliedFirst)) {

				// save the state of the context while we change the CTM
				context.SaveState ();
				
				// draw our circle
				context.SetFillColor (1, 0, 0, 1);
				context.TranslateCTM (currentLocation.X, currentLocation.Y);
				context.RotateCTM (currentRotation);
				context.ScaleCTM (currentScale, currentScale);
				context.FillRect (new CGRect (-10, -10, 20, 20));
				
				// restore our transformations
				context.RestoreState ();

				// draw our coordinates for reference
				DrawCoordinateSpace (context);
				
				// output the drawing to the view
				imageView.Image = UIImage.FromImage (context.ToImage ());
			}
		}
		
		protected void InitializeButtons ()
		{
			InitButton (ref btnUp, new CGPoint (600, 20), 50, @"/\");
			View.AddSubview (btnUp);
			InitButton (ref btnRight, new CGPoint (660, 60), 50, ">");
			View.AddSubview (btnRight);
			InitButton (ref btnDown, new CGPoint (600, 100), 50, @"\/");
			View.AddSubview (btnDown);
			InitButton (ref btnLeft, new CGPoint (540, 60), 50, @"<");
			View.AddSubview (btnLeft);
			InitButton (ref btnReset, new CGPoint (600, 60), 50, @"X");
			View.AddSubview (btnReset);
			InitButton (ref btnRotateLeft, new CGPoint (540, 140), 75, "<@");
			View.AddSubview (btnRotateLeft);
			InitButton (ref btnRotateRight, new CGPoint (635, 140), 75, "@>");
			View.AddSubview (btnRotateRight);
			InitButton (ref btnScaleUp, new CGPoint (540, 180), 75, "+");
			View.AddSubview (btnScaleUp);
			InitButton (ref btnScaleDown, new CGPoint (635, 180), 75, "-");
			View.AddSubview (btnScaleDown);
			
			btnReset.TouchUpInside += delegate {
				currentScale = initialScale;
				currentLocation = initialLocation;
				currentRotation = initialRotation;
				DrawScreen();
			};
			
			btnUp.TouchUpInside += delegate {
				currentLocation.Y += movementIncrement;
				DrawScreen ();
			};
			btnDown.TouchUpInside += delegate {
				currentLocation.Y -= movementIncrement;
				DrawScreen ();
			};
			btnLeft.TouchUpInside += delegate {
				currentLocation.X -= movementIncrement;
				DrawScreen ();
			};
			btnRight.TouchUpInside += delegate {
				currentLocation.X += movementIncrement;
				DrawScreen ();
			};
			btnScaleUp.TouchUpInside += delegate {
				currentScale = currentScale * scaleIncrement;
				DrawScreen ();
			};
			btnScaleDown.TouchUpInside += delegate {
				currentScale = currentScale / scaleIncrement;
				DrawScreen ();
			};
			btnRotateLeft.TouchUpInside += delegate {
				currentRotation += rotationIncrement;
				DrawScreen ();
			};
			btnRotateRight.TouchUpInside += delegate {
				currentRotation -= rotationIncrement;
				DrawScreen ();
			};
			
		}
		
		protected void InitButton (ref UIButton button, CGPoint location, float width, string text)
		{
			button = UIButton.FromType (UIButtonType.RoundedRect);
			button.SetTitle (text, UIControlState.Normal);
			
			button.Frame = new CGRect (location, new CGSize (width, 33));
		}
	
		// Draws the specified text starting at x,y of the specified height to the context.
		protected void DrawTextAtPoint (CGContext context, float x, float y, string text, int textHeight)
		{
			// configure font
			context.SelectFont ("Helvetica-Bold", textHeight, CGTextEncoding.MacRoman);
			// set it to fill in our text, don't just outline
			context.SetTextDrawingMode (CGTextDrawingMode.Fill);
			// call showTextAtPoint
			context.ShowTextAtPoint (x, y, text, text.Length);
		}
		
		/// <summary>
		/// 
		/// </summary>
		protected void DrawCenteredTextAtPoint (CGContext context, float centerX, float y, string text, int textHeight)
		{
			context.SelectFont ("Helvetica-Bold", textHeight, CGTextEncoding.MacRoman);
			context.SetTextDrawingMode (CGTextDrawingMode.Invisible);
			context.ShowTextAtPoint (centerX, y, text, text.Length);
			context.SetTextDrawingMode (CGTextDrawingMode.Fill);
			context.ShowTextAtPoint (centerX - (context.TextPosition.X - centerX) / 2, y, text, text.Length);
		}
				
		/// <summary>
		/// Draws our coordinate grid
		/// </summary>
		protected void DrawCoordinateSpace (CGBitmapContext context)
		{
			// declare vars
			int remainder;
			int textHeight = 20;
			
			#region -= vertical ticks =-
			
			// create our vertical tick lines
			using (CGLayer verticalTickLayer = CGLayer.Create (context, new CGSize (20, 3))) {
				
				// draw a single tick
				verticalTickLayer.Context.FillRect (new CGRect (0, 1, 20, 2));
				
				// draw a vertical tick every 20 pixels
				float yPos = 20;
				int numberOfVerticalTicks = ((context.Height / 20) - 1);
				for (int i = 0; i < numberOfVerticalTicks; i++) {
					
					// draw the layer
					context.DrawLayer (verticalTickLayer, new CGPoint (0, yPos));
					
					// starting at 40, draw the coordinate text nearly to the top
					if (yPos > 40 && i < (numberOfVerticalTicks - 2)) {
						
						// draw it every 80 points
						Math.DivRem ((int)yPos, (int)80, out remainder);
						if (remainder == 0)
							DrawTextAtPoint (context, 30, (yPos - (textHeight / 2)), yPos.ToString (), textHeight);
					}
					
					// increment the position of the next tick
					yPos += 20;
				}
			}
			
			#endregion
			
			#region -= horizontal ticks =-
			
			// create our horizontal tick lines
			using (CGLayer horizontalTickLayer = CGLayer.Create (context, new CGSize (3, 20))) {
				
				horizontalTickLayer.Context.FillRect (new CGRect (1, 0, 2, 20));
				
				// draw a horizontal tick every 20 pixels
				float xPos = 20;
				int numberOfHorizontalTicks = ((context.Width / 20) - 1);
				for (int i = 0; i < numberOfHorizontalTicks; i++) {
					
					context.DrawLayer (horizontalTickLayer, new CGPoint (xPos, 0));
					
					// starting at 100, draw the coordinate text nearly to the top
					if (xPos > 100 && i < (numberOfHorizontalTicks - 1)) {
						
						// draw it every 80 points
						Math.DivRem ((int)xPos, (int)80, out remainder);
						if (remainder == 0)
							DrawCenteredTextAtPoint (context, xPos, 30, xPos.ToString (), textHeight);
					}
					
					// increment the position of the next tick
					xPos += 20;
				}
			}
			
			#endregion
			
			// draw our "origin" text
			DrawTextAtPoint (context, 20, (20 + (textHeight / 2)), "Origin (0,0)", textHeight);
		}
	}
}