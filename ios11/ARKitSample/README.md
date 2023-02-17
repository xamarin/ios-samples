---
name: Xamarin.iOS - ARKit Sample
description: "This sample is a simple ARKit augmented reality demo. To test this sample you'll need to deploy it on a device running iOS 11 beta software (iOS11)"
page_type: sample
languages:
- csharp
products:
- xamarin
extensions:
    tags:
    - ios11
urlFragment: ios11-arkitsample
---
# ARKit Sample

This sample is a simple ARKit augmented reality demo.

To test this sample you'll need to deploy it on a device running iOS 11 beta software.

![AR Jet](Screenshots/jet.png)

By default, the sample presents a jet in a fixed location. To animate the jet around the viewer find the `HACK` in **GameViewController.cs**: comment out the line `ship.Position = new SCNVector3(2f, -2f, -9f);` and uncomment the code that creates an animation around a pivot node.

## License

Xamarin port changes are released under the MIT license
