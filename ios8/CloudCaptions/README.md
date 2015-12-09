CloudCaptions
==============
This sample shows how to use `CloudKit` to upload and retrieve `CKRecords` and associated assets. In this example, there are two record types, an `Image` record type and a `Post` record type. Users are able to upload their own photos or select an image already found in an image record type. This example also uses an `NSPredicate` in its `CKQueries` to filter results based on tags.

Refs
----
It is strongly recommended to watch [WWDC 2014 videos](https://developer.apple.com/videos/wwdc/2014/) to become familiar with CloudKit:

* Introducing CloudKit
* Advanced CloudKit

How to setup
------------
The `CloudCaptions.entitlements` file lists two entitlements: `com.apple.developer.icloud-container-identifiers` and `com.apple.developer.icloud-services`. For this sample you must create an iCloud container via Apple's Member Center. You can find  instructions for creating iCloud containers [here](http://developer.xamarin.com/guides/ios/platform_features/introduction_to_the_document_picker/) – you are interested in the "Enabling iCloud in Xamarin" section.

Don't forget change iCloud container identifier from `iCloud.com.xamarin.cloudcaptions` to `iCloud.(Your Bundle ID)`

Try running CloudCaptions on your device. You may run into provisioning issues – in this case you should check your CloudCaptions.entitlements and provisioning settings.

Once the app is running, you may see a `CKInternalError` in the console output. To resolve this, all you need to do is view your container in [CloudKit dashboard](https://icloud.developer.apple.com/dashboard/).

Try running the app again. You will start seeing `CKUnknownItem` errors in the log output. These occur because we're querying for records that CloudKit has never seen before. To resolve these, all we need to do is upload a new post. Tap compose, then tap `Take Photo`, take a picture and write a caption. Finally, tap `post`.

You will no longer see `CKUnknownItem` errors in the console. You now will start seeing `CKInvalidArgument` errors. By default, CloudKit can't sort or query by record metadata like creation date or last modified date. To change this, go to your container's dashboard and click `Record Types`. You should see two new record types: `Image` and `Post`. For each record type, click `Metadata Index` and check the box to sort by date created.

Run CloudCaptions one more time and you should see the post you just made. Your container is now set up to use CloudCaptions.


Build Requirements
------------------

Building this sample requires Xcode 6.0 and iOS 8.0 SDK

Target
------
This sample runnable on iPhoneSimulator/iPadSimulator iPhone/iPad

Author
------ 
iOS:
Copyright (C) 2014 Xamarin Inc. All rights reserved.

Ported to Xamarin.iOS by Rustam Zaitov
