using System;
using CoreImage;
using Foundation;
using UIKit;

namespace NonConsumables {
	// WARNING: this is a trivial example of tracking non-consumable
	// in-app purchases. In reality this should be encrypted and
	// possibly even managed remotely on your server (with a strategy for offline use).
	// NSUserDefaults are EASY for iOS users to edit with a little bit of knowledge,
	// which means they could bypass paying for the features.
	// Basically, this is ONLY intended as a demo of the StoreKit code,
	// NOT how you should build a feature-management system for iOS apps.
	public static class HostedProductManager {
		static HostedProductManager ()
		{
		}
		/// <summary>
		/// Each purchase is stored as a boolean user-defaults key-value
		/// </summary>
		/// <remarks>
		/// This is inherently insecure and easily manipulated by end-users,
		/// effectively bypassing the payment mechanism (assuming a little bit 
		/// of technical knowledge). 
		/// </remarks>
		public static void Purchase (string productId) {
			var key = new NSString(productId);
			NSUserDefaults.StandardUserDefaults.SetBool(true, key);
			NSUserDefaults.StandardUserDefaults.Synchronize ();
		}
		public static bool HasPurchased (string productId) {
			var key = new NSString(productId);
			return NSUserDefaults.StandardUserDefaults.BoolForKey (key);
		}

		/// <summary>
		/// Purchaseable feature. This function can't be called until the 
		/// user has purchased AND downloaded the hosted in-app product.
		/// </summary>
		/// <param name="productId">The content is stored in a directory named with the productId</param>
		public static void Read (string productId, UITextView view, UIImageView icon)
		{	
			// determine the path to the content
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
			var targetfolder = System.IO.Path.Combine (documentsPath, productId);

			// the content is _always_ a text file and a png file. the names are constant
			// but they are stored in a directory named for the productId, eg
			// com.xamarin.storekitdoc.monotouchimages
			// com.xamarin.storekitdoc.monotouchfilesystem
			// ... other hosted products that can be added later

			var textFile = System.IO.Path.Combine (targetfolder, "Chapter.txt");
			view.Text = System.IO.File.ReadAllText(textFile);

			var iconFile = System.IO.Path.Combine (targetfolder, "icon.png");
			icon.Image = UIImage.FromFile(iconFile);
		}
	}
}