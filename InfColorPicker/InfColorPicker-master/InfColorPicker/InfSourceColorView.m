//==============================================================================
//
//  InfSourceColorView.m
//  InfColorPicker
//
//  Created by Troy Gaul on 8/10/10.
//
//  Copyright (c) 2011-2013 InfinitApps LLC: http://infinitapps.com
//	Some rights reserved: http://opensource.org/licenses/MIT
//
//==============================================================================

#import "InfSourceColorView.h"

//------------------------------------------------------------------------------

#if !__has_feature(objc_arc)
#error This file must be compiled with ARC enabled (-fobjc-arc).
#endif

//==============================================================================

@implementation InfSourceColorView

//------------------------------------------------------------------------------
#pragma mark	UIView overrides
//------------------------------------------------------------------------------

- (void) drawRect: (CGRect) rect
{
	[super drawRect: rect];
	
	if (self.enabled && self.trackingInside) {
		CGRect bounds = [self bounds];
		
		[[UIColor whiteColor] set];
		CGContextStrokeRectWithWidth(UIGraphicsGetCurrentContext(),
		                             CGRectInset(bounds, 1, 1), 2);
		
		[[UIColor blackColor] set];
		UIRectFrame(CGRectInset(bounds, 2, 2));
	}
}

//------------------------------------------------------------------------------
#pragma mark	UIControl overrides
//------------------------------------------------------------------------------

- (void) setTrackingInside: (BOOL) newValue
{
	if (newValue != _trackingInside) {
		_trackingInside = newValue;
		[self setNeedsDisplay];
	}
}

//------------------------------------------------------------------------------

- (BOOL) beginTrackingWithTouch: (UITouch*) touch
                      withEvent: (UIEvent*) event
{
	if (self.enabled) {
		self.trackingInside = YES;
		
		return [super beginTrackingWithTouch: touch withEvent: event];
	}
	else {
		return NO;
	}
}

//------------------------------------------------------------------------------

- (BOOL) continueTrackingWithTouch: (UITouch*) touch withEvent: (UIEvent*) event
{
	BOOL isTrackingInside = CGRectContainsPoint([self bounds], [touch locationInView: self]);
	
	self.trackingInside = isTrackingInside;
	
	return [super continueTrackingWithTouch: touch withEvent: event];
}

//------------------------------------------------------------------------------

- (void) endTrackingWithTouch: (UITouch*) touch withEvent: (UIEvent*) event
{
	self.trackingInside = NO;
	
	[super endTrackingWithTouch: touch withEvent: event];
}

//------------------------------------------------------------------------------

- (void) cancelTrackingWithEvent: (UIEvent*) event
{
	self.trackingInside = NO;
	
	[super cancelTrackingWithEvent: event];
}

//------------------------------------------------------------------------------

@end

//==============================================================================
