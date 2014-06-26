using System;
using CoreGraphics;
using Foundation;

using UIKit;
using System.Threading.Tasks;

namespace NSZombieApocalypse
{
	public partial class WalkingDead : UIControl
	{
		Head head;
		Body body;
		RightArm rightArm;
		LeftArm leftArm;
		RightLeg rightLeg;
		LeftLeg leftLeg;

		double startedWalking;
		nfloat startedWalkingX;

		bool animated;
		bool walkingForward;

		class Head : BodyPart
		{
			public Head (CGRect rect) : base (rect)
			{
			}

			public override void Draw (CGRect rect)
			{
				rect = rect.Inset (4, 4);

				rect = new CGRect ((rect.Size.Width - rect.Size.Height) / 2 + 4, 8, rect.Size.Height, rect.Size.Height);
				UIBezierPath path = UIBezierPath.FromOval (rect);
				UIColor.Black.SetStroke ();
				UIColor.White.SetFill ();
				path.LineWidth = 2;
				path.Fill ();
				path.Stroke ();
			
				UIBezierPath rightEye, leftEye, mouth = new UIBezierPath ();
				if (MovingRight) {
					rightEye = UIBezierPath.FromArc (new CGPoint (rect.GetMidX () - 5, rect.Y + 15), 4, 0, 180, true);
					leftEye = UIBezierPath.FromArc (new CGPoint (rect.GetMidX () + 10, rect.Y + 15), 4, 0, 180, true);

					mouth.MoveTo (new CGPoint (rect.GetMidX (), rect.Y + 30));
					mouth.AddLineTo (new CGPoint (rect.GetMidX () + 13, rect.Y + 30));
				} else {
					rightEye = UIBezierPath.FromArc (new CGPoint (rect.GetMidX () - 10, rect.Y + 15), 4, 0, 180, true);
					leftEye = UIBezierPath.FromArc (new CGPoint (rect.GetMidX () + 5, rect.Y + 15), 4, 0, 180, true);

					mouth.MoveTo (new CGPoint (rect.GetMidX (), rect.Y + 30));
					mouth.AddLineTo (new CGPoint (rect.GetMidX () - 13, rect.Y + 30));
				
				}
				rightEye.LineWidth = 2;
				rightEye.Stroke ();
			
				leftEye.LineWidth = 2;
				leftEye.Stroke ();
			
				mouth.LineWidth = 2;
				mouth.Stroke ();
			}
		}

		class Body : BodyPart
		{
			public Body (CGRect rect) : base (rect)
			{
			}

			public override void Draw (CGRect rect)
			{
				rect = rect.Inset (2, 2);
				nfloat bodyWidth = rect.Size.Width / 2;
				UIBezierPath path = UIBezierPath.FromRoundedRect (new CGRect ((rect.Size.Width - bodyWidth) / 2, 0, bodyWidth, rect.Size.Height), UIRectCorner.TopLeft | UIRectCorner.TopRight, new CGSize (8, 8));
				UIColor.Black.SetStroke ();
				UIColor.White.SetFill ();
				path.Fill ();
				path.LineWidth = 2;
				path.Stroke ();
			}
		}

		class RightLeg : BodyPart
		{
			public RightLeg (CGRect rect) : base (rect)
			{
			}

			public override void Draw (CGRect rect)
			{
				UIView body = ((WalkingDead)Superview).body;
				CGRect bodyFrame = body.Frame;
				nfloat legWidth = rect.Size.Width / 3;
				UIBezierPath path = UIBezierPath.FromRoundedRect (new CGRect (20, bodyFrame.GetMaxY () - 5, legWidth, rect.Size.Height * .25f), UIRectCorner.TopRight | UIRectCorner.BottomRight, new CGSize (3, 3));
				path.LineWidth = 2;
				UIColor.White.SetFill ();
				path.Fill ();
				path.Stroke ();
			}
		}

		class LeftLeg : BodyPart
		{
			public LeftLeg (CGRect rect) : base (rect)
			{
			}

			public override void Draw (CGRect rect)
			{
				UIView body = ((WalkingDead)Superview).body;
				CGRect bodyFrame = body.Frame;
				nfloat legWidth = rect.Size.Width / 3;
				UIBezierPath path = UIBezierPath.FromRoundedRect (new CGRect (30, bodyFrame.GetMaxY () - 5, legWidth, (rect.Size.Height) * .25f), UIRectCorner.TopRight | UIRectCorner.BottomRight, new CGSize (3, 3));
		
				UIColor.Black.SetColor ();
				path.LineWidth = 2;
				UIColor.White.SetFill ();
				path.Fill ();
				path.Stroke ();
			}
		}

		class RightArm : BodyPart
		{
			public RightArm (CGRect rect) : base (rect)
			{
			}

			public override void Draw (CGRect rect)
			{
				UIView head = ((WalkingDead)Superview).head;
				var path = new UIBezierPath ();
				path.LineCapStyle = CGLineCap.Round;
				CGRect headFrame = head.Frame;

				if (!MovingRight) {
					path.MoveTo (new CGPoint (rect.GetMidX () - 10, headFrame.GetMaxY () + 10));
					path.AddLineTo (new CGPoint (rect.GetMidX () - 10 + rect.Size.Width / 4, headFrame.GetMaxY () + 10));
					path.AddLineTo (new CGPoint (rect.GetMidX () - 10 + rect.Size.Width / 2, headFrame.GetMaxY () + 10 + rect.Size.Height / 10));
				} else {
					path.MoveTo (new CGPoint (rect.GetMidX () + 10, headFrame.GetMaxY () + 10));
					path.AddLineTo (new CGPoint (rect.GetMidX () + 10 - rect.Size.Width / 4, headFrame.GetMaxY () + 10));
					path.AddLineTo (new CGPoint (rect.GetMidX () + 10 - rect.Size.Width / 2, headFrame.GetMaxY () + 10 + rect.Size.Height / 10));

				}
				UIColor.Black.SetStroke ();
				path.LineWidth = 12;
				path.Stroke ();

				UIColor.White.SetStroke ();
				path.LineWidth = 8;
				path.Stroke ();
			}
		}

		class LeftArm : BodyPart
		{
			public LeftArm (CGRect rect) : base (rect)
			{
			}

			public override void Draw (CGRect rect)
			{
				UIView head = ((WalkingDead)Superview).head;
				UIBezierPath path = new UIBezierPath ();
				path.LineCapStyle = CGLineCap.Round;
				CGRect headFrame = head.Frame;
			
				if (!MovingRight) {
					rect.X -= 20;
					path.MoveTo (new CGPoint (rect.GetMidX () + 20, headFrame.GetMaxY () + 10));
					path.AddLineTo (new CGPoint (rect.GetMidX () + 20 + rect.Size.Width / 6, headFrame.GetMaxY () + 10));
					path.AddLineTo (new CGPoint (rect.GetMidX () + 20 + rect.Size.Width / 6 + 10, headFrame.GetMaxY () + 10 + 20));
				} else {
					path.MoveTo (new CGPoint (rect.GetMidX () - 20, headFrame.GetMaxY () + 10));
					path.AddLineTo (new CGPoint (rect.GetMidX () - 20 - rect.Size.Width / 6, headFrame.GetMaxY () + 10));
					path.AddLineTo (new CGPoint (rect.GetMidX () - 20 - rect.Size.Width / 6 - 10, headFrame.GetMaxY () + 10 + 20));
				
				}
				UIColor.Black.SetStroke ();
				path.LineWidth = 12;
				path.Stroke ();
			
				UIColor.White.SetStroke ();
				path.LineWidth = 8;
				path.Stroke ();
			}
		}

		public event DidDisassembleHandler WalkingDeadDidDisassemble;

		public WalkingDead (CGRect frame) :base (frame)
		{
			BackgroundColor = UIColor.Clear;
			ClipsToBounds = false;

			head = new Head (new CGRect (0, 0, Frame.Size.Width, Frame.Size.Height * .25f));
			AddSubview (head);

			body = new Body (new CGRect (0, head.Frame.GetMaxY (), Frame.Size.Width, Frame.Size.Height * .375f));
			AddSubview (body);

			leftArm = new LeftArm (new CGRect (0, 0, Frame.Size.Width + 20, Frame.Size.Height));
			AddSubview (leftArm);

			rightArm = new RightArm (new CGRect (0, 0, Frame.Size.Width, Frame.Size.Height));
			AddSubview (rightArm);

			rightLeg = new RightLeg (new CGRect (0, 0, Frame.Size.Width, Frame.Size.Height));
			AddSubview (rightLeg);

			leftLeg = new LeftLeg (new CGRect (0, 0, Frame.Size.Width, Frame.Size.Height));
			AddSubview (leftLeg);

			TurnAround ();
		}

		public override bool IsAccessibilityElement {
			get {
				return true;
			}
		}

		public override string AccessibilityLabel {
			get {
				return "Zombie";
			}
		}

		public void TurnAround ()
		{
			walkingForward = !walkingForward;
			head.MovingRight = !head.MovingRight;
			body.MovingRight = !head.MovingRight;
			leftArm.MovingRight = !head.MovingRight;
			rightArm.MovingRight = !head.MovingRight;
			rightLeg.MovingRight = !head.MovingRight;
			leftLeg.MovingRight = !head.MovingRight;
		}

		public void Walk ()
		{
			if (!animated)
				return;

			CGRect superviewFrame = Superview.Frame;
			startedWalking = NSDate.Now.SecondsSinceReferenceDate;
			startedWalkingX = Frame.X;
			UIView.Animate (10, 0, UIViewAnimationOptions.AllowUserInteraction, () => {
				if (!animated) 
					return;
				
				if (!walkingForward)
					TurnAround ();

				CGRect frame = Frame;
				frame.X = superviewFrame.Size.Width - frame.Size.Width - 50;
				Frame = frame;
			}, () => {
				if (!animated) 
					return;

				TurnAround ();

				startedWalking = NSDate.Now.SecondsSinceReferenceDate;
				startedWalkingX = Frame.X;
				CGRect frame = Frame;
				frame.X = 50;
				UIView.Animate (10, 0, UIViewAnimationOptions.AllowUserInteraction, () => {
					Frame = frame;}, 
				                () => {
					Walk ();
				});
			});
		}

		 public void Disassemble ()
		{
			animated = false;

			UIView.Animate (.75, () => {
				var frame = new CGRect (head.Frame.X, -100, head.Frame.Width, head.Frame.Height);
				head.Frame = frame;

				frame = new CGRect (-100, leftArm.Frame.Y, leftArm.Frame.Width, leftArm.Frame.Height);
				leftArm.Frame = frame;

				frame = new CGRect (rightArm.Frame.Size.Width + 100, rightArm.Frame.Y, rightArm.Frame.Width, rightArm.Frame.Height);
				rightArm.Frame = frame;

				frame = new CGRect (leftLeg.Frame.X - 50, leftLeg.Frame.Size.Height, leftLeg.Frame.Width, leftLeg.Frame.Height);
				leftLeg.Frame = frame;

				frame = new CGRect (rightLeg.Frame.X + 50, rightLeg.Frame.Size.Height, rightLeg.Frame.Width, rightLeg.Frame.Height);
				rightLeg.Frame = frame;
			}, async () => {
				await UIView.AnimateAsync (.5, () => {
					this.Alpha = 0;
				});
				RemoveFromSuperview ();
			});
		}

		public void MoveArms ()
		{
			if (!animated)
				return;

			float armRotation = 10 * (float)Math.PI / 180;
			UIView.Animate (1.75, () => {
				rightArm.Transform = CGAffineTransform.MakeRotation (armRotation);
				leftArm.Transform = CGAffineTransform.MakeRotation (-armRotation);
			}, async () => {
				if (!animated)
					return;
			
				await UIView.AnimateAsync (1.75, () => {
					rightArm.Transform = CGAffineTransform.MakeIdentity ();
					leftArm.Transform = CGAffineTransform.MakeIdentity ();
				});

				MoveArms ();
			});
		}

		public void MoveLegs ()
		{
			if (!animated)
				return;
			
			float legRotation = (float)(Math.PI / 4 * .35);
			UIView.Animate (2.5, () => {
				rightLeg.Transform = CGAffineTransform.MakeRotation (legRotation);
				leftLeg.Transform = CGAffineTransform.MakeRotation (-legRotation);
			}, async () => {
				if (!animated)
					return;
				
				await UIView.AnimateAsync (2.5, () => {
					rightLeg.Transform = CGAffineTransform.MakeRotation (-legRotation);
					leftLeg.Transform = CGAffineTransform.MakeRotation (legRotation);
				});

				MoveLegs ();
			});
		}

		public void Animate ()
		{
			animated = true;
			MoveArms ();
			MoveLegs ();
			Walk ();
		}

		public void DeAnimate ()
		{
			animated = false;
			Layer.RemoveAllAnimations ();
			rightArm.Layer.RemoveAllAnimations ();
			leftArm.Layer.RemoveAllAnimations ();
			rightLeg.Layer.RemoveAllAnimations ();
			leftLeg.Layer.RemoveAllAnimations ();

			nfloat percentage = (float)(NSDate.Now.SecondsSinceReferenceDate - startedWalking / 10);
			nfloat xNow = (nfloat)Math.Abs (Frame.X - startedWalkingX) * percentage;
			CGRect frame = Frame;
			Frame = new CGRect (xNow + (frame.Size.Width / 2), frame.Y, frame.Height, frame.Width);
		}

		public delegate void DidDisassembleHandler (WalkingDead walkingdead);
	}
}
		

