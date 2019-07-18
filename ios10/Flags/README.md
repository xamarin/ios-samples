---
name: Xamarin.iOS - A demonstration of automatic RTL support in Asset Catalogs and UIStackViews
description: This sample project illustrates the usage of Directional Image Assets in an iOS project. By using Directional Image Assets, images shown on-screen...
page_type: sample
languages:
- csharp
products:
- xamarin
technologies:
- xamarin-ios
urlFragment: ios10-flags
---
# A demonstration of automatic RTL support in Asset Catalogs and UIStackViews

This sample project illustrates the usage of Directional Image Assets in an iOS project. By using Directional Image Assets, images shown on-screen can automatically adapt to different layout directions (e.g. right-to-left contexts when running in Arabic or Hebrew), without requiring special code for loading different image variations at runtime.

## Instructions

This can be seen in the project by:

* Running the application
* Tapping on `Start`

The `Back` and `Forward` arrows have been marked as mirrored images in the Xcode project's asset catalog. Therefore, when running in a right-to-left context, these images will automatically mirror themselves horizontally. This can be seen by changing current language in iPhone's Settings.

When running in the environment above, both the forward and back arrows will be pointing in the opposite direction, to reflect their new positions relative to English UI.

## Build Requirements

Xcode 8.0 or later; iOS 10.0 SDK or later

## Target
iOS 10.0 or later

## Refs
[Original sample](https://developer.apple.com/library/prerelease/content/samplecode/Flags/Introduction/Intro.html#//apple_ref/doc/uid/TP40017471)
[Unicode’s encoding of national flags](https://esham.io/2014/06/unicode-flags)

## Copyright

Xamarin port changes are released under the MIT license.

## Author

Ported to Xamarin.iOS by Rustam Zaitov
