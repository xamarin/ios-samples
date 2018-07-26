using System;
using System.Collections.Generic;
using UIKit;

namespace PhotoAlbum
{
	/// <summary>
	/// A model object that represents an album of photos.
	/// </summary>
	public class PhotoAlbum
    {
        public PhotoAlbum()
        {
        }

        public readonly Guid Identifier = Guid.NewGuid();
        public string Title = "";
        public List<Photo> Photos = new List<Photo>();

        public UIImage Thumbnail {
            get {
                if (Photos.Count > 0)
                    return Photos[0]?.thumbnail;
                else 
                    return null;
            }
        }

        public bool Contains (Photo photo)
        {
            return Photos.Contains(photo);
        }

		public override bool Equals(System.Object obj)
		{
            return ((PhotoAlbum)obj).Identifier == Identifier;
		}
        public bool Equals(PhotoAlbum p)
		{
            return p.Identifier == Identifier;
		}
		public override int GetHashCode()
		{
            return Identifier.GetHashCode();
		}
    }
}
