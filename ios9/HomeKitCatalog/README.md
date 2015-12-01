HomeKitCatalog
==============

HomeKit Catalog demonstrates how to use the HomeKit API, to create homes, to associate accessories with homes, to group the accessories into rooms and zones, to create actions sets to tie together multiple actions, to create timer triggers to fire actions sets at specific times, and to create service groups to group services into contexts.

To build the sample, you will need to define an application bundle ID and enable `HomeKit` and `Wireless Accessory Configuration` within `Application Services` via [MemberCenter](https://developer.apple.com/account/ios/identifiers/bundle/bundleList.action).

If you find any problems with this sample, please [file a bug against it](https://github.com/xamarin/monotouch-samples/issues).

Instructions
------------

To use the sample, you should have HomeKit accessories already associated with the current WiFi LAN with which your device is attached. Alternatively, you can use the HomeKit Accessory Simulator running on you OS X System, to simulate the presence of a variety of HomeKit Accessories. When you launch the app, switch to the Configure tab to add new homes.

You may then select a home and perform the following actions:

1. Define the names of the rooms (Bedroom, Living Room, etc) in the home, define zones as a collection of rooms in the home (first floor),
2. Define Action Sets (turn off Kitchen lights),
3. Define Triggers (turn off lights at 10PM),
4. Define Service Groups (subset of accessories in a room), and
5. Define other users who can control the accessories in your home.

Note: For information on using the HomeKit Accessory Simulator, please refer to the HomeKit Accessory Simulator Help under the Help menu.

Use the Configure tab to set up the home, associate accessories with each room, and to perform the actions described above. Use the Control button to control the accessories in the home.

Considerations
--------------

HomeKit operates asynchronously. Frequently, you will have to defer some UI response until all operations associated with a particular action are finished. For example, when this sample wants to save a trigger, it must:

1. Create a new trigger object
2. Add the trigger to the home
3. Add all of the specified Action Sets individually
4. Update its name
5. Enable it

This sample makes heavy use of `DispatchGroup`s to ensure all actions are completed before confirming with UI.

This sample also includes many convenience functions implemented as Extensions on HomeKit classes, and provides a very basic, flexible UI that adapts based on HMCharacteristic metadata.

Build Requirements
------------------

HomeKit Catalog requires Xcode 7.0 with the iOS 9.0 SDK to build the application. You can either run the sample code within the iOS Simulator or on a device with iOS 9.0 installed. You can use the HomeKit Accessory Simulator running under OS X to simulate accessories on your local Wi-Fi network. For the Xcode 6.1.1 timeframe, the HomeKit Accessory Simulator is available from the Apple Developer site as part of the [Hardware IO Tools disk image](https://developer.apple.com/downloads/index.action).

Refs
----
[Original sample](https://developer.apple.com/library/prerelease/ios/samplecode/HomeKitCatalog/Introduction/Intro.html#//apple_ref/doc/uid/TP40015048)
[Documentation](https://developer.apple.com/homekit/)

Target
------
This sample runnable on iPhoneSimulator/iPadSimulator iPhone/iPad

Author
------ 
Copyright (C) 2015 Xamarin Inc. All rights reserved.

Ported to Xamarin.iOS by Rustam Zaitov