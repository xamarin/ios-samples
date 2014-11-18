//
//  XMCustomViewDelegate.h
//  XMBindingLibrarySample
//
//  Created by Anuj Bhatia on 1/18/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import "XMCustomView.h"

@protocol XMCustomViewDelegate<NSObject>

@required
-(void)viewWasTouched:(UIView *)view;


@end
