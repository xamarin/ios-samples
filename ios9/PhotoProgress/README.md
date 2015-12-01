PhotoProgress: Using NSProgress
==============

This sample demonstrates how to create and compose NSProgress objects, and show their progress in your app.

The app presents a collection view of photos, which will initially be placeholder images. Tap the "Import" button to import the album of photos, showing progress for each individual import as well as an overall progress. The import operation for each photo is composed of a faked "download" step followed by a filter step. Once the import of a photo finishes, the final image is displayed in the collection view cell.

Build Requirements
------------------

Building this sample requires Xcode 7.0, iOS 9.0 SDK. This sample use new C# 6 features which means that you need IDE with C# 6 support(Xamarin Studio, Visual Studio 2015).

Runtime Requirements
------------------

Running the sample requires iPhone/iPhoneSimulator with iOS 9.0 or later.

Useful links
-------------

[Swift version of sample](https://developer.apple.com/library/prerelease/ios/samplecode/PhotoProgress/Introduction/Intro.html#//apple_ref/doc/uid/TP40016186)

Author
======
Copyright (C) 2015 Xamarin Inc. All rights reserved.

Ported to Xamarin.iOS by Oleg Demchenko
