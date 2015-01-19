//
//  Copyright 2012  abhatia
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

namespace XMBindingLibrarySample
{
	using System;
	using System.Drawing;
	using Foundation;
	using UIKit;
	using ObjCRuntime;

	public delegate void XMUtilityCallback (NSString message);

	[BaseType (typeof (NSObject))]
	interface XMUtilities
	{
//		Note: that we DO NOT have to bind a constructor since
//		BTOUCH will automatically create the default / empty constructor for us, cool!
//		[Export("init")]
//		IntPtr Constructor();

		// Note that this is attributed with static because it is a class method

		[Static]
		[Export("echo:")]
		string Echo(string message);

		// Methods without a parameter do not need the trailing colon (":")
		// But methods with parameters do!

		[Export("hello:")]
		string Hello(string name);

		// Again, here we have two parameters, but the method name
		// and each argument are suffxed with a colon.

		[Export("add:and:")]
		nint Add(nint operandUn, nint operandDeux);

		[Export("multiply:and:")]
		nint Multiply(nint operandUn, nint operandDeux);

		[Export("setCallback:")]
		void SetCallback(XMUtilityCallback callback);

		[Export("invokeCallback:")]
		void InvokeCallback(NSString message);
	}

	[BaseType(typeof(UIView), Delegates = new string [] {"WeakDelegate"}, Events = new Type[] { (typeof(XMCustomViewDelegate)) })]
	interface XMCustomView
	{
		[Export("name")]
		string Name { get; [NullAllowed] set; }

		[Export("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap("WeakDelegate")]
		[NullAllowed]
		XMCustomViewDelegate Delegate { get; set; }

		[Export("customizeViewWithText:")]
		void CustomizeViewWithText(string message);
	}

	[Model]
	[BaseType(typeof (NSObject))]
	interface XMCustomViewDelegate
	{
		// Notice the use of [Abstract] here since the -(void)viewWasTouched:(UIView *)view; method
		// Was defined using the @required keyword in the native library.
		[Abstract]
	    [Export ("viewWasTouched:")]
		void ViewWasTouched(XMCustomView view);
	}
}

