using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using Foundation;
using CoreAnimation;

namespace DropDestination {
	/**
	A GridView is responsible for laying out UIViews
	of a given fixed size in an evenly spaced grid.
	While a collection view is normally better suited
	for these purposes, we subclass a regular UIView
	to demonstrate that UICollectionView-like drag and
	drop support can also be supported in any arbitrary
	type of UIView using drag and drop interactions.
	*/
	public class GridView : UIView {
		#region Computed Properties
		public float GridViewMargin { get; set; } = 18f;
		public CGSize FixedCellSize { get; set; } = CGSize.Empty;
		public List<UIView> ArrangedViews { get; set; } = new List<UIView> ();

		public nfloat Width {
			get { return Frame.Width; }
			set {
				Frame = new CGRect (Frame.X, Frame.Y, value, Height);
				RaiseGridSizeChanged ();
			}
		}

		public nfloat Height {
			get { return (FixedCellSize.Height + (GridViewMargin * 2f)) * NumberOfRows; }
		}

		public int NumberOfColumns {
			get { return (int) (Width / (FixedCellSize.Width + (GridViewMargin * 2f))); }
		}

		public int NumberOfRows {
			get {
				if (NumberOfColumns < 1 || ArrangedViews.Count < 1) return 0;
				return (ArrangedViews.Count / NumberOfColumns) + 1;
			}
		}

		public override CGSize IntrinsicContentSize {
			get {
				return new CGSize (Width, Height);
			}
		}
		#endregion

		#region Constructors
		public GridView ()
		{
		}

		public GridView (NSCoder coder) : base (coder)
		{
		}

		public GridView (CGSize cellSize, CGRect rect) : base (rect)
		{
			// Initialize
			FixedCellSize = cellSize;
		}
		#endregion

		#region Public Methods
		public int RowForViewAtIndex (int index)
		{
			if (index < 0 || index >= ArrangedViews.Count || NumberOfColumns < 1) return 0;
			return (index / NumberOfColumns);
		}

		public int ColumnForViewAtIndex (int index)
		{
			if (index < 0 || index >= ArrangedViews.Count || NumberOfColumns < 1) return 0;
			return index - (NumberOfColumns * RowForViewAtIndex (index));

		}

		public CGRect FrameForViewAtIndex (int index)
		{
			if (index < 0 || index >= ArrangedViews.Count) return CGRect.Empty;
			return new CGRect (GridViewMargin + ColumnForViewAtIndex (index) * (GridViewMargin + FixedCellSize.Width),
							  GridViewMargin + RowForViewAtIndex (index) * (GridViewMargin + FixedCellSize.Height),
							  FixedCellSize.Width,
							  FixedCellSize.Height
							 );
		}

		public void AddArrangedView (UIView view)
		{
			ArrangedViews.Add (view);
			view.Frame = FrameForViewAtIndex (ArrangedViews.Count - 1);
			AddSubview (view);
			RaiseGridSizeChanged ();
		}

		public void RemoveArrangedViewsAtIndexes (NSIndexSet indices)
		{
			for (int n = ArrangedViews.Count - 1; n >= 0; --n) {
				if (indices.Contains ((nuint) n)) {
					ArrangedViews [n].RemoveFromSuperview ();
					ArrangedViews.RemoveAt (n);
				}
			}
			RaiseGridSizeChanged ();
		}

		public int IndexForView (UIView view)
		{

			// Scan all views
			for (int n = 0; n < ArrangedViews.Count; ++n) {
				if (ArrangedViews [n] == view) return n;
			}

			// Not found
			return -1;
		}
		#endregion

		#region Override Methods
		public override void LayoutSubviews ()
		{
			for (int n = 0; n < ArrangedViews.Count; ++n) {
				ArrangedViews [n].Frame = FrameForViewAtIndex (n);
			}

			base.LayoutSubviews ();
		}
		#endregion

		#region Events
		public delegate void GridSizeChangedDelegate (CGSize size);
		public event GridSizeChangedDelegate GridSizeChanged;

		internal void RaiseGridSizeChanged ()
		{
			Frame = new CGRect (0, 0, Width, Height);
			GridSizeChanged?.Invoke (IntrinsicContentSize);
		}
		#endregion
	}
}
