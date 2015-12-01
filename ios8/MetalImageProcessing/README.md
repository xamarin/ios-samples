MetalImageProcessing
====================

This sample extends the textured quad sample by adding a Metal compute
encoder to convert the image to greyscale. Note the compute encoder is
part of the same pass as the render encoder and hence demonstrates how
you can use the same shared CPU/GPU data across compute and rendering.


Build Requirements
------------------

This sample requires Xcode 6.0 or later and iOS 8.0 SDK

Runtime
------------------
device with iOS 8 or later and A7 chip

Author
------ 
Copyright (C) 2014 Xamarin Inc. All rights reserved.

Ported to Xamarin.iOS by Oleg Demchenko
