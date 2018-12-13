//
//  XMCustomView.m
//  XMBindingLibrary
//
//  Created by Anuj Bhatia on 1/18/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import "XMCustomView.h"

@implementation XMCustomView {
    UILabel *txtView;
}

-(id) init {
    if (self = [super init]) {
        // create text box
        txtView = [[UILabel alloc] init];
        txtView.textAlignment = NSTextAlignmentCenter;
        txtView.textColor = [UIColor blueColor];
        txtView.lineBreakMode = NSLineBreakByWordWrapping;
        txtView.adjustsFontSizeToFitWidth = NO;
        txtView.numberOfLines = 0;
        
        [self addSubview:txtView];

        self.backgroundColor = [UIColor whiteColor];

        // set inner shadow
        self.layer.cornerRadius = 8;
        self.layer.shadowOffset = CGSizeMake(10, 10);
        self.layer.shadowRadius = 5;
        self.layer.shadowOpacity = 0.5;
        self.layer.shadowColor = [[UIColor blackColor] CGColor];
    }
    
    return self;
}

-(void) touchesBegan:(NSSet *)touches withEvent:(UIEvent *)event {
    [self.delegate viewWasTouched:self];
}

-(void) customizeViewWithText:(NSString *)message {
    if ([message length] > 0) {
        txtView.text = message;
    } else {
        txtView.text = @"";
    }
}

-(void) doTouch:(id<XMCustomViewDelegate>)delegate {
    [delegate viewWasTouched:self];
}

- (void)layoutSubviews {
    [super layoutSubviews];
    
    txtView.frame = self.bounds;
}

@end
