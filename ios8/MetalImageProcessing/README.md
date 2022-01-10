---
name: Xamarin.iOS - MetalImageProcessing
description: "This sample extends the textured quad sample by adding a Metal compute encoder to convert the image to greyscale (iOS8)"
page_type: sample
languages:
- csharp
products:
- xamarin
extensions:
    tags:
    - ios8
urlFragment: ios8-metalimageprocessing
---
# MetalImageProcessing

This sample extends the textured quad sample by adding a Metal compute
encoder to convert the image to greyscale. Note the compute encoder is
part of the same pass as the render encoder and hence demonstrates how
you can use the same shared CPU/GPU data across compute and rendering.


## Build Requirements

This sample requires Xcode 6.0 or later and iOS 8.0 SDK

## Runtime
device with iOS 8 or later and A7 chip

## Copyright

Xamarin port changes are released under the MIT license

## Author 

Ported to Xamarin.iOS by Oleg Demchenko
