using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CoreGraphics;
using CoreAnimation;
using SceneKit;
using ARKit;
using CoreFoundation;
using Photos;

namespace PlacingObjects
{

	public class SessionDelegateCallbackProxy : ARSessionDelegate
	{
		ViewController owner;

		public SessionDelegateCallbackProxy(ViewController owner)
		{
			this.owner = owner;
		}

		public override void CameraDidChangeTrackingState(ARSession session, ARCamera camera)
		{
			owner.SessionDelegate_CameraDidChangeTrackingState(session, camera);
		}

		public override void DidFail(ARSession session, NSError error)
		{
			owner.SessionDelegate_DidFail(session, error);
		}

		public override void WasInterrupted(ARSession session)
		{
			owner.SessionDelegate_WasInterrupted(session);
		}

		public override void InterruptionEnded(ARSession session)
		{
			owner.SessionDelegate_InterruptionEnded(session);
		}
	}

	public partial class ViewController : UIViewController, ISCNSceneRendererDelegate, IARSessionObserver, IVirtualObjectSelectionViewControllerDelegate
	{
		private bool isLoadingObject = false;

		public CGPoint? ScreenCenter { get; protected set; }
		public TextManager UserFeedback { get; set; }
		public bool DragOnInfinitePlanesEnabled { get; set; } = false;

		public Dictionary<ARPlaneAnchor, Plane> Planes { get; set; } = new Dictionary<ARPlaneAnchor, Plane>();

		public ARSession Session { get; set; } = new ARSession();
		public ARWorldTrackingConfiguration worldTrackingConfig { get; set; } = new ARWorldTrackingConfiguration();
		public VirtualObject DisplayedObject { get; set; }
		public FocusSquare FocusSquare { get; set; }
		protected UIActivityIndicatorView Spinner { get; set; }

		protected VirtualObjectManager virtualObjectManager { get; set; }
		protected DispatchQueue serialQueue { get; set; }

		static private ARFrame currentFrame;
		static public ARFrame CurrentFrame
		{
			get => currentFrame;

			private set
			{
				if (currentFrame != null)
				{
					currentFrame.Dispose();
				}
				currentFrame = value;
			}
		}


		public bool IsLoadingObject
		{
			get { return isLoadingObject; }
			set
			{
				isLoadingObject = value;

				SettingsButton.Enabled = !isLoadingObject;
				AddObjectButton.Enabled = !isLoadingObject;
				RestartExperienceButton.Enabled = !isLoadingObject;
			}
		}

		protected ViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			AppSettings.RegisterDefaults();
			SetupUIControls();
			SetupScene();
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			// Prevent the screen from being dimmed after awhile
			UIApplication.SharedApplication.IdleTimerDisabled = true;

			if (ARWorldTrackingConfiguration.IsSupported)
			{
				// Start the ARSession
				ResetTracking();
			}
			else
			{
				// This device does not support 6DOF world tracking.
				var sessionErrorMsg = "This app requires world tracking. World tracking is only available on iOS devices with A9 processor or newer. " +
					"Please quit the application.";
				DisplayErrorMessage("Unsupported platform", sessionErrorMsg, false);
			}
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			Session.Pause();
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			virtualObjectManager.ReactToTouchesBegan(touches, evt, SceneView);
		}

		public override void TouchesMoved(NSSet touches, UIEvent evt)
		{
			virtualObjectManager.ReactToTouchesMoved(touches, evt);
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			if (virtualObjectManager.VirtualObjects.Count == 0)
			{
				ChooseObject(AddObjectButton);
			}
			virtualObjectManager.ReactToTouchesEnded(touches, evt);
		}

		public override void TouchesCancelled(NSSet touches, UIEvent evt)
		{
			virtualObjectManager.ReactToTouchesCancelled(touches, evt);
		}


		public void AddPlane(SCNNode node, ARPlaneAnchor anchor)
		{
			var plane = new Plane(anchor);

			Planes.Add(anchor, plane);
			node.AddChildNode(plane);

			UserFeedback.CancelScheduledMessage(MessageType.PlaneEstimation);
			UserFeedback.ShowMessage("SURFACE DETECTED");
			if (virtualObjectManager.VirtualObjects.Count == 0)
			{
				UserFeedback.ScheduleMessage("TAP + TO PLACE AN OBJECT", 7.5, MessageType.ContentPlacement);
			}
		}

		public void UpdatePlane(ARPlaneAnchor anchor)
		{
			if (Planes.ContainsKey(anchor))
			{
				Planes[anchor].Update(anchor);
			}
		}

		public void RemovePlane(ARPlaneAnchor anchor)
		{
			if (Planes.ContainsKey(anchor))
			{
				Planes[anchor].RemoveFromParentNode();
				Planes.Remove(anchor);
			}
		}

		public void ResetTracking()
		{
			Session.Run(StandardConfiguration(), ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors);

			UserFeedback.ScheduleMessage("FIND A SURFACE TO PLACE AN OBJECT", 7.5, MessageType.PlaneEstimation);
		}

		private ARConfiguration StandardConfiguration()
		{
			var config = new ARWorldTrackingConfiguration();
			config.PlaneDetection = ARPlaneDetection.Horizontal;
			return config;
		}

		public void SetupFocusSquare()
		{
			if (FocusSquare != null)
			{
				FocusSquare.Hidden = true;
				FocusSquare.RemoveFromParentNode();
			}

			FocusSquare = new FocusSquare();
			SceneView.Scene.RootNode.AddChildNode(FocusSquare);

			UserFeedback.ScheduleMessage("TRY MOVING LEFT OR RIGHT", 5.0, MessageType.FocusSquare);
		}

		private void UpdateFocusSquare()
		{
			if (ScreenCenter == null)
			{
				return;
			}

			DispatchQueue.MainQueue.DispatchAsync(() =>
			{
				var objectVisible = false;
				foreach (var vo in virtualObjectManager.VirtualObjects)
				{
					if (SceneView.IsNodeInsideFrustum(vo, SceneView.PointOfView))
					{
						objectVisible = true;
						break;
					}
				}

				if (objectVisible)
				{
					FocusSquare?.Hide();
				}
				else
				{
					FocusSquare?.Show();
				}

				var (worldPos, planeAnchor, _) = virtualObjectManager.WorldPositionFromScreenPosition(ScreenCenter.Value, SceneView, FocusSquare?.Position);
				if (worldPos != null)
				{
					if (Session != null && ViewController.CurrentFrame != null)
					{
						FocusSquare.Update((SCNVector3)worldPos, planeAnchor, ViewController.CurrentFrame.Camera);
					}
					UserFeedback.CancelScheduledMessage(MessageType.FocusSquare);
				}
			});
		}

		public void SetupScene()
		{
			// Synchronize updates via the 'serialQueue' 
			serialQueue = new DispatchQueue(label: "com.xamarin.PlacingObjects.serialSceneKitQueue");
			virtualObjectManager = new VirtualObjectManager(serialQueue);
			virtualObjectManager.Delegate = this;


			// Setup the scene view
			SceneView.Setup();
			SceneView.Delegate = this;
			SceneView.Session = Session;

			SceneView.Scene.EnableEnvironmentMapWithIntensity(25.0f, serialQueue);
			SetupFocusSquare();

			DispatchQueue.MainQueue.DispatchAsync(() => ScreenCenter = SceneView.Bounds.GetMidpoint());
		}

		public void SetupUIControls()
		{
			UserFeedback = new TextManager(this);

			// Set appareance of message output panel
			MessagePanel.Layer.CornerRadius = 3.0f;
			MessagePanel.ClipsToBounds = true;
			MessagePanel.Hidden = true;
			MessageLabel.Text = "";
		}

		private void DisplayErrorMessage(string title, string message, bool allowRestart = false)
		{
			// Blur the background
			UserFeedback.BlurBackground();

			if (allowRestart)
			{
				// Present an alert informing about the error that has occurred.
				var restartAction = UIAlertAction.Create("Reset", UIAlertActionStyle.Default, (obj) =>
				{
					UserFeedback.UnblurBackground();
					RestartExperience(this);
				});
				UserFeedback.ShowAlert(title, message, new UIAlertAction[] { restartAction });
			}
			else
			{
				UserFeedback.ShowAlert(title, message, new UIAlertAction[] { });
			}
		}
	}
}
