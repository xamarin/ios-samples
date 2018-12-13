XMBindingLibrary Native Project
======================================

This project contains the following items:

- XMBindingLibrary.xcodeproj  
  This is the Xcode project file.
- Makefile  
  The Makefile that will create a fat archive.

## Building the Native Library

To compile the Xcode Project and binding classes execute the `make` command from the root directory.

The make command will:

- Compile the Xcode Project for ARMv7, ARM64 (Devices), i386 and x86_64 (Simulators) using `xcodebuild`
- Create a multi-architecture binary using `lipo`

## Creating a Universal Binary

A "fat" or multi-architecture library is a compiled binary that is usable on multiple targets, for example: ARMv7, ARM64 (Devices), i386 and x86_64 (Simulators). In this sample we illustrate how to create a universal binary in two ways:

### Using lipo

Once we have built our library against the desired architectures we can create the universal binary via `lipo`. This will create a "universal" file from the architecture specific inputs we have provided. For instance:

	lipo -create libNative-armv7.a libNative-arm64.a libNative-i386.a libNative-x86_64.a -output libNative.a


Similarly, in our Makefile script we have `lipo -create -output $@ $^` which will take the libraries compiled for ARM64, ARMv7, i386 and x86_64 using `xcodebuild` and output them to the current directory with the name of our build target.

### Using Xcode

In our Xcode project we have created a separate `Build Target` that will execute a post build `Run Script` to output a
"universal" file.

![screenshot](https://imgur.com/meVHNH4.png "Build Target - Run Script")
