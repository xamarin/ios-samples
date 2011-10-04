APPDIR = MonoCatalog.app
EXE = bin/monocatalog.exe
APP = $(APPDIR)/monocatalog

CSFILES = \
	addressbook.cs	\
	alert.cs	\
	buttons.cs 	\
	controls.cs	\
	images.cs	\
	monocatalog.cs 	\
	picker.cs	\
	pickercustom.cs	\
	searchbar.cs	\
	segment.cs	\
	textfield.cs	\
	textview.cs	\
	toolbar.cs	\
	transition.cs	\
	web.cs

IMAGES = \
	images/whiteButton.png \
	images/blueButton.png \
	images/UIButton_custom.png \
	images/segment_check.png \
	images/slider_ball.png \
	images/yellowslide.png \
	images/orangeslide.png \
	images/12-6AM.png \
	images/6-12AM.png \
	images/12-6PM.png \
	images/6-12PM.png \
	images/scene1.jpg \
	images/scene2.jpg \
	images/scene3.jpg \
	images/scene4.jpg \
	images/scene5.jpg \
	images/segment_search.png \
	images/segment_tools.png	\
	images/Icon.png

XIBS = \
	xibs/MainWindow.xib 			\
	xibs/AddressBookController.xib 			\
	xibs/AlertsViewController.xib		\
	xibs/ButtonsViewController.xib 		\
	xibs/ControlsViewController.xib		\
	xibs/ImagesViewController.xib		\
	xibs/PickerViewController.xib		\
	xibs/SearchBarController.xib		\
	xibs/SegmentViewController.xib		\
	xibs/TextFieldController.xib		\
	xibs/TextViewController.xib		\
	xibs/ToolbarViewController.xib		\
	xibs/TransitionViewController.xib	\
	xibs/WebViewController.xib 

NIBS = $(addsuffix .nib, $(basename $(XIBS)))

GENERATED = $(addsuffix .designer.cs, $(XIBS))

EXTRA_DEPS = $(IMAGES) \
	$(NIBS)

DATA = $(EXTRA_DEPS)
SOURCES = $(CSFILES) $(GENERATED)
MFLAGS=-icon=Icon.png -bundleid=com.mono-project.monocatalog

xibs/%.nib: xibs/%.xib
	ibtool $< --compile $@

xibs/%.xib.designer.cs: xibs/%.xib
	$(MIBTOOL) $^

include ../Make.common

xcodeproj:
	-mkdir -p xcode/MonoCatalog
	-mkdir bin
	$(MTOUCH) -nostrip -v -icon=Icon.png -bundleid=com.mono-project.monocatalog -xcode xcode/MonoCatalog -res:xibs/AlertsViewController.xib -res:xibs/ImagesViewController.xib -res=xibs/SearchBarController.xib -res=xibs/TextViewController.xib -res=xibs/WebViewController.xib -res=xibs/ButtonsViewController.xib -res=xibs/MainWindow.xib -res=xibs/SegmentViewController.xib -res=xibs/ToolbarViewController.xib -res=xibs/ControlsViewController.xib -res=xibs/PickerViewController.xib -res=xibs/TextFieldController.xib -res=xibs/TransitionViewController.xib -res=images/12-6AM.png -res=images/6-12PM.png -res=images/blueButton.png -res=images/scene2.jpg -res=images/scene5.jpg -res=images/segment_tools.png -res=images/yellowslide.png -res=images/12-6PM.png -res=images/Icon.png -res=images/orangeslide.png -res=images/scene3.jpg -res=images/segment_check.png -res=images/slider_ball.png -res=images/6-12AM.png -res=images/UIButton_custom.png -res=images/scene1.jpg -res=images/scene4.jpg -res=images/segment_search.png -res=images/whiteButton.png $(EXE) -res=xibs/AddressBookController.xib
