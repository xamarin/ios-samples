namespace TablePartsFS
open System
open UIKit
open Foundation
open Conversion

[<Register("AppDelegate")>]
type AppDelegate () =
    inherit UIApplicationDelegate()
    let mutable window = null
           
    override x.FinishedLaunching (app, options) =

        // create our window
        window <- new UIWindow (UIScreen.MainScreen.Bounds)
        window.MakeKeyAndVisible ()

        // create the home screen and modify it's frame
        let iPhoneHome = new HomeScreen()
        iPhoneHome.View.Frame <- CoreGraphics.CGRect (nfloat 0,
                                                      UIApplication.SharedApplication.StatusBarFrame.Height,
                                                      UIScreen.MainScreen.ApplicationFrame.Width,
                                                      UIScreen.MainScreen.ApplicationFrame.Height)
        window.RootViewController <- iPhoneHome
        true

module Main = 
    [<EntryPoint>]
    let main args = 
        UIApplication.Main(args, null, "AppDelegate")
        0

