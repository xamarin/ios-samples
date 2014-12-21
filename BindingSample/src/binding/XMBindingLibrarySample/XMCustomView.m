//
//  XMCustomView.m
//  XMBindingLibrarySample
//
//  Created by Anuj Bhatia on 1/18/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import "XMCustomView.h"

@interface XMCustomView ()
@property (nonatomic, assign) BOOL isCustomized;
@end


@implementation XMCustomView

@synthesize name = _name, delegate = _delegate, isCustomized = _isCustomized;


-(id) init
{
    if(self = [super init]) {
        // do initialization hurr
        self.isCustomized = false;
    }
    
    return self;
}

-(void) touchesBegan:(NSSet *)touches withEvent:(UIEvent *)event
{
    [self.delegate viewWasTouched:self];
}

-(void) customizeViewWithText:(NSString *)message
{
    if(self.isCustomized == false && [message length] > 0) {
        
        UITextView *txtView = [[UITextView alloc] init];
        txtView.textAlignment = NSTextAlignmentCenter;
        txtView.textColor = [UIColor blueColor];
        txtView.frame = CGRectMake(self.frame.origin.x, ((self.frame.origin.y / 2) - 25), self.frame.size.width, 100);
//        txtView.lineBreakMode = UILineBreakModeWordWrap;

        // set inner shadow
        txtView.layer.masksToBounds = NO;
        txtView.layer.cornerRadius = 8;
        txtView.layer.shadowOffset = CGSizeMake(-15, 20);
        txtView.layer.shadowRadius = 5;
        txtView.layer.shadowOpacity = 0.5;
        txtView.layer.shadowColor = [[UIColor blackColor] CGColor];
        
        txtView.text = message;
//        [txtView sizeToFit];

        [self addSubview:txtView];
    }
}

@end
