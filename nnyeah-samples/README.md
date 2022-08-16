# nnyeah Samples

## What is nnyeah?

nnyeah is a tool that can convert legacy Xamarin platform assemblies to run under .NET 6 or later. The repository for it is here: https://github.com/xamarin/xamarin-macios/tree/main/tools/nnyeah
nnyeah takes 3 inputs:

1. an assembly compiled against a Xamarin platform assembly
2. a Xamarin platform assembly (Xamarin.iOS.dll or Xamarin.macOS.dll)
3. a Microsoft platform assembly (Microsoft.iOS.dll or Microsoft.macOS.dll)

and will generate an output assembly which should act as it were compiled against the Microsoft platform assembly.


## Getting nnyeah

nnyeah is packaged as a nuget package and can be downloaded from here: https://github.com/xamarin/ios-samples/nnyeah-samples/nupkg/nnyeah.0.1.0.nupkg
In addition, nnyeah can be built from the github repository above.

Installing nnyeah as a dotnet tool
nnyeah can be be installed as a dotnet tool with the following command:

    dotnet tool install --add-source /path/to/nupkg-directory -g nnyeah

where `/path/to/nupkg-directory` is a directory which contains the nnyeah nupkg file.

Building nnyeah Samples

First, you will need a path to your target Microsoft iOS dll. You can can get a list of what you have installed with this command:

    find $(dirname $(which dotnet)) -name Microsoft.iOS.dll -print | grep ref

This may return several results. The best choice is probably the one with the highest version.
Set an environment variable named `MS_PLATFORM_DLL` to the path to the dll to target:

    export MS_PLATFORM_DLL=/path/to/Microsoft.iOS.dll

then execute `make` in the directory of the sample you want to try.

Provided samples:

- geo - provides access to geolocation data
- perm - handles permissions (this does not have a sample, but is required by geo
- sidebar - provides a navigation sidebar



