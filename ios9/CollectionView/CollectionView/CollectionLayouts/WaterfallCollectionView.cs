using System;

using UIKit;
using Foundation;

namespace CollectionView {
	/// <summary>
	/// Waterfall collection view.
	/// </summary>
	/// <remarks>
	/// Origionally created by Nicholas Tau on 6/30/14.
	/// Copyright (c) 2014 Nicholas Tau. All rights reserved.
	/// Ported from http://nshint.io/blog/2015/07/16/uicollectionviews-now-have-easy-reordering/ to
	/// Xamarin.iOS by Kevin Mullins.
	/// </remarks>
	[Register("WaterfallCollectionView")]
	public class WaterfallCollectionView : UICollectionView {
		#region Computed Properties
		public WaterfallCollectionSource Source {
			get {
				return (WaterfallCollectionSource)DataSource;
			}
		}
		#endregion

		#region Constructors
		public WaterfallCollectionView (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Initialize
			DataSource = new WaterfallCollectionSource (this);
			Delegate = new WaterfallCollectionDelegate (this);
		}
		#endregion
	}
}

