using System;
using CoreGraphics;
using UIKit;

namespace NSZombieApocalypse
{
	public class BodyPart : UIView
	{
		public BodyPart (CGRect frame) : base (frame)
		{
			BackgroundColor = UIColor.Clear;
			ClipsToBounds = false;
		}

		public  bool movingright ;
		public bool MovingRight {
			get { 
				return movingright;
			}
			set {
				movingright = value;
				SetNeedsDisplay (); 				
			}
		}
	}
}

