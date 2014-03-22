using System;
using System.Drawing;
using MonoTouch.UIKit;

namespace NSZombieApocalypse
{
	public class BodyPart : UIView
	{
		public BodyPart (RectangleF frame) : base (frame)
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

