using System;
using System.Drawing;
using CoreMotion;
using Foundation;
using SceneKit;
using SpriteKit;
using UIKit;
using CoreGraphics;

namespace SceneKitVehicle
{
	public class OverlayScene : SKScene
	{
		public SKNode SpeedNeedle { get; set; }

		public OverlayScene (CGSize size) : base (size)
		{
			AnchorPoint = new CGPoint (0.5f, 0.5f);
			ScaleMode = SKSceneScaleMode.ResizeFill;

			float scale = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 1.5f : 1f;

			var myImage = SKSpriteNode.FromImageNamed (ResourceManager.GetResourcePath ("speedGauge.png"));
			myImage.AnchorPoint = new CGPoint (0.5f, 0f);
			myImage.Position = new CGPoint (size.Width * 0.33f, -size.Height * 0.5f);
			myImage.XScale = 0.8f * scale;
			myImage.YScale = 0.8f * scale;
			AddChild (myImage);

			var needleHandle = SKNode.Create ();
			var needle = SKSpriteNode.FromImageNamed (ResourceManager.GetResourcePath ("needle.png"));
			needleHandle.Position = new CGPoint (0f, 16f);
			needle.AnchorPoint = new CGPoint (0.5f, 0f);
			needle.XScale = 0.7f;
			needle.YScale = 0.7f;
			needle.ZRotation = (float)Math.PI / 2f;
			needleHandle.AddChild (needle);
			myImage.AddChild (needleHandle);

			SpeedNeedle = needleHandle;

			var cameraImage = SKSpriteNode.FromImageNamed (ResourceManager.GetResourcePath ("video_camera.png"));
			cameraImage.Position = new CGPoint (-size.Width * 0.4f, -size.Height * 0.4f);
			cameraImage.Name = "camera";
			cameraImage.XScale = 0.6f * scale;
			cameraImage.YScale = 0.6f * scale;
			AddChild (cameraImage);
		}
	}

}
