using System;
using CoreGraphics;
using SpriteKit;

namespace SpriteTour {

	public static class SpriteKitExtensions {

		public static SKLabelNode AddDescription (this SKNode self, string description, CGPoint position)
		{
			SKLabelNode label = new SKLabelNode ("Helvetica") {
				Text = description,
				FontSize = 18,
				Position = position
			};
			self.AddChild (label);
			return label;
		}
	}
}