//
// how to capture still images, video and audio using iOS AVFoundation and the AVCAptureSession
// 
// This sample handles all of the low-level AVFoundation and capture graph setup required to capture and save media.  This code also exposes the
// capture, configuration and notification capabilities in a more '.Netish' way of programming.  The client code will not need to deal with threads, delegate classes
// buffer management, or objective-C data types but instead will create .NET objects and handle standard .NET events.  The underlying iOS concepts and classes are detailed in 
// the iOS developer online help (TP40010188-CH5-SW2).
//
// https://developer.apple.com/library/mac/#documentation/AudioVideo/Conceptual/AVFoundationPG/Articles/04_MediaCapture.html#//apple_ref/doc/uid/TP40010188-CH5-SW2
//
// Enhancements, suggestions and bug reports can be sent to steve.millar@infinitekdev.com
//
using System;
using System.Collections.Generic;
using System.IO;
using MonoTouch.Dialog;
using Foundation;
using UIKit;

namespace MediaCapture
{
	public class MediaBrowserDialog
	{
		private string rootImagePath = null;
		private string rootVideoPath = null;
		
		private RootElement menu = null;
		private RootElement moviesElement = null;
		private RootElement imagesElement = null;
		
		private MediaBrowserDialog(){}
		
		public MediaBrowserDialog( string rootImagePath, string rootVideoPath)
		{
			this.rootImagePath = rootImagePath;
			this.rootVideoPath = rootVideoPath;
		}
		
		public EventHandler<FileSelectedEventArgs> MovieFileSelected;
		private void onMovieFileSelected( string file )
		{
			if ( MovieFileSelected != null )
			{
				FileSelectedEventArgs args = new FileSelectedEventArgs();
				args.File = file;
				MovieFileSelected( this, args );
			}
		}
		
		public EventHandler<FileSelectedEventArgs> ImageFileSelected;
		private void onImageFileSelected( string file )
		{
			if ( ImageFileSelected != null )
			{
				FileSelectedEventArgs args = new FileSelectedEventArgs();
				args.File = file;
				ImageFileSelected( this, args );
			}
		}
		
		public RootElement Menu
		{
			get
			{
				return buildRootMenu();
			}
		}
		
		private RootElement buildRootMenu()
		{
			menu = new RootElement ("Recorded Media");
			moviesElement = buildMoviesElement();
			imagesElement = buildImagesElement();
			Section rootSection = new Section("File Types");
			rootSection.Add((Element)moviesElement);
			rootSection.Add((Element)imagesElement);
			menu.Add(rootSection);
			return menu;
		}
		
		private RootElement buildMoviesElement()
		{
			RootElement element = new RootElement("Movies");
			Section section = new Section("Recorded At");
			section.AddAll( getMediaElements(this.rootVideoPath, MediaFileType.Movie ) );
			element.Add( section );
			return element;
		}
		
		private RootElement buildImagesElement()
		{
			RootElement element = new RootElement("Images");
			Section section = new Section("Captured At");
			section.AddAll( getMediaElements(this.rootImagePath, MediaFileType.Image ) );
			element.Add( section );
			return element;
		}
		
		private RootElement[] getMediaElements( string rootPath, MediaFileType fileType )
		{
			List<RootElement> elements = new List<RootElement>();
			if ( Directory.Exists( rootPath ) )
			{
				foreach ( string directory in Directory.GetDirectories( rootPath ) )
				{
					string shortDirName = Path.GetFileName( directory );
					RootElement element = new RootElement( shortDirName );
					string text = fileType == MediaFileType.Movie ? "Touch file to play" : "Touch file to view";
					Section fileSection = new Section( text );
					element.Add( fileSection );
					foreach ( string file in Directory.GetFiles( directory ) )
					{
						FileElement fileElement = new FileElement( Path.GetFileName( file ), fileType, handleFileElementTap );
						fileElement.Path = file;
						fileSection.Add( fileElement );
					}
					elements.Add( element );
				}
			}
			return elements.ToArray();
		}
		
		private void handleFileElementTap( string file, MediaFileType fileType )
		{
			if ( fileType == MediaFileType.Movie )
			{
				onMovieFileSelected( file );		
			}
			else if ( fileType == MediaFileType.Image )
			{
				onImageFileSelected( file );		
			}
		}
	}
	
	public class FileSelectedEventArgs : EventArgs
	{
		public string File;		
	}
	
	public enum MediaFileType
	{
		Movie,
		Image
	}
	
	internal delegate void fileElementTapHandler( string file, MediaFileType fileType );

	internal class FileElement : StringElement
	{
		public FileElement( string caption, MediaFileType fileType, fileElementTapHandler handler ) : base(caption)
		{
			this.Tapped += delegate 
			{
				handler( this.Path, FileType );
			};
			this.FileType = fileType;
		}
		
		public string Path = null;
		public MediaFileType FileType;
	}
}

