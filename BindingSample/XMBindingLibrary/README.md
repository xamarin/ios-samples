XMBindingLibrary Binding Project
======================================

This project contains the following items:

- Extras.cs  
  This is where we define the partial types for our interface definitions.
- ApiDefinition.cs  
  The API definition file which contains the interfaces which have been attributed to drive the binding.
- Structs.cs  
  The enumerations and structs file which contains any non Objective-C types.
- BindingActions.targets  
  The MSBuild targets file that will make sure the native library is built before trying to bind it.

## Building the Binding Library

The resulting assembly is created using [Native References](https://docs.microsoft.com/en-us/xamarin/cross-platform/macios/native-references) and will automatically embed the native library in your application.
