using System;

using Foundation;
using UIKit;

namespace ViewControllerPreview {
	public partial class DetailViewController : UIViewController {
		public string DetailItemTitle { get; set; }

		public override IUIPreviewActionItem[] PreviewActionItems {
			get {
				return PreviewActions;
			}
		}

		IUIPreviewActionItem[] PreviewActions {
			get {
				var action1 = PreviewActionForTitle ("Default Action");
				var action2 = PreviewActionForTitle ("Destructive Action", UIPreviewActionStyle.Destructive);

				var subAction1 = PreviewActionForTitle ("Sub Action 1");
				var subAction2 = PreviewActionForTitle ("Sub Action 2");
				var groupedActions = UIPreviewActionGroup.Create ("Sub Actions…", UIPreviewActionStyle.Default, new [] { subAction1, subAction2 });

				return new IUIPreviewActionItem [] { action1, action2, groupedActions };
			}
		}

		[Export ("initWithCoder:")]
		public DetailViewController (NSCoder coder) : base (coder)
		{
		}

		public override void ViewDidLoad ()
		{
			// Update the user interface for the detail item.
			if (!string.IsNullOrEmpty (DetailItemTitle))
				DetailDescriptionLabel.Text = DetailItemTitle;

			NavigationItem.LeftBarButtonItem = SplitViewController?.DisplayModeButtonItem;
			NavigationItem.LeftItemsSupplementBackButton = true;
		}

		static UIPreviewAction PreviewActionForTitle (string title, UIPreviewActionStyle style = UIPreviewActionStyle.Default)
		{
			return UIPreviewAction.Create (title, style, (action, previewViewController) => {
				var detailViewController  = (DetailViewController)previewViewController;
				var item = detailViewController?.DetailItemTitle;

				Console.WriteLine ("{0} triggered from `DetailViewController` for item: {1}", action.Title, item);
			});
		}
	}
}

