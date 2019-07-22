---
name: Xamarin.iOS - PictureInPicture
description: "Demonstrates the use of AVPictureInPictureController to get picture in picture playback of video content from an application #ios9"
page_type: sample
languages:
- csharp
products:
- xamarin
urlFragment: ios9-pictureinpicture
---
# PictureInPicture

This sample demonstrates the use of `AVPictureInPictureController` to get picture in picture playback of video content from an application. It shows the steps required to be able to start and stop picture in picture mode and also on how to setup a delegate to receive event callbacks. Clients of AVFoundation using `AVPlayerLayer` for media playback should use `AVPictureInPictureController` class, whereas clients of AVKit who use `AVPlayerViewController` get picture in picture mode without any additional setup.

The sample also demonstrates the configuration setup required by an application to be able to use picture in picture. This configuration involves:

1. Setting UIBackgroundMode to audio under the project settings.

2. Setting audio session category to AVAudioSessionCategoryPlayback or AVAudioSessionCategoryPlayAndRecord (as appropriate)

If an application is not configured correctly, `AVPictureInPictureController.PictureInPicturePossible ()` returns false.

The AppDelegate class configures the application as described above.

The `PlayerViewController` class creates and manages an `AVPictureInPictureController` object. It also handles delegate callbacks to setup / restore UI when in picture in picture. This class also handles the playback setup and UI.


## Build Requirements

Building this sample requires Xcode 7.0 and iOS 9.0 SDK

## Refs
[Original sample](https://developer.apple.com/library/prerelease/ios/samplecode/AVFoundationPiPPlayer/Introduction/Intro.html)

## Target
This sample runnable on iPad/iPadSimulator

## Copyright

Xamarin port changes are released under the MIT license

![PictureInPicture application screenshot](Screenshots/fullScreen.png "PictureInPicture application screenshot")

## Author 

Ported to Xamarin.iOS by Rustam Zaitov
