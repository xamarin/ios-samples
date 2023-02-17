using System;
using CoreGraphics;
using Foundation;
using UIKit;
using SpriteKit;

namespace SpriteTour {

	public partial class BlendingSpritesViewController : CommonSpritesViewController {

		Random rand;
		const float range = 300f;
		float basex, basey;

		public BlendingSpritesViewController (IntPtr handle) : base (handle)
		{
			rand = new Random ();
		}

		protected override void CreateSceneContents ()
		{
			base.CreateSceneContents ();

			// no point in recomputing constant values each time
			var half_range = range / 2.0f;
			basex = (float) Scene.Frame.GetMidX () - half_range;
			basey = (float) Scene.Frame.GetMidY () - half_range;

			Scene.AddDescription ("These textured sprite nodes are combined using an additive blend",
				new CGPoint (Scene.Frame.GetMidX (), 100));

			Scene.RunAction (SKAction.RepeatActionForever (SKAction.Sequence (
				SKAction.Run (AddLight),
				SKAction.WaitForDuration (0.5f, 0.1f)
			)));
		}

		CGPoint GetRandomPosition ()
		{
			var x = basex + range * (float) rand.NextDouble ();
			var y = basey + range * (float) rand.NextDouble ();
			return new CGPoint (x, y);
		}

		void AddLight ()
		{
			SKSpriteNode sprite = new SKSpriteNode ("Art/spark.png") {
				Position = GetRandomPosition (),
				BlendMode = SKBlendMode.Add,
				Alpha = 0.5f
			};

			sprite.SetScale (2f);
			Scene.AddChild (sprite);
		}
	}
}
