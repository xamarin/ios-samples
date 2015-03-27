//==============================================================================
//
//  InfColorSquarePicker.h
//  InfColorPicker
//
//  Created by Troy Gaul on 8/9/10.
//
//  Copyright (c) 2011-2013 InfinitApps LLC: http://infinitapps.com
//	Some rights reserved: http://opensource.org/licenses/MIT
//
//==============================================================================

#import <UIKit/UIKit.h>

//------------------------------------------------------------------------------

@interface InfColorSquareView : UIImageView

@property (nonatomic) float hue;

@end

//------------------------------------------------------------------------------

@interface InfColorSquarePicker : UIControl

@property (nonatomic) float hue;
@property (nonatomic) CGPoint value;

@end

//------------------------------------------------------------------------------
