//==============================================================================
//
//  InfColorSquarePicker.m
//  InfColorPicker
//
//  Created by Troy Gaul on 8/9/10.
//
//  Copyright (c) 2011-2013 InfinitApps LLC: http://infinitapps.com
//	Some rights reserved: http://opensource.org/licenses/MIT
//
//==============================================================================

#import "InfColorSquarePicker.h"

#import "InfColorIndicatorView.h"
#import "InfHSBSupport.h"

//------------------------------------------------------------------------------

#if !__has_feature(objc_arc)
#error This file must be compiled with ARC enabled (-fobjc-arc).
#endif

//------------------------------------------------------------------------------

#define kContentInsetX 20
#define kContentInsetY 20

#define kIndicatorSize 24

//==============================================================================

@implementation InfColorSquareView

//------------------------------------------------------------------------------

- (void) updateContent
{
	CGImageRef imageRef = createSaturationBrightnessSquareContentImageWithHue(self.hue * 360);
	self.image = [UIImage imageWithCGImage: imageRef];
	CGImageRelease(imageRef);
}

//------------------------------------------------------------------------------
#pragma mark	Properties
//------------------------------------------------------------------------------

- (void) setHue: (float) value
{
	if (value != _hue || self.image == nil) {
		_hue = value;
		
		[self updateContent];
	}
}

//------------------------------------------------------------------------------

@end

//==============================================================================

@implementation InfColorSquarePicker {
	InfColorIndicatorView* indicator;
}

//------------------------------------------------------------------------------
#pragma mark	Appearance
//------------------------------------------------------------------------------

- (void) setIndicatorColor
{
	if (indicator == nil)
		return;
	
	indicator.color = [UIColor colorWithHue: self.hue
	                             saturation: self.value.x
	                             brightness: self.value.y
	                                  alpha: 1.0f];
}

//------------------------------------------------------------------------------

- (NSString*) spokenValue
{
	return [NSString stringWithFormat: @"%d%% saturation, %d%% brightness", 
						(int) (self.value.x * 100), (int) (self.value.y * 100)];
}

//------------------------------------------------------------------------------

- (void) layoutSubviews
{
	if (indicator == nil) {
		CGRect indicatorRect = { CGPointZero, { kIndicatorSize, kIndicatorSize } };
		indicator = [[InfColorIndicatorView alloc] initWithFrame: indicatorRect];
		[self addSubview: indicator];
	}
	
	[self setIndicatorColor];
	
	CGFloat indicatorX = kContentInsetX + (self.value.x * (self.bounds.size.width - 2 * kContentInsetX));
	CGFloat indicatorY = self.bounds.size.height - kContentInsetY
					   - (self.value.y * (self.bounds.size.height - 2 * kContentInsetY));
	
	indicator.center = CGPointMake(indicatorX, indicatorY);
}

//------------------------------------------------------------------------------
#pragma mark	Properties
//------------------------------------------------------------------------------

- (void) setHue: (float) newValue
{
	if (newValue != _hue) {
		_hue = newValue;
		
		[self setIndicatorColor];
	}
}

//------------------------------------------------------------------------------

- (void) setValue: (CGPoint) newValue
{
	if (!CGPointEqualToPoint(newValue, _value)) {
		_value = newValue;
		
		[self sendActionsForControlEvents: UIControlEventValueChanged];
		[self setNeedsLayout];
	}
}

//------------------------------------------------------------------------------
#pragma mark	Tracking
//------------------------------------------------------------------------------

- (void) trackIndicatorWithTouch: (UITouch*) touch
{
	CGRect bounds = self.bounds;
	
	CGPoint touchValue;
	
	touchValue.x = ([touch locationInView: self].x - kContentInsetX)
				 / (bounds.size.width - 2 * kContentInsetX);
	
	touchValue.y = ([touch locationInView: self].y - kContentInsetY)
				 / (bounds.size.height - 2 * kContentInsetY);
	
	touchValue.x = pin(0.0f, touchValue.x, 1.0f);
	touchValue.y = 1.0f - pin(0.0f, touchValue.y, 1.0f);
	
	self.value = touchValue;
}

//------------------------------------------------------------------------------

- (BOOL) beginTrackingWithTouch: (UITouch*) touch
                      withEvent: (UIEvent*) event
{
	[self trackIndicatorWithTouch: touch];
	return YES;
}

//------------------------------------------------------------------------------

- (BOOL) continueTrackingWithTouch: (UITouch*) touch
                         withEvent: (UIEvent*) event
{
	[self trackIndicatorWithTouch: touch];
	return YES;
}

//------------------------------------------------------------------------------

@end

//==============================================================================
