//==============================================================================
//
//  PickerSamplePhoneViewController.h
//  PickerSamplePhone
//
//  Created by Troy Gaul on 8/12/10.
//
//  Copyright (c) 2011-2013 InfinitApps LLC: http://infinitapps.com
//	Some rights reserved: http://opensource.org/licenses/MIT
//
//==============================================================================

#import <UIKit/UIKit.h>

#import "InfColorPickerController.h"

//------------------------------------------------------------------------------

@interface PickerSamplePhoneViewController : UIViewController <InfColorPickerControllerDelegate>

- (IBAction) changeBackgroundColor;

@end

//------------------------------------------------------------------------------
