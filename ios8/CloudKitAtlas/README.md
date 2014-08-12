CloudKitAtlas
======================

CloudKitAtlas is a sample intended as a quick introduction to CloudKit. It teaches you how to use Discoverability to get the first name and last name of the user logged into iCloud. It can add a CKRecord with a location and query for CKRecords near a location. You can upload and retrieve images as CKAssets. It also shows how to use CKReferences. Finally, it also shows how to use CKSubscription to get push notifications when a new item is added for a record type.

## Instructions

1. In the project editor, change the bundle identifier in Signing Options. The bundle identifier is used to create the app’s default container.
2. In the Entitlements enable iCloud and check the CloudKit option.
3. Make sure you are signed into your iCloud account in the simulator or device before running the app.

Author
======
Copyright (C) 2014 Apple Inc. All rights reserved.

Ported to Xamarin.iOS by Oleg Demchenko and Timothy Risi