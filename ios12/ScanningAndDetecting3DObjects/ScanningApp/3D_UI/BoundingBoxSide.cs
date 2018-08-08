using System;
using System.Collections.Generic;
using CoreGraphics;
using OpenTK;
using SceneKit;
using UIKit;
using System.Linq;

namespace ScanningAndDetecting3DObjects
{
	internal class BoundingBoxSide : SCNNode
	{
		internal enum PositionName
		{
			Front,
			Back,
			Left,
			Right,
			Bottom,
			Top
		}

		// The bounding box face that is represented by this node
		private PositionName face;

		internal bool IsBusyUpdatingTiles {
			get;
			private set;
		}

		// The completion of this side in range [0,1]
		internal double Completion {
			get {
				if (Tiles.Count == 0)
				{
					return 0; 
				}
				var capturedTiles = Tiles.FindAll(t => t.Captured);
				return (double)capturedTiles.Count / Tiles.Count;
			}
		}

		// The normal vector of this side.
		internal SCNVector3 Normal
		{
			get
			{
				switch (face)
				{
					case PositionName.Front:
					case PositionName.Right:
					case PositionName.Top:
						return DragAxis.Normal().ToSCNVector3();
					case PositionName.Back:
					case PositionName.Left:
					case PositionName.Bottom:
						return -DragAxis.Normal().ToSCNVector3();
				}
				throw new ArgumentOutOfRangeException();
			}
		}

		// The drag axis for this side
		internal Axis DragAxis {
			get
			{
				switch(face)
				{
					case PositionName.Left :
					case PositionName.Right :
						return Axis.X;
					case PositionName.Bottom : 
					case PositionName.Top :
						return Axis.Y;
					case PositionName.Front : 
					case PositionName.Back :
						return Axis.Z;
				}
				throw new ArgumentOutOfRangeException();
			}
		}

		// The tiles of this side
		internal List<Tile> Tiles 
		{
			get;
			private set;
		}

		private CGSize size = CGSize.Empty;
		private readonly UIColor color = Utilities.AppYellow;

		// Maximum width or height of a tile. If the size of the side exceeds this value, a new row or column is added
		private const double maxTileSize = 0.1;

		// Maximum number of tiles per row / column 
		private const int maxTileCount = 4;

		private const double lineThickness = 0.002;
		private const double extensionLength = 0.05;

		// The size of the bounding box side when the tiles were updated the last time
		private CGSize sizeOnLastTileUpdate = CGSize.Empty;

		private bool TilesNeedUpdateForChangedSize => sizeOnLastTileUpdate != size;

		private readonly SCNNode xAxisExtNode = new SCNNode();
		private readonly List<SCNNode> xAxisExtLines = new List<SCNNode>();
		private readonly SCNNode yAxisExtNode = new SCNNode();
		private readonly List<SCNNode> yAxisExtLines = new List<SCNNode>();
		private readonly SCNNode zAxisExtNode = new SCNNode();
		private readonly List<SCNNode> zAxisExtLines = new List<SCNNode>();


		internal BoundingBoxSide(PositionName position, NVector3 extent, UIColor color = null) : base()
		{
			if (color == null)
			{
				color = Utilities.AppYellow;
			}
			Tiles = new List<Tile>();
			this.color = color;
			this.face = position;

			// inline Swift setup() and setupExtensions() functions
			size = Size(extent);

			var yAxis = Axis.Y.Normal().ToSCNVector3();
			var xAxis = Axis.X.Normal().ToSCNVector3();
			var zAxis = Axis.Z.Normal().ToSCNVector3();
			float halfTurn = (float)Math.PI;
			float quarterTurn = (float)Math.PI / 2;

			switch(face)
			{
				case PositionName.Front :
					LocalTranslate(new SCNVector3(0, 0, extent.Z / 2));
					break;
				case PositionName.Back :
					LocalTranslate(new SCNVector3(0, 0, -extent.Z / 2));
					LocalRotate(SCNQuaternion.FromAxisAngle(yAxis, halfTurn));
					break;
				case PositionName.Left :
					LocalTranslate(new SCNVector3(-extent.X / 2, 0, 0));
					LocalRotate(SCNQuaternion.FromAxisAngle(yAxis, -quarterTurn));
					break;
				case PositionName.Right :
					LocalTranslate(new SCNVector3(extent.X / 2, 0, 0));
					LocalRotate(SCNQuaternion.FromAxisAngle(yAxis, quarterTurn));
					break;
				case PositionName.Bottom :
					LocalTranslate(new SCNVector3(0, -extent.Y / 2, 0));
					LocalRotate(SCNQuaternion.FromAxisAngle(xAxis, halfTurn));
					break;
				case PositionName.Top :
					LocalTranslate(new SCNVector3(0, extent.Y / 2, 0));
					LocalRotate(SCNQuaternion.FromAxisAngle(xAxis, -quarterTurn));
					break;
			}

			for (int index = 0; index < 12; index++)
			{
				var line = new SCNNode();
				line.Geometry = Cylinder(lineThickness, extensionLength);
				if (index < 4)
				{
					xAxisExtLines.Add(line);
					line.LocalRotate(SCNQuaternion.FromAxisAngle(zAxis, -quarterTurn));
					if (index == 2 || index == 3)
					{
						line.LocalRotate(SCNQuaternion.FromAxisAngle(xAxis, halfTurn));
					}
					xAxisExtNode.AddChildNode(line);
				}
				else if (index < 8)
				{
					yAxisExtLines.Add(line);
					if (index == 5 || index == 7)
					{
						line.LocalRotate(SCNQuaternion.FromAxisAngle(xAxis, halfTurn));
					}
					yAxisExtNode.AddChildNode(line);
				}
				else
				{
					zAxisExtLines.Add(line);
					line.LocalRotate(SCNQuaternion.FromAxisAngle(xAxis, -quarterTurn));
					zAxisExtNode.AddChildNode(line);
				}
			}

			UpdateExtensions();
			HideXAxisExtensions();
			HideYAxisExtensions();
			HideZAxisExtensions();

			AddChildNode(xAxisExtNode);
			AddChildNode(yAxisExtNode);
			AddChildNode(zAxisExtNode);
		}

		private SCNGeometry Cylinder(double width, double height)
		{
			var cylinderGeometry = SCNCylinder.Create((nfloat) width / 2, (nfloat) height);
			var gradYellowMat = Utilities.Material(UIImage.FromBundle("yellowimage"));
			var clearMaterial = Utilities.Material(UIColor.Clear);
			cylinderGeometry.Materials = new[] { gradYellowMat, clearMaterial, clearMaterial };
			return cylinderGeometry;
		}

		private void UpdateExtensions()
		{
			if ( xAxisExtLines.Count != 4 || yAxisExtLines.Count != 4 || zAxisExtLines.Count != 4)
			{
				return;
			}

			var halfWidth = (float) size.Width / 2;
			var halfHeight = (float) size.Height / 2;
			var halfLength = (float) extensionLength / 2;

			xAxisExtLines[0].Position = new SCNVector3(-halfWidth - halfLength, -halfHeight, 0);
			yAxisExtLines[0].Position = new SCNVector3(-halfWidth, -halfHeight - halfLength, 0);
			zAxisExtLines[0].Position = new SCNVector3(-halfWidth, -halfHeight, halfLength);

			xAxisExtLines[1].Position = new SCNVector3(-halfWidth - halfLength, halfHeight, 0);
			yAxisExtLines[1].Position = new SCNVector3(-halfWidth, halfHeight + halfLength, 0);
			zAxisExtLines[1].Position = new SCNVector3(-halfWidth, halfHeight, halfLength);

			xAxisExtLines[2].Position = new SCNVector3(halfWidth + halfLength, -halfHeight, 0);
			yAxisExtLines[2].Position = new SCNVector3(halfWidth, -halfHeight - halfLength, 0);
			zAxisExtLines[2].Position = new SCNVector3(halfWidth, -halfHeight, halfLength);

			xAxisExtLines[3].Position = new SCNVector3(halfWidth + halfLength, halfHeight, 0);
			yAxisExtLines[3].Position = new SCNVector3(halfWidth, halfHeight + halfLength, 0);
			zAxisExtLines[3].Position = new SCNVector3(halfWidth, halfHeight, halfLength);
		}

		private CGSize Size(NVector3 extent)
		{
			switch (face)
			{
				case PositionName.Front :
				case PositionName. Back:
					return new CGSize(extent.X, extent.Y);
				case PositionName.Left : 
				case PositionName.Right :
					return new CGSize(extent.Z, extent.Y);
				case PositionName.Bottom : 
				case PositionName.Top :
					return new CGSize(extent.X, extent.Z);

			}
			throw new Exception("Somehow fell through switch statement that should be exhaustive.");
		}


		internal void UpdateBoundingBoxExtent(NVector3 extent)
		{
			switch (face)
			{
				case PositionName.Front : 
					Position = new SCNVector3(0, 0, extent.Z / 2);
					break;
				case PositionName.Back :
					Position = new SCNVector3(0, 0, -extent.Z / 2);
					break;
				case PositionName.Left :
					Position = new SCNVector3(-extent.X / 2, 0, 0);
					break;
				case PositionName.Right :
					Position = new SCNVector3(extent.X / 2, 0, 0);
					break;
				case PositionName.Bottom :
					Position = new SCNVector3(0, -extent.Y / 2, 0);
					break;
				case PositionName.Top :
					Position = new SCNVector3(0, extent.Y / 2, 0);
					break;
			}

			// Update extensions if the size has changed.
			var newSize = Size(extent);
			if (newSize != size)
			{
				size = newSize;
				UpdateExtensions();
			}
		}

		internal void ShowZAxisExtensions()
		{
			zAxisExtNode.Hidden = false;
		}

		internal void HideZAxisExtensions()
		{
			zAxisExtNode.Hidden = true;
		}

		internal void ShowYAxisExtensions()
		{
			yAxisExtNode.Hidden = false;
		}

		internal void HideYAxisExtensions()
		{
			yAxisExtNode.Hidden = true;
		}

		internal void ShowXAxisExtensions()
		{
			xAxisExtNode.Hidden = false;
		}

		internal void HideXAxisExtensions()
		{
			xAxisExtNode.Hidden = true;
		}

		internal void UpdateVisualizationIfNeeded()
		{
			if (! IsBusyUpdatingTiles && TilesNeedUpdateForChangedSize)
			{
				SetupTiles();
			}
		}

		private void SetupTiles()
		{
			IsBusyUpdatingTiles = true;

			// Determine number of rows and columns
			var numRows = Math.Min(maxTileCount, (int) (Math.Ceiling(size.Height / maxTileSize)));
			var numColumns = Math.Min(maxTileCount, (int)(Math.Ceiling(size.Width / maxTileSize)));

			var newTiles = new List<Tile>();

			// Create updates tiles and lay them out
			for (var row = 0; row < numRows; row++)
			{
				for (var col = 0; col < numColumns; col++)
				{
					var plane = SCNPlane.Create(size.Width / numColumns, size.Height / numRows);
					plane.Materials = new[] { Utilities.Material(color, false, false) };

					var xPos = -size.Width / 2 + plane.Width / 2 + col * plane.Width;
					var yPos = size.Height / 2 - plane.Height / 2 - row * plane.Height;

					var tileNode = new Tile(plane);
					tileNode.Position = new SCNVector3((float)xPos, (float)yPos, 0);
					newTiles.Add(tileNode);
				}
			}

			// Replace the nodes in the scene graph.
			foreach(var tile in Tiles)
			{
				tile.RemoveFromParentNode();
			}
			foreach(var tile in newTiles)
			{
				AddChildNode(tile);
			}
			Tiles = newTiles;

			sizeOnLastTileUpdate = size;
			IsBusyUpdatingTiles = false;
		}
	}
}