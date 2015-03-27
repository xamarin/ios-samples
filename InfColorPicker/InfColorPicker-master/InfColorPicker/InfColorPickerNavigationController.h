//==============================================================================
//
//  InfColorPickerNavigationController.h
//  InfColorPicker
//
//  Created by Troy Gaul on 11 Dec 2013.
//
//  Copyright (c) 2013 InfinitApps LLC: http://infinitapps.com
//	Some rights reserved: http://opensource.org/licenses/MIT
//
//==============================================================================

#import <UIKit/UIKit.h>

//------------------------------------------------------------------------------
// This navigation controller subclass forwards orientation requests to
// the top view controller hosted within it so that it can choose to limit
// the orientations it is displayed in.

@interface InfColorPickerNavigationController : UINavigationController

@end

//------------------------------------------------------------------------------
