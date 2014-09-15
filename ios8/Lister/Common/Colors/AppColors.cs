using System;
using UIKit;

namespace Common
{
	public static class AppColors
	{
		public static readonly UIColor GrayColor = UIColor.DarkGray;
		public static readonly UIColor BlueColor = UIColor.FromRGBA (0.42f, 0.7f, 0.88f, 1);
		public static readonly UIColor BreenColor = UIColor.FromRGBA (0.71f, 0.84f, 0.31f, 1);
		public static readonly UIColor YellowColor = UIColor.FromRGBA (0.95f, 0.88f, 0.15f, 1);
		public static readonly UIColor OrangeColor = UIColor.FromRGBA (0.96f, 0.63f, 0.20f, 1);
		public static readonly UIColor RedColor = UIColor.FromRGBA (0.96f, 0.42f, 0.42f, 1);

		public static UIColor ColorFrom(ListColor colorType)
		{
			switch (colorType) {
				case ListColor.Gray:
					return GrayColor;

				case ListColor.Blue:
					return BlueColor;

				case ListColor.Green:
					return BreenColor;

				case ListColor.Yellow:
					return YellowColor;

				case ListColor.Orange:
					return OrangeColor;

				case ListColor.Red:
					return RedColor;

				default:
					throw new NotImplementedException ();
			}
		}
	}
}

