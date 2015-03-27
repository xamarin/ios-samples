//==============================================================================
//
//  PickerSamplePhoneAppDelegate.m
//  PickerSamplePhone
//
//  Created by Troy Gaul on 8/12/10.
//
//  Copyright (c) 2011-2013 InfinitApps LLC: http://infinitapps.com
//	Some rights reserved: http://opensource.org/licenses/MIT
//
//==============================================================================

#import "PickerSamplePhoneAppDelegate.h"

#import "PickerSamplePhoneViewController.h"

//==============================================================================

@implementation PickerSamplePhoneAppDelegate

//------------------------------------------------------------------------------

@synthesize window;
@synthesize viewController;

//------------------------------------------------------------------------------

- (BOOL)              application: (UIApplication*) application
    didFinishLaunchingWithOptions: (NSDictionary*) launchOptions
{
	[window setRootViewController: viewController];
	[window makeKeyAndVisible];
	
	return YES;
}

//------------------------------------------------------------------------------


//------------------------------------------------------------------------------

@end

//==============================================================================
