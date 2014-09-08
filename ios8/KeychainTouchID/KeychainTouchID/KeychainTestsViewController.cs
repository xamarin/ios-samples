using System;
using System.Collections.Generic;
using CoreFoundation;
using Foundation;
using ObjCRuntime;
using Security;
using UIKit;
using System.Text;

namespace KeychainTouchID
{
	public partial class KeychainTestsViewController : BasicTestViewController
	{
		public KeychainTestsViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Tests = new List<Test> {
				new Test { Name = Text.ADD_ITEM, Details = Text.ADD_SEC_ITEM, Method = AddItemAsync },
				new Test { Name = Text.QUERY_FOR_ITEM, Details = Text.COPY_SEC_ITEM, Method = CopyMatchingAsync },
				new Test { Name = Text.UPDATE_ITEM, Details = Text.UPDATE_SEC_ITEM, Method = UpdateItemAsync },
				new Test { Name = Text.DELETE_ITEM, Details = Text.DELETE_SEC_ITEM, Method = DeleteItemAsync }
			};

			tableView.WeakDataSource = this;
			tableView.WeakDelegate = this;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			textView.ScrollRangeToVisible (new NSRange (0, textView.Text.Length));
		}

		public override void ViewDidLayoutSubviews ()
		{
			var height = (nfloat)Math.Min (View.Bounds.Size.Height, tableView.ContentSize.Height);
			dynamicViewHeight.Constant = height;
			View.LayoutIfNeeded ();
		}

		void AddItemAsync ()
		{
			var secObject = new SecAccessControl (SecAccessible.WhenPasscodeSetThisDeviceOnly, SecAccessControlCreateFlags.UserPresence);

			if (secObject == null) {
				string message = Text.CANT_CREATE_SEC_OBJ;
				Console.WriteLine (message);
				textView.Text += string.Format (Text.SEC_ITEM_ADD_CAN_CREATE_OBJECT, message);
			}

			var securityRecord = new SecRecord (SecKind.GenericPassword) {
				Service = Text.SERVICE_NAME,
				ValueData = new NSString (Text.SECRET_PASSWORD_TEXT).Encode (NSStringEncoding.UTF8),
				UseNoAuthenticationUI = true,
				AccessControl = secObject
			};

			DispatchQueue.MainQueue.DispatchAsync (() => {
				SecStatusCode status = SecKeyChain.Add (securityRecord);

				var message = string.Format (Text.SEC_ITEM_ADD_STATUS, status.GetDescription ());
				PrintResult (textView, message);
			});
		}

		void CopyMatchingAsync ()
		{
			var securityRecord = new SecRecord (SecKind.GenericPassword) {
				Service = Text.SERVICE_NAME,
				UseOperationPrompt = Text.AUTHENTICATE_TO_ACCESS_SERVICE_PASSWORD
			};

			DispatchQueue.MainQueue.DispatchAsync (() => {
				SecStatusCode status;
				NSData resultData = SecKeyChain.QueryAsData (securityRecord, false, out status);

				var result = resultData != null ? new NSString (resultData, NSStringEncoding.UTF8) : Text.USER_CANCELED_ACTION;

				var sb = new StringBuilder ();
				sb.AppendFormat (Text.SEC_ITEM_COPY_MATCHING_STATUS, status.GetDescription ());
				sb.AppendFormat (Text.RESULT, result);
				PrintResult (textView, sb.ToString ());
			});
		}

		void UpdateItemAsync ()
		{
			var securityRecord = new SecRecord (SecKind.GenericPassword) {
				Service = Text.SERVICE_NAME,
				UseOperationPrompt = Text.AUTH_TO_UPDATE
			};

			var recordUpdates = new SecRecord (SecKind.Identity) {
				ValueData = new NSString (Text.UPDATED_SECRET_PASSWORD_TEXT).Encode (NSStringEncoding.UTF8),
			};

			DispatchQueue.MainQueue.DispatchAsync (() => {
				var status = SecKeyChain.Update (securityRecord, recordUpdates);

				var message = string.Format (Text.SEC_ITEM_UPDATE_STATUS, status.GetDescription ());
				PrintResult (textView, message);
			});
		}

		void DeleteItemAsync ()
		{
			var securityRecord = new SecRecord (SecKind.GenericPassword) {
				Service = Text.SERVICE_NAME
			};

			DispatchQueue.MainQueue.DispatchAsync (() => {
				var status = SecKeyChain.Remove (securityRecord);

				var message = string.Format (Text.SEC_ITEM_DELETE_STATUS, status.GetDescription ());
				PrintResult (textView, message);
			});
		}
	}
}
