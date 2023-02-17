using Foundation;
using PeekPopNavigation.Models;
using System;
using UIKit;

namespace PeekPopNavigation {
	/// <summary>
	/// A view controller to show a preview of a color and provide two alternative techniques for
	/// starring/unstarring and deleting it.The first technique is an action method linked from the 
	/// navigation bar in the storyboard and the second is to support Peek Quick Actions by overriding the previewActionItems property.
	/// </summary>
	public partial class ColorItemViewController : UIViewController {
		public ColorItemViewController (IntPtr handle) : base (handle) { }

		public ColorData ColorData { get; set; }

		public ColorItem ColorItem { get; set; }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			base.Title = this.ColorItem.Name;
			base.View.BackgroundColor = this.ColorItem.Color;
			this.starButton.Title = this.GetStarButtonTitle ();
		}

		partial void TriggerStar (NSObject sender)
		{
			this.ColorItem.Starred = !this.ColorItem.Starred;
			this.starButton.Title = this.GetStarButtonTitle ();
		}

		partial void Delete (NSObject sender)
		{
			this.ColorData.Delete (this.ColorItem);

			// The color no longer exists so dismiss this view controller.
			base.NavigationController.PopViewController (true);
		}

		private string GetStarButtonTitle ()
		{
			return this.ColorItem.Starred ? "Unstar" : "Star";
		}

		#region Supporting Peek Quick Actions

		public override IUIPreviewActionItem [] PreviewActionItems {
			get {
				var starAction = UIPreviewAction.Create (this.GetStarButtonTitle (),
														UIPreviewActionStyle.Default,
														(_, __) => {
															this.ColorItem.Starred = !this.ColorItem.Starred;
														});

				var deleteAction = UIPreviewAction.Create ("Delete",
														  UIPreviewActionStyle.Destructive,
														  (_, __) => {
															  this.ColorData.Delete (this.ColorItem);
														  });

				return new IUIPreviewActionItem [] { starAction, deleteAction };
			}
		}

		#endregion
	}
}
