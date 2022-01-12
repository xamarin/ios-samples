namespace DragBoard;

public partial class DragBoardViewController : IUIDropInteractionDelegate
{
    [Export("dropInteraction:canHandleSession:")]
    public bool CanHandleSession(UIDropInteraction interaction, IUIDropSession session)
    {
        return session.CanLoadObjects(typeof(UIImage));
    }

    [Export("dropInteraction:sessionDidUpdate:")]
    public UIDropProposal SessionDidUpdate(UIDropInteraction interaction, IUIDropSession session)
    {
        UIDropOperation operation;
        if (session.LocalDragSession == null)
        {
            operation = UIDropOperation.Copy;
        }
        else
        {
            operation = UIDropOperation.Move;
        }
        return new UIDropProposal(operation);
    }

    [Export("dropInteraction:performDrop:")]
    public void PerformDrop(UIDropInteraction interaction, IUIDropSession session)
    {
        if (session.LocalDragSession == null)
        {
            DropPoint = session.LocationInView(interaction.View);
            foreach (var dragItem in session.Items)
            {
                LoadImage (dragItem.ItemProvider, DropPoint);
            }
        }
        else
        {
			MovePoint = session.LocationInView(interaction.View);
        }
    }

    [Export("dropInteraction:previewForDroppingItem:withDefault:")]
    public UITargetedDragPreview GetPreviewForDroppingItem(UIDropInteraction interaction, UIDragItem item, UITargetedDragPreview defaultPreview)
    {
        if (item.LocalObject == null)
        {
            return null;
        }
        else
        {
            DropPoint = defaultPreview.View.Center;
            var target = new UIDragPreviewTarget(View, DropPoint); // HACK: why is this null?
            //return defaultPreview.GetRetargetedPreview(target);
            return defaultPreview;
        }
    }

    [Export("dropInteraction:item:willAnimateDropWithAnimator:")]
    public void WillAnimateDrop(UIDropInteraction interaction, UIDragItem item, IUIDragAnimating animator)
    {
        animator.AddAnimations(() =>{
            FadeItems(new UIDragItem[] { item }, 0f);
        });

        var movePoint = MovePoint; //DropPoint
        animator.AddCompletion( (err) => {
            var index = item.LocalObject as NSNumber;
            if (index != null)
            {
                var i = index.Int32Value;
                if (i >= 0)
                {
                    Views[i].Center = movePoint;
                    Views[i].Alpha = 1f;
                }
            }
        });
    }
}
