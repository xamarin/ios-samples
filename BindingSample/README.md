MonoTouch BTouch Binding Sample
===============================

This example shows how we can utilize an existing Objective-C library and expose it for use in a MonoTouch project. For instance, you may have existing code written in Objective-C that you may want to bind to C# to consume in your MonoTouch project. This sample provides a basic template/overview of the steps involved, including:

- Creating a "fat" or multi-architecture library that can be target both the iOS simulator and device.

- Defining an API definition file in the form of a C# interface against the Objective-C API.

- Building a `*.dll` that contains both the binding and and the embedded native library.

##Understanding this Sample


This sample consists of three distinct source projects:

- Xcode Project in Objective-C
- MonoTouch Binding classes
- MonoTouch Sample Project

Please see the README in each project folder for more details.

##Building this Sample


To compile the Xcode Project and binding classes execute the `make` command from the root directory.

The make command will:

- Compile the Xcode Project for ARM6, ARM7, and Simulator
- Create a multi-architecture binary using `lipo`
- Create a `*.dll` in the binding folder using `btouch-native`

The resulting .dll is created using the [LinkWithAttribute](http://docs.xamarin.com/ios/advanced_topics/binding_objective-c_types#Linking_the_Dependencies) and will automatically embed the native library in your application.

##Creating a Universal Binary


A "fat" or multi-architecture library is a compiled binary that is usable on multiple targets, for example: armv6, armv7, i386
(simulator). In this sample we illustrate how to create a universal binary in two ways:

###Using lipo

Once we have built our library against the desired architectures we can create the universal binary via `lipo`. This will create a "universal" file from the architecture specific inputs we have provided. For instance:

	lipo -create libXMBindingLibrarySample-armv7.a libXMBindingLibrarySample-armv6.a libXMBindingLibrarySample-i386.a -output 	libXMBindingLibrarySampleUniversal.a


Similarly, in our Makefile script we have `lipo -create -output $@ $^` which will take the libraries compiled for armv6, armv7, and i386 using xbuild and output them to the current directory with the name of our build target.

###Using Xcode

In our Xcode project we have created a separate `Build Target` that will execute a post build `Run Script` to output a
"universal" file.

![screenshot](http://i.imgur.com/6SIsx.png "Build Target - Run Script")

This Run Script is also available for reference in the "Post-Build Run Script" group of the Xcode project.
