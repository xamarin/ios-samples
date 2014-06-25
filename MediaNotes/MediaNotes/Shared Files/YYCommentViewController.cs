using System;
using System.Collections.Generic;
using CoreGraphics;

using Foundation;
using UIKit;


namespace MediaNotes
{
	public partial class YYCommentViewController : UIViewController
	{
		bool toolBarIsvisible;
	    UIPopoverController shareController;
		public PhotoViewController associatedObject { get; set;}

		public YYCommentViewController ()
		{
		}

		public YYCommentViewController (string name, NSBundle bundle) : base (name, bundle)
		{
			toolBarIsvisible = true;
		}
	
		public override void ViewDidLoad ()
		{
			// Perform any additional setup after loading the view, typically from a nib.
			base.ViewDidLoad ();
			//check
			textView.BackgroundColor  = UIColor.FromWhiteAlpha (.25f, .75f);
			textView.TextColor = UIColor.White;
			textView.Editable = false;
			View.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleBottomMargin;
		}

		public string Comment ()
		{
			return associatedObject.AssociatedComment ();
		}

		public void SetComment (string comment)
		{
			associatedObject.AssociatedCommentDidChange (comment);
		}

		public void ToggleToolbar (UIGestureRecognizer gesturerecognizer)
		{
			ShowToolbar (!toolBarIsvisible, true, 0.5f);
		}

		public void setEditing (bool flag)
		{
			textView.Editable = flag;
		}

		//Delegate and callback methods
		public override void ViewWillAppear (bool animated)
		{
			//Console.WriteLine("{0}:{1}",this, char cmd);
		    textView.Text = Comment();
		}

		public override void ViewWillDisappear (bool animated)
		{
			//	Console.WriteLine (String.Format ("{0} : {1}",
			//	                                   this,cmd));
		}

		public override void ViewDidAppear (bool animated)
		{
			//	Console.WriteLine (String.Format ("{0} : {1}",
			//	                                   this,cmd));
		}

		public override void ViewDidDisappear (bool animated)
		{
			//	Console.WriteLine (String.Format ("{0} : {1}",
			//	                                   this,cmd));
		}


		public void TextViewDidEndEditing (UITextView textView)
		{
			if (!Comment ().Equals(textView.Text)){
				SetComment(textView.Text);
			}
			setEditing(true);
		}

		//YYCommentViewController API
		
		public void ShowToolbar (bool show, bool animated, float duration)
		{
			UIView.Animate (.5, () => {
				toolbar.Alpha = show ? 1.0f : 0.0f;
				toolBarIsvisible = true;
			}, () =>{
				if (show) {
					View.AddSubview (toolbar);
					toolBarIsvisible = true;
				} else {
					toolbar.RemoveFromSuperview ();
					toolBarIsvisible = false;
				}
			});
		}

		public void AssociatedObjectDidChange (PhotoViewController obj)
		{
			if (obj != associatedObject) {
				associatedObject = obj;
			} 
			textView.Text = obj.AssociatedComment();
			Console.WriteLine (textView.Text);
		}

		partial void enableTextEditing (UIKit.UIBarButtonItem sender)
		{
			if (textView.Editable) {
				setEditing(false);
				textView.ResignFirstResponder();
			}
			else {
				setEditing(true);
				textView.BecomeFirstResponder();
			}
		}

		partial void share (UIKit.UIBarButtonItem sender)
		{
			if(shareController == null){
				List<UIImage> items = associatedObject.ItemsForSharing();
				NSObject [] itemsForSharing = new NSObject [(items == null? 0 : items.Count) + 1];
            	int i = 0;
				if (items!= null) {
					for (i = 0; i < items.Count; i++)
						itemsForSharing [i] = items [i];
				}
				itemsForSharing [i] = new NSString (Comment ());
			
			UIActivityViewController activityController = new UIActivityViewController(itemsForSharing, null);
			shareController = new UIPopoverController(activityController);
			shareController.Delegate = new MyDelegate(this);
			shareController.PresentFromBarButtonItem(shareButton, UIPopoverArrowDirection.Any, true);
			}
			else{
				shareController.Dismiss(true);
				shareController = null;
				}
		}

		partial void shootPicture (UIKit.UIBarButtonItem sender)
		{
			UIImagePickerController picker = new UIImagePickerController();

			picker.SourceType = UIImagePickerControllerSourceType.Camera;
			picker.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
			PresentViewController(picker, true, null);
		}

		class MyDelegate : UIPopoverControllerDelegate
		{
			YYCommentViewController _parent;

			public MyDelegate (YYCommentViewController parent)
			{
				_parent = parent;
			}

		    public override void DidDismiss (UIPopoverController popoverController)
			{
				_parent.shareController = null;
			}
		}
	}
}