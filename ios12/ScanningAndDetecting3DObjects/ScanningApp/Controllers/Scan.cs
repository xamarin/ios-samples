using System;
using Foundation;
using SceneKit;
using ARKit;
using UIKit;
using CoreVideo;
using CoreImage;
namespace ScanningAndDetecting3DObjects
{
	internal class Scan : NSObject, IDisposable
	{
		internal enum ScanState
		{
			Ready,
			DefineBoundingBox,
			Scanning,
			AdjustingOrigin,
		}

		internal static readonly NSString ScanningStateChangedNotificationName = new NSString("ScanningStateChanged");
		internal static readonly NSString StateUserInfoKey = new NSString("ScanState");

		ScanState state = ScanState.Ready;



		internal ScanState State
		{
			get => state;
			set
			{
				Action tooBigOrSmall = () =>
				{
					var title = "Scanned object too big or small";
					var message = @"""
Each dimension of the bounding box should be at least 1 cm and not exceed 5 meters.
In addition, the volume of the bounding box should be at least 500 cubic cm. 
Do you want to go back and adjust the bounding box of the scanned object?""";
					var prevState = state;
					ViewController.Instance?.ShowAlert(title, message, "Yes", true, (_) => this.State = prevState);
				};

				// Check that the preconditions for the state change are met.
				switch (value)
				{
					case ScanState.Ready: break;
					case ScanState.DefineBoundingBox:
						if (!BoundingBoxExists && !GhostBoundingBoxExists)
						{
							Console.WriteLine("Error: Ghost bounding box not yet created.");
							return;
						}
						break;
					case ScanState.Scanning:
						if (!BoundingBoxExists)
						{
							Console.WriteLine("Error: Bounding box not yet created.");
							return;
						}
						if (state == ScanState.DefineBoundingBox && !IsReasonablySized())
						{
							tooBigOrSmall();
							break;
						}
						// When entering the scanning state, take a screenshot of the object to be scanned.
						// This screenshot will later be saved in the *.arbobject file
						CreateScreenshot();
						break;
					case ScanState.AdjustingOrigin:
						if (!BoundingBoxExists)
						{
							Console.WriteLine("Error: Bounding box not yet created.");
							return;
						}
						if (state == ScanState.Scanning && !IsReasonablySized())
						{
							tooBigOrSmall();
							break;
						}
						if (state == ScanState.Scanning && QualityIsLow)
						{
							var title = "Not enough detail";
							var message = $"This scan has not enough detail (it contains {pointCloud.Count} features -- " +
								$"aim for at least {minFeatureCount}).\n" +
								"It is unlikely that a good reference object can be generated.\n" +
								"Do you want to go back and continue the scan?";
							ViewController.Instance?.ShowAlert(title, message, "Yes", true, (_) => State = ScanState.Scanning);
							break;
						}
						if (state == ScanState.Scanning)
						{
							var boundingBox = scannedObject.BoundingBox;
							if (boundingBox != null && boundingBox.ProgressPercentage < 100)
							{
								var title = "Scan not complete";
								var message = $"The object was not scanned from all sides, scanning progress is {boundingBox.ProgressPercentage}%.\n" +
									"It is likely that it won't detect from all angles.\n" +
									"Do you want to go back and continue the scan?";
								ViewController.Instance?.ShowAlert(title, message, "Yes", true, (_) => State = ScanState.Scanning);
							}
						}
						break;
					default: break;
				}

				//Apply the new state
				state = value;

				var userInfo = NSDictionary.FromObjectAndKey(new SimpleBox<ScanState>(state), Scan.StateUserInfoKey);
				var notification = NSNotification.FromName(Scan.ScanningStateChangedNotificationName, this, userInfo);
				NSNotificationCenter.DefaultCenter.PostNotification(notification);
			}
		}

		// The object which we want to scan
		ScannedObject scannedObject;
		internal ScannedObject ScannedObject
		{
			get => scannedObject;
		}

		internal SCNNode ObjectToManipulate
		{
			get
			{
				switch (state)
				{
					case ScanState.AdjustingOrigin: return scannedObject.Origin;
					default: return scannedObject.EitherBoundingBox;
				}
			}
		}

		// The result of this scan
		ARReferenceObject scannedReferenceObject;

		// The node for visualizing the point cloud
		ScannedPointCloud pointCloud;

		ARSCNView sceneView;

		bool isBusyCreatingReferenceObject = false;

		UIImage screenshot = new UIImage();
		internal UIImage Screenshot { get => screenshot; }

		bool hasWarnedAboutLowLight = false;

		const int minFeatureCount = 100;

		internal Scan(ARSCNView scnView)
		{
			this.sceneView = scnView;

			scannedObject = new ScannedObject(sceneView);
			pointCloud = new ScannedPointCloud();

			NSNotificationCenter.DefaultCenter.AddObserver(ViewControllerApplicationState.ApplicationStateChangedNotificationName, ApplicationStateChanged);
			sceneView.Scene.RootNode.AddChildNode(scannedObject);
			sceneView.Scene.RootNode.AddChildNode(pointCloud);
		}

		internal new void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				scannedObject?.RemoveFromParentNode();
				pointCloud?.RemoveFromParentNode();

				scannedObject?.Dispose();
				pointCloud?.Dispose();
			}
		}

		private void ApplicationStateChanged(NSNotification notification)
		{
			var newAppState = notification.UserInfo[ViewControllerApplicationState.AppStateUserInfoKey] as SimpleBox<AppState>;
			switch (newAppState.Value)
			{
				case AppState.Scanning:
					scannedObject.Hidden = false;
					pointCloud.Hidden = false;
					break;
				default:
					scannedObject.Hidden = true;
					pointCloud.Hidden = true;
					break;
			}
		}

		internal void DidOneFingerPan(UIPanGestureRecognizer gesture)
		{
			var gesturePos = gesture.LocationInView(sceneView);
			switch (State)
			{
				case ScanState.Ready:
					State = ScanState.DefineBoundingBox;
					// Recurse (note that state has changed, so DefineBoundingBox clause will execute)
					DidOneFingerPan(gesture);
					break;
				case ScanState.DefineBoundingBox:
				case ScanState.Scanning:
					switch (gesture.State)
					{
						case UIGestureRecognizerState.Possible:
							break;
						case UIGestureRecognizerState.Began:
							scannedObject.BoundingBox?.StartSidePlaneDrag(gesturePos);
							break;
						case UIGestureRecognizerState.Changed:
							scannedObject.BoundingBox?.UpdateSidePlaneDrag(gesturePos);
							break;
						default:
							scannedObject.BoundingBox?.EndSidePlaneDrag();
							break;
					}
					break;
				case ScanState.AdjustingOrigin:
					switch (gesture.State)
					{
						case UIGestureRecognizerState.Possible:
							break;
						case UIGestureRecognizerState.Began:
							scannedObject.Origin?.StartAxisDrag(gesturePos);
							break;
						case UIGestureRecognizerState.Changed:
							scannedObject.Origin?.UpdateAxisDrag(gesturePos);
							break;
						default:
							scannedObject.Origin?.EndAxisDrag();
							break;
					}
					break;
			}
		}

		internal void DidTwoFingerPan(ThresholdPanGestureRecognizer gesture)
		{
			var gestureOffsetPos = gesture.OffsetLocationIn(sceneView);
			Action<Action> RunIfTwoTouch = (fn) =>
			{
				if (gesture.NumberOfTouches == 2) fn();
			};

			switch (State)
			{
				case ScanState.Ready:
					State = ScanState.DefineBoundingBox;
					// Recurse (note that state has changed, so DefineBoundingBox clause will execute)
					DidTwoFingerPan(gesture);
					break;
				case ScanState.DefineBoundingBox:
				case ScanState.Scanning:
					switch (gesture.State)
					{
						case UIGestureRecognizerState.Possible: break;
						case UIGestureRecognizerState.Began:
							RunIfTwoTouch(() => scannedObject.BoundingBox?.StartGroundPlaneDrag(gestureOffsetPos));
							break;
						case UIGestureRecognizerState.Changed:
							if (gesture.ThresholdExceeded)
							{
								RunIfTwoTouch(() => scannedObject.BoundingBox?.UpdateGroundPlaneDrag(gestureOffsetPos));
							}
							break;
						default:
							scannedObject.BoundingBox?.EndGroundPlaneDrag();
							break;
					}
					break;
				case ScanState.AdjustingOrigin:
					switch (gesture.State)
					{
						case UIGestureRecognizerState.Possible: break;
						case UIGestureRecognizerState.Began:
							RunIfTwoTouch(() => scannedObject.Origin?.StartPlaneDrag(gestureOffsetPos));
							break;
						case UIGestureRecognizerState.Changed:
							if (gesture.ThresholdExceeded)
							{
								RunIfTwoTouch(() => scannedObject.Origin?.UpdatePlaneDrag(gestureOffsetPos));
							}
							break;
						default:
							scannedObject.Origin?.EndPlaneDrag();
							break;
					}
					break;
			}
		}

		internal void DidRotate(ThresholdRotationGestureRecognizer gesture)
		{
			switch (State)
			{
				case ScanState.Ready:
					State = ScanState.DefineBoundingBox;
					// Recurse (note that state has changed, so DefineBoundingBox clause will execute)
					DidRotate(gesture);
					break;
				case ScanState.DefineBoundingBox:
				case ScanState.Scanning:
					if (gesture.State == UIGestureRecognizerState.Changed)
					{
						scannedObject.RotateOnYAxis((float)-gesture.RotationDelta);
					}
					break;
				case ScanState.AdjustingOrigin:
					if (gesture.State == UIGestureRecognizerState.Changed)
					{
						scannedObject.Origin?.RotateWithSnappingOnYAxis((float)-gesture.RotationDelta);
					}
					break;
			}
		}

		internal void DidLongPress(UILongPressGestureRecognizer gesture)
		{
			var gesturePos = gesture.LocationInView(sceneView);
			switch (State)
			{
				case ScanState.Ready:
					State = ScanState.DefineBoundingBox;
					// Recurse (note that state has changed, so DefineBoundingBox clause will execute)
					DidLongPress(gesture);
					break;
				case ScanState.DefineBoundingBox:
				case ScanState.Scanning:
					switch (gesture.State)
					{
						case UIGestureRecognizerState.Possible:
							break;
						case UIGestureRecognizerState.Began:
							scannedObject.BoundingBox?.StartSideDrag(gesturePos);
							break;
						case UIGestureRecognizerState.Changed:
							scannedObject.BoundingBox?.UpdateSideDrag(gesturePos);
							break;
						default:
							scannedObject.BoundingBox?.EndSideDrag();
							break;
					}
					break;
				case ScanState.AdjustingOrigin:
					switch (gesture.State)
					{
						case UIGestureRecognizerState.Possible: break;
						case UIGestureRecognizerState.Began:
							scannedObject.Origin?.StartAxisDrag(gesturePos);
							break;
						case UIGestureRecognizerState.Changed:
							scannedObject.Origin?.UpdateAxisDrag(gesturePos);
							break;
						default:
							scannedObject.Origin?.EndAxisDrag();
							break;
					}
					break;
			}
		}

		internal void DidTap(UITapGestureRecognizer gesture)
		{
			var gesturePos = gesture.LocationInView(sceneView);
			switch (State)
			{
				case ScanState.Ready:
					State = ScanState.DefineBoundingBox;
					// Recurse (note that state has changed, so DefineBoundingBox clause will execute)
					DidTap(gesture);
					break;
				case ScanState.DefineBoundingBox:
				case ScanState.Scanning:
					if (gesture.State == UIGestureRecognizerState.Ended)
					{
						scannedObject.CreateOrMoveBoundingBox(gesturePos);
					}
					break;
				case ScanState.AdjustingOrigin:
					if (gesture.State == UIGestureRecognizerState.Ended)
					{
						scannedObject.Origin?.FlashOrReposition(gesturePos);
					}
					break;
				default:
					break;
			}
		}

		internal void DidPinch(ThresholdPinchGestureRecognizer gesture)
		{
			switch (State)
			{
				case ScanState.Ready:
					State = ScanState.DefineBoundingBox;
					// Recurse (note that state has changed, so DefineBoundingBox clause will execute)
					DidPinch(gesture);
					break;
				case ScanState.DefineBoundingBox:
				case ScanState.Scanning:
					switch (gesture.State)
					{
						case UIGestureRecognizerState.Possible:
						case UIGestureRecognizerState.Began:
							break;
						case UIGestureRecognizerState.Changed:
							if (gesture.ThresholdExceeded)
							{
								scannedObject.ScaleBoundingBox((float) gesture.Scale);
								gesture.Scale = 1;
							}
							break;
						default:
							break;
					}
					break;
				case ScanState.AdjustingOrigin:
					switch (gesture.State)
					{
						case UIGestureRecognizerState.Possible:
						case UIGestureRecognizerState.Began:
							break;
						case UIGestureRecognizerState.Changed:
							if (gesture.ThresholdExceeded)
							{
								scannedObject.Origin?.UpdateScale((float) gesture.Scale);
								gesture.Scale = 1;
							}
							break;
						default:
							break;
					}
					break;
			}
		}

		internal void UpdateOnEveryFrame(ARFrame frame)
		{
			// Note: unlike gesture recognizer handlers above, this function is not structured with a switch, since multiple clauses execute 

			if (State == ScanState.Ready || State == ScanState.DefineBoundingBox)
			{
				if (frame.RawFeaturePoints != null)
				{
					//Automatically adjust the size of the bounding box 
					scannedObject.FitOverPointCloud(frame.RawFeaturePoints);
				}
			}

			if (State == ScanState.Ready || State == ScanState.DefineBoundingBox || State == ScanState.Scanning)
			{
				if (frame.LightEstimate != null)
				{
					var lightEstimate = frame.LightEstimate;
					if (lightEstimate.AmbientIntensity < 500 && !hasWarnedAboutLowLight)
					{
						hasWarnedAboutLowLight = true;
						var title = "Too dark for scanning";
						var message = "Consider moving to an environment with more light.";
						ViewController.Instance.ShowAlert(title, message);
					}
				}

				// Try a preliminary creation of the reference object based off the current 
				// bounding box & update the point cloud visualization based on that. 
				if (scannedObject.EitherBoundingBox != null)
				{
					var boundingBox = scannedObject.EitherBoundingBox;
					// Note: Creating the reference object is asynchronous and likely
					// takes some time to complete. Avoid calling it again while we 
					// still wait for the previous call to complete
					if (!isBusyCreatingReferenceObject)
					{
						isBusyCreatingReferenceObject = true;
						scannedObject.LockBoundingBoxForReferenceObjectCreation(isBusyCreatingReferenceObject);
						sceneView.Session.CreateReferenceObject(boundingBox.WorldTransform.ToNMatrix4(), new OpenTK.NVector3(), boundingBox.Extent, (referenceObject, error) =>
						{
							// Ignoring error because most of the time, it's a "Too few features" message (i.e., not an exception, just an error). 
							if (referenceObject != null)
							{
								pointCloud.Update(referenceObject.RawFeaturePoints, boundingBox);
							}
							isBusyCreatingReferenceObject = false;
							scannedObject.LockBoundingBoxForReferenceObjectCreation(isBusyCreatingReferenceObject);
						});
					}
				}
			}

			// Update bounding box side coloring to visualize scanning coverage
			if (State == ScanState.Scanning)
			{
				scannedObject.BoundingBox?.HighlightCurrentTitle();
				scannedObject.BoundingBox?.UpdateCapturingProcess();
			}

			scannedObject.UpdateOnEveryFrame();
			pointCloud.UpdateOnEveryFrame();
		}

		bool QualityIsLow => pointCloud.Count < Scan.minFeatureCount;

		internal bool BoundingBoxExists => scannedObject.BoundingBox != null;

		internal bool GhostBoundingBoxExists => scannedObject.GhostBoundingBox != null;

		bool IsReasonablySized()
		{
			if (scannedObject.BoundingBox == null)
			{
				return false;
			}

			var boundingBox = scannedObject.BoundingBox;
			// The bounding box should not be too small and not too large.
			// Note: 3D object detection is optimized for tabletop scenarios.
			Func<double, bool> validSizeRange = (sz) => sz > 0.01 && sz < 5.0;
			if (validSizeRange(boundingBox.Extent.X)
			   && validSizeRange(boundingBox.Extent.Y)
			   && validSizeRange(boundingBox.Extent.Z))
			{
				// Check that the volume of the bounding box is at least 500 cubic centimeters.
				var volume = boundingBox.Extent.X * boundingBox.Extent.Y * boundingBox.Extent.Z;
				return volume >= 0.0005;
			}

			return false;
		}

		internal void CreateReferenceObject(Action<ARReferenceObject> creationFinished)
		{
			if (scannedObject.BoundingBox == null || scannedObject.Origin == null)
			{
				Console.WriteLine("Error: No bounding box or object origin present.");
				creationFinished(null);
				return;
			}
			var boundingBox = scannedObject.BoundingBox;
			var origin = scannedObject.Origin;

			// Extract the reference object based on the position & orientation of the bounding box.
			sceneView.Session.CreateReferenceObject(boundingBox.WorldTransform.ToNMatrix4(), new OpenTK.NVector3(), boundingBox.Extent, (referenceObject, err) =>
			{
				if (referenceObject != null)
				{
					// Adjust the object's origin with the user-provided transform
					scannedReferenceObject = referenceObject.ApplyTransform(origin.Transform.ToNMatrix4());
					scannedReferenceObject.Name = scannedObject.ScanName;
					creationFinished(scannedReferenceObject);
				}
				else
				{
					Console.WriteLine($"Error: Failed to create reference object. ({err.LocalizedDescription})");
					creationFinished(null);
				}
			});
		}

		void CreateScreenshot()
		{
			if (sceneView.Session.CurrentFrame == null)
			{
				Console.WriteLine("Error: Failed to create a screenshot - no current ARFrame exists.");
				return;
			}
			var frame = sceneView.Session.CurrentFrame;

			var orientation = UIImageOrientation.Right;
			switch (UIDevice.CurrentDevice.Orientation)
			{
				case UIDeviceOrientation.Portrait: orientation = UIImageOrientation.Right; break;
				case UIDeviceOrientation.PortraitUpsideDown: orientation = UIImageOrientation.Left; break;
				case UIDeviceOrientation.LandscapeLeft: orientation = UIImageOrientation.Up; break;
				case UIDeviceOrientation.LandscapeRight: orientation = UIImageOrientation.Down; break;
				default: break;
			}

			var ciImage = new CIImage(frame.CapturedImage);
			using (var ctxt = new CIContext(new CIContextOptions()))
			{
				using (var cgImage = ctxt.CreateCGImage(ciImage, ciImage.Extent))
				{
					screenshot = UIImage.FromImage(cgImage, 1.0f, orientation);
				}
			}
		}
	}
}
