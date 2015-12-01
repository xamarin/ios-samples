LookInside
==============

This example shows how to use a custom presentation controller to
create a custom view controller presentation. It provides a
transitioning delegate to the view controller, which vends a
presentation controller and animator object.

The LookInside project contains the following interesting classes:

* `RootViewController`. This is the root view controller of the
  application. It provides a scrolling grid of photos through a
  `UICollectionView`. When one of the photos is tapped, it performs a
  view controller presentation

* `OverlayViewController`. This is the view controller for the photo
  editing interface. It has an image view, some sliders, and a save
  button. It uses `CoreImage` to change the HSV of the image. Note
  that the view controller provides no dimming views, borders, or
  other chrome

* `OverlayTransitioningDelegate`. This is the transitioning delegate
  used for the Overlay presentation style. It is a subclass from the
  `UIViewControllerTransitioningDelegate` and provides a custom
  animator object and presentation controller

* `OverlayAnimatedTransitioning`. This is the animator object used for
  the Overlay presentation style. It animates the presented view
  controller in from the right side of the display with a spring
  animation

* `OverlayPresentationController`. This is the presentation controller
  used for the Overlay presentation style. It provides sizing
  information for the presented view controller to position it on the
  right edge of the display. It also provides a dimming view for use
  in the presentation. It implements a gesture on the dimming view to
  dismiss the presented view controller

* `CoolTransitioningDelegate`. This is the transitioning delegate used
  for the Cool presentation style. It is a subclass from the
  `UIViewControllerTransitioningDelegate` and provides a custom
  animator object and presentation controller

* `CoolAnimatedTransitioning`. This is the animator object used for
  the Cool presentation style. It animates the presented view
  controller in from the center of the display using a scale animation

* `CoolPresentationController`. This is the presentation controller
  used for the Cool presentation style. It provides sizing information
  for the presented view controller to position it in the center of
  the display. It positions a pink view behind the presented view
  controller. It also provides custom chrome - leopard print borders,
  a pink flower, and a unicorn


Instructions
------------

For shifting between Normal and Cool transitions use the switch at the
top left corner. Tap image then change settings with sliders and hit
Save button.

Known issue
-----------

This sampe and original sample from `Apple` consume lot of memory so
you can find a crash by applying filters to all images within the app.

Build Requirements
------------------

* Building this sample requires Xcode 6.0 and iOS 8.0 SDK
* Running this sample requires Xamarin.iOS 8.2 or later.

Target
------
This sample runnable on iPhoneSimulator/iPadSimulator iPhone/iPad

Author
------ 
IOS:
Copyright (C) 2014 Xamarin Inc. All rights reserved.

Ported to Xamarin.iOS by Rustam Zaitov
