using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace GoogleAdMobAds
{	
	///<summary>
	///A <scref see="T:MonoTouch.UIKit.UIView"/> that displays small HTML5 ads that respond to user events.
	///<summary>
	[BaseType (typeof(UIView))]
	interface GADBannerView
	{
		///<summary>The application developer's AdMob Publisher ID</summary>
		//@property (nonatomic, copy) NSString *adUnitID;
		[Export ("adUnitID", ArgumentSemantic.Copy)]
		string AdUnitID { get; set; }
		
		///<summary>The <scref see="T:MonoTouch.UIKit.UIViewController"/> to #tk check -> # restore after the ad has been visited</summary>
		//@property (nonatomic, assign) UIViewController *rootViewController;
		[Export ("rootViewController", ArgumentSemantic.Assign)]
		UIViewController RootViewController { get; set; }
		
		///<summary>The <scref see="T:GoogleAdMobAds.GADBannerViewDelegate"/> delegate with which to handle events relating to this <scref see="T:GoogleAdMobAds.GADBannerView"/>.</summary>
		//@property (nonatomic, assign) NSObject<GADBannerViewDelegate> *delegate;
		[Wrap ("WeakDelegate")] 
		GADBannerViewDelegate Delegate { get; set; }
		
		//@property (nonatomic, assign) NSObject<GADBannerViewDelegate> *delegate;
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }
		
		///<summary>#tk check# Requests an ad to be displayed in this <scref see="T:GoogleAdMobAds.GADBannerView"/>.</summary>
		//- (void)loadRequest:(GADRequest *)request;
		[Export ("loadRequest:")]
		void LoadRequest ([NullAllowed] GADRequest request);
		
		///<summary>tk tk tk (I was going to say "True if the ad has auto-refreshed since..." ... but since when?)</summary>
		//@property (nonatomic, readonly) BOOL hasAutoRefreshed;
		[Export ("hasAutoRefreshed")]
		bool HasAutoRefreshed { get; }
		
		///<summary>Initializes the <see cref="T:GoogleAdMobAds.GADBannerView"/> with the specified frame.</summary>
		[Export ("initWithFrame:")]
		IntPtr Constructor (RectangleF frame);
	}
	
	
	///<summary>A delegate object for handling events relating to a <see cref="T:GoogleAdMobAds.GADBannerView"/>.</summary>
	//@protocol GADBannerViewDelegate <NSObject>
	[BaseType (typeof(NSObject))]
	[Model]
	interface GADBannerViewDelegate
	{
		///<summary>Subsequent to <see cref="M:GoogleAdMobAds.GADBannerView.LoadRequest"/> indicates that the <see cref="T:GoogleAdMobAds.GADBannerView"/> received a new ad.</summary>
		//- (void)adViewDidReceiveAd:(GADBannerView *)view;
		[Export ("adViewDidReceiveAd:")]
		void AdViewDidReceiveAd (GADBannerView view);
		
		///<summary>Subsequent to <see cref="M:GoogleAdMobAds.GADBannerView.LoadRequest"/> indicates that the <see cref="T:GoogleAdMobAds.GADBannerView"/> failed to receive a new ad.</summary>
		//- (void)adView:(GADBannerView *)view didFailToReceiveAdWithError:(GADRequestError *)error;
		[Export ("adView:didFailToReceiveAdWithError:")]
		void AdView (GADBannerView view, GADRequestError error);
		
		///<summary>Indicates that the <see cref="T:GoogleAdMobAds.GADBannerView"/> is about to present a full-screen ad in response to application user input.</summary>
		//- (void)adViewWillPresentScreen:(GADBannerView *)adView;
		[Export ("adViewWillPresentScreen:")]
		void AdViewWillPresentScreen (GADBannerView adView);
		
		///<summary>Indicates that the application user has asked to dismiss the full-screen ad.</summary>
		//- (void)adViewWillDismissScreen:(GADBannerView *)adView;
		[Export ("adViewWillDismissScreen:")]
		void AdViewWillDismissScreen (GADBannerView adView);
		
		///<summary>Indicates that the full-screen ad has been dismissed.</summary>
		///<remarks>
		///Generally, after a full-screen ad has been dismissed control will return to the application. However, it is also possible for the ad to 
		///cause the App Store or iTunes to launch, in which case the application will be sent to the background and potentially terminated. If that
		///is the situation, the application will receive the <see cref="P:MonoTouch.UIApplication.WillResignActiveNotification"/> notification 
		///(see <see cref="M:MonoTouch.UIApplication+Notifications.ObserveWillResignActive"/>) and the 
		///<see cref="M:GoogleAdMobAds.GADBannerViewDelegate.adViewWillLeaveApplication"/> method of <c>this</c> will be called.
		///</remarks>
		//- (void)adViewDidDismissScreen:(GADBannerView *)adView;
		[Export ("adViewDidDismissScreen:")]
		void AdViewDidDismissScreen (GADBannerView adView);
		
		///<summary>Indicates that application has been sent to the background due to application user input in the <see cref="T:GoogleAdMobAds.GADBannerView"/>.</summary>
		//- (void)adViewWillLeaveApplication:(GADBannerView *)adView;
		[Export ("adViewWillLeaveApplication:")]
		void adViewWillLeaveApplication (GADBannerView adView);
	}
	
	///<summary>
	///Rich ads that provide an in-app browsing experience. Richer and more immersive than <see cref="T:GoogleAdMobs.GADBannerView"/>s.
	///</summary>
	///<remarks>
	/// <para>
	///<see cref="T:GoogleAdMobAds.GADInterstitial"/>s provide immediate rich in-app browswing experiences, in contrast with <see cref="T:MonoTouch.UIKit.GADBannerView"/>s,
	///which initially present a small ad. <see cref="T:GoogleAdMobAds.GADInterstitial"/>s are intended to be placed at natural transition points within an app, 
	///such as at launch, before loading an article or level, before launching video, etc. 
	///</para>
	///<para>
	///Because <see cref="T:GoogleAdMobAds.GADInterstitial"/>s have more responsibilities than <see cref="T:GoogleAdMobAds.GADBannerView"/>s, they are not <see cref="T:MonoTouch.UIKit.UIView"/>s
	///but descend directly from <see cref="T:MonoTouch.Foundation.NSObject"/>. However, for the application developer, instantiating and creating a <see cref="T:GoogleAdMobAds.GADInterstitial"/>
	///is similar to the process for a <see cref="T:GoogleAdMobAds.GADBannerView"/>. #tk code
	///</para>
	/// <para>
	///One important difference between <see cref="T:GoogleAdMobAds.GADInterstitial"/>s and <see cref="T:GoogleAdMobAds.GADBannerView"/>s is that <see cref="T:GoogleAdMobAds.GADInterstitial"/>s
	///are single-use. Any request to load or display an interstitial after the first request will cause the <see cref="M:GoogleAdMobAds.GADInterstitialDelegate.Interstitial(GADInterstitial, GADRequestError)"/>
	///method to be called.  
	///</para>
	///</remarks>
	//@interface GADInterstitial : NSObject
	[BaseType (typeof(NSObject))]
	interface GADInterstitial
	{
		///<summary>The application developer's AdMob Publisher ID</summary>
		//@property (nonatomic, copy) NSString *adUnitID;
		[Export ("adUnitID", ArgumentSemantic.Copy)]
		string AdUnitID { get; set; }
		
		///<summary>The <scref see="T:GoogleAdMobAds.GADBInterstitialDelegate"/> delegate with which to handle events relating to this <scref see="T:GoogleAdMobAds.GADInterstitial"/>.</summary>
		//@property (nonatomic, assign) NSObject<GADInterstitialDelegate> *delegate;
		[Wrap ("WeakDelegate")] 
		GADInterstitialDelegate Delegate { get; set; }
		
		//@property (nonatomic, assign) NSObject<GADInterstitialDelegate> *delegate;
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }
		
		//- (void)loadRequest:(GADRequest *)request;
		[Export ("loadRequest:")]
		void LoadRequest (GADRequest request);
		
		//- (void)loadAndDisplayRequest:(GADRequest *)request usingWindow:(UIWindow *)window initialImage:(UIImage *)image;
		[Export ("loadAndDisplayRequest:usingWindow:initialImage:")]
		void LoadAndDisplayRequest (GADRequest request, UIWindow window, UIImage image);
		
		///<summary><see langword="true"/> when the <see cref="T:GoogleAdMobAds.GADInterstitial"/> is ready to display. Used for "splash interstitials."</summary>
		///<para>A "splash interstitial" appears at app launch. In this scenario, the <see cref="M:GADInterstitial.LoadAndDisplayRequest"/> method is called
		///by the notification-handler registered with the <see cref="M:MonoTouch.UIKit.UIApplication.DidFinishLaunching"/> method.</para>
		///<para>Subsequently, as soon as the interstitial is ready, it will appear #tk check</para>
		//@property (nonatomic, readonly) BOOL isReady;
		[Export ("isReady")]
		bool IsReady { get; }
		
		///<summary>Presents an <see cref="P:GoogleAdMobAds.GADInterstitial.IsReady"/> <see cref="T:GoogleAdMobAds.GADInterstitial"/>.</summary>
		//- (void)presentFromRootViewController:(UIViewController *)rootViewController;
		[Export ("presentFromRootViewController:")]
		void PresentFromRootViewController (UIViewController rootViewController);
	}
	
	///<summary>A delegate object for handling events relating to a <see cref="T:GoogleAdMobAds.GADInterstitial"/>.</summary>
	//@protocol GADInterstitialDelegate <NSObject>
	[BaseType (typeof(NSObject))]
	[Model]
	interface GADInterstitialDelegate
	{
		///<summary>Subsequent to <see cref="M:GoogleAdMobAds.GADInterstitial.LoadRequest"/> indicates that the <see cref="T:GoogleAdMobAds.GADInterstitial"/> received a new ad.</summary>
		//- (void)interstitialDidReceiveAd:(GADInterstitial *)ad;
		[Export ("interstitialDidReceiveAd:")]
		void interstitialDidReceiveAd (GADInterstitial ad);

		///<summary>Subsequent to <see cref="M:GoogleAdMobAds.GADInterstitial.LoadRequest"/> indicates that the <see cref="T:GoogleAdMobAds.GADInterstitial"/> failed to receive a new ad.</summary>
		//- (void)interstitial:(GADInterstitial *)ad didFailToReceiveAdWithError:(GADRequestError *)error;
		[Export ("interstitial:didFailToReceiveAdWithError:")]
		void Interstitial (GADInterstitial ad, GADRequestError error);
	
		///<summary>Indicates that the <see cref="T:GoogleAdMobAds.GADInterstitial"/> is about to present a full-screen ad.</summary>
		//- (void)interstitialWillPresentScreen:(GADInterstitial *)ad;
		[Export ("interstitialWillPresentScreen:")]
		void InterstitialWillPresentScreen (GADInterstitial ad);
	
		///<summary>Indicates that the application user has asked to dismiss the full-screen ad.</summary>
		//- (void)interstitialWillDismissScreen:(GADInterstitial *)ad;
		[Export ("interstitialWillDismissScreen:")]
		void InterstitialWillDismissScreen (GADInterstitial ad);
		
		///<summary>Indicates that the full-screen ad has been dismissed.</summary>
		//- (void)interstitialDidDismissScreen:(GADInterstitial *)ad;
		[Export ("interstitialDidDismissScreen:")]
		void InterstitialDidDismissScreen (GADInterstitial ad);
	
		///<summary>Indicates that application has been sent to the background due to application user input in the <see cref="T:GoogleAdMobAds.GADInterstitial"/>.</summary>
		//- (void)interstitialWillLeaveApplication:(GADInterstitial *)ad;
		[Export ("interstitialWillLeaveApplication:")]
		void InterstitialWillLeaveApplication (GADInterstitial ad);
		
	}
	
	/// <summary>
	/// An ad request that can be customized to facilitate better ad-targeting.
	/// </summary>
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
		
		///<summary>An array of identifiers to receive test ads and thus avoid false impressions.</summary>
		//@property (nonatomic, retain) NSArray *testDevices;
		[Export ("testDevices", ArgumentSemantic.Retain)]
		string [] TestDevices { get; set; }
		
		///<summary>The gender best targeted by the ad.</summary>
		//@property (nonatomic, assign) GADGender gender;
		[Export ("gender", ArgumentSemantic.Assign)]
		GADGender Gender { get; set; }

		///<summary>The birthday best targeted by the ad.</summary>
		//@property (nonatomic, retain) NSDate *birthday;
		[Export ("birthday", ArgumentSemantic.Retain)]
		NSDate Birthday { get; set; } 
		
		///<summary>Initializes the <see cref="P:GoogleAdMobAds.GADRequest.Birthday"/> property.</summary>
		//@TODO: SetBirthda and SetLocationy ? Typo source?
		//- (void)setBirthdayWithMonth:(NSInteger)m day:(NSInteger)d year:(NSInteger)y;
		[Export ("setBirthdayWithMonth:day:year:")]
		void SetBirthda (int m, int d, int y);
		
		///<summary>The location best targeted by the ad.</summary>
		///<remarks>Cannot be simultaneously used with <see cref="M:GoogleAdMobAds.GADRequest.SetLocationWithDescription"/>.</remarks>
		//- (void)setLocationWithLatitude:(CGFloat)latitude longitude:(CGFloat)longitude accuracy:(CGFloat)accuracyInMeters;
		[Export ("setLocationWithLatitude:longitude:accuracy:")]
		void SetLocationy (float latitude, float longitude, float accuracyInMeters);
		
		///<summary>The location best targeted by the ad.</summary>
		///<remarks><para>Cannot be simultaneously used with <see cref="T:GoogleAdMobAds.GADRequest.SetLocationy"/>.</para>
		///<para>Can be used with strings such as <c>SetLocationWithDescription("94104 US");</c></para></remarks>
		//- (void)setLocationWithDescription:(NSString *)locationDescription;
		[Export ("setLocationWithDescription:")]
		void SetLocationWithDescription (string locationDescription);
		
		///<summary>An array of keywords suggesting appropriate ads.</summary>
		//@property (nonatomic, retain) NSMutableArray *keywords;
		[Export ("keywords", ArgumentSemantic.Retain), NullAllowed]
		string [] keywords { get; set; }
		
		///<summary>Appends a keyword to the <see cref="P:GoogleAdMobAds.GADRequest.keywords"/> property.</summary>
		//- (void)addKeyword:(NSString *)keyword;
		[Export ("addKeyword:")]
		void AddKeyword (string keyword);
		
		///<summary><see langword="true"/> if the request is in a testing context.</summary>
		//@property (nonatomic, getter=isTesting) BOOL testing;
		[Export ("testing")]
		bool Testing { [Bind ("isTesting")] get; set; }
	}
	
	///<summary>Interface indicating an error associated with a <see cref="T:GoogleAdMobAds.GADRequest"/>.</summary>
	[BaseType (typeof(NSError))]
	interface GADRequestError
	{
		
	}
}
