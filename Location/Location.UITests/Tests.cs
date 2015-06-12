using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.iOS;
using Xamarin.UITest.Queries;

namespace Location.UITests
{
	[TestFixture]
	public class Tests
	{
		iOSApp app;

		[SetUp]
		public void BeforeEachTest ()
		{
			app = ConfigureApp.iOS.StartApp ();
		}

		[Test]
		public void ViewIsDisplayed ()
		{
			AppResult[] results = app.WaitForElement (c => c.Child ("UIView"));
			app.Screenshot ("First screen.");

			Assert.IsTrue (results.Any ());
		}
	}
}


