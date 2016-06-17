//			// Create a new still image session
//			session = new AVCaptureSession();
//			var camera = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);
//			var input = AVCaptureDeviceInput.FromDevice(camera);
//			session.AddInput(input);
//
//			// Configure and attach a still image grabber for bracketed capture
//			var StillImageOutput = new AVCaptureStillImageOutput ();
//			var dict = new NSMutableDictionary();
//			dict[AVVideo.CodecKey] = new NSNumber((int) AVVideoCodec.JPEG);
//			session.AddOutput (StillImageOutput);
//
//			// Attach to camera view
//			var previewLayer = new AVCaptureVideoPreviewLayer(session);
//			previewLayer.Orientation = AVCaptureVideoOrientation.Portrait;
//			previewLayer.VideoGravity = "AVLayerVideoGravityResizeAspectFill";
//			previewLayer.Frame = CameraView.Frame;
//          CameraView.Layer.AddSublayer (previewLayer);