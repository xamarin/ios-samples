using UIKit;
using CoreGraphics;
using System;
using Foundation;


namespace SimpleTextInput
{
	public partial class SimpleTextInputViewController : UIViewController
	{
		EditableCoreTextView editableCoreTextView { get; set; }

		public SimpleTextInputViewController (string nibName, NSBundle bundle) : base (nibName, bundle)
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			//any additional setup after loading the view, typically from a nib.
			//editableCoreTextView = new EditableCoreTextView ();
			editableCoreTextView = new EditableCoreTextView (View.Bounds.Inset (5, 40));
			View.BackgroundColor = UIColor.White;
			View.Add (editableCoreTextView);
			editableCoreTextView.ViewWillEdit += HandleEditableCoreTextViewViewWillEdit;
		}

		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Release any retained subviews of the main view.
			// e.g. this.myOutlet = null;
			editableCoreTextView.ViewWillEdit -= HandleEditableCoreTextViewViewWillEdit;
			editableCoreTextView = null;
		}
		
		// Action method to handle when user presses "Done" button in NavBar.  
		// We want to resignFirstResponder in our EditableCoreTextView and remove
		// the Done button.
		void doneEditingAction ()
		{
			// finish typing text/dismiss the keyboard by removing it as the first responder
			editableCoreTextView.ResignFirstResponder ();
			navigationBar.TopItem.RightBarButtonItem = null; // this will remove the "Done" button
		}
		
		// Protocol method called after EditableCoreTextView has determined that user has
		// invoked "edit" mode (via touching inside EditableCoreTextView).  For this sample
		// we provide a "Done" button at this point that the user can use to finish text
		// editing mode.
		void HandleEditableCoreTextViewViewWillEdit (EditableCoreTextView editableCoreTextView)
		{
			// provide "Done" button to dismiss the keyboard
			UIBarButtonItem doneItem = new UIBarButtonItem (UIBarButtonSystemItem.Done, (a, b) => doneEditingAction ());
			navigationBar.TopItem.RightBarButtonItem = doneItem;
		}
		
	}
}
