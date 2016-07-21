using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.iOS;
using Xamarin.UITest.Queries;
using System.Threading;

namespace SamplePhotoAppTest
{
	[TestFixture]
	public class Tests
	{
		iOSApp app;
		const string API_KEY = "024b0d715a7e9c22388450cf0069cb19";
		const string DEVICE_WIFI_IP = "192.168.2.5";
		const string BUNDLE_IDENTIFIER = "com.xamarin.SamplePhotosApp";

		//[TestFixtureSetUp]
		[SetUp]
		public void BeforeEachTest ()
		{
			// TODO: If the iOS app being tested is included in the solution then open
			// the Unit Tests window, right click Test Apps, select Add App Project
			// and select the app projects that should be tested.
			//
			// The iOS project should have the Xamarin.TestCloud.Agent NuGet package
			// installed. To start the Test Cloud Agent the following code should be
			// added to the FinishedLaunching method of the AppDelegate:
			//
			//    #if ENABLE_TEST_CLOUD
			//    Xamarin.Calabash.Start();
			//    #endif
			//app = ConfigureApp
			//.iOS
			// TODO: Update this path to point to your iOS app and uncomment the
			// code if the app is not included in the solution.
			//.AppBundle ("/Users/360_macmini/Desktop/UITest/monotouch-samples/ios9/SamplePhotoApp/SamplePhotoApp/bin/iPhoneSimulator/Debug/SamplePhotoApp.app")
			//.StartApp ();
			DeployOnDevice ();
		}


		void DeployOnDevice ()
		{
			app = ConfigureApp.Debug ().iOS.EnableLocalScreenshots ().InstalledApp (BUNDLE_IDENTIFIER).DeviceIp (DEVICE_WIFI_IP).StartApp ();
		}


		[Test]
		public void AppLaunches ()
		{
			//app.Screenshot ("First screen.");
			//app.Repl ();
		}

		[Test]
		public void AddPhoto ()
		{
			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UITableViewCell").Text ("All Photos"));
			}, "Unable to tap All Photos");


			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UINavigationButton"));
			}, "Image was not added.");

		}


		[Test]
		public void DeletePhoto ()
		{
			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UITableViewCell").Text ("All Photos"));
			}, "Unable to tap All Photos");


			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UIImageView"));
			}, "Unable to click Image");

			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UIToolbarButton"));
			}, "Unable to Click delete button");

			Assert.DoesNotThrow (() => {
				app.InvokeUia ("uia.tapMark(\"Delete\")");
			}, "Unable Delete Image.");

		}



		[Test, TestCaseSource ("UiEditElementList")]
		public void CheckEditPhotosUi(string classname, string uiElementText)
		{
			app.Screenshot ("Application Started");

			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UITableViewCell").Text ("All Photos"));
			}, "Unable to tap All Photos");

			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UINavigationButton"));
			}, "Unable to tap Edit option");

			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UIImageView"));
			}, "Unable to tap on Image.");

			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UIButtonLabel").Text ("Edit"));
			}, "Unable to tap Edit option");

			Assert.IsTrue (app.Query (x => x.Class (classname).Text (uiElementText)).Any (), uiElementText + " was not found");
			app.Screenshot ("First screen.");
		}

		public void GoToAllPhotos()
		{
			app.Screenshot ("Application Started");


			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UITableViewCell").Text ("All Photos"));
			}, "Unable to tap All Photos");


			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UIImageView"));
			}, "Unable to click Image");

		}

		[Test, TestCaseSource("Tapfaourite_unfavorite")]
		public void TapFaourite_UnFavorite(string classname, string uiElementText)
		{
			GoToAllPhotos ();
			app.Tap (x=>x.Marked("Edit"));
			AppResult [] result = app.Query (x => x.Class ("UILabel"));

			uiElementText = result [2].Text;

				Assert.DoesNotThrow (() => {
					app.Tap (x => x.Class (classname).Text (uiElementText));

				}, "Unable to tap" + uiElementText + " Element."
				      );

		}

		[Test, TestCaseSource ("UiEditElementList")]
		public void TapEditPhotosUi (string classname, string uiElementText)
		{
			if (uiElementText == "Unfavorite") {
				GoToAllPhotos ();
				Assert.DoesNotThrow (() => {
					app.Tap (x => x.Marked ("Edit"));
					app.Tap (x => x.Class (classname).Text (uiElementText));

				}, "Unable to tap" + uiElementText + " Element."
				      );

				app.Screenshot ("First screen.");
			}
			
			if (uiElementText == "Favorite") 
			{
				GoToAllPhotos ();
				Assert.DoesNotThrow (() => {
					app.Tap (x => x.Marked ("Edit"));
					app.Tap (x => x.Class (classname).Text (uiElementText));

				}, "Unable to tap" + uiElementText + " Element."
				      );

				app.Screenshot ("First screen.");
			}

			if (uiElementText == "Sepia") 
			{
				GoToAllPhotos ();
				Assert.DoesNotThrow (() => {
					app.Tap (x => x.Marked ("Edit"));
					app.Tap (x => x.Class (classname).Text (uiElementText));
					}, "Unable to tap " + uiElementText + " Element.");

				Assert.DoesNotThrow (() => {
					Thread.Sleep (20000);
					app.InvokeUia ("uia.tapMark(\"Modify\")");
				}, "Unable Modify image.");

				app.Screenshot ("First screen.");
			}

			if (uiElementText == "Chrome") {
				GoToAllPhotos ();
				Assert.DoesNotThrow (() => {
					app.Tap (x => x.Marked ("Edit"));
					app.Tap (x => x.Class (classname).Text (uiElementText));

				}, "Unable to tap" + uiElementText + " Element."
				      );

				Assert.DoesNotThrow (() => {
					Thread.Sleep (20000);
					app.InvokeUia ("uia.tapMark(\"Modify\")");
				}, "Unable Modify image.");

				app.Screenshot ("First screen.");
			}

			if (uiElementText == "Revert") {
				GoToAllPhotos ();
				Assert.DoesNotThrow (() => {
					app.Tap (x => x.Marked ("Edit"));
					app.Tap (x => x.Class (classname).Text (uiElementText));

				}, "Unable to tap" + uiElementText + " Element."
				      );

				Assert.DoesNotThrow (() => {
					Thread.Sleep (20000);
					app.InvokeUia ("uia.tapMark(\"Revert\")");
				}, "Unable Modify image.");

				app.Screenshot ("First screen.");
			}
			if (uiElementText == "Cancel") {
				GoToAllPhotos ();
				Assert.DoesNotThrow (() => {
					app.Tap (x => x.Marked ("Edit"));
					app.Tap (x => x.Class (classname).Text (uiElementText));

				}, "Unable to tap" + uiElementText + " Element."
				      );

			}
		}


		[Test, TestCaseSource ("UiElementList")]
		public void CheckInitialUiElements(string classname, string uiElementText)
		{
			Func<AppQuery, AppQuery> result = x => x.Marked (uiElementText);

			app.Screenshot ("Application Started");
			app.ScrollDownTo (result);
			Assert.IsTrue (app.Query (x => x.Class (classname).Text (uiElementText)).Any(), uiElementText + " was not found");
			app.Screenshot ("First screen.");

		}

		[Test, TestCaseSource ("UiElementList")]
		public void TapInitialUIElements (string classname, string uiElementText)
		{
			Func<AppQuery, AppQuery> result = x => x.Marked (uiElementText);
			
			Assert.DoesNotThrow (() => {
				app.ScrollDownTo (result);
				app.Tap (x => x.Class (classname).Text (uiElementText));

			}, "Unable to tap Element."
			      );
			
		}

		static object [] UiElementList = {
			new []{"UITableViewCell", "All Photos"},
			new []{"UITableViewCell", "Favorites"},
			new []{"UITableViewCell", "Recently Deleted"},
			new []{"UITableViewCell", "Panoramas"},
			new []{"UITableViewCell", "Slo-mo"},
			new []{"UITableViewCell", "Screenshots"},
			new []{"UITableViewCell", "Bursts"},
			new []{"UITableViewCell", "Videos"},
			new []{"UITableViewCell", "Selfies"},
			new []{"UITableViewCell", "Hidden"},
			new []{"UITableViewCell", "Time-lapse"},
			new []{"UITableViewCell", "Recently Added"},
			new []{"UITableViewCell", "Camera Roll"},
		};


		static object [] UiEditElementList = {
			new []{"UILabel", "Sepia"},
			new []{"UILabel", "Chrome"},
			new []{"UILabel", "Revert"},
			new []{"UILabel", "Cancel"},
		};

		static object [] Tapfaourite_unfavorite = {
			new []{"UILabel", "Fav/Unfavorite"},

		};



		[Test]
		public void AddNewFolder()
		{
			Func<AppQuery, AppQuery> result = x => x.Marked ("TestFolder");
			
			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UINavigationButton"));
				app.EnterText (x => x.Class ("UITextFieldLabel"), "TestFolder");
				app.Tap (x => x.Class ("UILabel").Text ("Create"));
				app.ScrollDownTo (result);
				app.Tap (x => x.Marked ("TestFolder"));
				BackToLastPage ();

			}, "Unable to Find/Create new folder.");

		}


		[Test]
		public void AddImagesToNewFolder()
		{
			Func<AppQuery,AppQuery> result = x => x.Marked ("TestFolder");
			
			Assert.DoesNotThrow (() => {
				app.ScrollDownTo (result);
				app.Tap (x => x.Marked ("TestFolder"));
				app.Tap (x => x.Class ("UINavigationButton"));
				BackToLastPage ();

			}, "Unable add image to new folder.");

		}

		[Test]
		public void CheckFavorite ()
		{
			Func<AppQuery, AppQuery> result = x => x.Marked ("Favorites");

			Assert.DoesNotThrow (() => {
				app.ScrollDownTo (result);
				app.Tap (x => x.Class ("UITableViewCell").Text ("Favorites"));
			}, "Unable to tap Favorites");

			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UIImageView"));
			}, "Favorites images are not avilable.");

		}


		[Test]
		public void CheckSelfies ()
		{
			Func<AppQuery, AppQuery> result = x => x.Marked ("Selfies");

			Assert.DoesNotThrow (() => {
				app.ScrollDownTo (result);
				app.Tap (x => x.Class ("UITableViewCell").Text ("Selfies"));
			}, "Unable to tap Selfies");

			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UIImageView"));
			}, "Selfies are not avilable.");


		}

		[Test]
		public void CheckCameraRoll ()
		{
			Func<AppQuery, AppQuery> result = x => x.Marked ("Camera Roll");
			
			Assert.DoesNotThrow (() => {
				app.ScrollDownTo (result);
				app.Tap (x => x.Class ("UITableViewCell").Text ("Camera Roll"));
			}, "Unable to Camera Roll");

			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UIImageView"));
			}, "Images are not avilable.");

		}

		[Test]
		public void CheckRecentlyAdded ()
		{
			Func<AppQuery, AppQuery> result = x => x.Marked ("Recently Added");

			Assert.DoesNotThrow (() => {
				app.ScrollDownTo (result);
				app.Tap (x => x.Class ("UITableViewCell").Text ("Recently Added"));
			}, "Unable to tap Recently Added");

			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UIImageView"));
			}, "Recently added Images are not avilable.");



		}

		[Test]
		public void CheckRecentlyDeleted ()
		{
			Func<AppQuery, AppQuery> result = x => x.Marked ("Recently Deleted");
			
			Assert.DoesNotThrow (() => {
				app.ScrollDownTo (result);
				app.Tap (x => x.Class ("UITableViewCell").Text ("Recently Deleted"));
			}, "Unable to tap Recently Deletd");

			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UIImageView"));
			}, "Recently Deleted Images are not avilable.");


		}

		[Test]
		public void CheckVideos()
		{
			Func<AppQuery, AppQuery> result = x => x.Marked ("Videos");
			
			Assert.DoesNotThrow (() => {
				app.ScrollDownTo (result);
				app.Tap (x => x.Class ("UITableViewCell").Text ("Videos"));
			}, "Unable to tap Videos");

			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UIImageView"));
			}, "Videos are not avilable.");



		}

		[Test]
		public void CheckHidden ()
		{
			Func<AppQuery, AppQuery> result = x => x.Marked ("Hidden");

			Assert.DoesNotThrow (() => {
				app.ScrollDownTo (result);
				app.Tap (x => x.Class ("UITableViewCell").Text ("Hidden"));
			}, "Unable to tap Hidden");

			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UIImageView"));
			}, "Hidden files are not avilable.");



		}
		[Test]
		public void CheckSlo_mo ()
		{
			Func<AppQuery, AppQuery> result = x => x.Marked ("Slo-mo");

			Assert.DoesNotThrow (() => {
				app.ScrollDownTo (result);
				app.Tap (x => x.Class ("UITableViewCell").Text ("Slo-mo"));
			}, "Unable to tap Slo-mo");

			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UIImageView"));
			}, "Slo-mo are not avilable.");


		}
		[Test]
		public void CheckBursts()
		{
			Func<AppQuery, AppQuery> result = x => x.Marked ("Bursts");

			Assert.DoesNotThrow (() => {
				app.ScrollDownTo (result);
				app.Tap (x => x.Class ("UITableViewCell").Text ("Bursts"));
			}, "Unable to tap Bursts");

			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UIImageView"));
			}, "Bursts are not avilable.");


		}
		[Test]
		public void CheckPanoramas ()
		{
			Func<AppQuery, AppQuery> result = x => x.Marked ("Panoramas");

			Assert.DoesNotThrow (() => {
				app.ScrollDownTo (result);
				app.Tap (x => x.Class ("UITableViewCell").Text ("Panoramas"));
			}, "Unable to tap Panoramas");

			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UIImageView"));
			}, "Panoramas are not avilable.");


		}
		[Test]
		public void CheckScreenshots ()
		{
			Func<AppQuery, AppQuery> result = x => x.Marked ("Screenshots");

			Assert.DoesNotThrow (() => {
				app.ScrollDownTo (result);
				app.Tap (x => x.Class ("UITableViewCell").Text ("Screenshots"));
			}, "Unable to tap Screenshots");

			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UIImageView"));
			}, "Screenshots are not avilable.");


		}

		[Test]
		public void CheckTime_Lapse ()
		{
			Func<AppQuery, AppQuery> result = x => x.Marked ("Time-lapse");

			Assert.DoesNotThrow (() => {
				app.ScrollDownTo (result);
				app.Tap (x => x.Class ("UITableViewCell").Text ("Time-lapse"));
			}, "Unable to tap Time-lapse");

			Assert.DoesNotThrow (() => {
				app.Tap (x => x.Class ("UIImageView"));
			}, "Time-lapse are not avilable.");


		}


		public void BackToLastPage()
		{
			Assert.DoesNotThrow (() =>{
				app.Back ();
			},"Back Page does not Exist");
			

		}

		
		[TestFixtureTearDown]
                 public void TearDownTest ()
                 {
                   app = null;
                 }
	}
}

