Simple Text Input Demo
======================

An example showing how to implement a simple text-editing application using Core Text that adopts
various iOS 5 Objective-C protocols in a single class.

It shows how to use the UITextInput and UIKeyInput protocols to connect to the Core Text API.

The file EditableCoreTextView.cs contains a sample showing how to adopt multiple protocols in 
C# using the [Adopts] attribute.

Based on the Apple sample:
http://developer.apple.com/library/ios/#samplecode/SimpleTextInput/Introduction/Intro.html

Known Issues
------------

Autocorrection suggestions don't show up as they do in the original Apple sample. This is
possibly due to http://bugzilla.xamarin.com/show_bug.cgi?id=265.

Authors
-------

Rolf Bjarne Kvinge
