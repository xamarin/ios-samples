using System;
using System.Collections.Generic;

using CoreGraphics;
using Foundation;
using UIKit;

namespace CollectionView {
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
	public class WaterfallCollectionLayout : UICollectionViewLayout {
		#region Variables
		int columnCount = 2;
		nfloat minimumColumnSpacing = 10f;
		nfloat minimumInterItemSpacing = 10f;
		nfloat unionSize = 20f;
		nfloat headerHeight = 0f;
		nfloat footerHeight = 0f;
		UIEdgeInsets sectionInset = new UIEdgeInsets (0f, 0f, 0f, 0f);
		WaterfallCollectionRenderDirection itemRenderDirection = WaterfallCollectionRenderDirection.ShortestFirst;
		Dictionary<nint,UICollectionViewLayoutAttributes> headersAttributes = new Dictionary<nint, UICollectionViewLayoutAttributes> ();
		Dictionary<nint,UICollectionViewLayoutAttributes> footersAttributes = new Dictionary<nint, UICollectionViewLayoutAttributes> ();
		List<CGRect> unionRects = new List<CGRect> ();
		List<nfloat> columnHeights = new List<nfloat> ();
		List<UICollectionViewLayoutAttributes> allItemAttributes = new List<UICollectionViewLayoutAttributes> ();
		List<List<UICollectionViewLayoutAttributes>> sectionItemAttributes = new List<List<UICollectionViewLayoutAttributes>> ();
		#endregion

		#region Computed Properties
		[Export("ColumnCount")]
		public int ColumnCount {
			get { return columnCount; }
			set {
				WillChangeValue ("ColumnCount");
				columnCount = value;
				DidChangeValue ("ColumnCount");

				InvalidateLayout ();
			}
		}

		[Export("MinimumColumnSpacing")]
		public nfloat MinimumColumnSpacing {
			get { return minimumColumnSpacing; }
			set {
				WillChangeValue ("MinimumColumnSpacing");
				minimumColumnSpacing = value;
				DidChangeValue ("MinimumColumnSpacing");

				InvalidateLayout ();
			}
		}

		[Export("MinimumInterItemSpacing")]
		public nfloat MinimumInterItemSpacing {
			get { return minimumInterItemSpacing; }
			set {
				WillChangeValue ("MinimumInterItemSpacing");
				minimumInterItemSpacing = value;
				DidChangeValue ("MinimumInterItemSpacing");

				InvalidateLayout ();
			}
		}

		[Export("HeaderHeight")]
		public nfloat HeaderHeight {
			get { return headerHeight; }
			set {
				WillChangeValue ("HeaderHeight");
				headerHeight = value;
				DidChangeValue ("HeaderHeight");

				InvalidateLayout ();
			}
		}

		[Export("FooterHeight")]
		public nfloat FooterHeight {
			get { return footerHeight; }
			set {
				WillChangeValue ("FooterHeight");
				footerHeight = value;
				DidChangeValue ("FooterHeight");

				InvalidateLayout ();
			}
		}

		[Export("SectionInset")]
		public UIEdgeInsets SectionInset {
			get { return sectionInset; }
			set {
				WillChangeValue ("SectionInset");
				sectionInset = value;
				DidChangeValue ("SectionInset");

				InvalidateLayout ();
			}
		}

		[Export("ItemRenderDirection")]
		public WaterfallCollectionRenderDirection ItemRenderDirection {
			get { return itemRenderDirection; }
			set {
				WillChangeValue ("ItemRenderDirection");
				itemRenderDirection = value;
				DidChangeValue ("ItemRenderDirection");

				InvalidateLayout ();
			}
		}
		#endregion

		#region Constructors
		public WaterfallCollectionLayout ()
		{
		}

		public WaterfallCollectionLayout(NSCoder coder) : base(coder)
		{
		}
		#endregion 

		#region Override Methods
		public override void PrepareLayout ()
		{
			base.PrepareLayout ();

			// Get the number of sections
			var numberofSections = CollectionView.NumberOfSections ();
			if (numberofSections == 0)
				return;

			// Reset collections
			headersAttributes.Clear ();
			footersAttributes.Clear ();
			unionRects.Clear ();
			columnHeights.Clear ();
			allItemAttributes.Clear ();
			sectionItemAttributes.Clear ();

			// Initialize column heights
			for (int n = 0; n < ColumnCount; n++)
				columnHeights.Add ((nfloat)0);

			// Process all sections
			nfloat top = 0f;
			var attributes = new UICollectionViewLayoutAttributes ();
			int columnIndex = 0;

			for (nint section = 0; section < numberofSections; ++section) {

				// Calculate widths
				var width = CollectionView.Bounds.Width - SectionInset.Left - SectionInset.Right;
				var itemWidth = (nfloat)Math.Floor ((width - ((ColumnCount - 1) * MinimumColumnSpacing)) / ColumnCount);

				// Calculate section header
				var heightHeader = (HeightForHeader == null) ? HeaderHeight : 
					HeightForHeader (CollectionView, this, section);

				if (heightHeader > 0) {
					attributes = UICollectionViewLayoutAttributes.CreateForSupplementaryView (UICollectionElementKindSection.Header, NSIndexPath.FromRowSection (0, section));
					attributes.Frame = new CGRect (0, top, CollectionView.Bounds.Width, heightHeader);
					headersAttributes.Add (section, attributes);
					allItemAttributes.Add (attributes);

					top = attributes.Frame.GetMaxY ();
				}

				top += SectionInset.Top;
				for (int n = 0; n < ColumnCount; n++)
					columnHeights [n] = top;

				// Calculate Section Items
				var itemCount = CollectionView.NumberOfItemsInSection (section);
				var itemAttributes = new List<UICollectionViewLayoutAttributes> ();

				for (nint n = 0; n < itemCount; n++) {
					var indexPath = NSIndexPath.FromRowSection (n, section);
					columnIndex = NextColumnIndexForItem (n);
					var xOffset = SectionInset.Left + (itemWidth + MinimumColumnSpacing) * (nfloat)columnIndex;
					var yOffset = columnHeights [columnIndex];
					var itemSize = (SizeForItem == null) ? new CGSize (0, 0) : SizeForItem (CollectionView, this, indexPath);
					nfloat itemHeight = 0.0f;

					if (itemSize.Height > 0.0f && itemSize.Width > 0.0f)
						itemHeight = (nfloat)Math.Floor (itemSize.Height * itemWidth / itemSize.Width);

					attributes = UICollectionViewLayoutAttributes.CreateForCell (indexPath);
					attributes.Frame = new CGRect (xOffset, yOffset, itemWidth, itemHeight);
					itemAttributes.Add (attributes);
					allItemAttributes.Add (attributes);
					columnHeights [columnIndex] = attributes.Frame.GetMaxY () + MinimumInterItemSpacing;
				}

				sectionItemAttributes.Add (itemAttributes);

				// Calculate Section Footer
				columnIndex = LongestColumnIndex ();
				top = columnHeights [columnIndex] - MinimumInterItemSpacing + SectionInset.Bottom;
				footerHeight = (HeightForFooter == null) ? FooterHeight : HeightForFooter (CollectionView, this, section);

				if (footerHeight > 0) {
					attributes = UICollectionViewLayoutAttributes.CreateForSupplementaryView (UICollectionElementKindSection.Footer, NSIndexPath.FromRowSection (0, section));
					attributes.Frame = new CGRect (0, top, CollectionView.Bounds.Width, footerHeight);
					footersAttributes.Add (section, attributes);
					allItemAttributes.Add (attributes);
					top = attributes.Frame.GetMaxY ();
				}

				for (int n = 0; n < ColumnCount; n++)
					columnHeights [n] = top;
			}

			int i = 0;
			int attrs = allItemAttributes.Count;
			while (i < attrs) {
				var rect1 = allItemAttributes [i].Frame;
				i = (int)Math.Min (i + unionSize, attrs) - 1;
				var rect2 = allItemAttributes [i].Frame;
				unionRects.Add (CGRect.Union (rect1, rect2));
				i++;
			}
		}

		public override CGSize CollectionViewContentSize {
			get {
				if (CollectionView.NumberOfSections () == 0)
					return new CGSize (0f, 0f);

				var contentSize = CollectionView.Bounds.Size;
				contentSize.Height = columnHeights [0];
				return contentSize;
			}
		}

		public override UICollectionViewLayoutAttributes LayoutAttributesForItem (NSIndexPath indexPath)
		{
			if (indexPath.Section >= sectionItemAttributes.Count)
				return null;

			if (indexPath.Item >= sectionItemAttributes [indexPath.Section].Count)
				return null;

			var list = sectionItemAttributes [indexPath.Section];
			return list [(int)indexPath.Item];
		}

		public override UICollectionViewLayoutAttributes LayoutAttributesForSupplementaryView (NSString kind, NSIndexPath indexPath)
		{
			var attributes = new UICollectionViewLayoutAttributes ();

			switch (kind) {
			case "header":
				attributes = headersAttributes [indexPath.Section];
				break;
			case "footer":
				attributes = footersAttributes [indexPath.Section];
				break;
			}

			return attributes;
		}

		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect (CGRect rect)
		{
			int begin = 0;
			int end = unionRects.Count;
			var attrs = new List<UICollectionViewLayoutAttributes> ();


			for (int i = 0; i < end; i++)
				if (rect.IntersectsWith (unionRects [i]))
					begin = i * (int)unionSize;

			for (int i = end - 1; i >= 0; i--) {
				if (rect.IntersectsWith (unionRects [i])) {
					end = Math.Min ((i + 1) * (int)unionSize, allItemAttributes.Count);
					break;
				}
			}

			for (int i = begin; i < end; i++) {
				var attr = allItemAttributes [i];
				if (rect.IntersectsWith (attr.Frame)) {
					attrs.Add (attr);
				}
			}

			return attrs.ToArray ();
		}

		public override bool ShouldInvalidateLayoutForBoundsChange (CGRect newBounds)
		{
			var oldBounds = CollectionView.Bounds;
			return (newBounds.Width != oldBounds.Width);
		}
		#endregion

		#region Methods
		int ShortestColumnIndex()
		{
			int index = 0;
			nfloat shortestHeight = nfloat.MaxValue;
			int n = 0;

			// Scan each column for the shortest height
			foreach (nfloat height in columnHeights) {
				if (height < shortestHeight) {
					shortestHeight = height;
					index = n;
				}
				++n;
			}

			return index;
		}

		int LongestColumnIndex ()
		{
			int index = 0;
			nfloat longestHeight = nfloat.MinValue;
			int n = 0;

			// Scan each column for the shortest height
			foreach (nfloat height in columnHeights) {
				if (height > longestHeight) {
					longestHeight = height;
					index = n;
				}
				++n;
			}

			return index;
		}

		int NextColumnIndexForItem (nint item)
		{
			int index = 0;

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

