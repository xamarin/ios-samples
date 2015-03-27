//==============================================================================
//
//  PickerSamplePhoneViewController.m
//  PickerSamplePhone
//
//  Created by Troy Gaul on 8/12/10.
//
//  Copyright (c) 2011-2013 InfinitApps LLC: http://infinitapps.com
//	Some rights reserved: http://opensource.org/licenses/MIT
//
//==============================================================================

#import "PickerSamplePhoneViewController.h"

#import "InfColorPicker.h"

//------------------------------------------------------------------------------

@implementation PickerSamplePhoneViewController

//------------------------------------------------------------------------------

- (IBAction) changeBackgroundColor
{
	InfColorPickerController* picker = [InfColorPickerController colorPickerViewController];
	
	picker.sourceColor = self.view.backgroundColor;
	picker.delegate = self;
	
	[picker presentModallyOverViewController: self];
}

//------------------------------------------------------------------------------

- (void) colorPickerControllerDidFinish: (InfColorPickerController*) picker
{
	self.view.backgroundColor = picker.resultColor;
	
	[self dismissModalViewControllerAnimated: YES];
}

//------------------------------------------------------------------------------

@end

//------------------------------------------------------------------------------
