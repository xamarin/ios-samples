using System;
using Foundation;
using UIKit;
using CoreGraphics;
using SceneKit;
using ARKit;

namespace PlacingObjects
{
	/*
	The focus square consists of eight segments as follows, which can be individually animated.

		s1  s2
		_   _
	s3 |     | s4

	s5 |     | s6
		-   -
		s7  s8
	*/
	public enum Corner
	{
        TopLeft, // s1, s3
        TopRight, // s2, s4
        BottomRight, // s6, s8
        BottomLeft // s5, s7
	}

	public enum Alignment
	{
        Horizontal, // s1, s2, s7, s8
        Vertical // s3, s4, s5, s6
	}
	enum Direction
	{
		Up, Down, Left, Right
	}

	static class Direction_Extensions
	{
		public static Direction Reversed(this Direction self)
		{
			switch (self)
			{
				case Direction.Up : return Direction.Down;
				case Direction.Down : return Direction.Up;
				case Direction.Left : return Direction.Right;
				case Direction.Right : return Direction.Left;
				default :
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	public class FocusSquareSegment : SCNNode
	{
		// Thickness and length in meters
		static float thickness = 0.018f;
		static float length = 0.5f;

		// Side length of the focus square segments when open (w.r.t. to a 1x1 square)
		static float openLength = 0.2f;

		private Corner Corner { get; set;  }
		private Alignment Alignment { get; set; }

		public FocusSquareSegment(string name, Corner corner, Alignment alignment) : base()
		{
			this.Name = name;
			this.Corner = corner;
			this.Alignment = alignment;

			switch (Alignment)
			{
				case Alignment.Vertical:
					this.Geometry = new SCNPlane { Width = FocusSquareSegment.thickness, Height = FocusSquareSegment.length };
					break;
				case Alignment.Horizontal :
					this.Geometry = new SCNPlane { Width = FocusSquareSegment.length, Height = FocusSquareSegment.thickness };
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var material = Geometry.FirstMaterial;
			material.Diffuse.ContentColor = FocusSquare.PrimaryColor;
			material.DoubleSided = true;
			material.Ambient.ContentColor = UIColor.Black;
			material.LightingModelName = SCNLightingModel.Constant;
			material.Emission.ContentColor = FocusSquare.PrimaryColor;
		}

		public FocusSquareSegment(NSCoder coder) : base(coder)
		{
			// Initialize
			Name = "Focus Square Segment";
		}

		private Direction OpenDirection ()
		{
			switch (this.Corner)
			{
				case Corner.TopLeft : 
					return this.Alignment == Alignment.Horizontal ? Direction.Left : Direction.Up;
				case Corner.TopRight :
					return this.Alignment == Alignment.Horizontal ? Direction.Right : Direction.Up;
				case Corner.BottomLeft :
					return this.Alignment == Alignment.Horizontal ? Direction.Left : Direction.Down;
				case Corner.BottomRight :
					return this.Alignment == Alignment.Horizontal ? Direction.Right : Direction.Down;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void Open() {
			var plane = Geometry as SCNPlane;
			if (plane == null)
			{
				return;
			}

			var direction = OpenDirection();

			if (Alignment == Alignment.Horizontal)
			{
				plane.Width = FocusSquareSegment.openLength;
			}
			else
			{
				plane.Height = FocusSquareSegment.openLength;
			}

			var offset = FocusSquareSegment.length / 2 - FocusSquareSegment.openLength / 2;
			switch(direction)
			{
				case Direction.Left : this.Position = new SCNVector3(Position.X - offset, Position.Y, Position.Z); break;
				case Direction.Right : this.Position = new SCNVector3(Position.X + offset, Position.Y, Position.Z); break;
				case Direction.Up : this.Position = new SCNVector3(Position.X, Position.Y - offset, Position.Z); break;
				case Direction.Down : this.Position = new SCNVector3(Position.X, Position.Y + offset, Position.Z); break;
			}
		}

		public void Close() {
			var plane = Geometry as SCNPlane;
			if (plane == null)
			{
				return;
			}
			var direction = this.OpenDirection().Reversed();

			var oldLength = 0.0f;
			if (Alignment == Alignment.Horizontal)
			{
				oldLength = (float)plane.Width;
				plane.Width = 0.5f;
			}
			else
			{
				oldLength = (float)plane.Height;
				plane.Height = 0.5f;
			}

			var x = Position.X;
			var y = Position.Y;
			var z = Position.Z;
			var offset = FocusSquareSegment.length / 2 - FocusSquareSegment.openLength / 2;
			switch (direction)
			{
				case Direction.Left: x -= offset; break;
				case Direction.Right: x += offset; break;
				case Direction.Up : y -= offset; break;
				case Direction.Down : y += offset; break;
			}
			Position = new SCNVector3(x, y, z);
		}
	}
}
