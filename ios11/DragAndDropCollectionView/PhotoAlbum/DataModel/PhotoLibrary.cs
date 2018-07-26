using System;
using System.Collections.Generic;
using System.Timers;
using CoreFoundation;
using UIKit;

namespace PhotoAlbum
{
    /// <summary>
    /// The backing data store for the application that contains an array of photo albums, and methods to retrieve data from and mutate the contents of these albums.
    /// </summary>
    public class PhotoLibrary
    {
        static PhotoLibrary sharedInstance = new PhotoLibrary();
        public static PhotoLibrary SharedInstance
        {
            get
            {
                return sharedInstance;
            }
        }

        public List<PhotoAlbum> Albums = new List<PhotoAlbum>();

        #region Identifier Lookups
        public PhotoAlbum Album(Guid identifier)
        {
            return Albums.Find(photo => photo.Identifier == identifier);
        }

        public Photo Photo(Guid identifier)
        {
            var album = Albums.Find(a =>
            {
                return a.Photos.Contains(a.Photos.Find(p => p.Identifier == identifier));
            });
            return album?.Photos.Find(photo => photo.Identifier == identifier);
        }
        #endregion

        #region Photo Library Mutations
        /// <summary>
        /// Adds a photo to the album.
        /// </summary>
        public void Add(Photo photo, PhotoAlbum toAlbum)
        {
            var albumIndex = Albums.IndexOf(toAlbum);

            Albums[albumIndex].Photos.Insert(0, photo);
        }

        /// <summary>
        /// Inserts the photo at a specific index in the album.
        /// </summary>
        public void Insert(Photo photo, PhotoAlbum inAlbum, int index)
        {
            var albumIndex = Albums.IndexOf(inAlbum);

            Albums[albumIndex].Photos.Insert(index, photo);
        }

        /// <summary>
        /// Moves an album from one index to another.
        /// </summary>
        public void MoveAlbum(int sourceIndex, int destinationIndex)
        {
            var album = Albums[sourceIndex];
            Albums.Remove(album);
            Albums.Insert(destinationIndex, album);
        }

        /// <summary>
        /// Moves a photo from one index to another in the album.
        /// </summary>
        public void MovePhoto(PhotoAlbum album, int sourceIndex, int destinationIndex)
        {
            var albumIndex = Albums.IndexOf(album);
            if (albumIndex < 0) return;

            var photo = Albums[albumIndex].Photos[sourceIndex];
            Albums[albumIndex].Photos.Remove(photo);
            Albums[albumIndex].Photos.Insert(destinationIndex, photo);
        }

        /// <summary>
        /// Moves a photo to a different album at a specific index in that album. Defaults to inserting at the beginning of the album if no index is specified.
        /// </summary>
        public void MovePhoto(Photo photo, PhotoAlbum toAlbum, int index = 0)
        {
            var sourceAlbumIndex = Albums.IndexOf(Albums.Find(a => a.Photos.Contains(photo)));
            var indexOfPhotoInSourceAlbum = Albums[sourceAlbumIndex].Photos.IndexOf(photo);
            var destinationAlbumIndex = Albums.IndexOf(toAlbum);

            if (sourceAlbumIndex < 0 || indexOfPhotoInSourceAlbum < 0 || destinationAlbumIndex < 0) return;

            var movePhoto = Albums[sourceAlbumIndex].Photos[indexOfPhotoInSourceAlbum];
            Albums[sourceAlbumIndex].Photos.Remove(movePhoto);
            Albums[destinationAlbumIndex].Photos.Insert(index, movePhoto);
        }
        #endregion

        #region Timed Automatic Insertions
        PhotoAlbum albumForAutomaticInsertions;
        Timer automaticInsertionTimer;

        /// <summary>
        /// Starts a timer that performs an automatic insertion of a photo into the album when it fires.
        /// </summary>
        public void StartAutomaticInsertions(PhotoAlbum album, PhotoCollectionViewController photoCollectionViewController)
        {
            StopAutomaticInsertions();

            albumForAutomaticInsertions = album;
            automaticInsertionTimer = new Timer(1000);
            automaticInsertionTimer.Elapsed += (sender, e) => {
                var intoAlbum = albumForAutomaticInsertions;
                var albumIndex = Albums.IndexOf(intoAlbum);
                if (intoAlbum == null || albumIndex < 0) return;

                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    var image = UIImage.FromBundle("AutomaticInsertion.png");

                    var photo = new Photo(image);
                    var insertionIndex = new Random().Next(Albums[albumIndex].Photos.Count);
                    Console.WriteLine("insert" + insertionIndex);
                    Albums[albumIndex].Photos.Insert(insertionIndex, photo);
                    photoCollectionViewController.InsertedItem(insertionIndex);
                });
			};
            automaticInsertionTimer.Start();
        }

        /// <summary>
        /// Stops the timer that performs automatic insertions.
        /// </summary>
        public void StopAutomaticInsertions()
        {
            albumForAutomaticInsertions = null;
            automaticInsertionTimer?.Stop();
            automaticInsertionTimer = null;
        }
        #endregion

        #region Initialization and Loading Sample Data
        public PhotoLibrary()
		{
			LoadSampleData();
		}

        void LoadSampleData()
        {
            var albumIndex = 0;
            bool foundAlbum;

            do
            {
                foundAlbum = false;
                var photos = new List<Photo>();

                var photoIndex = 0;
                bool foundPhoto;

                do
                {
                    foundPhoto = false;
                    var imageName = $"Sample Data/Album{albumIndex}Photo{photoIndex}.jpg";
                    var image = UIImage.FromBundle(imageName);
                    if (image != null)
                    {
                        foundPhoto = true;
                        var photo = new Photo(image);
                        photos.Add(photo);
                    }
                    photoIndex++;
                } while (foundPhoto);

                if (photos.Count != 0)
                {
                    foundAlbum = true;
                    var title = $"Album {albumIndex + 1}";
                    var album = new PhotoAlbum { Title = title, Photos = photos };
                    Albums.Add(album);
                }
                albumIndex++;
            } while (foundAlbum);
            Console.WriteLine("" + albumIndex);
        }
        #endregion
    }
}
