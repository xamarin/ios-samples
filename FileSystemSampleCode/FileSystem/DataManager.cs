using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace FileSystem {
	/// <summary>
	/// This class contains the code shown in the article "Working with the File System"
	/// </summary>
	public static class DataManager {
		public static string ReadText ()
		{
			var text = File.ReadAllText ("TestData/ReadMe.txt");
			Console.WriteLine (text);

			return text;
		}

		public static string ReadDirectories ()
		{
			var result = new StringBuilder ();

			var directories = Directory.EnumerateDirectories ("./");
			foreach (var directory in directories) {
				result.AppendLine (directory);
				Console.WriteLine (directory);
			}

			return result.ToString ();
		}

		public static string ReadAll ()
		{
			var result = new StringBuilder ();

			var fileOrDirectory = Directory.EnumerateFileSystemEntries ("./");
			foreach (var entry in fileOrDirectory) {
				result.AppendLine (entry);
				Console.WriteLine (entry);
			}

			return result.ToString ();
		}

		public static string ReadXml ()
		{
			var result = string.Empty;

			using (TextReader reader = new StreamReader ("TestData/Test.xml")) {
				var serializer = new XmlSerializer (typeof (TestXml));
				var xml = serializer.Deserialize (reader) as TestXml;

				result = "XML was deserialized." + Environment.NewLine
					   + "-----------------" + Environment.NewLine + Environment.NewLine
					   + "Title: " + xml.Title + Environment.NewLine
					   + "Description: " + xml.Description;
			}

			return result;
		}

		public static string WriteFile ()
		{
			var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var filename = Path.Combine (documents, "Write.txt");
			File.WriteAllText (filename, "Write this text into a file!");

			return "Text was written to a file." + Environment.NewLine
				 + "-----------------" + Environment.NewLine
				 + File.ReadAllText (filename);
		}

		public static string WriteJson ()
		{
			// Create a new record
			var account = new Account () {
				Email = "monkey@xamarin.com",
				Active = true,
				CreatedDate = new DateTime (2015, 5, 27, 0, 0, 0, DateTimeKind.Utc),
				Roles = new List<string> { "User", "Admin" }
			};

			// Serialize object
			var json = JsonConvert.SerializeObject (account, Formatting.Indented);

			// Save to file
			var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var filename = Path.Combine (documents, "account.json");
			File.WriteAllText (filename, json);

			return json;
		}

		public static string CreateDirectory ()
		{
			var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var directoryName = Path.Combine (documents, "NewDirectory");
			Directory.CreateDirectory (directoryName);

			return "A directory was created." + Environment.NewLine
				 + "-----------------" + Environment.NewLine
				 + directoryName;
		}
	}
}
