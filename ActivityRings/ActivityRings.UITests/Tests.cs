using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.iOS;
using Xamarin.UITest.Queries;

namespace ActivityRings.UITests
{
	[TestFixture]
	public class Tests
	{
		iOSApp app;

		[SetUp]
		public void BeforeEachTest()
		{
			app = ConfigureApp.iOS.StartApp();
		}

		[Test]
		public void TextViewIsDisplayed()
		{
			AppResult[] results = app.WaitForElement(c => c.Marked("InfoTextView"));
			app.Screenshot("First screen.");

			Assert.IsTrue(results.Any());
		}
	}
}

