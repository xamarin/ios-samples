WatchKit Catalog: Using WatchKit Interface Elements
==================================================
 
WatchKit Catalog is an exploration of the UI elements available in the WatchKit framework. Throughout the sample, you'll find tips and configurations that will help guide the development of your WatchKit app.
 
Tips
----

- Tapping the Glance will launch the WatchKit app. In GlanceController, `UpdateUserActivity ()` is called in `WillActivate ()` and takes advantage of Handoff to launch the wearer into the image detail controller (ImageDetailController). When the WatchKit app is launched from the Glance, `HandleUserActivity ()` is called in InterfaceController. ImageDetailController will be pushed to, as its controller's Identifier string is passed in the userActivity dictionary.

- ButtonDetailController has two examples of how to hide and show UI elements at runtime. First, tapping on button "1" will toggle the hidden property of button "2." When hiding the button, the layout will change to make use of the newly available space. When showing it again, the layout will change to make room for it. The second example is by setting the alpha property of button "2" to 0.0 or 1.0. Tapping on button "3" will toggle this and while button "2" may look invisible, the space it takes up does not change and no layout changes will occur.

- WKInterfaceDevice's screenBounds property is used in ImageDetailController to decide which image resource to load at runtime, depending on the wearer having a 38mm or 42mm Apple Watch. This technique should also be used for setting the height or width of fixed-sized UI elements at runtime, as is needed.

- In ImageDetailController, note the comments where the "Walkway" image is being sent across to Apple Watch from the WatchKit Extension bundle. The animated image sequence is stored in the WatchKit app bundle. Comments are made throughout the sample project where images are used from one bundle or another.

- In the storyboard scene for GroupDetailController, note the use of nested groups to achieve more sophisticated layouts of images and labels. This is highly encouraged and will be necessary to achieve specific designs.

- TableDetailController has an example of inserting more rows into a table after the initial set of rows have been added to a table.

- ControllerDetailController shows how to present a modal controller, as well as how to present a controller that does not match the navigation style of your root controller. In this case, the WatchKit app has a hierarchical navigation style. Using the presentation of a modal controller though, we are able to present a page-based set of controllers.

- ControllerDetailController can present a modal controller. The "Dismiss" text of the modal controller is set in the Title field in the Attributes Inspector of the scene for PageController.

- TextInputController presents the text input controller with a set of suggestions. The result is sent to the parent iOS application and a confirmation message is sent back to the WatchKit app extension.
 
WatchKit Controls
--------------
 
WatchKit Catalog demonstrates how to configure and customize the following controls:
 
* WKInterfaceController
* WKInterfaceDevice
* WKInterfaceGroup
* WKInterfaceImage
* WKInterfaceLabel
* WKInterfaceMap
* WKInterfaceSeparator
* WKInterfaceSlider
* WKInterfaceSwitch
* WKInterfaceTable
* WKInterfaceTimer
* WKUserNotificationInterfaceController

Build/Runtime Requirements 
--------------------------

* Xcode 6.2 or newer.
* Xamarin.iOS 8.8 or newer.
 
Author 
------

Copyright (C) 2015 Xamarin Inc. All rights reserved.  
Ported to Xamarin.iOS by Vincent Dondain.
