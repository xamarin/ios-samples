
namespace XamarinShot {
	using ARKit;
	using CoreFoundation;
	using Foundation;
	using XamarinShot.Models;
	using XamarinShot.Models.Enums;
	using XamarinShot.Utils;
	using System;
	using System.Linq;
	using UIKit;

	/// <summary>
	/// Maps saving and loading methods for the Game Scene View Controller.
	/// </summary>
	partial class GameViewController : IUIDocumentPickerDelegate, IWorldMapSelectorDelegate {
		#region Relocalization Help

		private void ConfigureRelocalizationHelp ()
		{
			if (UserDefaults.ShowARRelocalizationHelp) {
				switch (this.sessionState) {
				case SessionState.LookingForSurface:
				case SessionState.PlacingBoard:
				case SessionState.AdjustingBoard:
					this.ShowRelocalizationHelp (false);
					break;

				case SessionState.LocalizingToBoard:
					var anchor = this.targetWorldMap?.KeyPositionAnchors ()?.FirstOrDefault ();
					if (anchor != null) {
						this.ShowRelocalizationHelp (true);
						this.SetKeyPositionThumbnail (anchor.Image);
					}
					break;

				default:
					this.HideRelocalizationHelp ();
					break;
				}
			} else {
				this.HideRelocalizationHelp ();
			}
		}

		private void ShowRelocalizationHelp (bool isClient)
		{
			if (!isClient) {
				this.saveAsKeyPositionButton.Hidden = false;
			} else {
				this.keyPositionThumbnail.Hidden = false;
				this.nextKeyPositionThumbnailButton.Hidden = false;
				this.previousKeyPositionThumbnailButton.Hidden = false;
			}
		}

		private void HideRelocalizationHelp ()
		{
			this.keyPositionThumbnail.Hidden = true;
			this.saveAsKeyPositionButton.Hidden = true;
			this.nextKeyPositionThumbnailButton.Hidden = true;
			this.previousKeyPositionThumbnailButton.Hidden = true;
		}

		private void SetKeyPositionThumbnail (UIImage image)
		{
			this.keyPositionThumbnail.Image = image;
		}

		partial void saveAsKeyPositionPressed (UIButton sender)
		{
			// Save the current position as an ARAnchor and store it in the worldmap for later use when re-localizing to guide users
			UIImage image = null;
			OpenTK.NMatrix4? pose = null;
			var mappingStatus = ARWorldMappingStatus.NotAvailable;

			if (this.sceneView.Session.CurrentFrame != null) {
				image = this.sceneView.CreateScreenshot (UIDevice.CurrentDevice.Orientation);
				pose = this.sceneView.Session.CurrentFrame.Camera.Transform;
				mappingStatus = this.sceneView.Session.CurrentFrame.WorldMappingStatus;
			}

			// Add key position anchor to the scene
			if (pose.HasValue && image != null) {
				var newKeyPosition = new KeyPositionAnchor (image, pose.Value, mappingStatus);
				this.sceneView.Session.AddAnchor (newKeyPosition);
			}
		}

		partial void showNextKeyPositionThumbnail (UIButton sender)
		{
			if (this.keyPositionThumbnail.Image != null) {
				// Get the key position anchors from the current world map
				var keyPositionAnchors = this.targetWorldMap?.KeyPositionAnchors ();
				if (keyPositionAnchors != null) {
					// Get the current key position anchor displayed
					var currentKeyPositionAnchor = keyPositionAnchors.FirstOrDefault (anchor => anchor.Image == this.keyPositionThumbnail.Image);
					if (currentKeyPositionAnchor != null) {
						var currentIndex = keyPositionAnchors.IndexOf (currentKeyPositionAnchor);
						if (currentIndex != -1) {
							var nextIndex = (currentIndex + 1) % keyPositionAnchors.Count;
							this.SetKeyPositionThumbnail (keyPositionAnchors [nextIndex].Image);
						}
					}
				}
			}
		}

		partial void showPreviousKeyPositionThumbnail (UIButton sender)
		{
			var image = this.keyPositionThumbnail.Image;
			if (image != null) {
				// Get the key position anchors from the current world map
				var keyPositionAnchors = this.targetWorldMap?.KeyPositionAnchors ();
				if (keyPositionAnchors != null) {
					// Get the current key position anchor displayed
					var currentKeyPositionAnchor = keyPositionAnchors.FirstOrDefault (anchor => anchor.Image == image);
					if (currentKeyPositionAnchor != null) {
						var currentIndex = keyPositionAnchors.IndexOf (currentKeyPositionAnchor);
						if (currentIndex != -1) {
							var nextIndex = currentIndex;
							if (currentIndex == 0 && keyPositionAnchors.Count > 1) {
								nextIndex = keyPositionAnchors.Count - 1;
							} else if (currentIndex - 1 >= 0) {
								nextIndex = currentIndex - 1;
							} else {
								nextIndex = 0;
							}

							this.SetKeyPositionThumbnail (keyPositionAnchors [nextIndex].Image);
						}
					}
				}
			}
		}

		#endregion

		#region Saving and Loading Maps

		private void ConfigureMappingUI ()
		{
			var showMappingState = this.sessionState != SessionState.GameInProgress &&
								   this.sessionState != SessionState.Setup &&
								   this.sessionState != SessionState.LocalizingToBoard &&
								   UserDefaults.ShowARDebug;
			//TODO:
			this.mappingStateLabel.Hidden = !showMappingState;
			//this.saveButton.Hidden = this.sessionState == SessionState.setup;
			//this.loadButton.Hidden = this.sessionState == SessionState.setup;
		}

		private void UpdateMappingStatus (ARWorldMappingStatus mappingStatus)
		{
			// Check the mapping status of the worldmap to be able to save the worldmap when in a good state
			switch (mappingStatus) {
			case ARWorldMappingStatus.NotAvailable:
				this.mappingStateLabel.Text = "Mapping state: Not Available";
				this.mappingStateLabel.TextColor = UIColor.Red;
				this.saveAsKeyPositionButton.Enabled = false;
				//this.saveButton.Enabled = false;
				break;

			case ARWorldMappingStatus.Limited:
				this.mappingStateLabel.Text = "Mapping state: Limited";
				this.mappingStateLabel.TextColor = UIColor.Red;
				this.saveAsKeyPositionButton.Enabled = false;
				//this.saveButton.Enabled = false;
				break;

			case ARWorldMappingStatus.Extending:
				this.mappingStateLabel.Text = "Mapping state: Extending";
				this.mappingStateLabel.TextColor = UIColor.Red;
				this.saveAsKeyPositionButton.Enabled = false;
				//this.saveButton.Enabled = false;
				break;

			case ARWorldMappingStatus.Mapped:
				this.mappingStateLabel.Text = "Mapping state: Mapped";
				this.mappingStateLabel.TextColor = UIColor.Green;
				this.saveAsKeyPositionButton.Enabled = true;
				//this.saveButton.Enabled = true;
				break;
			}
		}

		private void GetCurrentWorldMapData (Action<NSData, NSError> closure)
		{
			//os_log(.info, "in getCurrentWordMapData")
			// When loading a map, send the loaded map and not the current extended map
			if (this.targetWorldMap != null) {
				//os_log(.info, "using existing worldmap, not asking session for a new one.")
				this.CompressMap (this.targetWorldMap, closure);
			} else {
				//os_log(.info, "asking ARSession for the world map")
				this.sceneView.Session.GetCurrentWorldMap ((map, error) => {
					//os_log(.info, "ARSession getCurrentWorldMap returned")
					if (error != null) {
						//os_log(.error, "didn't work! %s", "\(error)")
						closure (null, error);
					}

					if (map != null) {
						//os_log(.info, "got a worldmap, compressing it")
						this.CompressMap (map, closure);
					}
				});
			}
		}

		[Action ("savePressed:")]
		private void SavePressed (UIButton sender)
		{
			this.activityIndicator.StartAnimating ();
			this.GetCurrentWorldMapData ((data, error) => {
				DispatchQueue.MainQueue.DispatchAsync (() => {
					this.activityIndicator.StopAnimating ();
					if (error != null) {
						var title = error.LocalizedDescription;
						var message = error.LocalizedFailureReason;
						this.ShowAlert (title, message);
						return;
					}

					if (data != null) {
						this.ShowSaveDialog (data);
					}
				});
			});
		}

		[Action ("loadPressed:")]
		private void LoadPressed (UIButton sender)
		{
			var picker = new UIDocumentPickerViewController (new string [] { "com.apple.xamarin-shot.worldmap" }, UIDocumentPickerMode.Open) {
				AllowsMultipleSelection = false,
				Delegate = this,
			};
			this.PresentViewController (picker, true, null);
		}

		private void ShowSaveDialog (NSData data)
		{
			var dialog = UIAlertController.Create ("Save World Map", null, UIAlertControllerStyle.Alert);
			dialog.AddTextField (null);
			var saveAction = UIAlertAction.Create ("Save", UIAlertActionStyle.Default, (action) => {
				var fileName = dialog.TextFields?.FirstOrDefault ()?.Text;
				if (!string.IsNullOrEmpty (fileName)) {
					DispatchQueue.GetGlobalQueue (DispatchQueuePriority.Background).DispatchAsync (() => {
						var docs = NSFileManager.DefaultManager.GetUrl (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User, null, true, out NSError error);
						if (error == null) {
							var maps = docs.Append ("maps", true);
							NSFileManager.DefaultManager.CreateDirectory (maps, true, null, out NSError creationError);
							var targetURL = maps.Append (fileName, false).AppendPathExtension ("xamarinshotmap");
							data.Save (targetURL, NSDataWritingOptions.Atomic, out NSError saveingError);
							DispatchQueue.MainQueue.DispatchAsync (() => {
								this.ShowAlert ("Saved");
							});
						} else {
							DispatchQueue.MainQueue.DispatchAsync (() => {
								this.ShowAlert (error.LocalizedDescription, null);
							});
						}
					});
				}
			});

			var cancelAction = UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null);

			dialog.AddAction (saveAction);
			dialog.AddAction (cancelAction);

			this.PresentViewController (dialog, true, null);
		}

		/// <summary>
		/// Get the archived data from a URL Path
		/// </summary>
		private void FetchArchivedWorldMap (NSUrl url, Action<NSData, NSError> closure)
		{
			DispatchQueue.DefaultGlobalQueue.DispatchAsync (() => {
				try {
					_ = url.StartAccessingSecurityScopedResource ();
					var data = NSData.FromUrl (url);
					closure (data, null);
				} catch {
					// TODO:
					//DispatchQueue.MainQueue.DispatchAsync(() =>
					//{
					//    this.ShowAlert(error.LocalizedDescription);
					//});

					//closure(null, error);
				} finally {
					url.StopAccessingSecurityScopedResource ();
				}
			});
		}

		private void CompressMap (ARWorldMap map, Action<NSData, NSError> closure)
		{
			DispatchQueue.DefaultGlobalQueue.DispatchAsync (() => {
				var data = NSKeyedArchiver.ArchivedDataWithRootObject (map, true, out NSError error);
				if (error == null) {
					closure (data, null);
				} else {
					// archiving failed 
					closure (null, error);
				}
			});
		}

		#endregion

		#region UIDocumentPickerDelegate

		[Export ("documentPicker:didPickDocumentsAtURLs:")]
		public void DidPickDocument (UIDocumentPickerViewController controller, NSUrl [] urls)
		{
			var selected = urls.FirstOrDefault ();
			if (selected != null) {
				this.FetchArchivedWorldMap (selected, (data, error) => {
					if (error == null && data != null) {
						this.LoadWorldMap (data);
					}
				});
			}
		}

		public void DidPickDocument (UIDocumentPickerViewController controller, NSUrl url)
		{
			this.DidPickDocument (controller, new NSUrl [] { url });
		}

		#endregion

		#region IWorldMapSelectorDelegate

		public void WorldMapSelector (WorldMapSelectorViewController worldMapSelector, Uri selectedMap)
		{
			this.FetchArchivedWorldMap (selectedMap, (data, error) => {
				if (error == null && data != null) {
					this.LoadWorldMap (data);
				}
			});
		}

		#endregion
	}
}
