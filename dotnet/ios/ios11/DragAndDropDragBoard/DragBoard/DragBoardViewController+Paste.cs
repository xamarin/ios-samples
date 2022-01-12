namespace DragBoard;

public partial class DragBoardViewController
{
	/// <summary>
	/// Sets up the gestures to display and dismiss the menu performing the paste operation on the pin board.
	/// </summary>
	void SetupPasteMenu ()
	{
		if (View is null)
			throw new InvalidOperationException ("View");

		var longPressGesture = new UILongPressGestureRecognizer ( (longPress) =>
		{
			if (longPress.State == UIGestureRecognizerState.Began)
			{
				DropPoint = longPress.LocationInView (View);

				// Only show the paste menu if we are
				// not over an image in the pin board.
				if (ImageIndex (DropPoint) < 0)
				{
					View.BecomeFirstResponder ();

					var menu = UIMenuController.SharedMenuController;
					var rect = new CGRect (DropPoint, new CGSize (10, 10));
					menu.ShowMenu (View, rect);
					//menu.SetTargetRect(rect, View);
					//menu.SetMenuVisible (true, true);
				}
			}
			else if (longPress.State == UIGestureRecognizerState.Cancelled)
			{
				UIMenuController.SharedMenuController.HideMenu ();
				//UIMenuController.SharedMenuController.SetMenuVisible (false, true);
			}
		});
		View.AddGestureRecognizer (longPressGesture);

		var tapGesture = new UITapGestureRecognizer ( (obj) =>
		{
			UIMenuController.SharedMenuController.HideMenu ();
			//UIMenuController.SharedMenuController.SetMenuVisible (false, true);
		});
		View.AddGestureRecognizer (tapGesture);
	}
}
