Xamarin.iOS Binding Sample
===============================

This example shows how we can utilize an existing Objective-C library and expose it for use in a Xamarin.iOS project. For instance, you may have existing code written in Objective-C that you may want to bind to C# to consume in your Xamarin.iOS project. This sample provides a basic template/overview of the steps involved, including:

- Creating a "fat" or multi-architecture library that can be target both the iOS simulator and device.
- Defining an API definition file in the form of a C# interface against the Objective-C API.
- Building a `*.dll` that contains both the binding and and the embedded native library.

## Understanding this Sample

This sample consists of three distinct source projects:

- Xcode Project in Objective-C
- Xamarin.iOS Binding classes
- Xamarin.iOS Sample Project

Please see the README in each project folder for more details.
