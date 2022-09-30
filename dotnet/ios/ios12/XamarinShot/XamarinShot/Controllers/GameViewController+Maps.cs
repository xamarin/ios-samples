namespace XamarinShot;


/// <summary>
/// Maps saving and loading methods for the Game Scene View Controller.
/// </summary>
partial class GameViewController : IUIDocumentPickerDelegate, IWorldMapSelectorDelegate
{
        #region Relocalization Help

        void ConfigureRelocalizationHelp ()
        {
                if (UserDefaults.ShowARRelocalizationHelp)
                {
                        switch (sessionState)
                        {
                                case SessionState.LookingForSurface:
                                case SessionState.PlacingBoard:
                                case SessionState.AdjustingBoard:
                                        ShowRelocalizationHelp (false);
                                        break;

                                case SessionState.LocalizingToBoard:
                                        var anchor = targetWorldMap?.KeyPositionAnchors ()?.FirstOrDefault ();
                                        if (anchor is not null)
                                        {
                                                ShowRelocalizationHelp (true);
                                                SetKeyPositionThumbnail (anchor.Image);
                                        }
                                        break;

                                default:
                                        HideRelocalizationHelp ();
                                        break;
                        }
                } else {
                        HideRelocalizationHelp ();
                }
        }

        void ShowRelocalizationHelp (bool isClient)
        {
                if (!isClient)
                {
                        saveAsKeyPositionButton.Hidden = false;
                }
                else
                {
                        keyPositionThumbnail.Hidden = false;
                        nextKeyPositionThumbnailButton.Hidden = false;
                        previousKeyPositionThumbnailButton.Hidden = false;
                }
        }

        void HideRelocalizationHelp ()
        {
                keyPositionThumbnail.Hidden = true;
                saveAsKeyPositionButton.Hidden = true;
                nextKeyPositionThumbnailButton.Hidden = true;
                previousKeyPositionThumbnailButton.Hidden = true;
        }

        void SetKeyPositionThumbnail (UIImage image)
        {
                keyPositionThumbnail.Image = image;
        }

        partial void saveAsKeyPositionPressed (UIButton sender)
        {
                // Save the current position as an ARAnchor and store it in the worldmap for later use when re-localizing to guide users
                UIImage? image = null;
                OpenTK.NMatrix4? pose = null;
                var mappingStatus = ARWorldMappingStatus.NotAvailable;

                if (sceneView.Session.CurrentFrame is not null)
                {
                        image = sceneView.CreateScreenshot (UIDevice.CurrentDevice.Orientation);
                        pose = sceneView.Session.CurrentFrame.Camera.Transform;
                        mappingStatus = sceneView.Session.CurrentFrame.WorldMappingStatus;
                }

                // Add key position anchor to the scene
                if (pose.HasValue && image is not null)
                {
                        var newKeyPosition = new KeyPositionAnchor (image, pose.Value, mappingStatus);
                        sceneView.Session.AddAnchor (newKeyPosition);
                }
        }

        partial void showNextKeyPositionThumbnail (UIButton sender)
        {
                if (keyPositionThumbnail.Image is not null)
                {
                        // Get the key position anchors from the current world map
                        var keyPositionAnchors = targetWorldMap?.KeyPositionAnchors ();
                        if (keyPositionAnchors is not null)
                        {
                                // Get the current key position anchor displayed
                                var currentKeyPositionAnchor = keyPositionAnchors.FirstOrDefault (anchor => anchor.Image == keyPositionThumbnail.Image);
                                if (currentKeyPositionAnchor is not null)
                                {
                                        var currentIndex = keyPositionAnchors.IndexOf (currentKeyPositionAnchor);
                                        if (currentIndex != -1)
                                        {
                                                var nextIndex = (currentIndex + 1) % keyPositionAnchors.Count;
                                                SetKeyPositionThumbnail (keyPositionAnchors [nextIndex].Image);
                                        }
                                }
                        }
                }
        }

        partial void showPreviousKeyPositionThumbnail (UIButton sender)
        {
                var image = keyPositionThumbnail.Image;
                if (image is not null)
                {
                        // Get the key position anchors from the current world map
                        var keyPositionAnchors = targetWorldMap?.KeyPositionAnchors ();
                        if (keyPositionAnchors is not null)
                        {
                                // Get the current key position anchor displayed
                                var currentKeyPositionAnchor = keyPositionAnchors.FirstOrDefault (anchor => anchor.Image == image);
                                if (currentKeyPositionAnchor is not null)
                                {
                                        var currentIndex = keyPositionAnchors.IndexOf (currentKeyPositionAnchor);
                                        if (currentIndex != -1)
                                        {
                                                var nextIndex = currentIndex;
                                                if (currentIndex == 0 && keyPositionAnchors.Count > 1)
                                                {
                                                        nextIndex = keyPositionAnchors.Count - 1;
                                                } else if (currentIndex - 1 >= 0) {
                                                        nextIndex = currentIndex - 1;
                                                } else {
                                                        nextIndex = 0;
                                                }

                                                SetKeyPositionThumbnail (keyPositionAnchors [nextIndex].Image);
                                        }
                                }
                        }
                }
        }

        #endregion

        #region Saving and Loading Maps

        void ConfigureMappingUI ()
        {
                var showMappingState = sessionState != SessionState.GameInProgress &&
                                       sessionState != SessionState.Setup &&
                                       sessionState != SessionState.LocalizingToBoard &&
                                       UserDefaults.ShowARDebug;
                //TODO:
                mappingStateLabel.Hidden = !showMappingState;
                //this.saveButton.Hidden = this.sessionState == SessionState.setup;
                //this.loadButton.Hidden = this.sessionState == SessionState.setup;
        }

        void UpdateMappingStatus (ARWorldMappingStatus mappingStatus)
        {
                // Check the mapping status of the worldmap to be able to save the worldmap when in a good state
                switch (mappingStatus)
                {
                        case ARWorldMappingStatus.NotAvailable:
                                mappingStateLabel.Text = "Mapping state: Not Available";
                                mappingStateLabel.TextColor = UIColor.Red;
                                saveAsKeyPositionButton.Enabled = false;
                                //this.saveButton.Enabled = false;
                                break;

                        case ARWorldMappingStatus.Limited:
                                mappingStateLabel.Text = "Mapping state: Limited";
                                mappingStateLabel.TextColor = UIColor.Red;
                                saveAsKeyPositionButton.Enabled = false;
                                //this.saveButton.Enabled = false;
                                break;

                        case ARWorldMappingStatus.Extending:
                                mappingStateLabel.Text = "Mapping state: Extending";
                                mappingStateLabel.TextColor = UIColor.Red;
                                saveAsKeyPositionButton.Enabled = false;
                                //this.saveButton.Enabled = false;
                                break;

                        case ARWorldMappingStatus.Mapped:
                                mappingStateLabel.Text = "Mapping state: Mapped";
                                mappingStateLabel.TextColor = UIColor.Green;
                                saveAsKeyPositionButton.Enabled = true;
                                //this.saveButton.Enabled = true;
                                break;
                }
        }

        void GetCurrentWorldMapData (Action<NSData?, NSError?> closure)
        {
                //os_log(.info, "in getCurrentWordMapData")
                // When loading a map, send the loaded map and not the current extended map
                if (targetWorldMap is not null)
                {
                        //os_log(.info, "using existing worldmap, not asking session for a new one.")
                        CompressMap (targetWorldMap, closure);
                } else {
                        //os_log(.info, "asking ARSession for the world map")
                        sceneView.Session.GetCurrentWorldMap ((map, error) =>
                        {
                                //os_log(.info, "ARSession getCurrentWorldMap returned")
                                if (error is not null)
                                {
                                        //os_log(.error, "didn't work! %s", "\(error)")
                                        closure (null, error);
                                }

                                if (map is not null)
                                {
                                        //os_log(.info, "got a worldmap, compressing it")
                                        CompressMap (map, closure);
                                }
                        });
                }
        }

        [Action ("savePressed:")]
        void SavePressed (UIButton sender)
        {
                activityIndicator.StartAnimating ();
                GetCurrentWorldMapData ((data, error) =>
                {
                        DispatchQueue.MainQueue.DispatchAsync (() =>
                        {
                                activityIndicator.StopAnimating ();
                                if (error is not null)
                                {
                                        var title = error.LocalizedDescription;
                                        var message = error.LocalizedFailureReason;
                                        ShowAlert (title, message);
                                        return;
                                }

                                if (data is not null)
                                {
                                        ShowSaveDialog (data);
                                }
                        });
                });
        }

        [Action ("loadPressed:")]
        void LoadPressed (UIButton sender)
        {
                var picker = new UIDocumentPickerViewController (new string [] { "com.apple.xamarin-shot.worldmap" }, UIDocumentPickerMode.Open)
                {
                        AllowsMultipleSelection = false,
                        Delegate = this,
                };
                PresentViewController (picker, true, null);
        }

        void ShowSaveDialog (NSData data)
        {
                var dialog = UIAlertController.Create ("Save World Map", null, UIAlertControllerStyle.Alert);
                dialog.AddTextField ((UITextField) => { });
                var saveAction = UIAlertAction.Create ("Save", UIAlertActionStyle.Default, (action) =>
                {
                        var fileName = dialog.TextFields?.FirstOrDefault ()?.Text;
                        if (!string.IsNullOrEmpty (fileName))
                        {
                                DispatchQueue.GetGlobalQueue (DispatchQueuePriority.Background).DispatchAsync (() =>
                                {
                                        var docs = NSFileManager.DefaultManager.GetUrl (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User, null, true, out NSError error);
                                        if (error is null)
                                        {
                                                var maps = docs.Append ("maps", true);
                                                NSFileManager.DefaultManager.CreateDirectory (maps, true, null, out NSError creationError);
                                                var targetURL = maps.Append (fileName, false).AppendPathExtension ("xamarinshotmap");
                                                data.Save (targetURL, NSDataWritingOptions.Atomic, out NSError? saveingError);
                                                DispatchQueue.MainQueue.DispatchAsync (() =>
                                                {
                                                        ShowAlert ("Saved");
                                                });
                                        } else {
                                                DispatchQueue.MainQueue.DispatchAsync (() =>
                                                {
                                                        ShowAlert (error.LocalizedDescription, null);
                                                });
                                        }
                                });
                        }
                });

                var cancelAction = UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null);

                dialog.AddAction (saveAction);
                dialog.AddAction (cancelAction);

                PresentViewController (dialog, true, null);
        }

        /// <summary>
        /// Get the archived data from a URL Path
        /// </summary>
        void FetchArchivedWorldMap (NSUrl url, Action<NSData?, NSError?> closure)
        {
                DispatchQueue.DefaultGlobalQueue.DispatchAsync (() =>
                {
                        try
                        {
                                _ = url.StartAccessingSecurityScopedResource ();
                                var data = NSData.FromUrl (url);
                                closure (data, null);
                        }
                        catch
                        {
                                // TODO:
                                //DispatchQueue.MainQueue.DispatchAsync(() =>
                                //{
                                //    this.ShowAlert(error.LocalizedDescription);
                                //});

                                //closure(null, error);
                        }
                        finally
                        {
                                 url.StopAccessingSecurityScopedResource ();
                        }
                });
        }

        void CompressMap (ARWorldMap map, Action<NSData?, NSError?> closure)
        {
                DispatchQueue.DefaultGlobalQueue.DispatchAsync (() =>
                {
                        var data = NSKeyedArchiver.ArchivedDataWithRootObject (map, true, out NSError? error);
                        if (error is not null)
                        {
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
                if (selected is not null)
                {
                        FetchArchivedWorldMap (selected, (data, error) =>
                        {
                                if (error is null && data is not null)
                                {
                                        LoadWorldMap (data);
                                }
                        });
                }
        }

        public void DidPickDocument (UIDocumentPickerViewController controller, NSUrl url)
        {
                DidPickDocument (controller, new NSUrl [] { url });
        }

        #endregion

        #region IWorldMapSelectorDelegate

        public void WorldMapSelector (WorldMapSelectorViewController worldMapSelector, Uri selectedMap)
        {
                FetchArchivedWorldMap (selectedMap, (data, error) =>
                {
                        if (error is null && data is not null)
                        {
                                LoadWorldMap (data);
                        }
                });
        }

        #endregion
}
