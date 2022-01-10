using System;

using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ColorFeed {
	// A class representing the response from the server for a single feed entry.
	public class Post {
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public DateTime Timestamp { get; set; }
		public double GradientDirection { get; set; }

		[ForeignKey (typeof (Color))]
		public int FirstColorId { get; set; }
		[OneToOne ("FirstColorId", CascadeOperations = CascadeOperation.All)]
		public Color FirstColor { get; set; }

		[ForeignKey (typeof (Color))]
		public int SecondColorId { get; set; }
		[OneToOne ("SecondColorId", CascadeOperations = CascadeOperation.All)]
		public Color SecondColor { get; set; }

		public Post ()
		{
		}

		public Post (DateTime timestamp, Color firstColor, Color secondColor, double gradientDirection)
		{
			Timestamp = timestamp;
			FirstColor = firstColor;
			SecondColor = secondColor;
			GradientDirection = gradientDirection;
		}
	}
}
