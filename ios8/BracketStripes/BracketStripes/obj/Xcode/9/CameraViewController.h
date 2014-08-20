// WARNING
// This file has been generated automatically by Xamarin Studio to
// mirror C# types. Changes in this file made by drag-connecting
// from the UI designer will be synchronized back to C#, but
// more complex manual changes may not transfer correctly.


#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#import "CapturePreviewView.h"

@interface CameraViewController : UIViewController {
	UISegmentedControl *_bracketModeControl;
	CapturePreviewView *_cameraPreviewView;
	UIButton *_cameraShutterButton;
}

@property (nonatomic, retain) IBOutlet UISegmentedControl *bracketModeControl;

@property (nonatomic, retain) IBOutlet CapturePreviewView *cameraPreviewView;

@property (nonatomic, retain) IBOutlet UIButton *cameraShutterButton;

- (IBAction)CameraShutterDidPress:(UIButton *)sender;

- (IBAction)BracketChangeDidChange:(id)sender;

- (IBAction)bracketChangeDidChange:(id)sender;

@end
