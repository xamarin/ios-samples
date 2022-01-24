namespace DragBoard;

public partial class DragBoardViewController : IUIDragInteractionDelegate
{
	public UIDragItem[] GetItemsForBeginningSession (UIDragInteraction interaction, IUIDragSession session)
	{
		var point = session.LocationInView (interaction.View);

		var index = ImageIndex (point);
		if (index >= 0) {
			var image = Images[index];
			var itemProvider = new NSItemProvider (image);
			var dragItem = new UIDragItem (itemProvider);

			dragItem.LocalObject = new NSNumber (index);

			return new UIDragItem[] { dragItem };
		}
		return new UIDragItem[0];
	}

	[Export ("dragInteraction:previewForLiftingItem:session:")]
	public UITargetedDragPreview GetPreviewForLiftingItem (UIDragInteraction interaction, UIDragItem item, IUIDragSession session)
	{
		var index = item.LocalObject as NSNumber ?? throw new InvalidOperationException ("index");
		var i = index.Int32Value;
		return new UITargetedDragPreview (Views[i]);
	}

	[Export ("dragInteraction:willAnimateLiftWithAnimator:session:")]
	public void WillAnimateLift (UIDragInteraction interaction, IUIDragAnimating animator, IUIDragSession session)
	{
		animator.AddCompletion ( (position) => {
			if (position == UIViewAnimatingPosition.End) {
				FadeItems (session.Items, 0.5f);
			}
		});
	}

	[Export ("dragInteraction:item:willAnimateCancelWithAnimator:")]
	public void WillAnimateCancel (UIDragInteraction interaction, UIDragItem item, IUIDragAnimating animator)
	{
		animator.AddAnimations ( () =>
		{
			FadeItems (new UIDragItem[] {item}, 1f);
		});
	}

	[Export ("dragInteraction:session:willEndWithOperation:")]
	public void SessionWillEnd (UIDragInteraction interaction, IUIDragSession session, UIDropOperation operation)
	{
		if (operation == UIDropOperation.Copy)
			FadeItems (session.Items, 1f);
	}

	// Returns the index of an image in the pin board
	// at the given point, if any.
	//
	// - Parameter point: the point in the pin board coordinate space.
	// - Returns: The index of an image if the point is over an image in pin board, nothing otherwise.
	public int ImageIndex (CGPoint point)
	{
		if (View is null)
			throw new InvalidOperationException ("View");

		var hitTestView = View.HitTest (point, null);
		if (hitTestView is null)
			return -1;

		// Search for view
		for (int i = 0; i < Views.Count; i++)
		{
			if (Views[i] == hitTestView)
				return i;
		}

		// Not found
		return -1;
	}
}
