
namespace XamarinShot
{
    using ARKit;
    using CoreFoundation;
    using CoreGraphics;
    using Foundation;
    using SceneKit;
    using XamarinShot.Models;
    using XamarinShot.Models.Enums;
    using XamarinShot.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UIKit;

    /// <summary>
    /// Main view controller for the AR game.
    /// </summary>
    public partial class GameViewController : UIViewController, 
                                              ISCNSceneRendererDelegate,
                                              IARSessionDelegate,
                                              IGameManagerDelegate, 
                                              IGameStartViewControllerDelegate
    {
        private readonly SCNNode audioListenerNode = new SCNNode();

        private GameManager gameManager;

        private SessionState sessionState = SessionState.Setup;
       
        private bool isSessionInterrupted;

        // used when state is localizingToWorldMap or localizingToSavedMap
        private ARWorldMap targetWorldMap;

        private GameBoard gameBoard = new GameBoard();

        // Root node of the level
        private SCNNode renderRoot = new SCNNode();

        private SCNVector3 panOffset = SCNVector3.Zero;

        private ButtonBeep buttonBeep;
        private ButtonBeep backButtonBeep;

        // Music player
        private readonly MusicCoordinator musicCoordinator = new MusicCoordinator();

        private GameLevel selectedLevel;

        // Proximity manager for beacons
        private readonly ProximityManager proximityManager = ProximityManager.Shared;

        protected GameViewController(IntPtr handle) : base(handle) { }

        public bool CanAdjustBoard => this.sessionState == SessionState.PlacingBoard || this.sessionState == SessionState.AdjustingBoard;

        public bool AttemptingBoardPlacement => this.sessionState == SessionState.LookingForSurface || this.sessionState == SessionState.PlacingBoard;

        private int teamACatapultCount = 0;
        public int TeamACatapultCount
        {
            get => this.teamACatapultCount;
            set
            {
                if (this.teamACatapultCount != value)
                {
                    this.teamACatapultCount = value;

                    // The "highlighted" state actually means that the catapult has been disabled.
                    for (var index = 0; index < this.teamACatapultImages.Length; index++)
                    {
                        var shouldAppear = index < this.teamACatapultCount;
                        this.teamACatapultImages[index].Highlighted = !shouldAppear;
                    }
                }
            }
        }

        private int teamBCatapultCount = 0;
        public int TeamBCatapultCount
        {
            get => this.teamBCatapultCount;
            set
            {
                if (this.teamBCatapultCount != value)
                {
                    this.teamBCatapultCount = value;

                    // The "highlighted" state actually means that the catapult has been disabled.
                    for (var index = 0; index < this.teamBCatapultImages.Length; index++)
                    {
                        var shouldAppear = index < this.teamBCatapultCount;
                        this.teamBCatapultImages[index].Highlighted = !shouldAppear;
                    }
                }
            }
        }

        public GameManager GameManager
        {
            get
            {
                return this.gameManager;
            }

            set
            {
                this.gameManager = value;
                if (this.gameManager != null)
                {
                    if (this.gameManager.IsNetworked && !this.gameManager.IsServer)
                    {
                        this.SessionState = SessionState.WaitingForBoard;
                    }
                    else
                    {
                        this.SessionState = SessionState.LookingForSurface;
                    }

                    this.gameManager.Delegate = this;
                    this.gameManager.MainScreenBounds = UIScreen.MainScreen.Bounds;
                }
                else
                {
                    this.SessionState = SessionState.Setup;
                }
            }
        }

        public SessionState SessionState
        {
            get
            {
                return this.sessionState;
            }

            set
            {
                if (this.sessionState != value)
                {
                    this.sessionState = value;
                    this.ConfigureView();
                    this.ConfigureARSession();
                }
            }
        }

        public bool IsSessionInterrupted
        {
            get
            {
                return this.isSessionInterrupted;
            }

            set
            {
                this.isSessionInterrupted = value;
                if (this.isSessionInterrupted && !UserDefaults.DisableInGameUI)
                {
                    this.instructionLabel.Hidden = false;
                    this.instructionLabel.Text = NSBundle.MainBundle.GetLocalizedString("Point the camera towards the table.");
                }
                else
                {
                    var localizedInstruction = sessionState.LocalizedInstruction();
                    if (!string.IsNullOrEmpty(localizedInstruction))
                    {
                        this.instructionLabel.Hidden = false;
                        this.instructionLabel.Text = localizedInstruction;
                    }
                    else
                    {
                        this.instructionLabel.Hidden = true;
                    }
                }
            }
        }

        public GameLevel SelectedLevel
        {
            get
            {
                return this.selectedLevel;
            }

            set
            {
                this.selectedLevel = value;
                if (this.selectedLevel != null)
                {
                    this.gameBoard.PreferredSize = this.selectedLevel.TargetSize;
                }
            }
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            this.gameManager.MainScreenBounds = UIScreen.MainScreen.Bounds;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Set the view's delegate
            this.sceneView.Delegate = this;

            // Explicitly set the listener position to be in an SCNNode that we control
            // because the scaling of the scaling difference between the render coordinate
            // space and the simulation coordinate space. This node isn't added to the node
            // hierarchy of the scene, so it isn't affected by changes to the scene scale.
            // On each frame update, however, its position is explicitly set to a transformed
            // value that is consistent with the game objects in the scene.
            this.sceneView.AudioListener = this.audioListenerNode;

            this.sceneView.Scene.RootNode.AddChildNode(this.gameBoard);

            this.SessionState = SessionState.Setup;
            this.sceneView.Session.Delegate = this;

            this.instructionLabel.ClipsToBounds = true;
            this.instructionLabel.Layer.CornerRadius = 8f;

            if (UserDefaults.AllowGameBoardAutoSize)
            {
                this.sceneView.AddGestureRecognizer(new UILongPressGestureRecognizer(this.HandleLongPress));
            }

            this.notificationLabel.ClipsToBounds = true;
            this.notificationLabel.Layer.CornerRadius = 8f;

            this.buttonBeep = ButtonBeep.Create("button_forward.wav", 0.5f);
            this.backButtonBeep = ButtonBeep.Create("button_backward.wav", 0.5f);

            this.renderRoot.Name = "_renderRoot";
            this.sceneView.Scene.RootNode.AddChildNode(this.renderRoot);

            NSNotificationCenter.DefaultCenter.AddObserver(NSProcessInfo.ThermalStateDidChangeNotification, (obj) => this.UpdateThermalStateIndicator());

            // this preloads the assets used by the level - materials and texture and compiles shaders
            this.PreloadLevel();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.ConfigureARSession();
            this.ConfigureView();
        }

        private void UpdateThermalStateIndicator()
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                // Show thermal state label if default enabled and state is critical
                this.thermalStateLabel.Hidden = !(UserDefaults.ShowThermalState && NSProcessInfo.ProcessInfo.ThermalState == NSProcessInfoThermalState.Critical);
            });
        }

        #region Configuration

        private void ConfigureView()
        {
            var debugOptions = SCNDebugOptions.None;

            // fix the scaling of the physics debug view to match the world
            if (UserDefaults.ShowPhysicsDebug)
            {
                debugOptions |= SCNDebugOptions.ShowPhysicsShapes;
            }

            // show where ARKit is detecting feature points
            if (UserDefaults.ShowARDebug)
            {
                debugOptions |= ARSCNDebugOptions.ShowFeaturePoints;
            }

            // see high poly-count and LOD transitions - wireframe overlay
            if (UserDefaults.ShowWireframe)
            {
                debugOptions |= SCNDebugOptions.ShowWireframe;
            }

            this.sceneView.DebugOptions = debugOptions;

            // perf stats
            this.sceneView.ShowsStatistics = UserDefaults.ShowSceneViewStats;

            this.trackingStateLabel.Hidden = !UserDefaults.ShowTrackingState;

            // smooth the edges by rendering at higher resolution
            // defaults to none on iOS, use on faster GPUs
            // 0, 2, 4 on iOS, 8, 16x on macOS
            this.sceneView.AntialiasingMode = UserDefaults.AntialiasingMode ? SCNAntialiasingMode.Multisampling4X : SCNAntialiasingMode.None;

            var localizedInstruction = this.sessionState.LocalizedInstruction();
            if(!string.IsNullOrEmpty(localizedInstruction))
            {
                this.instructionLabel.Hidden = false;
                this.instructionLabel.Text = localizedInstruction;
            } 
            else 
            {
                this.instructionLabel.Hidden = true;
            }

            if (this.sessionState == SessionState.WaitingForBoard)
            {
                this.activityIndicator.StartAnimating();
            }
            else
            {
                this.activityIndicator.StopAnimating();
            }

            if (!UserDefaults.ShowSettingsInGame)
            {
                this.settingsButton.Hidden = true;
            }

            if (UserDefaults.DisableInGameUI)
            {
                this.exitGameButton.SetImage(null, UIControlState.Normal);
            }
            else
            {
                this.exitGameButton.SetImage(UIImage.FromBundle("close"), UIControlState.Normal);
            }

            this.exitGameButton.Hidden = this.sessionState == SessionState.Setup;

            this.ConfigureMappingUI();
            this.ConfigureRelocalizationHelp();
            this.UpdateThermalStateIndicator();
        }

        private void ConfigureARSession()
        {
            var configuration = new ARWorldTrackingConfiguration { AutoFocusEnabled = UserDefaults.AutoFocus };
            var options = ARSessionRunOptions.None;

            switch (this.sessionState)
            {
                case SessionState.Setup:
                    // in setup
                    // AR session paused
                    this.sceneView.Session.Pause();
                    return;

                case SessionState.LookingForSurface:
                case SessionState.WaitingForBoard:
                    // both server and client, go ahead and start tracking the world
                    configuration.PlaneDetection = ARPlaneDetection.Horizontal;
                    options = ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors;

                    // Only reset session if not already running
                    if (this.sceneView.Playing)
                    {
                        return;
                    }
                    break;

                case SessionState.PlacingBoard:
                case SessionState.AdjustingBoard:
                    // we've found at least one surface, but should keep looking.
                    // so no change to the running session
                    return;

                case SessionState.LocalizingToBoard:
                    if (this.targetWorldMap != null) // should have had a world map
                    {
                        configuration.InitialWorldMap = this.targetWorldMap;
                        configuration.PlaneDetection = ARPlaneDetection.Horizontal;
                        options = ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors;
                        this.gameBoard.Anchor = this.targetWorldMap.BoardAnchor();
                        if (this.gameBoard.Anchor != null)
                        {
                            this.gameBoard.Transform = this.gameBoard.Anchor.Transform.ToSCNMatrix4();
                            var width = (float)this.gameBoard.Anchor.Size.Width;
                            this.gameBoard.Scale = new SCNVector3(width, width, width);
                        }

                        this.gameBoard.HideBorder(0);
                    }
                    break;

                case SessionState.SetupLevel:
                    // more init
                    return;
                case SessionState.GameInProgress:
                    // The game is in progress, no change to the running session
                    return;
            }

            // Turning light estimation off to test PBR on SceneKit file
            configuration.LightEstimationEnabled = false;

            // configured AR session
            this.sceneView.Session.Run(configuration, options);
        }

        #endregion

        #region UI Buttons

        partial void exitGamePressed(UIButton sender)
        {
            var stayAction = UIAlertAction.Create(NSBundle.MainBundle.GetLocalizedString("Stay"), UIAlertActionStyle.Default, null);
            var leaveAction = UIAlertAction.Create(NSBundle.MainBundle.GetLocalizedString("Leave"), UIAlertActionStyle.Cancel, (action) =>
            {
                this.ExitGame();
                // start looking for beacons again
                this.proximityManager.Start();
            });

            var actions = new UIAlertAction[] { stayAction, leaveAction };
            var localizedTitle = NSBundle.MainBundle.GetLocalizedString("Are you sure you want to leave the game?");

            string localizedMessage = null;
            if (this.gameManager != null && this.gameManager.IsServer)
            {
                localizedMessage = NSBundle.MainBundle.GetLocalizedString("You’re the host, so if you leave now the other players will have to leave too.");
            }

            this.ShowAlert(localizedTitle, localizedMessage, actions);
        }

        private void ExitGame()
        {
            this.backButtonBeep.Play();
            if (this.gameManager != null)
            {
                this.gameManager.Delegate = null;
                this.gameManager.ReleaseLevel();
                this.gameManager.Dispose();
                this.GameManager = null;
            }

            this.ShowOverlay();

            // Cleanup the current loaded map
            if (this.targetWorldMap != null)
            {
                this.targetWorldMap.Dispose();
                this.targetWorldMap = null;
            }

            foreach (var item in this.teamACatapultImages)
            {
                item.Hidden = true;
                item.Highlighted = false;
            }

            foreach (var item in this.teamBCatapultImages)
            {
                item.Hidden = true;
                item.Highlighted = false;
            }

            this.notificationLabel.Hidden = true;
            UserDefaults.HasOnboarded = false;

            // Reset game board
            this.gameBoard.Reset();
            if(this.gameBoard.Anchor != null)
            {
                this.sceneView.Session.RemoveAnchor(this.gameBoard.Anchor);
                this.gameBoard.Anchor = null;
            }
        }

        private void ShowAlert(string title, string message = null, IList<UIAlertAction> actions = null)
        {
            var alertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            if (actions != null)
            {
                foreach (var action in actions)
                {
                    alertController.AddAction(action);
                }
            } 
            else 
            {
                alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            }

            this.PresentViewController(alertController, true, null);
        }

        #endregion

        #region Board management

        protected CGPoint ScreenCenter
        {
            get
            {
                var bounds = this.sceneView.Bounds;
                return new CGPoint(bounds.GetMidX(), bounds.GetMidY());
            }
        }

        private void UpdateGameBoard(ARFrame frame)
        {
            if (this.sessionState == SessionState.SetupLevel)
            {
                // this will advance the session state
                this.SetupLevel();
            }
            // Only automatically update board when looking for surface or placing board
            else if (this.AttemptingBoardPlacement)
            {
                // Make sure this is only run on the render thread
                if (this.gameBoard.ParentNode == null)
                {
                    this.sceneView.Scene.RootNode.AddChildNode(this.gameBoard);
                }

                // Perform hit testing only when ARKit tracking is in a good state.
                if (frame.Camera.TrackingState == ARTrackingState.Normal)
                {
                    var result = this.sceneView.HitTest(this.ScreenCenter, ARHitTestResultType.EstimatedHorizontalPlane | ARHitTestResultType.ExistingPlaneUsingExtent).FirstOrDefault();
                    if (result != null)
                    {
                        using (result)
                        {
                            // Ignore results that are too close to the camera when initially placing
                            if (result.Distance > 0.5f || this.sessionState == SessionState.PlacingBoard)
                            {
                                this.SessionState = SessionState.PlacingBoard;
                                this.gameBoard.Update(result, frame.Camera);
                            }
                        }
                    }
                    else
                    {
                        this.SessionState = SessionState.LookingForSurface;
                        if (!this.gameBoard.IsBorderHidden)
                        {
                            this.gameBoard.HideBorder();
                        }
                    }
                }
            }
        }

        private void Process(BoardSetupAction boardAction, Player peer)
        {
            switch (boardAction.Type) 
            {
                case BoardSetupAction.BoardSetupActionType.BoardLocation:
                    switch (boardAction.BoardLocation.Type)
                    {
                        case GameBoardLocation.GameBoardLocationType.WorldMapData:
                            // Received WorldMap data
                            this.LoadWorldMap(boardAction.BoardLocation.WorldMapData);
                            break;
                        case GameBoardLocation.GameBoardLocationType.Manual:
                            // Received a manual board placement
                            this.SessionState = SessionState.LookingForSurface;
                            break;
                    }
                    break;

                case BoardSetupAction.BoardSetupActionType.RequestBoardLocation:
                    this.SendWorldTo(peer);
                    break;
            }
        }

        /// <summary>
        /// Load the World Map from archived data
        /// </summary>
        private void LoadWorldMap(NSData archivedData)
        {
            if (NSKeyedUnarchiver.GetUnarchivedObject(typeof(ARWorldMap), archivedData, out NSError error) is ARWorldMap worldMap)
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    this.targetWorldMap = worldMap;
                    this.SessionState = SessionState.LocalizingToBoard;
                });
            }
            else if (error != null)
            {
                // The WorldMap received couldn't be decompressed
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    this.ShowAlert("An error occured while loading the WorldMap (Failed to decompress)");
                    this.SessionState = SessionState.Setup;
                });
            }
            else
            {
                // The WorldMap received couldn't be read
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    this.ShowAlert("An error occured while loading the WorldMap (Failed to read)");
                    this.SessionState = SessionState.Setup;
                });
            }
        }
                
        private void PreloadLevel()
        {
            // Preloading assets started

            var main = DispatchQueue.MainQueue;
            var background = DispatchQueue.DefaultGlobalQueue;
            background.DispatchAsync(() =>
            {
                foreach (var level in GameLevel.AllLevels)
                {
                    // this is just a dummy scene to preload data
                    // this is not added to the sceneView
                    var scene = new SCNScene();
                    // this may result in two callers loading the scene
                    level.Load();
                    if (level.ActiveLevel != null)
                    {
                        this.SetLevelLighting(level.ActiveLevel);
                        scene.RootNode.AddChildNode(level.ActiveLevel);
                        scene.Paused = true;

                        // This doesn't actually add the scene to the ARSCNView, it just sets up a background task
                        // the preloading is done on a background thread, and the completion handler called

                        // prepare must be called from main thread
                        main.DispatchSync(() =>
                        {
                            // preparing a scene compiles shaders
                            this.sceneView.Prepare(new NSObject[] { scene }, (success) =>
                            {
                                Console.WriteLine(success ? "Preloading assets succeeded" : "Preloading assets failed");
                            });
                        });
                    }
                }
            });
        }

        private void SetLevelLighting(SCNNode node)
        {
            var light = node.FindChildNode("LightNode", true)?.Light;
            if (light != null)
            {
                light.ShadowRadius = 3;
                light.ShadowSampleCount = 8;
            }
        }

        private void SetupLevel()
        {
            if(this.gameManager == null)
            {
                throw new Exception("gameManager not initialized");
            }

            if (this.gameBoard.Anchor == null)
            {
                var boardTransform = this.gameBoard.Transform.Normalize();
                boardTransform.Transpose();

                var boardSize = new CGSize((float)this.gameBoard.Scale.X, (float)(this.gameBoard.Scale.X * this.gameBoard.AspectRatio));
                this.gameBoard.Anchor = new BoardAnchor(boardTransform, boardSize);
                this.sceneView.Session.AddAnchor(this.gameBoard.Anchor);
            }

            this.gameBoard.HideBorder();
            this.SessionState = SessionState.GameInProgress;

            GameTime.SetLevelStartTime();
            this.gameManager.Start();
            this.gameManager.AddLevel(this.renderRoot, this.gameBoard);
            this.gameManager.RestWorld();

            if (!UserDefaults.DisableInGameUI)
            {
                this.teamACatapultImages.ForEach((item) => item.Hidden = false);
                this.teamBCatapultImages.ForEach((item) => item.Hidden = false);
            }

            // stop ranging for beacons after placing board
            if (UserDefaults.GameRoomMode)
            {
                this.proximityManager.Stop();
                if (this.proximityManager.ClosestLocation != null)
                {
                    this.gameManager.UpdateSessionLocation(this.proximityManager.ClosestLocation);
                }
            }
        }

        private void SendWorldTo(Player peer)
        {
            if (this.gameManager != null && this.gameManager.IsServer)
            {
                switch (UserDefaults.BoardLocatingMode)
                {
                    case BoardLocatingMode.WorldMap:
                        // generating worldmap 
                        this.GetCurrentWorldMapData((data, error) =>
                        {
                            if (error == null && data != null)
                            {
                                // got a compressed map
                                var location = new GameBoardLocation { WorldMapData = data, Type = GameBoardLocation.GameBoardLocationType.WorldMapData };
                                DispatchQueue.MainQueue.DispatchAsync(() =>
                                {
                                    // sending worldmap
                                    this.gameManager.Send(new BoardSetupAction { BoardLocation = location, Type = BoardSetupAction.BoardSetupActionType.BoardLocation }, peer);
                                });
                            }
                            else if (error != null)
                            {
                                Console.WriteLine($"Didn't work! {error?.LocalizedDescription ?? string.Empty}");
                            }
                        });
                        break;

                    case BoardLocatingMode.Manual:
                        this.gameManager.Send(new BoardSetupAction { BoardLocation = new GameBoardLocation { Type = GameBoardLocation.GameBoardLocationType.Manual } }, peer);
                        break;
                }
            }
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (!string.IsNullOrEmpty(segue.Identifier) &&
               Enum.TryParse<GameSegue>(segue.Identifier, true, out GameSegue segueType))
            {
                switch (segueType)
                {
                    case GameSegue.EmbeddedOverlay:
                        if (segue.DestinationViewController is GameStartViewController gameStartViewController)
                        {
                            gameStartViewController.Delegate = this;
                            this.musicCoordinator.PlayMusic("music_menu", 0f);
                        }
                        break;

                    case GameSegue.WorldMapSelector:
                        if (segue.DestinationViewController is WorldMapSelectorViewController worldMapSelectorViewController)
                        {
                            worldMapSelectorViewController.Delegate = this;
                        }
                        break;
                }
            }
        }

        private void ShowOverlay()
        {
            UIView.Transition(this.View, 1d, UIViewAnimationOptions.TransitionCrossDissolve, () =>
            {
                this.overlayView.Hidden = false;
                this.inSceneButtons.ForEach((button) => button.Hidden = true);

                this.settingsButton.Hidden = true;
                this.instructionLabel.Hidden = true;
            }, () =>
            {
                this.overlayView.UserInteractionEnabled = true;
                UIApplication.SharedApplication.IdleTimerDisabled = false;
            });

            this.musicCoordinator.PlayMusic("music_menu", 0.5f);
        }

        private void HideOverlay()
        {
            UIView.Transition(this.View, 1d, UIViewAnimationOptions.TransitionCrossDissolve, () =>
            {
                this.overlayView.Hidden = true;
                this.inSceneButtons.ForEach((button) => button.Hidden = false);

                this.instructionLabel.Hidden = false;
                this.settingsButton.Hidden = !UserDefaults.ShowSettingsInGame;
            }, () =>
            {
                this.overlayView.UserInteractionEnabled = false;
                UIApplication.SharedApplication.IdleTimerDisabled = true;
            });

            this.musicCoordinator.StopMusic("music_menu", 3f);
        }

        #endregion

        #region ISCNSceneRendererDelegate

        // This is the ordering of delegate calls
        // https://developer.apple.com/documentation/scenekit/scnscenerendererdelegate

        [Export("renderer:updateAtTime:")]
        public void Update(ISCNSceneRenderer renderer, double timeInSeconds)
        {
            if (this.gameManager != null && this.gameManager.IsInitialized)
            {
                GameTime.UpdateAtTime(timeInSeconds);

                if (this.sceneView.PointOfView != null && this.selectedLevel != null && this.selectedLevel.Placed)
                {
                    // make a copy of the camera data that other threads can access
                    // ARKit has updated the transform right before this
                    this.gameManager.CopySimulationCamera();

                    // these can use the pointOfView since the render thread scales/unscales the camera around rendering
                    var pointOfViewTransform = this.sceneView.PointOfView.Transform;
                    pointOfViewTransform.Transpose();
                    var cameraTransform = this.gameManager.RenderSpaceTransformToSimulationSpace(pointOfViewTransform);
                    var cameraInfo = new CameraInfo(cameraTransform);
                   
                    this.gameManager.UpdateCamera(cameraInfo);

                    var canGrabCatapult = this.gameManager.CanGrabACatapult(cameraInfo.Ray);
                    var isGrabbingCatapult = this.gameManager.IsCurrentPlayerGrabbingACatapult();

                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        if (this.sessionState == SessionState.GameInProgress)
                        {
                            if (!UserDefaults.HasOnboarded && !UserDefaults.DisableInGameUI)
                            {
                                if (isGrabbingCatapult)
                                {
                                    this.instructionLabel.Text = NSBundle.MainBundle.GetLocalizedString("Release to shoot.");
                                }
                                else if (canGrabCatapult)
                                {
                                    this.instructionLabel.Text = NSBundle.MainBundle.GetLocalizedString("Tap anywhere and hold to pull back.");
                                }
                                else
                                {
                                    this.instructionLabel.Text = NSBundle.MainBundle.GetLocalizedString("Move closer to a slingshot.");
                                }
                            }
                            else
                            {
                                if (!this.instructionLabel.Hidden && !this.isSessionInterrupted)
                                {
                                    this.instructionLabel.Hidden = true;
                                }
                            }
                        }
                    });
                }

                this.gameManager.Update(GameTime.DeltaTime);
            }
        }

        [Export("renderer:didApplyConstraintsAtTime:")]
        public void DidApplyConstraints(ISCNSceneRenderer renderer, double atTime)
        {
            if (this.gameManager != null && this.gameManager.IsInitialized)
            {
                // scale up/down the camera to render space
                this.gameManager.ScaleCameraToRender();

                // render space from here until scaleCameraToSimulation() is called
                if (this.sceneView.PointOfView != null)
                {
                    this.audioListenerNode.WorldTransform = this.sceneView.PointOfView.WorldTransform;
                }

                // The only functionality currently controlled here is the trail on the projectile.
                // Therefore this part is used to turn on/off show projectile trail
                if (UserDefaults.ShowProjectileTrail)
                {
                    this.gameManager?.OnDidApplyConstraints(renderer);
                }
            }
        }

        [Export("renderer:didRenderScene:atTime:")]
        public void DidRenderScene(ISCNSceneRenderer renderer, SCNScene scene, double timeInSeconds)
        {
            // update visibility properties in renderloop because we have to scale the physics world down to render properly
            if (this.gameManager != null && this.gameManager.IsInitialized)
            {
                // this visibility test is in scaled space, using renderer frustum culling
                if (this.sceneView.PointOfView != null)
                {
                    this.gameManager?.UpdateCatapultVisibility(renderer, this.sceneView.PointOfView);
                }

                // return the pointOfView back from scaled space
                this.gameManager?.ScaleCameraToSimulation();
            }
        }

        #endregion

        #region ARSessionDelegate

        [Export("session:didUpdateFrame:")]
        public void DidUpdateFrame(ARSession session, ARFrame frame)
        {
            // Update game board placement in physical world
            if (this.gameManager != null)
            {
                // this is main thread calling into init code
                this.UpdateGameBoard(frame);
            }

            // Update mapping status for saving maps
            this.UpdateMappingStatus(frame.WorldMappingStatus);

            // dispose after updating
            frame.Dispose();
        }

        #endregion

        #region IGameManagerDelegate

        public void OnReceived(GameManager manager, BoardSetupAction boardAction, Player player)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                this.Process(boardAction, player);
            });
        }

        public void OnJoiningHost(GameManager manager, Player host)
        {
            // host joined the game
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                if (this.sessionState == SessionState.WaitingForBoard)
                {
                    manager.Send(new BoardSetupAction { Type = BoardSetupAction.BoardSetupActionType.RequestBoardLocation });
                }

                if (!UserDefaults.DisableInGameUI)
                {
                    this.notificationLabel.Text = "You joined the game!";
                    this.notificationLabel.FadeInFadeOut(1d);
                }
            });
        }

        public void OnJoiningPlayer(GameManager manager, Player player)
        {
            // non-host player joined the game
            if (!UserDefaults.DisableInGameUI)
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    this.notificationLabel.Text = $"{player.Username} joined the game.";
                    this.notificationLabel.FadeInFadeOut(1d);
                });

                // If the gameplay music is already running, start it on the newly
                // connected client.
                if (this.musicCoordinator.CurrentMusicPlayer?.Name == "music_gameplay")
                {
                    var musicTime = this.musicCoordinator.CurrentMusicTime();
                    //os_log(.debug, "music play position = %f", musicTime)
                    if (musicTime >= 0)
                    {
                        manager.StartGameMusic(player);
                    }
                }
            }
        }

        public void OnLeavingHost(GameManager manager, Player host)
        {
            // host left the game
            if (!UserDefaults.DisableInGameUI)
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    // the game can no longer continue
                    this.notificationLabel.Text = "The host left the game. Please join another game or start your own!";
                    this.notificationLabel.Hidden = false;
                });
            }
        }

        public void OnLeavingPlayer(GameManager manager, Player player)
        {
            // non-host player left the game
            if (!UserDefaults.DisableInGameUI)
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    this.notificationLabel.Text = $"{player.Username} left the game.";
                    this.notificationLabel.FadeInFadeOut(1d);
                });
            }
        }

        public void OnDidStartGame(GameManager manager) { }

        public void OnManagerDidWinGame(GameManager manager)
        {
            this.musicCoordinator.PlayMusic("music_win");
        }

        public void OnHasNetworkDelay(GameManager manager, bool hasNetworkDelay)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                if (UserDefaults.ShowNetworkDebug)
                {
                    this.networkDelayText.Hidden = !hasNetworkDelay;
                }
            });
        }

        public void OnGameStateUpdated(GameManager manager, GameState gameState)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                if (this.SessionState == SessionState.GameInProgress)
                {
                    this.TeamACatapultCount = gameState.TeamACatapults;
                    this.TeamBCatapultCount = gameState.TeamBCatapults;
                }
            });
        }

        #endregion

        #region IGameStartViewControllerDelegate

        private void CreateGameManager(NetworkSession session)
        {
            var level = UserDefaults.SelectedLevel;
            this.SelectedLevel = level;
            this.GameManager = new GameManager(this.sceneView,
                                               level,
                                               session,
                                               this.sceneView.AudioEnvironmentNode,
                                               this.musicCoordinator);
        }

        public void OnSoloGamePressed(UIViewController viewController, UIButton button)
        {
            this.HideOverlay();
            this.CreateGameManager(null);
        }

        public void OnGameSelected(UIViewController viewController, NetworkSession game)
        {
            this.HideOverlay();
            this.CreateGameManager(game);
        }

        public void OnGameStarted(UIViewController viewController, NetworkSession game)
        {
            this.HideOverlay();
            this.CreateGameManager(game);
        }

        public void OnSettingsSelected(UIViewController viewController)
        {
            this.PerformSegue(GameSegue.ShowSettings.ToString(), this);
        }

        #endregion
    }
}