namespace SceneKitFSharp
 
open System
open System.Collections.Generic
open System.IO
open System.Linq
open UIKit
open Foundation
open SceneKit
open CoreAnimation

[<Register("FSSceneKitViewController")>]
type FSSceneKitViewController() = 
    inherit UIViewController()

    let cache = new Dictionary<string, UIImage> ()

    let texture path = 
        if not (cache.ContainsKey (path)) then
            let image = UIImage.FromFile (path)
            cache.Add (path, image)

        cache.[path]

    let building width length height (posx:nfloat) (posy:nfloat) (scene:SCNScene) (rnd:Random) =
        let boxNode = new SCNNode ()
        boxNode.Geometry <- new SCNBox(
            Width = width, 
            Height = height, 
            Length = length, 
            ChamferRadius = nfloat 0.02f
        )
        boxNode.Position <- new SCNVector3(float32 posx, (float32 height)/2.0F, float32 posy)

        scene.RootNode.AddChildNode (boxNode)

        let buildings = ["Content/building1.jpg";"Content/building2.jpg";"Content/building3.jpg"]
        let material = new SCNMaterial ()
        material.Diffuse.Contents <- texture buildings.[rnd.Next(buildings.Length)]
        material.Diffuse.ContentsTransform <- SCNMatrix4.Scale ( new SCNVector3(float32 width, float32 height, 1.F))
        material.Diffuse.WrapS <- SCNWrapMode.Repeat
        material.Diffuse.WrapT <- SCNWrapMode.Repeat
        material.Diffuse.MipFilter <- SCNFilterMode.Linear
        material.Diffuse.MinificationFilter <- SCNFilterMode.Linear
        material.Diffuse.MagnificationFilter <- SCNFilterMode.Linear
        material.Specular.Contents <- UIColor.Gray

        material.LocksAmbientWithDiffuse <- true

        boxNode.Geometry.FirstMaterial <- material

    override this.ViewDidLoad () =

        let scene = new SCNScene ()

        //Positions everyone        
        let rnd = new Random()

        let random (min, max, clamp) =
            let num = float32 ((float (rnd.Next(min, max))) * rnd.NextDouble())
            match clamp with
            | false -> nfloat num
            | _ -> match num with
                   | i when i < 1.0F -> nfloat 1.0F
                   | _ -> nfloat num

        List.map (fun i -> 
            (building 
                (random (2, 5, true)) 
                (random (2, 5, true)) 
                (random (2, 10, true)) 
                (random (-20, 20, false)) 
                (random (-20, 20, false))
                scene 
                rnd)) [0..200] |> ignore
        

        //Lights!
        let lightNode = new SCNNode()
        lightNode.Light <- new SCNLight ()
        lightNode.Light.LightType <- SCNLightType.Omni
        lightNode.Position <- new SCNVector3 (30.0F, 20.0F, 60.0F)
        scene.RootNode.AddChildNode (lightNode)
 
        let ambientLightNode = new SCNNode ()
        ambientLightNode.Light <- new SCNLight ()
        ambientLightNode.Light.LightType <- SCNLightType.Ambient
        ambientLightNode.Light.Color <- UIColor.DarkGray
        scene.RootNode.AddChildNode (ambientLightNode)
 
        //Camera!
        let cameraNode = new SCNNode ()
        cameraNode.Camera <- new SCNCamera ()
        scene.RootNode.AddChildNode (cameraNode)
        cameraNode.Position <- new SCNVector3 (0.0F, 10.0F, 20.0F)

        let targetNode = new SCNNode ()
        targetNode.Position <- new SCNVector3 (00.0F, 1.5F, 0.0F);
        scene.RootNode.AddChildNode (targetNode)

        let lc = SCNLookAtConstraint.Create (targetNode);
        cameraNode.Constraints <- [lc].ToArray().Cast<SCNConstraint>().ToArray()
 
        let scnView = new SCNView(UIScreen.MainScreen.Bounds)
        scnView.Scene <- scene
        scnView.AllowsCameraControl <- true
        scnView.ShowsStatistics <- true
        scnView.BackgroundColor <- UIColor.FromRGB (52, 152, 219)
 
        let floorNode = new SCNNode ()
        floorNode.Geometry <- new SCNPlane(
            Height=nfloat 40.0F,
            Width=nfloat 40.0F
        )
        floorNode.Position <- SCNVector3.Zero

        let pi2 = Math.PI / 2.0
        floorNode.Orientation <- SCNQuaternion.FromAxisAngle(SCNVector3.UnitX, float32 (0.0 - pi2))

        scene.RootNode.AddChildNode (floorNode)

        let material = new SCNMaterial ()
        material.Diffuse.Contents <- UIImage.FromFile ("Content/road.jpg")
        material.Diffuse.ContentsTransform <- SCNMatrix4.Scale ( new SCNVector3(10.F,10.F,1.F))
        material.Diffuse.MinificationFilter <- SCNFilterMode.Linear
        material.Diffuse.MagnificationFilter <- SCNFilterMode.Linear
        material.Diffuse.MipFilter <- SCNFilterMode.Linear
        material.Diffuse.WrapS <- SCNWrapMode.Repeat
        material.Diffuse.WrapT <- SCNWrapMode.Repeat
        material.Specular.Contents <- UIColor.Gray

        floorNode.Geometry.FirstMaterial <- material

        this.View <- scnView
        
 
[<Register ("AppDelegate")>]
type AppDelegate () =
    inherit UIApplicationDelegate ()
 
    let mutable window:UIWindow = null

    // This method is invoked when the application is ready to run.
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