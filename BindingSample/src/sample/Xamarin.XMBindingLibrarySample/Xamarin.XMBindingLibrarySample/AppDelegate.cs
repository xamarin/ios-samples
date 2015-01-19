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
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Xamarin.XMBindingLibrarySample
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow _Window;
		RootViewController _RootViewController;

		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			_Window = new UIWindow(UIScreen.MainScreen.Bounds);
			_RootViewController = new RootViewController();

			_Window.RootViewController = _RootViewController;
			_Window.MakeKeyAndVisible();
			return true;
		}
	}
}

