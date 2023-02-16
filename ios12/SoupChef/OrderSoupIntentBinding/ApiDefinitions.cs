using System;
using Foundation;
using Intents;
using ObjCRuntime;

namespace SoupChef {
	// @interface OrderSoupIntent : INIntent
	// [Watch (5,0), iOS (12,0)]
	[BaseType (typeof (INIntent))]
	interface OrderSoupIntent {
		// @property (readwrite, copy, nonatomic) INObject * _Nullable soup;
		[NullAllowed, Export ("soup", ArgumentSemantic.Copy)]
		INObject Soup { get; set; }

		// @property (readwrite, copy, nonatomic) NSNumber * _Nullable quantity;
		[NullAllowed, Export ("quantity", ArgumentSemantic.Copy)]
		NSNumber Quantity { get; set; }

		// @property (readwrite, copy, nonatomic) NSArray<INObject *> * _Nullable options;
		[NullAllowed, Export ("options", ArgumentSemantic.Copy)]
		INObject [] Options { get; set; }
	}

	// @protocol OrderSoupIntentHandling <NSObject>
	// [Watch (5,0), iOS (12,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface OrderSoupIntentHandling {
		// @required -(void)handleOrderSoup:(OrderSoupIntent * _Nonnull)intent completion:(void (^ _Nonnull)(OrderSoupIntentResponse * _Nonnull))completion;
		[Abstract]
		[Export ("handleOrderSoup:completion:")]
		void HandleOrderSoup (OrderSoupIntent intent, Action<OrderSoupIntentResponse> completion);

		// @optional -(void)confirmOrderSoup:(OrderSoupIntent * _Nonnull)intent completion:(void (^ _Nonnull)(OrderSoupIntentResponse * _Nonnull))completion;
		[Export ("confirmOrderSoup:completion:")]
		void ConfirmOrderSoup (OrderSoupIntent intent, Action<OrderSoupIntentResponse> completion);
	}

	// @interface OrderSoupIntentResponse : INIntentResponse
	// [Watch (5,0), iOS (12,0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface OrderSoupIntentResponse {
		// -(instancetype _Nonnull)initWithCode:(OrderSoupIntentResponseCode)code userActivity:(NSUserActivity * _Nullable)userActivity __attribute__((objc_designated_initializer));
		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (OrderSoupIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		// +(instancetype _Nonnull)successIntentResponseWithSoup:(INObject * _Nonnull)soup waitTime:(NSNumber * _Nonnull)waitTime;
		[Static]
		[Export ("successIntentResponseWithSoup:waitTime:")]
		OrderSoupIntentResponse SuccessIntentResponseWithSoup (INObject soup, NSNumber waitTime);

		// +(instancetype _Nonnull)failureSoupUnavailableIntentResponseWithSoup:(INObject * _Nonnull)soup;
		[Static]
		[Export ("failureSoupUnavailableIntentResponseWithSoup:")]
		OrderSoupIntentResponse FailureSoupUnavailableIntentResponseWithSoup (INObject soup);

		// @property (readwrite, copy, nonatomic) INObject * _Nullable soup;
		[NullAllowed, Export ("soup", ArgumentSemantic.Copy)]
		INObject Soup { get; set; }

		// @property (readwrite, copy, nonatomic) NSNumber * _Nullable waitTime;
		[NullAllowed, Export ("waitTime", ArgumentSemantic.Copy)]
		NSNumber WaitTime { get; set; }

		// @property (readonly, nonatomic) OrderSoupIntentResponseCode code;
		[Export ("code")]
		OrderSoupIntentResponseCode Code { get; }
	}
}
