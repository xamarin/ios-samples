---
name: Xamarin.iOS - AVCustomEdit
description: AVCustomEdit is a simple AVFoundation based movie editing application demonstrating custom compositing to add transitions. It implements the...
page_type: sample
languages:
- csharp
products:
- xamarin
technologies:
- xamarin-ios
urlFragment: avcustomedit
---
# AVCustomEdit

AVCustomEdit is a simple AVFoundation based movie editing application demonstrating custom compositing to add transitions. It implements the CustomVideoCompositor and CustomVideoCompositionInstruction protocols to have access to individual source frames, which are then be rendered using OpenGL off screen rendering.

This is a port of Apple's iOS7 sample AVCustomEdit.

![Home View](Screenshots/screenshot-1.png)

## Instructions
1. Open the transition menu in the lower right. Select a transition.
2. Press play and observe the video.
3. When satisfied, export the video.

## Build
Building this sample requires Xcode 5.0 and iOS 7 or later.

## License

Xamarin port changes are released under the MIT license.

## Author

Ported to Xamarin.iOS by Timothy Risi/Mykyta Bondarenko.
