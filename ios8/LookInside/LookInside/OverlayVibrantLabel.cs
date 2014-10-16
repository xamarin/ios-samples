using System;

using UIKit;
using Foundation;

namespace LookInside
{
	public class OverlayVibrantLabel : UILabel
	{
		public OverlayVibrantLabel ()
		{
			TranslatesAutoresizingMaskIntoConstraints = false;
		}

		public override void TintColorDidChange ()
		{
			TextColor = TintColor;
		}
	}
}

