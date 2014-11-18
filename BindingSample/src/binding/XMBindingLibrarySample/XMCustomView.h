//
//  XMCustomView.h
//  XMBindingLibrarySample
//
//  Created by Anuj Bhatia on 1/18/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import "XMCustomViewDelegate.h"

@interface XMCustomView : UIView
{
    
}

@property (nonatomic, strong) NSString* name;
@property (nonatomic, assign) id <XMCustomViewDelegate> delegate;


-(void) customizeViewWithText:(NSString *)message;



@end
