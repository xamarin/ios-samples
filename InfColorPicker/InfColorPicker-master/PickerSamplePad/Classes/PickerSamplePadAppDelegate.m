//==============================================================================
//
//  PickerSamplePadAppDelegate.m
//  PickerSamplePad
//
//  Created by Troy Gaul on 8/17/10.
//
//  Copyright (c) 2011-2013 InfinitApps LLC: http://infinitapps.com
//	Some rights reserved: http://opensource.org/licenses/MIT
//
//==============================================================================

#import "PickerSamplePadAppDelegate.h"
#import "PickerSamplePadViewController.h"

//------------------------------------------------------------------------------

@implementation PickerSamplePadAppDelegate

//------------------------------------------------------------------------------

@synthesize window;
@synthesize viewController;

//------------------------------------------------------------------------------

- (BOOL) application: (UIApplication*) application didFinishLaunchingWithOptions: (NSDictionary*) launchOptions
{
	[window setRootViewController: viewController];
	[window makeKeyAndVisible];
	
	return YES;
}

//------------------------------------------------------------------------------

@end

//------------------------------------------------------------------------------
