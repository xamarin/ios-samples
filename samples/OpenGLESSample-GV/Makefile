APPDIR = OpenGLESSample-GV.app
EXE = OpenGLESSample-GV.exe
SOURCES = OpenGLESSample.cs
REFS = -r:OpenTK.dll
EXTRA_DEPS = MainWindow.nib
APP = $(APPDIR)/OpenGLESSample-GV
DATA = $(EXTRA_DEPS)

%.nib: %.xib
	ibtool $< --compile $@

include ../Make.common

