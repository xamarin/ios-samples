using System;

using SQLite;
using SQLiteNetExtensions.Attributes;
using UIKit;

namespace ColorFeed {
	public class Color {
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public double Red { get; set; }
		public double Green { get; set; }
		public double Blue { get; set; }

		public Color ()
		{
		}

		public Color (double red, double green, double blue)
		{
			Red = red;
			Green = green;
			Blue = blue;
		}

		public UIColor ToUIColor () => new UIColor ((nfloat)Red, (nfloat)Green, (nfloat)Blue, 1);
	}
}
