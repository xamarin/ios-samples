XBUILD=xcodebuild
PROJECT_ROOT=./
PROJECT=$(PROJECT_ROOT)/OrderSoupIntentStaticLib.xcodeproj
TARGET=OrderSoupIntentStaticLib

all: bind

lib$(TARGET)-simulator.a:
	$(XBUILD) -project $(PROJECT) -target $(TARGET) -sdk iphonesimulator -configuration Release clean build
	-mv $(PROJECT_ROOT)/build/Release-iphonesimulator/lib$(TARGET).a $@

lib$(TARGET)-device.a:
	$(XBUILD) -project $(PROJECT) -target $(TARGET) -sdk iphoneos -arch arm64 -configuration Release clean build
	-mv $(PROJECT_ROOT)/build/Release-iphoneos/lib$(TARGET).a $@

lib$(TARGET).a: lib$(TARGET)-simulator.a lib$(TARGET)-device.a
	xcrun -sdk iphoneos lipo -create -output $@ $^

bind: lib$(TARGET).a
	sharpie bind -scope OrderSoupIntentStaticLib -output bo -namespace SoupChef -sdk iphoneos$(shell xcrun --show-sdk-platform-version --sdk iphoneos) OrderSoupIntentStaticLib/OrderSoupIntent.h

clean:
	-rm -rf bo build *.a *.dll
