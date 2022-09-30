
namespace XamarinShot;

/// <summary>
/// Main view controller for the AR game.
/// </summary>
public partial class GameViewController : UIViewController,
                                      ISCNSceneRendererDelegate,
                                      IARSessionDelegate,
                                      IGameManagerDelegate,
                                      IGameStartViewControllerDelegate
{
        readonly SCNNode audioListenerNode = new SCNNode ();

        GameManager? gameManager;

        SessionState sessionState = SessionState.Setup;

        bool isSessionInterrupted;

        // used when state is localizingToWorldMap or localizingToSavedMap
        ARWorldMap? targetWorldMap;

        GameBoard gameBoard = new GameBoard ();

        // Root node of the level
        SCNNode renderRoot = new SCNNode ();

        SCNVector3 panOffset = SCNVector3.Zero;

        ButtonBeep? buttonBeep;
        ButtonBeep? backButtonBeep;

        // Music player
        readonly MusicCoordinator musicCoordinator = new MusicCoordinator ();

        GameLevel? selectedLevel;

        // Proximity manager for beacons
        readonly ProximityManager proximityManager = ProximityManager.Shared;

        protected GameViewController (IntPtr handle) : base (handle) { }

        public bool CanAdjustBoard => sessionState == SessionState.PlacingBoard || sessionState == SessionState.AdjustingBoard;

        public bool AttemptingBoardPlacement => sessionState == SessionState.LookingForSurface || sessionState == SessionState.PlacingBoard;

        int teamACatapultCount = 0;
        public int TeamACatapultCount
        {
                get => teamACatapultCount;
                set
                {
                        if (teamACatapultCount != value)
                        {
                                teamACatapultCount = value;

                                // The "highlighted" state actually means that the catapult has been disabled.
                                for (var index = 0; index < teamACatapultImages.Length; index++)
                                {
                                        var shouldAppear = index < teamACatapultCount;
                                        teamACatapultImages [index].Highlighted = !shouldAppear;
                                }
                        }
                }
        }

        int teamBCatapultCount = 0;
        public int TeamBCatapultCount
        {
                get => teamBCatapultCount;
                set
                {
                        if (teamBCatapultCount != value)
                        {
                                teamBCatapultCount = value;

                                // The "highlighted" state actually means that the catapult has been disabled.
                                for (var index = 0; index < teamBCatapultImages.Length; index++)
                                {
                                        var shouldAppear = index < teamBCatapultCount;
                                        teamBCatapultImages [index].Highlighted = !shouldAppear;
                                }
                        }
                }
        }

        public GameManager? GameManager
        {
                get
                {
                        return gameManager;
                }

                set
                {
                        gameManager = value;
                        if (gameManager is not null)
                        {
                                if (gameManager.IsNetworked && !gameManager.IsServer)
                                {
                                        SessionState = SessionState.WaitingForBoard;
                                } else {
                                        SessionState = SessionState.LookingForSurface;
                                }

                                gameManager.Delegate = this;
                                gameManager.MainScreenBounds = UIScreen.MainScreen.Bounds;
                        } else {
                                SessionState = SessionState.Setup;
                        }
                }
        }

        public SessionState SessionState
        {
                get
                {
                        return sessionState;
                }

                set
                {
                        if (sessionState != value)
                        {
                                sessionState = value;
                                ConfigureView ();
                                ConfigureARSession ();
                        }
                }
        }

        public bool IsSessionInterrupted
        {
                get
                {
                        return isSessionInterrupted;
                }

                set
                {
                        isSessionInterrupted = value;
                        if (isSessionInterrupted && !UserDefaults.DisableInGameUI)
                        {
                                instructionLabel.Hidden = false;
                                instructionLabel.Text = NSBundle.MainBundle.GetLocalizedString ("Point the camera towards the table.");
                        } else {
                                var localizedInstruction = sessionState.LocalizedInstruction ();
                                if (!string.IsNullOrEmpty (localizedInstruction))
                                {
                                        instructionLabel.Hidden = false;
                                        instructionLabel.Text = localizedInstruction;
                                } else {
                                        instructionLabel.Hidden = true;
                                }
                        }
                }
        }

        public GameLevel? SelectedLevel
        {
                get
                {
                        return selectedLevel;
                }

                set
                {
                        selectedLevel = value;
                        if (selectedLevel is not null)
                        {
                                gameBoard.PreferredSize = selectedLevel.TargetSize;
                        }
                }
        }

        public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
        {
                base.DidRotate (fromInterfaceOrientation);
                if (gameManager is not null)
                        gameManager.MainScreenBounds = UIScreen.MainScreen.Bounds;
        }

        public override void ViewDidLoad ()
        {
                base.ViewDidLoad ();

                // Set the view's delegate
                sceneView.Delegate = this;

                // Explicitly set the listener position to be in an SCNNode that we control
                // because the scaling of the scaling difference between the render coordinate
                // space and the simulation coordinate space. This node isn't added to the node
                // hierarchy of the scene, so it isn't affected by changes to the scene scale.
                // On each frame update, however, its position is explicitly set to a transformed
                // value that is consistent with the game objects in the scene.
                sceneView.AudioListener = audioListenerNode;

                sceneView.Scene.RootNode.AddChildNode (gameBoard);

                SessionState = SessionState.Setup;
                sceneView.Session.Delegate = this;

                instructionLabel.ClipsToBounds = true;
                instructionLabel.Layer.CornerRadius = 8f;

                if (UserDefaults.AllowGameBoardAutoSize)
                {
                        sceneView.AddGestureRecognizer (new UILongPressGestureRecognizer (HandleLongPress));
                }

                notificationLabel.ClipsToBounds = true;
                notificationLabel.Layer.CornerRadius = 8f;

                buttonBeep = ButtonBeep.Create ("button_forward.wav", 0.5f);
                backButtonBeep = ButtonBeep.Create ("button_backward.wav", 0.5f);

                renderRoot.Name = "_renderRoot";
                sceneView.Scene.RootNode.AddChildNode (renderRoot);

                NSNotificationCenter.DefaultCenter.AddObserver (NSProcessInfo.ThermalStateDidChangeNotification, (obj) => UpdateThermalStateIndicator ());

                // this preloads the assets used by the level - materials and texture and compiles shaders
                PreloadLevel ();
        }

        public override void ViewWillAppear (bool animated)
        {
                base.ViewWillAppear (animated);
                ConfigureARSession ();
                ConfigureView ();
        }

        void UpdateThermalStateIndicator ()
        {
                DispatchQueue.MainQueue.DispatchAsync (() =>
                {
                        // Show thermal state label if default enabled and state is critical
                        thermalStateLabel.Hidden = !(UserDefaults.ShowThermalState && NSProcessInfo.ProcessInfo.ThermalState == NSProcessInfoThermalState.Critical);
                });
        }

        #region Configuration

        void ConfigureView ()
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

                sceneView.DebugOptions = debugOptions;

                // perf stats
                sceneView.ShowsStatistics = UserDefaults.ShowSceneViewStats;

                trackingStateLabel.Hidden = !UserDefaults.ShowTrackingState;

                // smooth the edges by rendering at higher resolution
                // defaults to none on iOS, use on faster GPUs
                // 0, 2, 4 on iOS, 8, 16x on macOS
                sceneView.AntialiasingMode = UserDefaults.AntialiasingMode ? SCNAntialiasingMode.Multisampling4X : SCNAntialiasingMode.None;

                var localizedInstruction = sessionState.LocalizedInstruction ();
                if (!string.IsNullOrEmpty (localizedInstruction))
                {
                        instructionLabel.Hidden = false;
                        instructionLabel.Text = localizedInstruction;
                } else {
                        instructionLabel.Hidden = true;
                }

                if (sessionState == SessionState.WaitingForBoard)
                {
                        activityIndicator.StartAnimating ();
                } else {
                        activityIndicator.StopAnimating ();
                }

                if (!UserDefaults.ShowSettingsInGame)
                {
                        settingsButton.Hidden = true;
                }

                if (UserDefaults.DisableInGameUI)
                {
                        exitGameButton.SetImage (null, UIControlState.Normal);
                } else {
                        exitGameButton.SetImage (UIImage.FromBundle ("close"), UIControlState.Normal);
                }

                exitGameButton.Hidden = sessionState == SessionState.Setup;

                ConfigureMappingUI ();
                ConfigureRelocalizationHelp ();
                UpdateThermalStateIndicator ();
        }

        void ConfigureARSession ()
        {
                var configuration = new ARWorldTrackingConfiguration { AutoFocusEnabled = UserDefaults.AutoFocus };
                var options = ARSessionRunOptions.None;

                switch (sessionState)
                {
                        case SessionState.Setup:
                                // in setup
                                // AR session paused
                                sceneView.Session.Pause ();
                                return;

                        case SessionState.LookingForSurface:
                        case SessionState.WaitingForBoard:
                                // both server and client, go ahead and start tracking the world
                                configuration.PlaneDetection = ARPlaneDetection.Horizontal;
                                options = ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors;

                                // Only reset session if not already running
                                if (sceneView.Playing)
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
                                if (targetWorldMap is not null) // should have had a world map
                                {
                                        configuration.InitialWorldMap = targetWorldMap;
                                        configuration.PlaneDetection = ARPlaneDetection.Horizontal;
                                        options = ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors;
                                        gameBoard.Anchor = targetWorldMap.BoardAnchor ();
                                        if (gameBoard.Anchor is not null)
                                        {
                                                gameBoard.Transform = gameBoard.Anchor.Transform.ToSCNMatrix4 ();
                                                var width = (float)gameBoard.Anchor.Size.Width;
                                                gameBoard.Scale = new SCNVector3 (width, width, width);
                                        }

                                        gameBoard.HideBorder (0);
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
                sceneView.Session.Run (configuration, options);
        }

        #endregion

        #region UI Buttons

        partial void exitGamePressed (UIButton sender)
        {
                var stayAction = UIAlertAction.Create (NSBundle.MainBundle.GetLocalizedString ("Stay"), UIAlertActionStyle.Default, null);
                var leaveAction = UIAlertAction.Create (NSBundle.MainBundle.GetLocalizedString ("Leave"), UIAlertActionStyle.Cancel, (action) =>
                {
                        ExitGame ();
                        // start looking for beacons again
                        proximityManager.Start ();
                });

                var actions = new UIAlertAction [] { stayAction, leaveAction };
                var localizedTitle = NSBundle.MainBundle.GetLocalizedString ("Are you sure you want to leave the game?");

                string? localizedMessage = null;
                if (gameManager is not null && gameManager.IsServer)
                {
                        localizedMessage = NSBundle.MainBundle.GetLocalizedString ("You’re the host, so if you leave now the other players will have to leave too.");
                }

                ShowAlert (localizedTitle, localizedMessage ?? "", actions);
        }

        void ExitGame ()
        {
                backButtonBeep?.Play ();
                if (gameManager is not null)
                {
                        gameManager.Delegate = null;
                        gameManager.ReleaseLevel ();
                        gameManager.Dispose ();
                        GameManager = null;
                }

                ShowOverlay ();

                // Cleanup the current loaded map
                targetWorldMap?.Dispose ();
                targetWorldMap = null;

                foreach (var item in teamACatapultImages)
                {
                        item.Hidden = true;
                        item.Highlighted = false;
                }

                foreach (var item in teamBCatapultImages)
                {
                        item.Hidden = true;
                        item.Highlighted = false;
                }

                notificationLabel.Hidden = true;
                UserDefaults.HasOnboarded = false;

                // Reset game board
                gameBoard.Reset ();
                if (gameBoard.Anchor is not null)
                {
                        sceneView.Session.RemoveAnchor (gameBoard.Anchor);
                        gameBoard.Anchor = null;
                }
        }

        void ShowAlert (string title, string? message = null, IList<UIAlertAction>? actions = null)
        {
                var alertController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);
                if (actions is not null)
                {
                        foreach (var action in actions)
                        {
                                alertController.AddAction (action);
                        }
                } else {
                        alertController.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
                }

                PresentViewController (alertController, true, null);
        }

        #endregion

        #region Board management

        protected CGPoint ScreenCenter
        {
                get
                {
                        var bounds = sceneView.Bounds;
                        return new CGPoint (bounds.GetMidX (), bounds.GetMidY ());
                }
        }

        void UpdateGameBoard (ARFrame frame)
        {
                if (sessionState == SessionState.SetupLevel)
                {
                        // this will advance the session state
                        SetupLevel ();
                }
                // Only automatically update board when looking for surface or placing board
                else if (AttemptingBoardPlacement)
                {
                        // Make sure this is only run on the render thread
                        if (gameBoard.ParentNode is null)
                        {
                                sceneView.Scene.RootNode.AddChildNode (gameBoard);
                        }

                        // Perform hit testing only when ARKit tracking is in a good state.
                        if (frame.Camera.TrackingState == ARTrackingState.Normal)
                        {
                                var result = sceneView.HitTest (ScreenCenter, ARHitTestResultType.EstimatedHorizontalPlane | ARHitTestResultType.ExistingPlaneUsingExtent).FirstOrDefault ();
                                if (result is not null)
                                {
                                        using (result)
                                        {
                                                // Ignore results that are too close to the camera when initially placing
                                                if (result.Distance > 0.5f || sessionState == SessionState.PlacingBoard)
                                                {
                                                        SessionState = SessionState.PlacingBoard;
                                                        gameBoard.Update (result, frame.Camera);
                                                }
                                        }
                                } else {
                                        SessionState = SessionState.LookingForSurface;
                                        if (!gameBoard.IsBorderHidden)
                                        {
                                                gameBoard.HideBorder ();
                                        }
                                }
                        }
                }
        }

        void Process (BoardSetupAction boardAction, Player peer)
        {
                switch (boardAction.Type)
                {
                        case BoardSetupAction.BoardSetupActionType.BoardLocation:
                                switch (boardAction.BoardLocation.Type)
                                {
                                        case GameBoardLocation.GameBoardLocationType.WorldMapData:
                                                // Received WorldMap data
                                                LoadWorldMap (boardAction.BoardLocation.WorldMapData);
                                                break;
                                        case GameBoardLocation.GameBoardLocationType.Manual:
                                                // Received a manual board placement
                                                SessionState = SessionState.LookingForSurface;
                                                break;
                                }
                                break;

                        case BoardSetupAction.BoardSetupActionType.RequestBoardLocation:
                                SendWorldTo (peer);
                                break;
                }
        }

        /// <summary>
        /// Load the World Map from archived data
        /// </summary>
        void LoadWorldMap (NSData? archivedData)
        {
                if (archivedData is null)
		{
                        DispatchQueue.MainQueue.DispatchAsync (() =>
                        {
                                ShowAlert ("An error occured while loading the WorldMap (no data)");
                                SessionState = SessionState.Setup;
                        });
                        return;
                }
                if (NSKeyedUnarchiver.GetUnarchivedObject (typeof (ARWorldMap), archivedData, out NSError? error) is ARWorldMap worldMap)
                {
                        DispatchQueue.MainQueue.DispatchAsync (() =>
                        {
                                targetWorldMap = worldMap;
                                SessionState = SessionState.LocalizingToBoard;
                        });
                } else if (error is not null) {
                        // The WorldMap received couldn't be decompressed
                        DispatchQueue.MainQueue.DispatchAsync (() =>
                        {
                                ShowAlert ("An error occured while loading the WorldMap (Failed to decompress)");
                                SessionState = SessionState.Setup;
                        });
                } else {
                        // The WorldMap received couldn't be read
                        DispatchQueue.MainQueue.DispatchAsync (() =>
                        {
                                ShowAlert ("An error occured while loading the WorldMap (Failed to read)");
                                SessionState = SessionState.Setup;
                        });
                }
        }

        void PreloadLevel ()
        {
                // Preloading assets started

                var main = DispatchQueue.MainQueue;
                var background = DispatchQueue.DefaultGlobalQueue;
                background.DispatchAsync (() =>
                {
                        foreach (var level in GameLevel.AllLevels)
                        {
                                // this is just a dummy scene to preload data
                                // this is not added to the sceneView
                                var scene = new SCNScene ();
                                // this may result in two callers loading the scene
                                level.Load ();
                                if (level.ActiveLevel is not null)
                                {
                                        SetLevelLighting (level.ActiveLevel);
                                        scene.RootNode.AddChildNode (level.ActiveLevel);
                                        scene.Paused = true;

                                        // This doesn't actually add the scene to the ARSCNView, it just sets up a background task
                                        // the preloading is done on a background thread, and the completion handler called

                                        // prepare must be called from main thread
                                        main.DispatchSync (() =>
                                        {
                                                // preparing a scene compiles shaders
                                                sceneView.Prepare (new NSObject [] { scene }, (success) =>
                                                {
                                                        Console.WriteLine (success ? "Preloading assets succeeded" : "Preloading assets failed");
                                                });
                                        });
                                }
                        }
                });
        }

        void SetLevelLighting (SCNNode node)
        {
                var light = node.FindChildNode ("LightNode", true)?.Light;
                if (light is not null)
                {
                        light.ShadowRadius = 3;
                        light.ShadowSampleCount = 8;
                }
        }

        void SetupLevel ()
        {
                if (gameManager is null)
                {
                        throw new Exception ("gameManager not initialized");
                }

                if (gameBoard.Anchor is null)
                {
                        var boardTransform = gameBoard.Transform.Normalize ();
                        boardTransform.Transpose ();

                        var boardSize = new CGSize ((float)gameBoard.Scale.X, (float)(gameBoard.Scale.X * gameBoard.AspectRatio));
                        gameBoard.Anchor = new BoardAnchor (boardTransform, boardSize);
                        sceneView.Session.AddAnchor (gameBoard.Anchor);
                }

                gameBoard.HideBorder ();
                SessionState = SessionState.GameInProgress;

                GameTime.SetLevelStartTime ();
                gameManager.Start ();
                gameManager.AddLevel (renderRoot, gameBoard);
                gameManager.RestWorld ();

                if (!UserDefaults.DisableInGameUI)
                {
                        teamACatapultImages.ForEach ((item) => item.Hidden = false);
                        teamBCatapultImages.ForEach ((item) => item.Hidden = false);
                }

                // stop ranging for beacons after placing board
                if (UserDefaults.GameRoomMode)
                {
                        proximityManager.Stop ();
                        if (proximityManager.ClosestLocation is not null)
                        {
                                gameManager.UpdateSessionLocation (proximityManager.ClosestLocation);
                        }
                }
        }

        void SendWorldTo (Player peer)
        {
                if (gameManager is not null && gameManager.IsServer)
                {
                        switch (UserDefaults.BoardLocatingMode)
                        {
                                case BoardLocatingMode.WorldMap:
                                        // generating worldmap 
                                        GetCurrentWorldMapData ((data, error) =>
                                        {
                                                if (error is null && data is not null)
                                                {
                                                        // got a compressed map
                                                        var location = new GameBoardLocation { WorldMapData = data, Type = GameBoardLocation.GameBoardLocationType.WorldMapData };
                                                        DispatchQueue.MainQueue.DispatchAsync (() =>
                                                        {
                                                                // sending worldmap
                                                                gameManager.Send (new BoardSetupAction { BoardLocation = location, Type = BoardSetupAction.BoardSetupActionType.BoardLocation }, peer);
                                                        });
                                                } else if (error is not null) {
                                                        Console.WriteLine ($"Didn't work! {error?.LocalizedDescription ?? string.Empty}");
                                                }
                                        });
                                        break;

                                case BoardLocatingMode.Manual:
                                        gameManager.Send (new BoardSetupAction { BoardLocation = new GameBoardLocation { Type = GameBoardLocation.GameBoardLocationType.Manual } }, peer);
                                        break;
                        }
                }
        }

        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject? sender)
        {
                if (!string.IsNullOrEmpty (segue.Identifier) &&
                   Enum.TryParse<GameSegue> (segue.Identifier, true, out GameSegue segueType))
                {
                        switch (segueType)
                        {
                                case GameSegue.EmbeddedOverlay:
                                        if (segue.DestinationViewController is GameStartViewController gameStartViewController)
                                        {
                                                gameStartViewController.Delegate = this;
                                                musicCoordinator.PlayMusic ("music_menu", 0f);
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

        void ShowOverlay ()
        {
                UIView.Transition (View, 1d, UIViewAnimationOptions.TransitionCrossDissolve, () =>
                {
                        overlayView.Hidden = false;
                        inSceneButtons.ForEach ((button) => button.Hidden = true);

                        settingsButton.Hidden = true;
                        instructionLabel.Hidden = true;
                },
                () =>
                {
                        overlayView.UserInteractionEnabled = true;
                        UIApplication.SharedApplication.IdleTimerDisabled = false;
                });

                musicCoordinator.PlayMusic ("music_menu", 0.5f);
        }

        void HideOverlay ()
        {
                UIView.Transition (View, 1d, UIViewAnimationOptions.TransitionCrossDissolve, () =>
                {
                        overlayView.Hidden = true;
                        inSceneButtons.ForEach ((button) => button.Hidden = false);

                        instructionLabel.Hidden = false;
                        settingsButton.Hidden = !UserDefaults.ShowSettingsInGame;
                },
                () =>
                {
                        overlayView.UserInteractionEnabled = false;
                        UIApplication.SharedApplication.IdleTimerDisabled = true;
                });

                musicCoordinator.StopMusic ("music_menu", 3f);
        }

        #endregion

        #region ISCNSceneRendererDelegate

        // This is the ordering of delegate calls
        // https://developer.apple.com/documentation/scenekit/scnscenerendererdelegate

        [Export ("renderer:updateAtTime:")]
        public void Update (ISCNSceneRenderer renderer, double timeInSeconds)
        {
                if (gameManager is not null && gameManager.IsInitialized)
                {
                        GameTime.UpdateAtTime (timeInSeconds);

                        if (sceneView.PointOfView is not null && selectedLevel is not null && selectedLevel.Placed)
                        {
                                // make a copy of the camera data that other threads can access
                                // ARKit has updated the transform right before this
                                gameManager.CopySimulationCamera ();

                                // these can use the pointOfView since the render thread scales/unscales the camera around rendering
                                var pointOfViewTransform = sceneView.PointOfView.Transform;
                                pointOfViewTransform.Transpose ();
                                var cameraTransform = gameManager.RenderSpaceTransformToSimulationSpace (pointOfViewTransform);
                                var cameraInfo = new CameraInfo (cameraTransform);

                                gameManager.UpdateCamera (cameraInfo);

                                var canGrabCatapult = gameManager.CanGrabACatapult (cameraInfo.Ray);
                                var isGrabbingCatapult = gameManager.IsCurrentPlayerGrabbingACatapult ();

                                DispatchQueue.MainQueue.DispatchAsync (() =>
                                {
                                        if (sessionState == SessionState.GameInProgress)
                                        {
                                                if (!UserDefaults.HasOnboarded && !UserDefaults.DisableInGameUI)
                                                {
                                                        if (isGrabbingCatapult)
                                                        {
                                                                instructionLabel.Text = NSBundle.MainBundle.GetLocalizedString ("Release to shoot.");
                                                        } else if (canGrabCatapult) {
                                                                 instructionLabel.Text = NSBundle.MainBundle.GetLocalizedString ("Tap anywhere and hold to pull back.");
                                                        } else {
                                                                instructionLabel.Text = NSBundle.MainBundle.GetLocalizedString ("Move closer to a slingshot.");
                                                        }
                                                } else {
                                                        if (!instructionLabel.Hidden && !isSessionInterrupted)
                                                        {
                                                                 instructionLabel.Hidden = true;
                                                        }
                                                }
                                        }
                                });
                        }

                        gameManager.Update (GameTime.DeltaTime);
                }
        }

        [Export ("renderer:didApplyConstraintsAtTime:")]
        public void DidApplyConstraints (ISCNSceneRenderer renderer, double atTime)
        {
                if (gameManager is not null && gameManager.IsInitialized)
                {
                        // scale up/down the camera to render space
                        gameManager.ScaleCameraToRender ();

                        // render space from here until scaleCameraToSimulation() is called
                        if (sceneView.PointOfView is not null)
                        {
                                audioListenerNode.WorldTransform = sceneView.PointOfView.WorldTransform;
                        }

                        // The only functionality currently controlled here is the trail on the projectile.
                        // Therefore this part is used to turn on/off show projectile trail
                        if (UserDefaults.ShowProjectileTrail)
                        {
                                gameManager?.OnDidApplyConstraints (renderer);
                        }
                }
        }

        [Export ("renderer:didRenderScene:atTime:")]
        public void DidRenderScene (ISCNSceneRenderer renderer, SCNScene scene, double timeInSeconds)
        {
                // update visibility properties in renderloop because we have to scale the physics world down to render properly
                if (gameManager is not null && gameManager.IsInitialized)
                {
                        // this visibility test is in scaled space, using renderer frustum culling
                        if (sceneView.PointOfView is not null)
                        {
                                gameManager?.UpdateCatapultVisibility (renderer, sceneView.PointOfView);
                        }

                        // return the pointOfView back from scaled space
                        gameManager?.ScaleCameraToSimulation ();
                }
        }

        #endregion

        #region ARSessionDelegate

        [Export ("session:didUpdateFrame:")]
        public void DidUpdateFrame (ARSession session, ARFrame frame)
        {
                // Update game board placement in physical world
                if (gameManager is not null)
                {
                        // this is main thread calling into init code
                        UpdateGameBoard (frame);
                }

                // Update mapping status for saving maps
                UpdateMappingStatus (frame.WorldMappingStatus);

                // dispose after updating
                frame.Dispose ();
        }

        #endregion

        #region IGameManagerDelegate

        public void OnReceived (GameManager manager, BoardSetupAction boardAction, Player player)
        {
                DispatchQueue.MainQueue.DispatchAsync (() =>
                {
                        Process (boardAction, player);
                });
        }

        public void OnJoiningHost (GameManager manager, Player host)
        {
                // host joined the game
                DispatchQueue.MainQueue.DispatchAsync (() =>
                {
                         if (sessionState == SessionState.WaitingForBoard)
                         {
                                 manager.Send (new BoardSetupAction { Type = BoardSetupAction.BoardSetupActionType.RequestBoardLocation });
                         }

                         if (!UserDefaults.DisableInGameUI)
                         {
                                 notificationLabel.Text = "You joined the game!";
                                 notificationLabel.FadeInFadeOut (1d);
                         }
                });
        }

        public void OnJoiningPlayer (GameManager manager, Player player)
        {
                // non-host player joined the game
                if (!UserDefaults.DisableInGameUI)
                {
                        DispatchQueue.MainQueue.DispatchAsync (() =>
                        {
                                 notificationLabel.Text = $"{player.Username} joined the game.";
                                 notificationLabel.FadeInFadeOut (1d);
                        });

                        // If the gameplay music is already running, start it on the newly
                        // connected client.
                        if (musicCoordinator.CurrentMusicPlayer?.Name == "music_gameplay")
                        {
                                var musicTime = musicCoordinator.CurrentMusicTime ();
                                //os_log(.debug, "music play position = %f", musicTime)
                                if (musicTime >= 0)
                                {
                                        manager.StartGameMusic (player);
                                }
                        }
                }
        }

        public void OnLeavingHost (GameManager manager, Player host)
        {
                // host left the game
                if (!UserDefaults.DisableInGameUI)
                {
                        DispatchQueue.MainQueue.DispatchAsync (() =>
                        {
                                // the game can no longer continue
                                notificationLabel.Text = "The host left the game. Please join another game or start your own!";
                                notificationLabel.Hidden = false;
                        });
                }
        }

        public void OnLeavingPlayer (GameManager manager, Player player)
        {
                // non-host player left the game
                if (!UserDefaults.DisableInGameUI)
                {
                        DispatchQueue.MainQueue.DispatchAsync (() =>
                        {
                                notificationLabel.Text = $"{player.Username} left the game.";
                                notificationLabel.FadeInFadeOut (1d);
                        });
                }
        }

        public void OnDidStartGame (GameManager manager) { }

        public void OnManagerDidWinGame (GameManager manager)
        {
                musicCoordinator.PlayMusic ("music_win");
        }

        public void OnHasNetworkDelay (GameManager manager, bool hasNetworkDelay)
        {
                DispatchQueue.MainQueue.DispatchAsync (() =>
                {
                        if (UserDefaults.ShowNetworkDebug)
                        {
                                networkDelayText.Hidden = !hasNetworkDelay;
                        }
                });
        }

        public void OnGameStateUpdated (GameManager manager, GameState gameState)
        {
                DispatchQueue.MainQueue.DispatchAsync (() =>
                {
                        if (SessionState == SessionState.GameInProgress)
                        {
                                TeamACatapultCount = gameState.TeamACatapults;
                                TeamBCatapultCount = gameState.TeamBCatapults;
                        }
                });
        }

        #endregion

        #region IGameStartViewControllerDelegate

        void CreateGameManager (NetworkSession? session)
        {
                var level = UserDefaults.SelectedLevel;
                SelectedLevel = level;
                GameManager = new GameManager (sceneView,
                                                   level,
                                                   session,
                                                   sceneView.AudioEnvironmentNode,
                                                   musicCoordinator);
        }

        public void OnSoloGamePressed (UIViewController viewController, UIButton button)
        {
                HideOverlay ();
                CreateGameManager (null);
        }

        public void OnGameSelected (UIViewController viewController, NetworkSession game)
        {
                HideOverlay ();
                CreateGameManager (game);
        }

        public void OnGameStarted (UIViewController viewController, NetworkSession game)
        {
                HideOverlay ();
                CreateGameManager (game);
        }

        public void OnSettingsSelected (UIViewController viewController)
        {
                PerformSegue (GameSegue.ShowSettings.ToString (), this);
        }

        #endregion
}
