using System;
using ObjCRuntime;

[assembly: LinkWith ("libXMBindingLibrarySampleUniversal.a", LinkTarget.Simulator | LinkTarget.ArmV6 | LinkTarget.ArmV7, ForceLoad = true)]
