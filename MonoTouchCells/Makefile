APPDIR = MonoTouchCells.app
EXE = MonoTouchCells.exe
APP = $(APPDIR)/MonoTouchCells
REFS = -r:System.Xml

SOURCES = \
	AppDelegate.cs \
	AppDelegate.g.cs \
	AssemblyInfo.cs \
	CustomCell.cs \
	CustomTableViewController.cs \
	DetailViewController.cs \
	Item.cs \
	Main.cs

IMAGES = \
	images/Default.png \
	images/Icon.png \
	images/checked.png \
	images/unchecked.png 


NIBS = \
        xibs/MainWindow.nib                     \
        xibs/DetailViewController.nib

EXTRA_DEPS = $(IMAGES)	\
	$(NIBS)

DATA = $(EXTRA_DEPS) 

MFLAGS=-icon=Icon.png -bundleid=com.mono-project.MonoTouchCells

xibs/%.nib: xibs/%.xib
	ibtool $< --compile $@

include ../Make.common

