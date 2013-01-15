using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace GoogleAdMobAds
{	
	/// <summary>
	/// A <see cref="T:MonoTouch.UIKit.UIView"/> that displays small HTML5 ads that respond to user events.
	/// <summary>
	/// <remarks>
	///   <para>
	///   Google AdMob Ads banners allow the application user to click-through to a full-screen advertising experience.
	///   The user may exit that full-screen experience either by returning to the application or by activating other applications
	///   such as iTunes.
	///   </para>
	///   <para>
	///   There may be a delay of up to 2 minutes the first time AdMob sees the application's publisher ID (see 
	///   <see cref="T:GoogleAdMobAds.GADBannerView.AdUnitId"/>) in any 24-hour period. Application developers should tk tk tk
	///   </para>
	/// </remarks>
	[BaseType (typeof(UIView))]
	interface GADBannerView
	{
		/// <summary>
		/// The application developer's AdMob Publisher ID
		/// </summary>
		//@property (nonatomic, copy) NSString *adUnitID;
		[Export ("adUnitID", ArgumentSemantic.Copy)]
		string AdUnitID { get; set; }
		
		/// <summary>
		/// The <see cref="T:MonoTouch.UIKit.UIViewController"/> to #tk check -> # restore after the ad has been visited.
		/// </summary>
		//@property (nonatomic, assign) UIViewController *rootViewController;
		[Export ("rootViewController", ArgumentSemantic.Assign)]
		UIViewController RootViewController { get; set; }
		
		///<summary>
		/// The <see cref="T:GoogleAdMobAds.GADBannerViewDelegate"/> delegate with which to handle events relating to this <see cref="T:GoogleAdMobAds.GADBannerView"/>.
		/// </summary>
		//@property (nonatomic, assign) NSObject<GADBannerViewDelegate> *delegate;
		[Wrap ("WeakDelegate")] 
		GADBannerViewDelegate Delegate { get; set; }
		
		//@property (nonatomic, assign) NSObject<GADBannerViewDelegate> *delegate;
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }
		
		/// <summary>
		/// #tk check# Requests an ad to be displayed in this <see cref="T:GoogleAdMobAds.GADBannerView"/>.
		/// </summary>
		/// <param name="request">
		/// A <see cref="T:GoogleAdMobAds.GADRequest"/> that can be customized with demographic data or flagged as a test request.
		/// </param>
		/// <remarks>
		///  <para>
		///  The returned ad will be the largest ad that fits in the <see cref="T:GoogleAdMobAds.GADBannerView"/>. Standard sizes for iOS ads are shown in the following table.
		///  </para>
		///  <para>
		///  <list type="table">
		///    <listheader>
		///      <term>Size</term><description>Description</description><description>Devices</description><description>Property</description>
		///    </listheader>
		///    <term>320 x 50</term><description>Standard Banner</description><description>Phones &amp; Tablets</description><description><see cref="P:GoogleAdMobs.GADBannerView.GAD_SIZE_320x50"/></description>
		///    <term>300 x 250</term><description>IAB Medium Banner</description><description>Tablets</description><description><see cref="P:GoogleAdMobs.GADBannerView.GAD_SIZE_300x250"/></description>
		///    <term>468 x 60</term><description>IAB Full-Size Banner</description><description>Tablets</description><description><see cref="P:GoogleAdMobs.GADBannerView.GAD_SIZE_468x60"/></description>
		///    <term>728 x 90</term><description>IAB Leaderboard</description><description>Tablets</description><description><see cref="P:GoogleAdMobs.GADBannerView.GAD_SIZE_728x90"/></description>
		///  </list>
		///  </para>
		///  <para>
		///   #check What about Smart Banners? Can't find any relevant constants in the binding code.
		///  </para>
		///  <para>
		///  If the <see cref="T:GoogleAdMobAds.GADBannerView"/> is smaller than the smallest size (320x50), no ads will be served to it.
		///  </para>
		///  <para>
		///  Banner will auto-refresh every 30-120 seconds, if the application developer's AdMob account on the server specifies auto-refresh.
		///  Otherwise, banners can be programmatically refreshed by
		///  calling <see cref="M:GoogleAdMobAds.GADBannerView.LoadRequest"/>.
		///  </para>
		///  <para>
		///  Ad targeting can be improved by specifying demographic data in the <paramref name="request"/>.
		///  </para>
		///</remarks>
		//- (void)loadRequest:(GADRequest *)request;
		[Export ("loadRequest:")]
		void LoadRequest ([NullAllowed] GADRequest request);
		
		/// <summary>
		/// tk tk tk (I was going to say "True if the ad has auto-refreshed since..." ... but since when?)
		/// </summary>
		//@property (nonatomic, readonly) BOOL hasAutoRefreshed;
		[Export ("hasAutoRefreshed")]
		bool HasAutoRefreshed { get; }
		
		/// <summary>
		/// Initializes the <see cref="T:GoogleAdMobAds.GADBannerView"/> with the specified frame.
		/// </summary>
		/// <param name="frame">
		/// The <see cref="T:System.Drawing.RectangleF"/> specifying the frame for the <see cref="T:GoogleAdMobAds.GADBannerView"/>
		/// </param>
		[Export ("initWithFrame:")]
		IntPtr Constructor (RectangleF frame);
	}
	
	
	/// <summary> 
	/// A delegate object for handling events relating to a <see cref="T:GoogleAdMobAds.GADBannerView"/>.
	/// </summary>
	//@protocol GADBannerViewDelegate <NSObject>
	[BaseType (typeof(NSObject))]
	[Model]
	interface GADBannerViewDelegate
	{
		/// <summary>
		/// Subsequent to <see cref="M:GoogleAdMobAds.GADBannerView.LoadRequest"/> indicates that the <see cref="T:GoogleAdMobAds.GADBannerView"/> received a new ad.
		/// </summary>
		/// <remarks>
		///  This method is an appropriate place for the application developer to add the <paramref name="view"/> to the view hierarchy if it has been hidden.
		/// </remarks>
		/// <param name="view">
		/// The <see cref="T:GoogleAdMobAds.GADBannerView"/> associated with the event.
		/// </param>
		//- (void)adViewDidReceiveAd:(GADBannerView *)view;
		[Export ("adViewDidReceiveAd:")]
		void AdViewDidReceiveAd (GADBannerView view);
		
		/// <summary>
		/// Subsequent to <see cref="M:GoogleAdMobAds.GADBannerView.LoadRequest"/> indicates that the <see cref="T:GoogleAdMobAds.GADBannerView"/> failed to receive a new ad.
		/// </summary>
		/// <param name="error">
		/// A <see cref="T:GoogleAdMobAds.GADRequestError"/> with the specifics of the failure.
		/// </param>
		/// <param name="view">
		/// The <see cref="T:GoogleAdMobAds.GADBannerView"/> associated with the event.
		/// </para>
		//- (void)adView:(GADBannerView *)view didFailToReceiveAdWithError:(GADRequestError *)error;
		[Export ("adView:didFailToReceiveAdWithError:")]
		void AdView (GADBannerView view, GADRequestError error);
		
		/// <summary>
		/// Indicates that the <see cref="T:GoogleAdMobAds.GADBannerView"/> is about to present a full-screen ad in response to application user input.
		/// </summary>
		/// <param name="view">
		/// The <see cref="T:GoogleAdMobAds.GADBannerView"/> associated with the event.
		/// </param>
		/// <remarks>
		/// The application developer should take the same steps with the application that they would take if it were being sent to the background.
		/// This might include pausing animations and timers and saving application state.
		/// </remarks>
		//- (void)adViewWillPresentScreen:(GADBannerView *)adView;
		[Export ("adViewWillPresentScreen:")]
		void AdViewWillPresentScreen (GADBannerView adView);
		
		/// <summary>
		/// Indicates that the application user has asked to dismiss the full-screen ad.
		/// </summary>
		/// <param name="view">
		/// The <see cref="T:GoogleAdMobAds.GADBannerView"/> associated with the event.
		/// </param>
		/// <remarks>
		/// The application developer should prepare for their application to be brought to the foreground, reversing the steps taken 
		/// in <see cref="T:GoogleAdMobAds.GADBannerViewDelegate.AdViewWillPresentScreen"/>.
		/// </remarks>
		//- (void)adViewWillDismissScreen:(GADBannerView *)adView;
		[Export ("adViewWillDismissScreen:")]
		void AdViewWillDismissScreen (GADBannerView adView);
		
		/// <summary>
		/// Indicates that the full-screen ad has been dismissed.
		/// </summary>
		/// <param name="view">
		/// The <see cref="T:GoogleAdMobAds.GADBannerView"/> associated with the event.
		/// </para>
		/// <remarks>
		///  Generally, after a full-screen ad has been dismissed control will return to the application. However, it is also possible for the ad to 
		///  cause the App Store or iTunes to launch, in which case the application will be sent to the background and potentially terminated. If that
		///  is the situation, the application will receive the <see cref="P:MonoTouch.UIApplication.WillResignActiveNotification"/> notification 
		///  (see <see cref="M:MonoTouch.UIApplication+Notifications.ObserveWillResignActive"/>) and the 
		///  <see cref="M:GoogleAdMobAds.GADBannerViewDelegate.adViewWillLeaveApplication"/> method of <c>this</c> will be called.
		/// </remarks>
		//- (void)adViewDidDismissScreen:(GADBannerView *)adView;
		[Export ("adViewDidDismissScreen:")]
		void AdViewDidDismissScreen (GADBannerView adView);
		
		/// <summary>
		/// Indicates that application has been sent to the background due to application user input in the <see cref="T:GoogleAdMobAds.GADBannerView"/>.
		/// </summary>
		/// <param name="adView">
		/// The <see cref="T:GoogleAdMobAds.GADBannerView"/> associated with the event.
		/// </param>
		//- (void)adViewWillLeaveApplication:(GADBannerView *)adView;
		[Export ("adViewWillLeaveApplication:")]
		void adViewWillLeaveApplication (GADBannerView adView);
	}
	
	/// <summary>
	/// Rich ads that provide an in-app browsing experience. Richer and more immersive than <see cref="T:GoogleAdMobs.GADBannerView"/>s.
	/// </summary>
	/// <remarks>
	///  <para>
	///  <see cref="T:GoogleAdMobAds.GADInterstitial"/>s provide immediate rich in-app browsing experiences, in contrast with <see cref="T:MonoTouch.UIKit.GADBannerView"/>s,
	///  which initially present a small ad. <see cref="T:GoogleAdMobAds.GADInterstitial"/>s are intended to be placed at natural transition points within an app, 
	///  such as at launch, before loading an article or level, before launching video, etc. 
	///  </para>
	///  <para>
	///  Because <see cref="T:GoogleAdMobAds.GADInterstitial"/>s have more responsibilities than <see cref="T:GoogleAdMobAds.GADBannerView"/>s, they are not <see cref="T:MonoTouch.UIKit.UIView"/>s
	///  but descend directly from <see cref="T:MonoTouch.Foundation.NSObject"/>. However, for the application developer, instantiating and creating a <see cref="T:GoogleAdMobAds.GADInterstitial"/>
	///  is similar to the process for a <see cref="T:GoogleAdMobAds.GADBannerView"/>. #tk code
	///  </para>
	///  <para>
	///  One important difference between <see cref="T:GoogleAdMobAds.GADInterstitial"/>s and <see cref="T:GoogleAdMobAds.GADBannerView"/>s is that <see cref="T:GoogleAdMobAds.GADInterstitial"/>s
	///  are single-use. Any request to load or display an interstitial after the first request will cause the <see cref="M:GoogleAdMobAds.GADInterstitialDelegate.Interstitial(GADInterstitial, GADRequestError)"/>
	///  method to be called.  
	///  </para>
	///  <para>
	///  All publishers can run interstitial house ads, but paid interstitial ads are only offered to some publishers. Google's documntation states 
	///  "if you become eligible, Google will be sure to contact you."
	///  </para>
	///</remarks>
	//@interface GADInterstitial : NSObject
	[BaseType (typeof(NSObject))]
	interface GADInterstitial
	{
		/// <summary>
		/// The application developer's AdMob Publisher ID.
		/// </summary>
		//@property (nonatomic, copy) NSString *adUnitID;
		[Export ("adUnitID", ArgumentSemantic.Copy)]
		string AdUnitID { get; set; }
		
		/// <summary>
		/// The <see cref="T:GoogleAdMobAds.GADBInterstitialDelegate"/> delegate with which to handle events relating to this <see cref="T:GoogleAdMobAds.GADInterstitial"/>.
		/// </summary>
		//@property (nonatomic, assign) NSObject<GADInterstitialDelegate> *delegate;
		[Wrap ("WeakDelegate")] 
		GADInterstitialDelegate Delegate { get; set; }
		
		//@property (nonatomic, assign) NSObject<GADInterstitialDelegate> *delegate;
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }
		
		/// <summary>
		/// Requests an interstitial ad.
		/// </summary>
		/// <param name="request">
		/// A <see cref="T:GoogleAdMobAds.GADRequest"/> that can be customized with demographic data or flagged as a test request.
		/// </param>
		/// <remarks>
		///  <para>
		///  This method requests an interstitial ad. The default timeout for an interstitial ad is 5 seconds but can be adjusted in the AdMob account.
		///  </para>
		///  <para>
		///  The application developer must not attempt to display the content of an interstitial until 
		///  <see cref="M:GoogleAdMobAds.GADInterstitialDelegate.InterstitialDidReceiveAd"/> is called. If the load is unsuccessful, 
		///  <see cref="M:GoogleAdMobAds.GADInterstitialDelegate.Interstitial(GADInterstitial, GADRequestError)"/> will be called instead.
		///  </para>
		/// </remarks>
		//- (void)loadRequest:(GADRequest *)request;
		[Export ("loadRequest:")]
		void LoadRequest (GADRequest request);
		
		/// <summary>
		/// Used to create a "splash interstitial." The <paramref name="image"/> will display until the load request succeeds or fails.
		/// </summary>
		/// <param name="request">
		/// The <see cref="T:GoogleAdMobAds.GADRequest"/> for an ad.
		/// </param>
		/// <param name="image">
		/// The initial image (such as a splash image) to display prior to the interstitial load completing.
		/// </param>
		/// <param name="window">
		/// The <see cref="T:MonoTouch.UIKit.UIWindow"/> in which the interstitial will appear #tk check
		/// </param>
		/// <remarks>
		///  <para>
		///  A "splash interstitial" appears at app launch. Typically, one calls this method a notification-handler registered with the 
		///  <see cref="M:MonoTouch.UIKit.UIApplication.DidFinishLaunching"/> method. The <paramref name="image"/> will typically be an app's default splash
		///  screen. It remains on-screen until the load request succeeds or fails.
		/// </para>
		/// </remarks>
		//- (void)loadAndDisplayRequest:(GADRequest *)request usingWindow:(UIWindow *)window initialImage:(UIImage *)image;
		[Export ("loadAndDisplayRequest:usingWindow:initialImage:")]
		void LoadAndDisplayRequest (GADRequest request, UIWindow window, UIImage image);
		
		/// <summary>
		/// <see langword="true"/> when the <see cref="T:GoogleAdMobAds.GADInterstitial"/> is ready to display. Used for "splash interstitials."
		/// </summary>
		/// <remarks>
		///  <para>
		///  A "splash interstitial" appears at app launch. In this scenario, the <see cref="M:GADInterstitial.LoadAndDisplayRequest"/> method is called
		///  by the notification-handler registered with the <see cref="M:MonoTouch.UIKit.UIApplication.DidFinishLaunching"/> method.
		///  </para>
		///  <para>
		///  Subsequently, as soon as the interstitial is ready, it will appear #tk check
		///  </para>
		//@property (nonatomic, readonly) BOOL isReady;
		[Export ("isReady")]
		bool IsReady { get; }
		
		/// <summary>
		/// Presents a <see cref="T:GoogleAdMobAds.GADInterstitial"/>.
		/// </summary>
		/// <remarks>
		///  <para>
		///  The application developer must not call this prior to the 
		///  <see cref="M:GoogleAdMobAds.GADInterstitialDelegate.interstitialDidReceiveAd"/> method having been called. Also, the 
		///  <see cref="P:GoogleAdMobAds.GADInterstitial.IsReady"/> property must be <see langword="true"/> #tk check#
		///  </para>
		/// </remarks>
		//- (void)presentFromRootViewController:(UIViewController *)rootViewController;
		[Export ("presentFromRootViewController:")]
		void PresentFromRootViewController (UIViewController rootViewController);
	}
	
	/// <summary>
	/// A delegate object for handling events relating to a <see cref="T:GoogleAdMobAds.GADInterstitial"/>.
	/// </summary>
	//@protocol GADInterstitialDelegate <NSObject>
	[BaseType (typeof(NSObject))]
	[Model]
	interface GADInterstitialDelegate
	{
		/// <summary>
		/// Subsequent to <see cref="M:GoogleAdMobAds.GADInterstitial.LoadRequest"/> indicates that the 
		/// <see cref="T:GoogleAdMobAds.GADInterstitial"/> received a new ad.
		/// </summary>
		/// <param name="ad">
		/// The <see cref="T:GoogleAdMobAds.GADInterstitial"/> associated with this event.
		/// </param>
		//- (void)interstitialDidReceiveAd:(GADInterstitial *)ad;
		[Export ("interstitialDidReceiveAd:")]
		void interstitialDidReceiveAd (GADInterstitial ad);

		/// <summary>
		/// Subsequent to <see cref="M:GoogleAdMobAds.GADInterstitial.LoadRequest"/> indicates that the 
		/// <see cref="T:GoogleAdMobAds.GADInterstitial"/> failed to receive a new ad.
		/// </summary>
		/// <param name="ad">
		/// The <see cref="T:GoogleAdMobAds.GADInterstitial"/> associated with this event.
		/// </param>
		/// <param name="error">
		/// A <see cref="T:GoogleAdMobAds.GADRequestError"/> describing the specific failure.
		/// </param>
		//- (void)interstitial:(GADInterstitial *)ad didFailToReceiveAdWithError:(GADRequestError *)error;
		[Export ("interstitial:didFailToReceiveAdWithError:")]
		void Interstitial (GADInterstitial ad, GADRequestError error);
	
		/// <summary>
		/// Indicates that the <see cref="T:GoogleAdMobAds.GADInterstitial"/> is about to present an interstitial ad.
		/// </summary>
		/// <param name="ad">
		/// The <see cref="T:GoogleAdMobAds.GADInterstitial"/> associated with this event.
		/// </param>
		//- (void)interstitialWillPresentScreen:(GADInterstitial *)ad;
		[Export ("interstitialWillPresentScreen:")]
		void InterstitialWillPresentScreen (GADInterstitial ad);
	
		/// <summary>
		/// Indicates that the application user has asked to dismiss the interstitial ad.
		/// </summary>
		/// <param name="ad">
		/// The <see cref="T:GoogleAdMobAds.GADInterstitial"/> associated with this event.
		/// </param>
		//- (void)interstitialWillDismissScreen:(GADInterstitial *)ad;
		[Export ("interstitialWillDismissScreen:")]
		void InterstitialWillDismissScreen (GADInterstitial ad);
		
		/// <summary>
		/// Indicates that the interstitial ad has been dismissed.
		/// </summary>
		/// <param name="ad">
		/// The <see cref="T:GoogleAdMobAds.GADInterstitial"/> associated with this event.
		/// </param>
		//- (void)interstitialDidDismissScreen:(GADInterstitial *)ad;
		[Export ("interstitialDidDismissScreen:")]
		void InterstitialDidDismissScreen (GADInterstitial ad);
	
		/// <summary>
		/// Indicates that application has been sent to the background due to the presentation of a <see cref="T:GoogleAdMobAds.GADInterstitial"/>.
		/// </summary>
		/// <param name="ad">
		/// The <see cref="T:GoogleAdMobAds.GADInterstitial"/> associated with this event.
		/// </param>
		//- (void)interstitialWillLeaveApplication:(GADInterstitial *)ad;
		[Export ("interstitialWillLeaveApplication:")]
		void InterstitialWillLeaveApplication (GADInterstitial ad);
		
	}
	
	/// <summary>
	/// An ad request that can be customized to facilitate better ad-targeting.
	/// </summary>
	/// <remarks>
	/// Google requests that demographic data used to target ads be restricted to information that is already used in the application.
	/// </remarks>
	//@interface GADRequest : NSObject <NSCopying>
	[BaseType (typeof(NSObject))]
	interface GADRequest
	{
		//+ (GADRequest *)request;
		[Static, Export ("request")]
		GADRequest Request ();
		
		//@property (nonatomic, retain) NSDictionary *additionalParameters;
		[Export ("additionalParameters", ArgumentSemantic.Retain)]
		NSDictionary AdditionalParameters { get; set; }
		
		//+ (NSString *)sdkVersion;
		[Static, Export ("sdkVersion")]
		string SdkVersion ();
		
		/// <summary>
		/// An array of identifiers to receive test ads and thus avoid false impressions.
		/// </summary>
		/// <remarks>
		/// The identifier of a device is the value of its <see cref="P:MonoTouch.UIKit.UIDevice.UniqueIdentifier"/> property.
		/// </remarks>
		///<altmember cref="P:GoogleAdMobs.GADRequest.GAD_SIMULATOR_ID"/>
		//@property (nonatomic, retain) NSArray *testDevices;
		[Export ("testDevices", ArgumentSemantic.Retain)]
		string [] TestDevices { get; set; }
		
		/// <summary>
		/// The gender best targeted by the ad.
		/// </summary>
		//@property (nonatomic, assign) GADGender gender;
		[Export ("gender", ArgumentSemantic.Assign)]
		GADGender Gender { get; set; }

		/// <summary>
		/// The birthday best targeted by the ad.
		/// </summary>
		//@property (nonatomic, retain) NSDate *birthday;
		[Export ("birthday", ArgumentSemantic.Retain)]
		NSDate Birthday { get; set; } 
		
		/// <summary>
		/// Initializes the <see cref="P:GoogleAdMobAds.GADRequest.Birthday"/> property.
		/// </summary>
		//tk tk tk @TODO: SetBirthda and SetLocationy ? Typo source?
		//- (void)setBirthdayWithMonth:(NSInteger)m day:(NSInteger)d year:(NSInteger)y;
		[Export ("setBirthdayWithMonth:day:year:")]
		void SetBirthda (int m, int d, int y);
		
		/// <summary>
		/// The location best targeted by the ad.
		/// </summary>
		/// <remarks>
		/// Cannot be simultaneously used with <see cref="M:GoogleAdMobAds.GADRequest.SetLocationWithDescription"/>.
		/// </remarks>
		//- (void)setLocationWithLatitude:(CGFloat)latitude longitude:(CGFloat)longitude accuracy:(CGFloat)accuracyInMeters;
		[Export ("setLocationWithLatitude:longitude:accuracy:")]
		void SetLocationy (float latitude, float longitude, float accuracyInMeters);
		
		/// <summary>
		/// The location best targeted by the ad.
		/// </summary>
		/// <remarks>
		///  <para>
		///  Cannot be simultaneously used with <see cref="T:GoogleAdMobAds.GADRequest.SetLocationy"/>.
		///  </para>
		///  <para>
		///  Can be used with strings such as <c>SetLocationWithDescription("94104 US");</c>
		///  </para>
		/// </remarks>
		//- (void)setLocationWithDescription:(NSString *)locationDescription;
		[Export ("setLocationWithDescription:")]
		void SetLocationWithDescription (string locationDescription);
		
		/// <summary>
		/// An array of keywords suggesting appropriate ads.
		/// </summary>
		//@property (nonatomic, retain) NSMutableArray *keywords;
		[Export ("keywords", ArgumentSemantic.Retain), NullAllowed]
		string [] keywords { get; set; }
		
		/// <summary>
		/// Appends a keyword to the <see cref="P:GoogleAdMobAds.GADRequest.keywords"/> property.
		/// </summary>
		//- (void)addKeyword:(NSString *)keyword;
		[Export ("addKeyword:")]
		void AddKeyword (string keyword);
		
		/// <summary>
		/// <see langword="true"/> if the request is in a testing context.
		/// </summary>
		//@property (nonatomic, getter=isTesting) BOOL testing;
		[Export ("testing")]
		bool Testing { [Bind ("isTesting")] get; set; }
	}
	
	/// <summary>
	/// Interface indicating an error associated with a <see cref="T:GoogleAdMobAds.GADRequest"/>.
	/// </summary>
	[BaseType (typeof(NSError))]
	interface GADRequestError
	{
		
	}
}
