using System;
using UIKit;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using CoreAnimation;
using Foundation;
using ObjCRuntime;

namespace TicTacToe
{
	public class TTTGameView : UIView
	{
		public Func <TTTGameView, TTTMovePlayer, UIImage> ImageForPlayer;
		public Func <TTTGameView, TTTMovePlayer, UIColor> ColorForPlayer;
		public Func <TTTGameView, TTTMoveXPosition, TTTMoveYPosition, bool> CanSelect;
		public Action <TTTGameView, TTTMoveXPosition, TTTMoveYPosition> DidSelect;
		TTTGame game;

		public TTTGame Game {
			get { return game; }
			set {
				game = value;

				UpdateGameState ();
			}
		}

		UIColor gridColor;

		public UIColor GridColor {
			get { return gridColor; }
			set {
				if (!gridColor.Equals (value))
					gridColor = value;
				updateGridColor ();
			}
		}

		UIView[] horizontalLineViews;
		UIView[] verticalLineViews;
		List<UIImageView> moveImageViews;
		List<UIImageView> moveImageViewReuseQueue;
		TTTGameLineView lineView;
		const float LineWidth = 4f;

		public TTTGameView () : base ()
		{
			gridColor = UIColor.Black;
			horizontalLineViews = new UIView[] {
				newLineView (),
				newLineView ()
			};
			verticalLineViews = new UIView[] { 
				newLineView (), 
				newLineView () 
			};
			updateGridColor ();

			moveImageViews = new List <UIImageView> ();
			moveImageViewReuseQueue = new List <UIImageView> ();

			lineView = new TTTGameLineView ();
			lineView.Alpha = 0f;

			UITapGestureRecognizer gestureRecognizer = new UITapGestureRecognizer (tapGame);
			AddGestureRecognizer (gestureRecognizer);
		}

		UIView newLineView ()
		{
			UIView view = new UIView ();
			AddSubview (view);
			return view;
		}

		void tapGame (UITapGestureRecognizer gestureRecognizer)
		{
			if (gestureRecognizer.State == UIGestureRecognizerState.Recognized &&
				DidSelect != null) {
				CGPoint point = gestureRecognizer.LocationInView (this);
				CGRect bounds = Bounds;

				CGPoint normalizedPoint = point;
				normalizedPoint.X -= bounds.X + bounds.Size.Width / 2;
				normalizedPoint.X *= 3 / bounds.Size.Width;
				normalizedPoint.X = (float)Math.Round (normalizedPoint.X);
				normalizedPoint.X = (float)Math.Max (normalizedPoint.X, -1);
				normalizedPoint.X = (float)Math.Min (normalizedPoint.X, 1);
				TTTMoveXPosition xPosition = (TTTMoveXPosition)(int)normalizedPoint.X;

				normalizedPoint.Y -= bounds.Y + bounds.Size.Height / 2;
				normalizedPoint.Y *= 3 / bounds.Size.Height;
				normalizedPoint.Y = (float)Math.Round (normalizedPoint.Y);
				normalizedPoint.Y = (float)Math.Max (normalizedPoint.Y, -1);
				normalizedPoint.Y = (float)Math.Min (normalizedPoint.Y, 1);
				TTTMoveYPosition yPosition = (TTTMoveYPosition)(int)normalizedPoint.Y;

				if (CanSelect == null || CanSelect (this, xPosition, yPosition))
					DidSelect (this, xPosition, yPosition);
			}
		}

		UIImageView moveImageView ()
		{
			UIImageView moveView = moveImageViewReuseQueue.FirstOrDefault ();

			if (moveView != null)
				moveImageViewReuseQueue.Remove (moveView);
			else {
				moveView = new UIImageView ();
				AddSubview (moveView);
			}
			moveImageViews.Add (moveView);
			return moveView;
		}

		CGPoint pointForPosition (TTTMoveXPosition xPosition, TTTMoveYPosition yPosition)
		{
			CGRect bounds = Bounds;
			CGPoint point = new CGPoint (bounds.X + bounds.Size.Width / 2,
			                           bounds.Y + bounds.Size.Height / 2);
			point.X += (int)xPosition * bounds.Size.Width / 3;
			point.Y += (int)yPosition * bounds.Size.Height / 3;
			return point;
		}

		void setMove (TTTMove move, UIImageView moveView)
		{
			moveView.Image = ImageForPlayer (this, move.Player);
			moveView.Center = pointForPosition (move.XPosition, move.YPosition);
		}

		void setVisible (bool visible, UIImageView moveView)
		{
			if (visible) {
				moveView.SizeToFit ();
				moveView.Alpha = 1f;
			} else {
				moveView.Bounds = new CGRect (0, 0, 0, 0);
				moveView.Alpha = 0f;
			}
		}

		public void UpdateGameState ()
		{
			TTTMove[] moves = Game.Moves.ToArray ();
			int moveCount = moves.Length;

			UIImageView[] moveViews = new UIImageView[moveImageViews.Count];

			moveImageViews.CopyTo (moveViews);

			for (int i = 0; i < moveViews.Length; i++) {
				UIImageView moveView = moveViews [i];

				if (i < moveCount) {
					TTTMove move = moves [i];
					setMove (move, moveView);
					setVisible (true, moveView);
				} else {
					setVisible (false, moveViews [i]);
					moveImageViewReuseQueue.Add (moveView);
					moveImageViews.Remove (moveView);
				}
			}

			for (int moveIndex = moveImageViews.Count; moveIndex < moveCount; moveIndex++) {
				TTTMove move = moves [moveIndex];
				UIImageView moveView = moveImageView ();
				UIView.PerformWithoutAnimation (delegate {
					setMove (move, moveView);
					setVisible (false, moveView);
				});

				setVisible (true, moveView);
			}

			TTTMovePlayer winningPlayer;
			TTTMoveXPosition startXPosition, endXPosition;
			TTTMoveYPosition startYPosition, endYPosition;

			bool hasWinner = Game.GetWinningPlayer (out winningPlayer,
			                                        out startXPosition,
			                                        out startYPosition,
			                                        out endXPosition,
			                                        out endYPosition);
			if (hasWinner) {
				UIBezierPath path = new UIBezierPath ();
				path.LineWidth = LineWidth;
				path.MoveTo (pointForPosition (startXPosition, startYPosition));
				path.AddLineTo (pointForPosition (endXPosition, endYPosition));
				lineView.Path = path;
				lineView.Color = ColorForPlayer (this, winningPlayer);
				AddSubview (lineView);
			}

			lineView.Alpha = hasWinner ? 1f : 0f;
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			CGRect bounds = Bounds;
			for (int i = 0; i < horizontalLineViews.Length; i++) {
				UIView view = horizontalLineViews [i];
				view.Bounds = new CGRect (0, 0, bounds.Size.Width, LineWidth);
				view.Center = new CGPoint (bounds.X + bounds.Size.Width / 2,
				                          (float)Math.Round (bounds.Size.Height * (i + 1) / 3));
			}
			for (int i = 0; i < verticalLineViews.Length; i++) {
				UIView view = verticalLineViews [i];
				view.Bounds = new CGRect (0, 0, LineWidth, bounds.Size.Height);
				view.Center = new CGPoint ((float)Math.Round (bounds.Size.Width * (i + 1) / 3),
				                          bounds.Y + bounds.Size.Height / 2);
			}
			UpdateGameState ();
		}

		void updateGridColor ()
		{
			foreach (var view in horizontalLineViews)
				view.BackgroundColor = GridColor;
			foreach (var view in verticalLineViews)
				view.BackgroundColor = GridColor;
		}
	}

	public class TTTGameLineView : UIView
	{
		UIBezierPath path;

		public UIBezierPath Path {
			get { return path; }
			set {
				path = value;
				shapeLayer.Path = Path.CGPath;
			}
		}

		UIColor color;

		public UIColor Color {
			get { return color; }
			set {
				color = value;
				shapeLayer.StrokeColor = color.CGColor;
			}
		}

		public TTTGameLineView ()
		{
			shapeLayer.LineWidth = 2f;
		}

		[ExportAttribute ("layerClass")]
		public static Class LayerClass ()
		{
			return new Class (typeof(CAShapeLayer));
		}

		CAShapeLayer shapeLayer {
			get {
				return (CAShapeLayer)Layer;
			}
		}
	}
}
