//==============================================================================
//
//  PickerSamplePadViewController.h
//  PickerSamplePad
//
//  Created by Troy Gaul on 8/17/10.
//
//  Copyright (c) 2011-2013 InfinitApps LLC: http://infinitapps.com
//	Some rights reserved: http://opensource.org/licenses/MIT
//
//==============================================================================

#import <UIKit/UIKit.h>

#import "InfColorPicker.h"

//------------------------------------------------------------------------------

@interface PickerSamplePadViewController : UIViewController <InfColorPickerControllerDelegate,
															 UIPopoverControllerDelegate,
															 UITableViewDelegate>

- (IBAction) takeUpdateLive: (id) sender;
- (IBAction) changeColor: (id) sender;
- (IBAction) showColorTable: (id) sender;

@end

//------------------------------------------------------------------------------
