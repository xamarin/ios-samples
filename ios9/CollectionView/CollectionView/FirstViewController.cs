using System;
using UIKit;
using System.Collections.Generic;

namespace CollectionView
{
	/// <summary>
	/// First view controller.
	/// </summary>
	/// <remarks>
	/// Origionally created by Wojciech Łukaszuk on 7/16/15.
	/// Copyright (c) 2015 Wojciech Łukaszuk. Under The MIT License (MIT).
	/// Ported from http://nshint.io/blog/2015/07/16/uicollectionviews-now-have-easy-reordering/ to
	/// Xamarin.iOS by Kevin Mullins.
	/// </remarks>
	public partial class FirstViewController : UICollectionViewController
	{
		
		#region Constructors
		public FirstViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
		#endregion
	}
}

