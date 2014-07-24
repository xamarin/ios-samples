using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text.RegularExpressions;

using UIKit;
using Foundation;

namespace Quotes
{
	public class MasterViewController : UICollectionViewController
	{
		List<Page> pages;
		string[] titles = new string [5];
		NSString cellName = new NSString ("PagePreview");

		[Export ("initWithNibName:bundle:")]
		public MasterViewController (string nibNameOrNull, NSBundle nibBundleOrNull) : base (nibNameOrNull, nibBundleOrNull)
		{
			titles [0] = "Faust - Erste Szene";
			titles [1] = "Julius Caesar - Antony's speech";
			titles [2] = "Midsummer Night's Dream - Exit speech";
			titles [3] = "Romeo & Juliet - Queen Mab";
			titles [4] = "Troilus and Cressida - Ulysses";

			LoadPagesFromResource ("/Pages");
		}

		// Walk through all pages in the supplied folder in our resources and load the files we find.
		public void LoadPagesFromResource (string pageDirectory)
		{
			pages = new List<Page> ();

			int count = 0;
			foreach (var p in Directory.GetFiles (NSBundle.MainBundle.BundlePath + pageDirectory, "*.xml")) {
				var title = titles [count];
				count++;

				var xdoc = XDocument.Load (p);
				var paragraphs = new List<XElement> (xdoc.Root.Elements ());

				pages.Add (new Page (title, paragraphs));
			}

			Title = "Pages";
		}

		/* Create a collection view with a flow layout. Set up the cell, and assign it to 
		 * our base CollectionView property.
		  */
		public override void LoadView ()
		{
			var layout = new UICollectionViewFlowLayout ();
			layout.MinimumInteritemSpacing = 20;
			layout.ItemSize = new CGSize (200.0f, 290.0f);

			var collectionView = new UICollectionView (UIScreen.MainScreen.ApplicationFrame, layout);

			collectionView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			collectionView.BackgroundColor = UIColor.White;

			var nib = UINib.FromName (cellName, null);
			collectionView.RegisterNibForCell (nib, cellName);

			CollectionView = collectionView;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return pages.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (PagePreview)collectionView.DequeueReusableCell (cellName, indexPath);

			cell.Page = pages.ElementAt ((int)indexPath.Row);

			return cell;
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var pvc = new PageViewController ();

			pvc.controllerPage = pages.ElementAt ((int)indexPath.Row);
	
			NavigationController.PushViewController (pvc, true);
		}
	}
}
