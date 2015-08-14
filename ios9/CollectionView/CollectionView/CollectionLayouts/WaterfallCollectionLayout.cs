using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;

namespace CollectionView
{
	/// <summary>
	/// Waterfall collection layout.
	/// </summary>
	/// <remarks>
	/// Origionally created by Nicholas Tau on 6/30/14.
	/// Copyright (c) 2014 Nicholas Tau. All rights reserved.
	/// Ported from http://nshint.io/blog/2015/07/16/uicollectionviews-now-have-easy-reordering/ to
	/// Xamarin.iOS by Kevin Mullins.
	/// </remarks>
	[Register("WaterfallCollectionLayout")]
	public class WaterfallCollectionLayout : UICollectionViewLayout
	{
		#region Private Variables
		private int _columnCount = 2;
		private nfloat _minimumColumnSpacing = 10;
		private nfloat _minimumInterItemSpacing = 10;
		private nfloat _headerHeight = 0.0f;
		private nfloat _footerHeight = 0.0f;
		private UIEdgeInsets _sectionInset = new UIEdgeInsets(0, 0, 0, 0);
		private WaterfallCollectionRenderDirection _itemRenderDirection = WaterfallCollectionRenderDirection.ShortestFirst;
		private Dictionary<nint,UICollectionViewLayoutAttributes> _headersAttributes = new Dictionary<nint, UICollectionViewLayoutAttributes>();
		private Dictionary<nint,UICollectionViewLayoutAttributes> _footersAttributes = new Dictionary<nint, UICollectionViewLayoutAttributes>();
		private List<CGRect> _unionRects = new List<CGRect>();
		private List<nfloat> _columnHeights = new List<nfloat>();
		private List<UICollectionViewLayoutAttributes> _allItemAttributes = new List<UICollectionViewLayoutAttributes>();
		private List<List<UICollectionViewLayoutAttributes>> _sectionItemAttributes = new List<List<UICollectionViewLayoutAttributes>>();
		private nfloat _unionSize = 20;
		#endregion

		#region Computed Properties
		[Export("ColumnCount")]
		public int ColumnCount {
			get { return _columnCount; }
			set {
				WillChangeValue ("ColumnCount");
				_columnCount = value;
				DidChangeValue ("ColumnCount");

				InvalidateLayout ();
			}
		}

		[Export("MinimumColumnSpacing")]
		public nfloat MinimumColumnSpacing {
			get { return _minimumColumnSpacing; }
			set {
				WillChangeValue ("MinimumColumnSpacing");
				_minimumColumnSpacing = value;
				DidChangeValue ("MinimumColumnSpacing");

				InvalidateLayout ();
			}
		}

		[Export("MinimumInterItemSpacing")]
		public nfloat MinimumInterItemSpacing {
			get { return _minimumInterItemSpacing; }
			set {
				WillChangeValue ("MinimumInterItemSpacing");
				_minimumInterItemSpacing = value;
				DidChangeValue ("MinimumInterItemSpacing");

				InvalidateLayout ();
			}
		}

		[Export("HeaderHeight")]
		public nfloat HeaderHeight {
			get { return _headerHeight; }
			set {
				WillChangeValue ("HeaderHeight");
				_headerHeight = value;
				DidChangeValue ("HeaderHeight");

				InvalidateLayout ();
			}
		}

		[Export("FooterHeight")]
		public nfloat FooterHeight {
			get { return _footerHeight; }
			set {
				WillChangeValue ("FooterHeight");
				_footerHeight = value;
				DidChangeValue ("FooterHeight");

				InvalidateLayout ();
			}
		}

		[Export("SectionInset")]
		public UIEdgeInsets SectionInset {
			get { return _sectionInset; }
			set {
				WillChangeValue ("SectionInset");
				_sectionInset = value;
				DidChangeValue ("SectionInset");

				InvalidateLayout ();
			}
		}

		[Export("ItemRenderDirection")]
		public WaterfallCollectionRenderDirection ItemRenderDirection {
			get { return _itemRenderDirection; }
			set {
				WillChangeValue ("ItemRenderDirection");
				_itemRenderDirection = value;
				DidChangeValue ("ItemRenderDirection");

				InvalidateLayout ();
			}
		}
		#endregion

		#region Constructors
		public WaterfallCollectionLayout ()
		{
		}

		public WaterfallCollectionLayout(NSCoder coder) : base(coder) {

		}
		#endregion 

		#region Public Methods
		public nfloat ItemWidthInSectionAtIndex(int section) {

			var width = CollectionView.Bounds.Width - SectionInset.Left - SectionInset.Right;
			return (nfloat)Math.Floor ((width - ((ColumnCount - 1) * MinimumColumnSpacing)) / ColumnCount);
		}
		#endregion

		#region Override Methods
		public override void PrepareLayout ()
		{
			base.PrepareLayout ();

			// Get the number of sections
			var numberofSections = CollectionView.NumberOfSections();
			if (numberofSections == 0)
				return;

			// Reset collections
			_headersAttributes.Clear ();
			_footersAttributes.Clear ();
			_unionRects.Clear ();
			_columnHeights.Clear ();
			_allItemAttributes.Clear ();
			_sectionItemAttributes.Clear ();

			// Initialize column heights
			for (int n = 0; n < ColumnCount; n++) {
				_columnHeights.Add ((nfloat)0);
			}

			// Process all sections
			nfloat top = 0.0f;
			var attributes = new UICollectionViewLayoutAttributes ();
			var columnIndex = 0;
			for (nint section = 0; section < numberofSections; ++section) {
				// Calculate section specific metrics
				var minimumInterItemSpacing = (MinimumInterItemSpacingForSection == null) ? MinimumColumnSpacing : 
					MinimumInterItemSpacingForSection (CollectionView, this, section);

				// Calculate widths
				var width = CollectionView.Bounds.Width - SectionInset.Left - SectionInset.Right;
				var itemWidth = (nfloat)Math.Floor ((width - ((ColumnCount - 1) * MinimumColumnSpacing)) / ColumnCount);

				// Calculate section header
				var heightHeader = (HeightForHeader == null) ? HeaderHeight : 
					HeightForHeader (CollectionView, this, section);

				if (heightHeader > 0) {
					attributes = UICollectionViewLayoutAttributes.CreateForSupplementaryView (UICollectionElementKindSection.Header, NSIndexPath.FromRowSection (0, section));
					attributes.Frame = new CGRect (0, top, CollectionView.Bounds.Width, heightHeader);
					_headersAttributes.Add (section, attributes);
					_allItemAttributes.Add (attributes);

					top = attributes.Frame.GetMaxY ();
				}

				top += SectionInset.Top;
				for (int n = 0; n < ColumnCount; n++) {
					_columnHeights [n] = top;
				}

				// Calculate Section Items
				var itemCount = CollectionView.NumberOfItemsInSection(section);
				List<UICollectionViewLayoutAttributes> itemAttributes = new List<UICollectionViewLayoutAttributes> ();

				for (nint n = 0; n < itemCount; n++) {
					var indexPath = NSIndexPath.FromRowSection (n, section);
					columnIndex = NextColumnIndexForItem (n);
					var xOffset = SectionInset.Left + (itemWidth + MinimumColumnSpacing) * (nfloat)columnIndex;
					var yOffset = _columnHeights [columnIndex];
					var itemSize = (SizeForItem == null) ? new CGSize (0, 0) : SizeForItem (CollectionView, this, indexPath);
					nfloat itemHeight = 0.0f;

					if (itemSize.Height > 0.0f && itemSize.Width > 0.0f) {
						itemHeight = (nfloat)Math.Floor (itemSize.Height * itemWidth / itemSize.Width);
					}

					attributes = UICollectionViewLayoutAttributes.CreateForCell (indexPath);
					attributes.Frame = new CGRect (xOffset, yOffset, itemWidth, itemHeight);
					itemAttributes.Add (attributes);
					_allItemAttributes.Add (attributes);
					_columnHeights [columnIndex] = attributes.Frame.GetMaxY () + MinimumInterItemSpacing;
				}
				_sectionItemAttributes.Add (itemAttributes);

				// Calculate Section Footer
				nfloat footerHeight = 0.0f;
				columnIndex = LongestColumnIndex();
				top = _columnHeights [columnIndex] - MinimumInterItemSpacing + SectionInset.Bottom;
				footerHeight = (HeightForFooter == null) ? FooterHeight : HeightForFooter(CollectionView, this, section);

				if (footerHeight > 0) {
					attributes = UICollectionViewLayoutAttributes.CreateForSupplementaryView (UICollectionElementKindSection.Footer, NSIndexPath.FromRowSection (0, section));
					attributes.Frame = new CGRect (0, top, CollectionView.Bounds.Width, footerHeight);
					_footersAttributes.Add (section, attributes);
					_allItemAttributes.Add (attributes);
					top = attributes.Frame.GetMaxY ();
				}

				for (int n = 0; n < ColumnCount; n++) {
					_columnHeights [n] = top;
				}
			}

			var i =0;
			var attrs = _allItemAttributes.Count;
			while(i < attrs) {
				var rect1 = _allItemAttributes [i].Frame;
				i = (int)Math.Min (i + _unionSize, attrs) - 1;
				var rect2 = _allItemAttributes [i].Frame;
				_unionRects.Add (CGRect.Union (rect1, rect2));
				i++;
			}

		}

		public override CGSize CollectionViewContentSize {
			get {
				if (CollectionView.NumberOfSections () == 0) {
					return new CGSize (0, 0);
				}

				var contentSize = CollectionView.Bounds.Size;
				contentSize.Height = _columnHeights [0];
				return contentSize;
			}
		}

		public override UICollectionViewLayoutAttributes LayoutAttributesForItem (NSIndexPath indexPath)
		{
			if (indexPath.Section >= _sectionItemAttributes.Count) {
				return null;
			}

			if (indexPath.Item >= _sectionItemAttributes [indexPath.Section].Count) {
				return null;
			}

			var list = _sectionItemAttributes [indexPath.Section];
			return list [(int)indexPath.Item];
		}

		public override UICollectionViewLayoutAttributes LayoutAttributesForSupplementaryView (NSString kind, NSIndexPath indexPath)
		{
			var attributes = new UICollectionViewLayoutAttributes ();

			switch (kind) {
			case "header":
				attributes = _headersAttributes [indexPath.Section];
				break;
			case "footer":
				attributes = _footersAttributes [indexPath.Section];
				break;
			}

			return attributes;
		}

		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect (CGRect rect)
		{
			var begin = 0;
			var end = _unionRects.Count;
			List<UICollectionViewLayoutAttributes> attrs = new List<UICollectionViewLayoutAttributes> ();


			for (int i = 0; i < end; i++) {
				if (rect.IntersectsWith(_unionRects[i])) {
					begin = i * (int)_unionSize;
				}
			}

			for (int i = end - 1; i >= 0; i--) {
				if (rect.IntersectsWith (_unionRects [i])) {
					end = (int)Math.Min ((i + 1) * (int)_unionSize, _allItemAttributes.Count);
					break;
				}
			}

			for (int i = begin; i < end; i++) {
				var attr = _allItemAttributes [i];
				if (rect.IntersectsWith (attr.Frame)) {
					attrs.Add (attr);
				}
			}

			return attrs.ToArray();
		}

		public override bool ShouldInvalidateLayoutForBoundsChange (CGRect newBounds)
		{
			var oldBounds = CollectionView.Bounds;
			return (newBounds.Width != oldBounds.Width);
		}
		#endregion

		#region Private Methods
		private int ShortestColumnIndex() {
			var index = 0;
			var shortestHeight = nfloat.MaxValue;
			var n = 0;

			// Scan each column for the shortest height
			foreach (nfloat height in _columnHeights) {
				if (height < shortestHeight) {
					shortestHeight = height;
					index = n;
				}
				++n;
			}

			return index;
		}

		private int LongestColumnIndex() {
			var index = 0;
			var longestHeight = nfloat.MinValue;
			var n = 0;

			// Scan each column for the shortest height
			foreach (nfloat height in _columnHeights) {
				if (height > longestHeight) {
					longestHeight = height;
					index = n;
				}
				++n;
			}

			return index;
		}

		private int NextColumnIndexForItem(nint item) {
			var index = 0;

			switch (ItemRenderDirection) {
			case WaterfallCollectionRenderDirection.ShortestFirst:
				index = ShortestColumnIndex ();
				break;
			case WaterfallCollectionRenderDirection.LeftToRight:
				index = ColumnCount;
				break;
			case WaterfallCollectionRenderDirection.RightToLeft:
				index = (ColumnCount - 1) - ((int)item / ColumnCount);
				break;
			}

			return index;
		}
		#endregion

		#region Events
		public delegate CGSize WaterfallCollectionSizeDelegate(UICollectionView collectionView, WaterfallCollectionLayout layout, NSIndexPath indexPath);
		public delegate nfloat WaterfallCollectionFloatDelegate(UICollectionView collectionView, WaterfallCollectionLayout layout, nint section);
		public delegate UIEdgeInsets WaterfallCollectionEdgeInsetsDelegate(UICollectionView collectionView, WaterfallCollectionLayout layout, nint section);

		public event WaterfallCollectionSizeDelegate SizeForItem;
		public event WaterfallCollectionFloatDelegate HeightForHeader;
		public event WaterfallCollectionFloatDelegate HeightForFooter;
		public event WaterfallCollectionEdgeInsetsDelegate InsetForSection;
		public event WaterfallCollectionFloatDelegate MinimumInterItemSpacingForSection;
		#endregion
	}
}

