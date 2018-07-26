using System;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using SceneKit;
using UIKit;

namespace PlacingObjects
{
	public partial class ViewController : IVirtualObjectManagerDelegate, IVirtualObjectSelectionViewControllerDelegate
	{
		public void CouldNotPlace(VirtualObjectManager manager, VirtualObject virtualObject)
		{
			UserFeedback.ShowMessage("Cannot place object\nTry moving left or right.");
		}

		public void DidLoad(VirtualObjectManager manager, VirtualObject virtualObject)
		{
			IsLoadingObject = false;

			//Remove progress indicator
			Spinner.RemoveFromSuperview();
			AddObjectButton.SetImage(UIImage.FromBundle("add"), UIControlState.Normal);
			AddObjectButton.SetImage(UIImage.FromBundle("addPressed"), UIControlState.Highlighted);
		}

		public void WillLoad(VirtualObjectManager manager, VirtualObject virtualObject)
		{
			// Show progress indicator
			Spinner = new UIActivityIndicatorView();
			Spinner.Center = AddObjectButton.Center;
			Spinner.Bounds = new CGRect(0, 0, AddObjectButton.Bounds.Width - 5, AddObjectButton.Bounds.Height - 5);
			AddObjectButton.SetImage(UIImage.FromBundle("buttonring"), UIControlState.Normal);
			SceneView.AddSubview(Spinner);
			Spinner.StartAnimating();
			IsLoadingObject = true;
		}

		public void DidSelectObjectAt(int index)
		{
			if (Session == null || ViewController.CurrentFrame == null)
			{
				return;
			}
			var cameraTransform = ViewController.CurrentFrame.Camera.Transform;

			var definition = VirtualObjectManager.AvailableObjects[index];
			var vo = new VirtualObject(definition);
			var position = FocusSquare != null ? FocusSquare.LastPosition : new SCNVector3(0, 0, -1.0f);
			virtualObjectManager.LoadVirtualObject(vo, position, cameraTransform);
			if (vo.ParentNode == null)
			{
				SceneView.Scene.RootNode.AddChildNode(vo);
			}
		}

		public void DidDeselectObjectAt(int index)
		{
			virtualObjectManager.RemoveVirtualObject(index);
		}
	}
}
