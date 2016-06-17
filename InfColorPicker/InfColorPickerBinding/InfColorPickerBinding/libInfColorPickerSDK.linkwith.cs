using System;
using ObjCRuntime;

[assembly: LinkWith ("libInfColorPickerSDK.a", LinkTarget.Simulator, SmartLink = true, ForceLoad = true)]
