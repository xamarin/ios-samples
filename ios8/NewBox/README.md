NewBox
======

This sample shows you how to:
* Use `UIDocumentMenuViewController` and `UIDocumentMenuViewController`.
* Create Document Provider extensions: DocumentPickerViewController Extension and FileProvider Extension.
* Work with built-in iCloud storage provider.
 

We strongly recommend you to read the following guides before you start:
* [App Extension Programming Guide]
* [Document Picker Programming Guide]

When you run the app you will see two columns. First column shows Document Picker directly and the second shows you Document Picker Menu. Document Picker Menu allows you to select Document Picker and add custom options.

Document Provider extension consists in two parts: Document picker extension and File provider extension. These extensions are presented with the following projects:
* `NBox` – Document picker extension.
* `NBoxFileProvider` – File provider extension.

Setup
-----
You need to setup an Application ID with App Groups and iCloud. Then you need to generate a provisioning profile linked to the newly created AppID.
TODO: Provide a link when the official doc will be available.
To setup AppID, App Groups, Provisioning profiles please look at ...

iOS 8.0 provides a single and built-in iCloud storage provider. The goal of this sample is to add an other storage provider.

To start using the Document Picker and Open/Import from the iCloud storage Provider, you need to create some documents in iCloud iOS 8. The easiest way to do so is to use one of the following apps:
* Pages
* Numbers
* Keynote

You can donwload these apps from the [iOS Dev Center]. Choose iOS 8 tab and download the app's `*.dmg` file.

Also to use Document Picker's Export/Move feature you need to be able to export or move documents to an other app which is compatible with iCloud Drive feature. In order to use that, you need to update your mac to Yosemite. Then you will be able to export documents to Preview or TextEdit apps.

Open/Import documents from iCloud document provider with Document Picker
------------

* Create some documents in iCloud container. For this step you need Pages, Numbers or Keynote.
* Run the NewBox app.
* Choose `Open` or `Import`.
* Choose a file and tap on it. `UIDocumentViewController` will be dismissed.
* Look at the console output to see the results.

Export/Move documents to iCloud document provider with Document Picker
------------
* For this step you need mac with Yosemite.
* Run the NewBox app.
* Choose `Export` or `Move`.
* Select a destination (e.g. within `Preview` or `TextEdit` folder) and tap on `Export(Move) to this location`
* Go to `Finder` on your Mac and choose `iCloud Drive`. Choose a folder (e.g. `Preview` or `TextEdit`)
* You will find your file.

Open/Import documents from NBox file provider extension
-------------------------------------------------------
This section contains known issues. Look at known issues for details.

* Run the NewBox app.
* Choose `Open/Import`.
* Enable your extension. Tap on `Locations` then `More ...`. Enable extension. Tap `Done`.
* Choose `Open`/`Import` again. Tap on "Locations" and choose your extension.
* Tap on `Untitled.txt` button.
* Look at the console output.

Export/Move document to NBox file provider extension
----------------------------------------------------
* Run the NewBox app.
* Choose Export/Move.
* Enable your extension. Tap on `Locations` then `More ...`. Enable extension. Tap `Done`.
* Choose `Export/Move` again. Tap on `Locations` choose your extension.
* Tap on `Export/Move to this location` button.
* Look at the console output. You will see output from callback handler, but the actual document will not be exported/moved (this require additional steps from you as a developer).

Known issues
------------
By design you must be able to see document's content when you `Open` or `Import` it. However the Apple's sample blows up. You can find the chunk of commented code inside `OnDocumentPicked` method within `NewBox` project. Because of this you will see only console's messages from `OnDocumentPicked` method.

Build Requirements
------------------

Building this sample requires Xcode 6.0 and iOS 8.0 SDK.

Target
------
This sample is runnable on iPhone/iPad.

Author
------ 
iOS:
Copyright (C) 2014 Apple Inc. All rights reserved.

Ported to Xamarin.iOS by Rustam Zaitov.

[App Extension Programming Guide]:https://developer.apple.com/library/prerelease/mac/documentation/General/Conceptual/ExtensibilityPG/FileProvider.html

[Document Picker Programming Guide]:https://developer.apple.com/library/prerelease/ios/documentation/FileManagement/Conceptual/DocumentPickerProgrammingGuide/AccessingDocuments/AccessingDocuments.html

[iOS Dev Center]:https://developer.apple.com/devcenter/ios/index.action
