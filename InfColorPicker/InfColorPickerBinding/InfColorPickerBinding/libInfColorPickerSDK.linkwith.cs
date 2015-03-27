using System;
using ObjCRuntime;

[assembly: LinkWith ("libInfColorPickerSDK.a", LinkTarget.Simulator | LinkTarget.ArmV7, ForceLoad = true)]
