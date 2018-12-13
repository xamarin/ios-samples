//
//  XMUtilities.h
//  XMBindingLibrary
//
//  Created by Anuj Bhatia on 1/18/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>

// This is how to define a block function prototype.
typedef void (^XMUtilityCallback) (NSString *message);

typedef NS_ENUM(NSInteger, XMGreeting) {
    XMGreetingHello,
    XMGreetingGoodbye,
};

@interface XMUtilities : NSObject {
}

-(id) init;
+(NSString *) echo:(NSString *)message;
-(NSString *) speak;
-(NSString *) speak:(XMGreeting)greeting;
-(NSString *) hello:(NSString *)name;
-(NSInteger) add:(NSInteger)operandUn and:(NSInteger) operandDeux;
-(NSInteger) multiply:(NSInteger)operandUn and:(NSInteger)operandDeux;
-(void) setCallback:(XMUtilityCallback) callback;
-(void) invokeCallback:(NSString *) message;

@end
