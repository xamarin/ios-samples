namespace DragBoard;

/// <summary>
/// A custom view that can handle paste events.
/// </summary>
public partial class DragBoardView : UIView
{
	public DragBoardView (IntPtr handle) : base (handle)
	{
	}
		
	public override void AwakeFromNib ()
	{
		base.AwakeFromNib ();
		BackgroundColor = UIImage.FromBundle ("Cork") is not null ? UIColor.FromPatternImage (UIImage.FromBundle ("Cork")!) : UIColor.Blue;
	}

	public override bool CanBecomeFirstResponder
	{
		get => true;
	}
}
