XMBindingLibrarySample Sample Application
================

This project contains our Xamarin.iOS sample. It has a reference to XMBindingLibrary.dll and makes calls to our native library written in Objective-C from C#.

Example:

	var utility = new XMUtilities();
	var result = utility.Hello("Developer");

	Console.WriteLine(result);
