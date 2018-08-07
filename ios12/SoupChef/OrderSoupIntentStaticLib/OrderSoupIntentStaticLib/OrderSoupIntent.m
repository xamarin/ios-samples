//
// OrderSoupIntent.m
//
// This file was automatically generated and should not be edited.
//

#import "OrderSoupIntent.h"

@implementation OrderSoupIntent

@dynamic soup, quantity, options;

@end

@interface OrderSoupIntentResponse ()

@property (readwrite, NS_NONATOMIC_IOSONLY) OrderSoupIntentResponseCode code;

@end

@implementation OrderSoupIntentResponse

@synthesize code = _code;

@dynamic soup, waitTime;

- (instancetype)initWithCode:(OrderSoupIntentResponseCode)code userActivity:(nullable NSUserActivity *)userActivity {
    self = [super init];
    if (self) {
        _code = code;
        self.userActivity = userActivity;
    }
    return self;
}

+ (instancetype)successIntentResponseWithSoup:(INObject *)soup waitTime:(NSNumber *)waitTime {
    OrderSoupIntentResponse *intentResponse = [[OrderSoupIntentResponse alloc] initWithCode:OrderSoupIntentResponseCodeSuccess userActivity:nil];
    intentResponse.soup = soup;
    intentResponse.waitTime = waitTime;
    return intentResponse;
}

+ (instancetype)failureSoupUnavailableIntentResponseWithSoup:(INObject *)soup {
    OrderSoupIntentResponse *intentResponse = [[OrderSoupIntentResponse alloc] initWithCode:OrderSoupIntentResponseCodeFailureSoupUnavailable userActivity:nil];
    intentResponse.soup = soup;
    return intentResponse;
}

@end
