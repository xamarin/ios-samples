ImageInverter
=============

This sample code shows how to use an Action extension with a view
controller. The app shares an image through a
`UIActivityViewController` while the extension can be used to flip
vertically the shared image and return it to the host app. The host
app and extension are communicating using `NSExtensionItem` and
`NSItemProvider`. The extension shows the basic interaction with the
`NSExtensionContext`.


Instructions
------------

* Launch the app and tap the share icon at the bottom left corner, the
  iOS sharing view should appear

* Scroll through the 3rd row of icons and look for one marked
  ImageInverterExt. If you do not find it, click “More” (with the “…”
  icon) in that list, and enable ImageInverterExt it from the settings
  screen that appears.

* Tap on ImageInverterExt and a window will appear displaying the
  original image from the app vertically inverted.

* Tap “Done” in the top right corner of the window to return to the
  app’s main view.

* The app should now display the vertically inverted image
  ImageInverterExt.

Build Requirements
------------------

Building this sample required Xcode 6.0 and iOS 8.0 SDK.

Author
--------
Copyright (C) 2014 Xamarin Inc. All rights reserved.

Ported to Xamatin.iOS by Rustam Zaitov.
