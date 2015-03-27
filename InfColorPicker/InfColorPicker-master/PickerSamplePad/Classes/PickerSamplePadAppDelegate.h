//==============================================================================
//
//  PickerSamplePadAppDelegate.h
//  PickerSamplePad
//
//  Created by Troy Gaul on 8/17/10.
//
//  Copyright (c) 2011-2013 InfinitApps LLC: http://infinitapps.com
//	Some rights reserved: http://opensource.org/licenses/MIT
//
//==============================================================================

#import <UIKit/UIKit.h>

@class PickerSamplePadViewController;

//------------------------------------------------------------------------------

@interface PickerSamplePadAppDelegate : NSObject< UIApplicationDelegate >

@property (nonatomic) IBOutlet UIWindow* window;
@property (nonatomic) IBOutlet PickerSamplePadViewController* viewController;

@end

//------------------------------------------------------------------------------
