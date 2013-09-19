using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.SpriteKit;

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
			basex = Scene.Frame.GetMidX () - half_range;
			basey = Scene.Frame.GetMidY () - half_range;

			Scene.AddDescription ("These textured sprite nodes are combined using an additive blend",
				new PointF (Scene.Frame.GetMidX (), 100));

			Scene.RunAction (SKAction.RepeatActionForever (SKAction.Sequence (
				SKAction.RunBlock (AddLight),
				SKAction.WaitForDuration (0.5f, 0.1f)
			)));
		}

		PointF GetRandomPosition ()
		{
			var x = basex + range * (float) rand.NextDouble ();
			var y = basey + range * (float) rand.NextDouble ();
			return new PointF (x, y);
		}

		void AddLight ()
		{
			SKSpriteNode sprite = new SKSpriteNode ("Art/spark.png") {
				Position = GetRandomPosition (),
				BlendMode = SKBlendMode.Add,
				Alpha = 0.5f,
				Scale = 2.0f
			};
			Scene.AddChild (sprite);
		}
	}
}