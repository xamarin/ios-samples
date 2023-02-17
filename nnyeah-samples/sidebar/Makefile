
ifndef MS_PLATFORM_DLL
$(error MS_PLATFORM_DLL is not set. See the package README.md for more information.)
endif

PACKAGE=sidebarnavigation
VERSION=2.1.0
FULLPACKAGE=$(PACKAGE).$(VERSION).nupkg
NUGETAPI=http://api.nuget.org/v3-flatcontainer
XAMARIN_PLATFORM_DLL=/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS/Xamarin.iOS.dll

all: prepare
	dotnet build

prepare: ./nuget nuget/$(FULLPACKAGE)
	dotnet nnyeah \
		--input nuget/lib/xamarinios10/SidebarNavigation.dll \
		--output nuget/SidebarNavigation.dll \
		--xamarin-assembly=$(XAMARIN_PLATFORM_DLL) \
		--microsoft-assembly=$(MS_PLATFORM_DLL) \
		--force-overwrite
nuget:
	mkdir $@

nuget/$(FULLPACKAGE):
	wget $(NUGETAPI)/$(PACKAGE)/$(VERSION)/$(FULLPACKAGE) -O nuget/$(FULLPACKAGE)
	unzip nuget/$(FULLPACKAGE) -d nuget

clean:
	rm -rf ./nuget ./bin ./obj

