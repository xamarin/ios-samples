#define TILE_IMAGES
using System;
using System.Collections.Generic;
using CoreGraphics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Foundation;
using UIKit;

namespace FrogScroller
{
	// Class to hold FrogImage properties.
	public class ImageDetails {
		public float Height;
		public string Name;
		public float Width;
	}

	public class ImageScrollView : UIScrollView
	{
		CGSize _imageSize;
		UIImageView zoomView; // if tiling this contains a very low-res placeholder image. otherwise it contains the full image.
#if TILE_IMAGES
		TilingView tilingView;
#endif
		CGPoint _pointToCenterAfterResize;
		nfloat _scaleToRestoreAfterResize;
		int _index;
		
		public ImageScrollView () : base ()
		{
		}

		public ImageScrollView (CGRect frame) : base (frame)
		{
			ShowsVerticalScrollIndicator = false;
			ShowsHorizontalScrollIndicator = false;
			BouncesZoom = true;
			DecelerationRate = UIScrollView.DecelerationRateFast;
		}

		public int Index {
			get { 
				return _index;
			}
			set {
				_index = value;
#if TILE_IMAGES
				DisplayTiledImageNamed (ImageNameAtIndex (_index), ImageSizeAtIndex (_index));
 				DisplayImage (ImageAtIndex (_index));
#endif
			}
		}

  		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			//center the zoom view as it becomes smaller than the size of the screen
			var boundsSize = this.Bounds.Size;
			var frameToCenter = zoomView.Frame;

			//center horizontally
			if (frameToCenter.Size.Width < boundsSize.Width)
				frameToCenter.X = (boundsSize.Width - frameToCenter.Size.Width) / 2;
			else
				frameToCenter.X = 0;

			//center vertically
			if (frameToCenter.Size.Height < boundsSize.Height)
				frameToCenter.Y = (boundsSize.Height - frameToCenter.Size.Height) / 2;
			else
				frameToCenter.Y = 0;			

			zoomView.Frame = frameToCenter;
		}

		public override CGRect Frame {
			get {
				return base.Frame;
			}
			set {
				bool sizeChanging = Frame.Size != value.Size;
				if (sizeChanging) 
					PrepareToResize ();					

				base.Frame = value;
				
				if (sizeChanging) 
					RecoverFromResizing ();
			}
		}

		// - Configure scrollView to display new image (tiled or not)
 #if TILE_IMAGES
		public void DisplayTiledImageNamed (string imageName, CGSize image_Size)
		{
			//clear views for the previous image
			if (zoomView != null) {
				zoomView.RemoveFromSuperview ();
				zoomView = null;
				tilingView = null;
			}
			this.ZoomScale = 1.0f;
		
	    	//make views to display the new image
			zoomView = new UIImageView (new CGRect (CGPoint.Empty, image_Size)) {
				Image = PlaceholderImageNamed (imageName)
			};

			this.AddSubview(zoomView);
			tilingView = new TilingView (imageName, image_Size) {
				Frame = zoomView.Bounds
			};

			zoomView.AddSubview (tilingView);
			ConfigureForImageSize (image_Size);

			// Return the view to use when zooming
			ViewForZoomingInScrollView = (sv) => zoomView;
		}

		public void DisplayImage (UIImage image)
		{
			if (zoomView != null) {
				zoomView.RemoveFromSuperview ();
				zoomView = null;
				this.ZoomScale = 1.0f;
			}
			zoomView = new UIImageView (image);
			this.AddSubview (zoomView);
			ConfigureForImageSize (image.Size);
		}
#endif

		public void ConfigureForImageSize (CGSize imageSize)
		{
			_imageSize = imageSize;
			ContentSize = imageSize;
			SetMaxMinZoomScalesForCurrentBounds ();
			ZoomScale = this.MinimumZoomScale;
		}

		public void SetMaxMinZoomScalesForCurrentBounds ()
		{
			CGSize boundsSize = Bounds.Size;

			//calculate min/max zoomscale
			nfloat xScale = boundsSize.Width / _imageSize.Width; //scale needed to perfectly fit the image width-wise
			nfloat yScale = boundsSize.Height / _imageSize.Height; //scale needed to perfectly fit the image height-wise
				     
			//fill width if the image and phone are both portrait or both landscape; otherwise take smaller scale
			bool imagePortrait = _imageSize.Height > _imageSize.Width;
			bool phonePortrait = boundsSize.Height > boundsSize.Width;
			nfloat minScale = (imagePortrait == phonePortrait ? xScale : (nfloat)Math.Min (xScale, yScale));

			//on high resolution screens we have double the pixel density, so we will be seeing every pixel if we limit the maximum zoom scale to 0.5
			nfloat maxScale = 1.0f / UIScreen.MainScreen.Scale;

			if (minScale > maxScale) {
				minScale = maxScale;
			}
			// don't let minScale exceed maxScale. (If the image is smaller than the screen, we don't want to force it to be zoomed.) 
			this.MaximumZoomScale = maxScale;
			this.MinimumZoomScale = minScale;
		}
		
   		// Methods called during rotation to preserve the zoomScale and the visible portion of the image
		
		// Rotation support
		public void PrepareToResize ()
		{
			var boundsCenter = new CGPoint (Bounds.Width / 2, Bounds.Height / 2);
			_pointToCenterAfterResize = this.ConvertPointToView (boundsCenter, zoomView);
			_scaleToRestoreAfterResize = this.ZoomScale;
			// If we're at the minimum zoom scale, preserve that by returning 0, which will be converted to the minimum
			// allowable scale when the scale is restored.
			if (_scaleToRestoreAfterResize <= this.MinimumZoomScale + float.Epsilon)
				_scaleToRestoreAfterResize = 0;
		}

		public void RecoverFromResizing ()
		{
			SetMaxMinZoomScalesForCurrentBounds ();

			//Step 1: restore zoom scale, first making sure it is within the allowable range;
			ZoomScale = (nfloat)Math.Min (MaximumZoomScale, Math.Max (this.MinimumZoomScale, _scaleToRestoreAfterResize));

			// Step 2: restore center point, first making sure it is within the allowable range.			
			// 2a: convert our desired center point back to our own coordinate space
			var boundsCenter = this.ConvertPointFromView (_pointToCenterAfterResize, zoomView);
			// 2b: calculate the content offset that would yield that center point
			CGPoint offset = new CGPoint (boundsCenter.X - this.Bounds.Size.Width / 2.0f, boundsCenter.Y - this.Bounds.Size.Height / 2.0f);
			// 2c: restore offset, adjusted to be within the allowable range
			CGPoint maxOffset = MaximumContentOffset ();
			CGPoint minOffset = MinimumContentOffset ();
			offset.X = (nfloat)Math.Max (minOffset.X, Math.Min (maxOffset.X, offset.X));
			offset.Y = (nfloat)Math.Max (minOffset.Y, Math.Min (maxOffset.Y, offset.Y));
			this.ContentOffset = offset;
		}
	    
		public CGPoint MaximumContentOffset ()
		{
			CGSize contentSize = ContentSize;
			CGSize boundsSize = Bounds.Size;
			return new CGPoint (contentSize.Width - boundsSize.Width, contentSize.Height - boundsSize.Height);
		}

		public CGPoint MinimumContentOffset ()
		{
			return CGPoint.Empty;
		}

		static List<ImageDetails> data;

		// Method to Deserialize ImageDetails
		static List<ImageDetails> ImageData ()
		{
			if (data != null)
				return data;

			try {
				using (TextReader reader = new StreamReader ("Image/ImageDetails.xml")) {
					XmlSerializer serializer = new XmlSerializer (typeof (List<ImageDetails>));
					data = (List<ImageDetails>) serializer.Deserialize (reader);
				}
			} catch (XmlException e) {
				Console.WriteLine (e.ToString ());
				data = null;
			}
			return data;
		}

		public static int ImageCount {
			get {
				return ImageData ().Count;
			}
		}

		public static UIImage ImageAtIndex (int index)
		{
			var imageName = ImageNameAtIndex (index);
			var fullImage = string.Format ("/Image/FullImages/{0}", imageName);
			return UIImage.FromBundle (fullImage + ".jpg");
		}

		public static string ImageNameAtIndex (int index)
		{
			List<ImageDetails> _name = ImageData ();
			return _name [index].Name;
		}

		public static CGSize ImageSizeAtIndex (int index)
		{
			List<ImageDetails> _name = ImageData ();
			return new CGSize (_name [index].Width, _name [index].Height);
		}

		public static UIImage PlaceholderImageNamed (string name)
		{
			var placeholderImage = String.Format ("/Image/PlaceholderImages/{0}_Placeholder", name);
			return UIImage.FromBundle (placeholderImage + ".png");
		}
	}
}