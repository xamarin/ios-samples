XMBindingLibrarySample Sample Application
================

This project contains our MonoTouch sample. It has a reference to XMBindingLibrarySample.dll and makes calls to our native library written in Objective-C from C#

Example:

	var utility = new XMUtilities();
	var result = utility.Hello("Anuj");

	Console.WriteLine(result);