//==============================================================================
//
//  PickerSampleTableViewController.m
//  PickerSamplePad
//
//  Created by Troy Gaul on 9/7/11.
//
//  Copyright (c) 2011-2013 InfinitApps LLC: http://infinitapps.com
//	Some rights reserved: http://opensource.org/licenses/MIT
//
//==============================================================================

#import "PickerSampleTableViewController.h"

//==============================================================================

@implementation PickerSampleTableViewController {
	NSMutableArray* colors;
	int pickingColorIndex;
}

//------------------------------------------------------------------------------
#pragma mark - View lifecycle
//------------------------------------------------------------------------------

- (void) viewDidLoad
{
	[super viewDidLoad];
	
	if (colors == nil) {
		colors = [NSMutableArray array];
		
		[colors addObject: [UIColor blackColor]];
		[colors addObject: [UIColor redColor]];
		[colors addObject: [UIColor greenColor]];
	}
	
	self.contentSizeForViewInPopover = [InfColorPickerController idealSizeForViewInPopover];
}

//------------------------------------------------------------------------------

- (BOOL) shouldAutorotateToInterfaceOrientation: (UIInterfaceOrientation) interfaceOrientation
{
	return YES;
}

//------------------------------------------------------------------------------
#pragma mark - Table view data source
//------------------------------------------------------------------------------

- (NSInteger) tableView: (UITableView*) tableView numberOfRowsInSection: (NSInteger) section
{
	return colors.count;
}

//------------------------------------------------------------------------------

- (UITableViewCell*) tableView: (UITableView*) tableView cellForRowAtIndexPath: (NSIndexPath*) indexPath
{
	static NSString* CellIdentifier = @"Cell";
	
	UITableViewCell* cell = [tableView dequeueReusableCellWithIdentifier: CellIdentifier];
	
	if (cell == nil) {
		cell = [[UITableViewCell alloc] initWithStyle: UITableViewCellStyleDefault
		                              reuseIdentifier: CellIdentifier];
	}
	
	// Configure the cell:
	
	if (indexPath.row < colors.count)             // just a sanity test
		cell.textLabel.textColor = colors[indexPath.row];
	
	cell.textLabel.text = [NSString stringWithFormat: @"Color # %d", indexPath.row + 1];
	
	return cell;
}

//------------------------------------------------------------------------------
#pragma mark - Table view delegate
//------------------------------------------------------------------------------

- (void) tableView: (UITableView*) tableView didSelectRowAtIndexPath: (NSIndexPath*) indexPath
{
	UITableViewCell* cell = [self.tableView cellForRowAtIndexPath: indexPath];
	
	pickingColorIndex = indexPath.row;
	
	InfColorPickerController* picker = [InfColorPickerController colorPickerViewController];
	
	picker.sourceColor = colors[pickingColorIndex];
	picker.delegate = self;
	picker.navigationItem.title = cell.textLabel.text;
	
	[self.navigationController pushViewController: picker animated: YES];
}

//------------------------------------------------------------------------------
#pragma mark - InfColorPickerControllerDelegate
//------------------------------------------------------------------------------

- (void) colorPickerControllerDidChangeColor: (InfColorPickerController*) controller
{
	NSUInteger indexes[2] = { 0, pickingColorIndex };
	NSIndexPath* indexPath = [NSIndexPath indexPathWithIndexes: indexes length: 2];
	UITableViewCell* cell = [self.tableView cellForRowAtIndexPath: indexPath];
	
	colors[pickingColorIndex] = controller.resultColor;
	
	cell.textLabel.textColor = controller.resultColor;
}

//------------------------------------------------------------------------------

@end

//==============================================================================
