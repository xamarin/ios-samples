using System;
using Foundation;
using System.Collections.Generic;
using UIKit;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

namespace TicTacToe
{
	public enum TTTProfileIcon
	{
		X,
		O
	}

	[Serializable]
	public class TTTProfile
	{
		const string EncodingKeyIcon = "icon";
		const string EncodingKeyCurrentGame = "currentGame";
		const string EncodingKeygames = "games";
		public const string IconDidChangeNotification = "TTTProfileIconDidChangeNotification";
		TTTProfileIcon icon;

		public TTTProfileIcon Icon {
			get { return icon; }
			set {
				if (icon != value) {
					icon = value;

					NSNotificationCenter.DefaultCenter.PostNotificationName (
						IconDidChangeNotification, null);
				}
			}
		}

		public TTTGame CurrentGame;
		public List<TTTGame> Games;

		public TTTProfile ()
		{
			Games = new List<TTTGame> ();
			StartNewGame ();
		}

		public static TTTProfile FromPath (string path)
		{
			if (File.Exists (path)) {
				byte[] bytes = File.ReadAllBytes (path);
				return (TTTProfile)ByteArrayToObject (bytes);
			}

			return null;
		}

		public bool WriteToPath (string path)
		{
			byte[] bytes = ObjectToByteArray (this);
			File.WriteAllBytes (path, bytes);

			return true;
		}

		public TTTGame StartNewGame ()
		{
			if (CurrentGame != null && CurrentGame.Moves.Count == 0)
				return CurrentGame;

			TTTGame game = new TTTGame ();
			Games.Insert (0, game);

			CurrentGame = game;
			return game;
		}

		void setIcon (TTTProfileIcon value)
		{
			if (Icon != value) {
				Icon = value;

				NSNotificationCenter.DefaultCenter.PostNotificationName (
					IconDidChangeNotification, null);
			}
		}

		public int NumberOfGamesWithResult (TTTGameResult result)
		{
			int count = 0;
			foreach (var game in Games) {
				if (game.Result == result)
					count++;
			}

			return count;
		}

		#region Images
		TTTProfileIcon iconForPlayer (TTTMovePlayer player)
		{
			TTTProfileIcon myIcon = Icon;
			return player == TTTMovePlayer.Me ? myIcon : 1 - myIcon;
		}

		public UIImage ImageForPlayer (TTTMovePlayer player)
		{
			TTTProfileIcon icon = iconForPlayer (player);
			return ImageForIcon (icon);
		}

		public UIColor ColorForPlayer (TTTMovePlayer player)
		{
			TTTProfileIcon icon = iconForPlayer (player);
			return colorForIcon (icon);
		}

		public static UIImage ImageForIcon (TTTProfileIcon icon)
		{
			string imageName = icon.ToString ().ToLower ();
			return UIImage.FromBundle (imageName);
		}

		public static UIImage SmallImageForIcon (TTTProfileIcon icon)
		{
			string imageName = "small" + icon.ToString ();
			return UIImage.FromBundle (imageName);
		}

		UIColor colorForIcon (TTTProfileIcon icon)
		{
			return icon == TTTProfileIcon.X ? UIColor.Red : UIColor.Green;
		}
		#endregion

		#region Serialization
		// Convert an object to a byte array
		public static byte[] ObjectToByteArray (Object obj)
		{
			if (obj == null)
				return null;
			BinaryFormatter bf = new BinaryFormatter ();
			MemoryStream ms = new MemoryStream ();
			bf.Serialize (ms, obj);
			return ms.GetBuffer ();
		}

		// Convert a byte array to an Object
		public static Object ByteArrayToObject (byte[] arrBytes)
		{
			MemoryStream memStream = new MemoryStream ();
			BinaryFormatter binForm = new BinaryFormatter ();
			memStream.Write (arrBytes, 0, arrBytes.Length);
			memStream.Seek (0, SeekOrigin.Begin);
			Object obj = binForm.Deserialize (memStream);
			return obj;
		}
		#endregion
	}
}

