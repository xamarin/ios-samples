namespace SceneKitFSharp
 
open System
open System.IO
open System.Linq
open FSharp.Data
open MonoTouch.UIKit
open MonoTouch.Foundation
open MonoTouch.SceneKit
open MonoTouch.CoreAnimation
open System.Drawing
open MonoTouch.CoreMotion

[<Register("FSSceneKitViewController")>]
type FSSceneKitViewController() = 
    inherit UIViewController()
 
    let piover2 = float32(Math.PI) / 2.f

    //Creates a building of specified dimensions, randomly textured. Adds it to the scene.
    let building width length height posx posy (scene:SCNNode) (rnd:Random) =
        let boxNode = new SCNNode ()
        boxNode.Geometry <- new SCNBox(
            Width = width, 
            Height = height, 
            Length = length, 
            ChamferRadius = 0.02f
        )
        boxNode.Position <- new SCNVector3(posx, height/2.0F, posy)

        scene.AddChildNode (boxNode)
        let buildings = ["Content/building1.jpg"; "Content/building2.jpg"; "Content/building3.jpg"]
        let material = new SCNMaterial ()
        material.Diffuse.Contents <- UIImage.FromFile (buildings.[rnd.Next(buildings.Length)])
        material.Diffuse.ContentsTransform <- SCNMatrix4.Scale ( new SCNVector3(width,height,1.F))
        material.Diffuse.WrapS <- SCNWrapMode.Repeat
        material.Diffuse.WrapT <- SCNWrapMode.Repeat
        material.Diffuse.MipFilter <- SCNFilterMode.Linear
        material.Diffuse.MinificationFilter <- SCNFilterMode.Linear
        material.Diffuse.MagnificationFilter <- SCNFilterMode.Linear
        material.Specular.Contents <- UIColor.Gray

        material.LocksAmbientWithDiffuse <- true

        boxNode.Geometry.FirstMaterial <- material
        boxNode

    //Returns a float32 in the specified range
    let random (min, max, (rnd:Random)) =
        float32 (rnd.Next(min, max))

    //Creates a node with a camera in the specified location. Adds it to the scene.
    let buildCamera (scene : SCNNode) loc =
        let c = new SCNNode()
        c.Camera <- new SCNCamera()
        scene.AddChildNode (c)
        c.Position <- loc
        c

    //Sets basic options on the specified view
    let configView (view : SCNView) scene =
        view.ClipsToBounds <- true
        view.Scene <- scene
        view.AllowsCameraControl <- false
        view.ShowsStatistics <- false
        view.BackgroundColor <- UIColor.FromRGB(52, 152, 219)
        view

    //For gaze tracking
    member val motion = new CMMotionManager() with get

    override this.ViewDidLoad () =
        let scene = new SCNScene ()

        //Positions everyone!
        let rnd = new Random();

        let worldNode = new SCNNode()

        worldNode.Transform <- SCNMatrix4.CreateRotationX (piover2)

        scene.RootNode.AddChildNode worldNode

        let bs = List.map (fun i -> (building 
                                    (random (1, 5, rnd)) 
                                    (random (1, 5, rnd)) 
                                    (random (1, 10, rnd)) 
                                    (random (-50, 50, rnd)) 
                                    (random (-50, 50, rnd))
                                    worldNode 
                                    rnd)) [0..200] 

        //Lights!
        let lightNode = new SCNNode()
        lightNode.Light <- new SCNLight ()
        lightNode.Light.LightType <- SCNLightType.Omni
        lightNode.Position <- new SCNVector3 (30.0F, 20.0F, 60.0F)
        worldNode.AddChildNode (lightNode)
 
        let ambientLightNode = new SCNNode ()
        ambientLightNode.Light <- new SCNLight ()
        ambientLightNode.Light.LightType <- SCNLightType.Ambient
        ambientLightNode.Light.Color <- UIColor.DarkGray
        worldNode.AddChildNode (ambientLightNode)
 
         //Cameras!
        let camNode = new SCNNode()
        camNode.Position <- new SCNVector3 (0.0F, 0.0F, 9.0F)
        scene.RootNode.AddChildNode camNode
        let leftCameraNode = buildCamera camNode (new SCNVector3 (0.0F, 0.0F, 00.0F))

        let rightCameraNode = buildCamera camNode (new SCNVector3 (0.02F, 0.0F, 0.0F))


        //Configure views
        let outer = new UIView(UIScreen.MainScreen.Bounds)

        let ss = 
            [
                new RectangleF(new PointF(0.0f, 0.0f), new SizeF(float32 UIScreen.MainScreen.Bounds.Width / 2.0f - 1.0F, UIScreen.MainScreen.Bounds.Height));
                new RectangleF(new PointF(float32 UIScreen.MainScreen.Bounds.Width / 2.0f + 1.0F, 0.0f), new SizeF(UIScreen.MainScreen.Bounds.Width / 2.0f - 1.0F, UIScreen.MainScreen.Bounds.Height));
            ]
            |> List.map (fun r -> new SCNView(r))
            |> List.map (fun s -> outer.AddSubview(configView s scene); s)


        //Place cameras (which are displaced in X, to produce 3D)
        ss.Head.PointOfView <- leftCameraNode
        ss.Tail.Head.PointOfView <- rightCameraNode

        //Action!
        let rr = CMAttitudeReferenceFrame.XArbitraryCorrectedZVertical
        this.motion.DeviceMotionUpdateInterval <- float (1.0f / 30.0f)
        this.motion.StartDeviceMotionUpdates (rr,NSOperationQueue.CurrentQueue, (fun d e ->

            let a = this.motion.DeviceMotion.Attitude;

            let q = a.Quaternion

            let dq = new SCNQuaternion(float32 q.x, float32 q.y, float32 q.z, float32 q.w)
            let dqz = SCNQuaternion.Multiply (dq, SCNQuaternion.FromAxisAngle(SCNVector3.UnitZ, piover2))

            camNode.Orientation <- dqz

            ()
        ))

        this.View <- outer
 
[<Register ("AppDelegate")>]
type AppDelegate () =
    inherit UIApplicationDelegate ()
 
    let mutable window:UIWindow = null

    override this.FinishedLaunching (app, options) =
        window <- new UIWindow (UIScreen.MainScreen.Bounds)
        window.RootViewController <- new FSSceneKitViewController()
        window.MakeKeyAndVisible ()
        true
 
module Main =
    [<EntryPoint>]
    let main args =
        UIApplication.Main (args, null, "AppDelegate")
        0