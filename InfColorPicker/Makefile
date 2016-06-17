XBUILD=/Applications/Xcode.app/Contents/Developer/usr/bin/xcodebuild
BTOUCH=/Developer/MonoTouch/usr/bin/btouch
PROJECT_ROOT=InfColorPicker
PROJECT=$(PROJECT_ROOT)/InfColorPicker.xcodeproj
TARGET=InfColorPicker

all: libInfColorPickerSDK.a

libInfColorPicker-i386.a:
	$(XBUILD) -project $(PROJECT) -target $(TARGET) -sdk iphonesimulator -configuration Release clean build
	-mv $(PROJECT_ROOT)/build/Release-iphonesimulator/lib$(TARGET).a $@

libInfColorPicker-armv7.a:
	$(XBUILD) -project $(PROJECT) -target $(TARGET) -sdk iphoneos -arch armv7 -configuration Release clean build
	-mv $(PROJECT_ROOT)/build/Release-iphoneos/lib$(TARGET).a $@

libInfColorPicker-arm64.a:
	$(XBUILD) -project $(PROJECT) -target $(TARGET) -sdk iphoneos -arch arm64 -configuration Release clean build
	-mv $(PROJECT_ROOT)/build/Release-iphoneos/lib$(TARGET).a $@


libInfColorPickerSDK.a: libInfColorPicker-i386.a libInfColorPicker-armv7.a libInfColorPicker-arm64.a
	lipo -create -output $@ $^

clean: 
	-rm -f *.a *.dll