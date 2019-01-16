//
//  XMUtilities.m
//  XMBindingLibrary
//
//  Created by Anuj Bhatia on 1/18/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import "XMUtilities.h"

@implementation XMUtilities {
    XMUtilityCallback _callback;
}

-(id) init {
    if (self = [super init]) {
        // do initialization here after super init nil check!
    }
    
    return self;
}

-(void) dealloc {
    // this is an ARC project so we don't have to dealloc
    // we dont even have to call [super dealloc];
    // old habits die hard, Yippee-ki-yay!
}

// This is an example of a class method. It will echo the message you give it.
// Obj-C class methods are like C# static methods, but different.

+(NSString *) echo:(NSString *)message {
    if ([message length] == 0) {
        return @"Yo, you didn't give me a message!";
    }
    
    return [NSString stringWithFormat:@"%@", message];
}

// This is an example of an instance method.

-(NSString *) speak {
    return @"*Speaks* This is the Xamarin binding sample.";
}

-(NSString *) speak:(XMGreeting)greeting {
    switch (greeting) {
        case XMGreetingHello:
            return @"*Speaks* This is a big HELLO!";
        case XMGreetingGoodbye:
            return @"*Speaks* This is a big GOODBYE!";
        default:
            return @"Yo, you didn't give me valid greeting!";
    }
}

// This is an example of an instance method with a parameter.

-(NSString *) hello:(NSString *)name {
    if ([name length] == 0) {
        return @"Yo, you didn't give me a name!";
    }
    
    return [NSString stringWithFormat:@"*Waves* Hello %@! Welcome to the Xamarin binding sample!", name];
}

-(NSInteger) add:(NSInteger)operandUn and:(NSInteger) operandDeux {
    return operandUn + operandDeux;
}

-(NSInteger) multiply:(NSInteger)operandUn and:(NSInteger)operandDeux {
    return operandUn * operandDeux;
}

// This is an example of how to set a block function for later use.
-(void) setCallback:(XMUtilityCallback) callback {
	_callback = [callback copy];
}

// This is an example of how to invoke a block function.
-(void) invokeCallback:(NSString *) message {
    _callback (message);
}

@end
