using System;
using System.Collections.Generic;
using System.Linq;

using CoreGraphics;
using Foundation;
using Messages;
using UIKit;

namespace MessagesExtension {
	public class IceCream : IEquatable<IceCream>
	{
		CGSize size = new CGSize (300f, 300f);
		CGSize opaquePadding = new CGSize (60f, 10f);

		public Base Base { get; set; }

		public Scoops Scoops { get; set; }

		public Topping Topping { get; set; }

		public bool IsComplete {
			get {
				return Base != null && Scoops != null && Topping != null;
			}
		}

		public NSUrlQueryItem[] QueryItems
		{
			get {
				var items = new List<NSUrlQueryItem> ();
				if (Base != null)
					items.Add (Base.QueryItem);

				if (Scoops != null)
					items.Add (Scoops.QueryItem);

				if (Topping != null)
					items.Add (Topping.QueryItem);

				return items.ToArray ();
			}
		}

		public IceCream (BaseType baseType, ScoopsType scoopsType, ToppingType toppingType)
		{
			var iceCreamBase = new NSUrlQueryItem ("Base", baseType.ToString ());
			var iceCreamScoops = new NSUrlQueryItem ("Scoops", scoopsType.ToString ());
			var iceCreamTopping = new NSUrlQueryItem ("Topping", toppingType.ToString ());

			CheckQueryItems (new[] { iceCreamBase, iceCreamScoops, iceCreamTopping });
		}

		public IceCream (NSUrlQueryItem[] queryItems)
		{
			CheckQueryItems (queryItems);
		}

		public IceCream (MSMessage message)
		{
			if (message == null)
				return;

			var messageURL = message.Url;
			var urlComponents = new NSUrlComponents (messageURL, false);

			if (urlComponents.QueryItems == null)
				return;

			CheckQueryItems (urlComponents.QueryItems);
		}

		public override bool Equals (object obj)
		{
			if (obj == null)
				return false;

			var p = obj as IceCream;
			if (p == null)
				return false;

			return Base == p.Base && Scoops == p.Scoops && Topping == p.Topping;
		}

		public bool Equals (IceCream other)
		{
			if (other == null)
				return false;

			return Base == other.Base && Scoops == other.Scoops && Topping == other.Topping;
		}

		public override int GetHashCode ()
		{
			unchecked {
				var hashCode = 13;
				hashCode = (hashCode * 397) ^ Base.GetHashCode();
				hashCode = (hashCode * 397) ^ Scoops.GetHashCode();
				hashCode = (hashCode * 397) ^ Topping.GetHashCode();
				return hashCode;
			}
		}

		public UIImage RenderSticker (bool opaque)
		{
			var partsImage = RenderParts ();
			if (partsImage == null)
				return null;

			// Determine the size to draw as a sticker.
			CGSize outputSize = CGSize.Empty;
			CGSize iceCreamSize = CGSize.Empty;

			if (opaque) {
				// Scale the ice cream image to fit in the center of the sticker.
				var scale = NMath.Min ((size.Width - opaquePadding.Width) / partsImage.Size.Height, (size.Height - opaquePadding.Height) / partsImage.Size.Width);
				iceCreamSize = new CGSize (partsImage.Size.Width * scale, partsImage.Size.Height * scale);
				outputSize = size;
			} else {
				// Scale the ice cream to fit it's height into the sticker.
				var scale = size.Width / partsImage.Size.Height;
				iceCreamSize = new CGSize (partsImage.Size.Width * scale, partsImage.Size.Height * scale);
				outputSize = iceCreamSize;
			}

			// Scale the ice cream image to the correct size.
			var renderer = new UIGraphicsImageRenderer (outputSize);
			var image = renderer.CreateImage ((context) => {
				UIColor backgroundColor = opaque ? UIColor.FromRGBA (250f / 255f, 225f / 255f, 235f / 255f, 1f) : UIColor.Clear;

				// Draw the background
				backgroundColor.SetFill ();
				context.FillRect (new CGRect (CGPoint.Empty, size));

				// Draw the scaled composited image.
				var drawRect = new CGRect {
					Size = iceCreamSize,
					X = outputSize.Width / 2f - iceCreamSize.Width / 2f,
					Y = outputSize.Height / 2f - iceCreamSize.Height / 2f
				};

				partsImage.Draw (drawRect);
			});

			return image;
		}

		public UIImage RenderParts ()
		{
			// Determine which parts to draw.
			IceCreamPart[] allParts = { Topping, Scoops, Base };
			var partImages = new List<UIImage> ();

			foreach (var part in allParts) {
				if (part != null && part.Image != null)
					partImages.Add (part.StickerImage);
			}

			if (partImages == null)
				return null;

			// Calculate the size of the composited ice cream parts image.
			var outputImageSize = CGSize.Empty;
			outputImageSize.Width = partImages.OrderByDescending (i => i.Size.Width).FirstOrDefault ().Size.Width;
			outputImageSize.Height = (nfloat)partImages.Sum (i => i.Size.Height);

			// Render the part images into a single composite image.
			var renderer = new UIGraphicsImageRenderer (outputImageSize);
			var image = renderer.CreateImage (context => {
				// Draw each of the body parts in a vertica stack.
				nfloat nextYPosition = 0f;
				foreach (var partImage in partImages) {
					var position = new CGPoint (outputImageSize.Width / 2f - partImage.Size.Width / 2f, nextYPosition);

					partImage.Draw (position);
					nextYPosition += partImage.Size.Height;
				}
			});

			return image;
		}

		void CheckQueryItems (NSUrlQueryItem[] queryItems)
		{
			foreach (var queryItem in queryItems) {
				if (string.IsNullOrEmpty (queryItem.Value))
					continue;

				switch (queryItem.Name) {
					case "Base":
						var baseType = (BaseType)Enum.Parse (typeof (BaseType), queryItem.Value, true);
						Base = new Base (baseType);
						break;
					case "Scoops":
						var scoopsType = (ScoopsType)Enum.Parse(typeof(ScoopsType), queryItem.Value, true);
						Scoops = new Scoops (scoopsType);
						break;
					case "Topping":
						var toppingType = (ToppingType)Enum.Parse (typeof (ToppingType), queryItem.Value, true);
						Topping = new Topping (toppingType);
						break;
				}
			}
		}
	}
}

