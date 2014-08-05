AVCustomEdit
========================

Block by an issue in SimpleEditor.cs line 255.

Got an NullReferenceException when set playerItem.VideoComposition with this.videoComposition.CustomVideoCompositorClass is not a "Nil" Class

//If set the CustomVideoCompositorClass in playerItem.VideoComposition as this way:
playerItem.VideoComposition.CustomVideoCompositorClass = new MonoTouch.ObjCRuntime.Class(typeof(CrossDissolveCompositor));
It will get a System.NotImplementedException

This is a port of Apple's iOS7 sample AVCustomEdit.


