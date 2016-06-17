using System;
using System.Collections.Generic;
using System.IO;

using MonoTouch.Dialog;

namespace MediaCapture {
	public class MediaBrowserDialog {
		string rootImagePath = null;
		string rootVideoPath = null;

		RootElement menu = null;
		RootElement moviesElement = null;
		RootElement imagesElement = null;

		public MediaBrowserDialog (string rootImagePath, string rootVideoPath)
		{
			this.rootImagePath = rootImagePath;
			this.rootVideoPath = rootVideoPath;
		}

		public EventHandler<FileSelectedEventArgs> MovieFileSelected;
		void OnMovieFileSelected (string file)
		{
			if (MovieFileSelected == null)
				return;

			var args = new FileSelectedEventArgs {
				File = file
			};

			MovieFileSelected (this, args);
		}

		public EventHandler<FileSelectedEventArgs> ImageFileSelected;
		void OnImageFileSelected( string file )
		{
			if (ImageFileSelected == null)
				return;

			var args = new FileSelectedEventArgs {
				File = file
			};

			ImageFileSelected (this, args);
		}

		public RootElement Menu {
			get {
				return BuildRootMenu ();
			}
		}

		RootElement BuildRootMenu ()
		{
			menu = new RootElement ("Recorded Media");
			moviesElement = BuildMoviesElement ();
			imagesElement = BuildImagesElement ();
			var rootSection = new Section ("File Types");
			rootSection.Add ((Element)moviesElement);
			rootSection.Add ((Element)imagesElement);
			menu.Add (rootSection);
			return menu;
		}

		RootElement BuildMoviesElement()
		{
			var element = new RootElement ("Movies");
			var section = new Section ("Recorded At");
			section.AddAll (GetMediaElements (rootVideoPath, MediaFileType.Movie));
			element.Add (section);
			return element;
		}

		RootElement BuildImagesElement ()
		{
			var element = new RootElement ("Images");
			var section = new Section ("Captured At");
			section.AddAll (GetMediaElements (rootImagePath, MediaFileType.Image));
			element.Add (section);
			return element;
		}

		RootElement[] GetMediaElements (string rootPath, MediaFileType fileType)
		{
			var elements = new List<RootElement> ();
			if (Directory.Exists (rootPath)) {
				foreach ( string directory in Directory.GetDirectories (rootPath)) {
					string shortDirName = Path.GetFileName (directory);
					var element = new RootElement (shortDirName);
					string text = fileType == MediaFileType.Movie ? "Touch file to play" : "Touch file to view";
					var fileSection = new Section (text);
					element.Add (fileSection);
					foreach (string file in Directory.GetFiles (directory)) {
						var fileElement = new FileElement (Path.GetFileName (file), fileType, handleFileElementTap);
						fileElement.Path = file;
						fileSection.Add (fileElement);
					}
					elements.Add (element);
				}
			}
			return elements.ToArray ();
		}

		void handleFileElementTap( string file, MediaFileType fileType )
		{
			if (fileType == MediaFileType.Movie) {
				OnMovieFileSelected( file );
			} else if (fileType == MediaFileType.Image) {
				OnImageFileSelected( file );
			}
		}
	}

	public class FileSelectedEventArgs : EventArgs
	{
		public string File;
	}

	public enum MediaFileType {
		Movie,
		Image
	}

	internal delegate void fileElementTapHandler (string file, MediaFileType fileType);

	class FileElement : StringElement
	{
		public string Path = null;
		public MediaFileType FileType;

		public FileElement (string caption, MediaFileType fileType, fileElementTapHandler handler) : base (caption)
		{
			Tapped += delegate {
				handler (Path, FileType);
			};
			FileType = fileType;
		}
	}
}

