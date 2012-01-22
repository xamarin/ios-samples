//
//  XMUtilities.h
//  XMBindingLibrarySample
//
//  Created by Anuj Bhatia on 1/18/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface XMUtilities : NSObject {
    
}

-(id) init;
+(NSString *) echo:(NSString *)message;
-(NSString *) hello:(NSString *)name;
-(NSInteger) add:(NSInteger)operandUn and:(NSInteger) operandDeux;
-(NSInteger) multiply:(NSInteger)operandUn and:(NSInteger)operandDeux;

@end
