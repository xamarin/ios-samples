//==============================================================================
//
//  PickerSamplePadViewController.m
//  PickerSamplePad
//
//  Created by Troy Gaul on 8/17/10.
//
//  Copyright (c) 2011-2013 InfinitApps LLC: http://infinitapps.com
//	Some rights reserved: http://opensource.org/licenses/MIT
//
//==============================================================================

#import "PickerSamplePadViewController.h"

#import "PickerSampleTableViewController.h"

//==============================================================================

@implementation PickerSamplePadViewController {
	UIPopoverController* activePopover;
	BOOL updateLive;
}

//------------------------------------------------------------------------------

- (BOOL) shouldAutorotateToInterfaceOrientation: (UIInterfaceOrientation) interfaceOrientation
{
	return YES;
}

//------------------------------------------------------------------------------

- (void) applyPickedColor: (InfColorPickerController*) picker
{
	self.view.backgroundColor = picker.resultColor;
}

//------------------------------------------------------------------------------
#pragma mark	UIPopoverControllerDelegate methods
//------------------------------------------------------------------------------

- (void) popoverControllerDidDismissPopover: (UIPopoverController*) popoverController
{
	if ([popoverController.contentViewController isKindOfClass: [InfColorPickerController class]]) {
		InfColorPickerController* picker = (InfColorPickerController*) popoverController.contentViewController;
		[self applyPickedColor: picker];
	}
	
	if (popoverController == activePopover) {
		activePopover = nil;
	}
}

//------------------------------------------------------------------------------

- (void) showPopover: (UIPopoverController*) popover from: (id) sender
{
	popover.delegate = self;
	
	activePopover = popover;
	
	if ([sender isKindOfClass: [UIBarButtonItem class]]) {
		[activePopover presentPopoverFromBarButtonItem: sender
		                      permittedArrowDirections: UIPopoverArrowDirectionAny
		                                      animated: YES];
	} else {
		UIView* senderView = sender;
		
		[activePopover presentPopoverFromRect: [senderView bounds]
		                               inView: senderView
		             permittedArrowDirections: UIPopoverArrowDirectionAny
		                             animated: YES];
	}
}

//------------------------------------------------------------------------------

- (BOOL) dismissActivePopover
{
	if (activePopover) {
		[activePopover dismissPopoverAnimated: YES];
		[self popoverControllerDidDismissPopover: activePopover];
		
		return YES;
	}
	
	return NO;
}

//------------------------------------------------------------------------------
#pragma mark	InfHSBColorPickerControllerDelegate methods
//------------------------------------------------------------------------------

- (void) colorPickerControllerDidChangeColor: (InfColorPickerController*) picker
{
	if (updateLive)
		[self applyPickedColor: picker];
}

//------------------------------------------------------------------------------

- (void) colorPickerControllerDidFinish: (InfColorPickerController*) picker
{
	[self applyPickedColor: picker];
	
	[activePopover dismissPopoverAnimated: YES];
}

//------------------------------------------------------------------------------
#pragma mark	IB actions
//------------------------------------------------------------------------------

- (IBAction) takeUpdateLive: (UISwitch*) sender
{
	updateLive = [sender isOn];
}

//------------------------------------------------------------------------------

- (IBAction) finishColorTable
{
	[self dismissActivePopover];
}

- (IBAction) showColorTable: (id) sender
{
	if ([self dismissActivePopover])
		return;
	
	PickerSampleTableViewController* vc = [[PickerSampleTableViewController alloc] init];
	UINavigationController* nav = [[UINavigationController alloc] initWithRootViewController: vc];
	
	nav.navigationBar.barStyle = UIBarStyleBlackOpaque;
	
	vc.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithBarButtonSystemItem: UIBarButtonSystemItemDone
																						 target: self
																						 action: @selector(finishColorTable)];
	
	UIPopoverController* popover = [[UIPopoverController alloc] initWithContentViewController: nav];
	
	[self showPopover: popover from: sender];
}

//------------------------------------------------------------------------------

- (IBAction) changeColor: (id) sender
{
	if ([self dismissActivePopover]) return;
	
	InfColorPickerController* picker = [InfColorPickerController colorPickerViewController];
	
	picker.sourceColor = self.view.backgroundColor;
	picker.delegate = self;
	
	UIPopoverController* popover = [[UIPopoverController alloc] initWithContentViewController: picker];
	
	[self showPopover: popover from: sender];
}

//------------------------------------------------------------------------------

@end

//==============================================================================
