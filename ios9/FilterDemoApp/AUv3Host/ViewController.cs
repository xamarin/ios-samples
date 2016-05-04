using System;

using AVFoundation;
using Foundation;
using UIKit;

using FilterDemoFramework;

namespace AUv3Host {
	public partial class ViewController : UIViewController, IUITableViewDataSource, IUITableViewDelegate {
		SimplePlayEngine playEngine;

		UIViewController childViewController;
		UIView AudioUnitView {
			get {
				return childViewController.View;
			}
		}

		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			playEngine = new SimplePlayEngine (
				AudioUnitTableView.ReloadData
			);
		}

		[Export ("tableView:numberOfRowsInSection:")]
		public nint RowsInSection (UITableView tableView, nint section)
		{
			if (tableView == AudioUnitTableView)
				return playEngine.AvailableEffects.Length + 1;

			return (tableView == PresetTableView) ? playEngine.PresetList.Length : 0;
		}

		[Export ("tableView:cellForRowAtIndexPath:")]
		public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell ("cell", indexPath);
			if (tableView == AudioUnitTableView) {
				if (indexPath.Row > 0) {
					var component = playEngine.AvailableEffects [indexPath.Row - 1];
					cell.TextLabel.Text = string.Format ("{0} {1}", component.Name, component.ManufacturerName);
				} else
					cell.TextLabel.Text = "(No effect)";

				return cell;
			}

			if (tableView == PresetTableView) {
				cell.TextLabel.Text = playEngine.PresetList [indexPath.Row].Name;
				return cell;
			}

			throw new Exception ("This index path doesn't make sense for this table view.");
		}

		[Export ("tableView:didSelectRowAtIndexPath:")]
		public void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var row = indexPath.Row;

			if (tableView == AudioUnitTableView) {
				Action completionHandler = PresetTableView.ReloadData;
				AVAudioUnitComponent component = row > 0 ? playEngine.AvailableEffects [row - 1] : null;

				playEngine.SelectEffectComponent (component, completionHandler);

				RemoveChildController ();
				NoViewLabel.Hidden = false;
			} else if (tableView == PresetTableView) {
				playEngine.SelectPresetIndex (row);
			}
		}

		partial void TogglePlay (UIButton sender)
		{
			var isPlaying = playEngine.TogglePlay ();
			PlayButton.SetTitle (isPlaying ? "Stop" : "Play", UIControlState.Normal);
		}

		partial void ToggleView (UIButton sender)
		{
			if (RemoveChildController ())
				return;

			playEngine.AudioUnit?.RequestViewController (viewController => {
				if (viewController == null || viewController.View == null) {
					NoViewLabel.Hidden = false;
					return;
				}

				var view = viewController.View;
				AddChildViewController (viewController);
				view.Frame = ViewContainer.Bounds;

				ViewContainer.AddSubview (view);
				childViewController = viewController;

				viewController.DidMoveToParentViewController (this);
				NoViewLabel.Hidden = true;
			});
		}

		bool RemoveChildController ()
		{
			if (childViewController == null || AudioUnitView == null)
				return false;

			childViewController.WillMoveToParentViewController (null);
			AudioUnitView.RemoveFromSuperview ();
			childViewController.RemoveFromParentViewController ();
			childViewController = null;

			return true;
		}
	}
}

