using System;
using System.Collections.Generic;
using Foundation;
using SceneKit;
using SpriteKit;
using UIKit;
using System.Drawing;
using CoreGraphics;
using CoreFoundation;
using System.Linq;
using System.Diagnostics;

namespace SceneKitVehicle
{
	public partial class GameView : SCNView
	{
		public int TouchesCount { get; set; }

		public bool InCarView { get; set; }

		public GameView (IntPtr handle) : base (handle)
		{
		}

		public void ChangePointOfView ()
		{
			var pointOfViews = new List<SCNNode> ();
			foreach (var node in Scene.RootNode.ChildNodes) {
				if (node.Camera != null)
					pointOfViews.Add (node);

				foreach (var childNode in node.ChildNodes) {
					if (childNode.Camera != null)
						pointOfViews.Add (childNode);
				}
			}

			SCNNode currentPointOfView = PointOfView;
			int index = pointOfViews.IndexOf (currentPointOfView);
			index++;

			if (index >= pointOfViews.Count)
				index = 0;

			InCarView = index == 0;

			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 0.75f;
			PointOfView = pointOfViews [index];
			SCNTransaction.Commit ();
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			var touch = (UITouch)touches.AnyObject;

			SKScene scene = OverlayScene;
			CGPoint p = touch.LocationInView (this);
			p = scene.ConvertPointFromView (p);
			SKNode node = scene.GetNodeAtPoint (p);

			if (node.Name == "camera") {
				node.RunAction (SKAction.PlaySoundFileNamed (ResourceManager.GetResourcePath ("click.caf"), false));
				ChangePointOfView ();
				return;
			}

			NSSet allTouches = evt.AllTouches;
			TouchesCount = (int)allTouches.Count;
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			TouchesCount = 0;
		}
	}
}
