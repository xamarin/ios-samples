APPDIR = OpenGLESSample.app
EXE = OpenGLESSample.exe
SOURCES = OpenGLESSample.cs
REFS = -r:OpenTK.dll
EXTRA_DEPS = MainWindow.nib
APP = $(APPDIR)/OpenGLESSample
DATA = $(EXTRA_DEPS)

%.nib: %.xib
	ibtool $< --compile $@

include ../Make.common

