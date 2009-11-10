APPDIR = GLPaint.app
EXE = GLPaint.exe
SOURCES = GLPaint.cs ShakeMe.cs
REFS = -r:OpenTK.dll
APP = $(APPDIR)/GLPaint
DATA = \
	$(EXTRA_DEPS)		\
	Default.png		\
	Erase.caf		\
	Icon.png		\
	Images/Blue.png		\
	Images/Green.png	\
	Images/Purple.png	\
	Images/Red.png		\
	Images/Yellow.png	\
	Particle.png		\
	Recording.data		\
	Select.caf

include ../Make.common

xcodeproj:
	mkdir -p xcode/GLPaint
	$(MTOUCH) -nostrip -v -bundleid=com.mono-project.GLPaint -xcode xcode/GLPaint  $(addprefix -res:,$(DATA)) GLPaint.exe
