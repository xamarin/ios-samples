//==============================================================================
//
//  InfColorPickerNavigationController.m
//  InfColorPicker
//
//  Created by Troy Gaul on 11 Dec 2013.
//
//  Copyright (c) 2011-2013 InfinitApps LLC: http://infinitapps.com
//	Some rights reserved: http://opensource.org/licenses/MIT
//
//==============================================================================

#import "InfColorPickerNavigationController.h"

//------------------------------------------------------------------------------

#if !__has_feature(objc_arc)
#error This file must be compiled with ARC enabled (-fobjc-arc).
#endif

//==============================================================================

@implementation InfColorPickerNavigationController

//------------------------------------------------------------------------------

- (BOOL) shouldAutorotate
{
	return [self.topViewController shouldAutorotate];
}

//------------------------------------------------------------------------------

- (BOOL) shouldAutorotateToInterfaceOrientation: (UIInterfaceOrientation) interfaceOrientation
{
	return [self.topViewController shouldAutorotateToInterfaceOrientation: interfaceOrientation];
}

//------------------------------------------------------------------------------

- (NSUInteger) supportedInterfaceOrientations
{
	return self.topViewController.supportedInterfaceOrientations;
}

//------------------------------------------------------------------------------

@end

//==============================================================================
