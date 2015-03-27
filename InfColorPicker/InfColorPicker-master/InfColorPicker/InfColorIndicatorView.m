//==============================================================================
//
//  InfColorIndicatorView.m
//  InfColorPicker
//
//  Created by Troy Gaul on 8/10/10.
//
//  Copyright (c) 2011-2013 InfinitApps LLC: http://infinitapps.com
//	Some rights reserved: http://opensource.org/licenses/MIT
//
//==============================================================================

#import "InfColorIndicatorView.h"

//------------------------------------------------------------------------------

#if !__has_feature(objc_arc)
#error This file must be compiled with ARC enabled (-fobjc-arc).
#endif

//==============================================================================

@implementation InfColorIndicatorView

//------------------------------------------------------------------------------

- (id) initWithFrame: (CGRect) frame
{
	self = [super initWithFrame: frame];
	
	if (self) {
		self.opaque = NO;
		self.userInteractionEnabled = NO;
	}
	
	return self;
}

//------------------------------------------------------------------------------

- (void) setColor: (UIColor*) newColor
{
	if (![_color isEqual: newColor]) {
		_color = newColor;
		
		[self setNeedsDisplay];
	}
}

//------------------------------------------------------------------------------

- (void) drawRect: (CGRect) rect
{
	CGContextRef context = UIGraphicsGetCurrentContext();
	
	CGPoint center = { CGRectGetMidX(self.bounds), CGRectGetMidY(self.bounds) };
	CGFloat radius = CGRectGetMidX(self.bounds);
	
	// Fill it:
	
	CGContextAddArc(context, center.x, center.y, radius - 1.0f, 0.0f, 2.0f * (float) M_PI, YES);
	[self.color setFill];
	CGContextFillPath(context);
	
	// Stroke it (black transucent, inner):
	
	CGContextAddArc(context, center.x, center.y, radius - 1.0f, 0.0f, 2.0f * (float) M_PI, YES);
	CGContextSetGrayStrokeColor(context, 0.0f, 0.5f);
	CGContextSetLineWidth(context, 2.0f);
	CGContextStrokePath(context);
	
	// Stroke it (white, outer):
	
	CGContextAddArc(context, center.x, center.y, radius - 2.0f, 0.0f, 2.0f * (float) M_PI, YES);
	CGContextSetGrayStrokeColor(context, 1.0f, 1.0f);
	CGContextSetLineWidth(context, 2.0f);
	CGContextStrokePath(context);
}

//------------------------------------------------------------------------------

@end

//==============================================================================
