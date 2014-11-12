using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UIKit;

namespace FileSystem
{
	/// <summary>
	/// This class contains the code shown in the article "Working with the File System"
	/// </summary>
	public class SampleCode
	{
		public static void ReadText(UITextView display)
		{
			display.Text = "";

			// Sample code from the article
			var text = System.IO.File.ReadAllText("TestData/ReadMe.txt");
			Console.WriteLine(text);
			
			// Output to app UITextView
			display.Text = text;
		}
		
		public static void ReadDirectories(UITextView display)
		{
			display.Text = "";

			// Sample code from the article
			var directories = Directory.EnumerateDirectories("./");
			foreach (var directory in directories) {
				Console.WriteLine(directory);
			}
			
			// Output to app UITextView
			foreach (var directory in directories) {
				display.Text += directory + Environment.NewLine;
			}
		}

		public static void ReadAll(UITextView display)
		{
			display.Text = "";

			// Sample code from the article
			var fileOrDirectory = Directory.EnumerateFileSystemEntries("./");
			foreach (var entry in fileOrDirectory) {
				Console.WriteLine(entry);
			}
			
			// Output to app UITextView
			foreach (var entry in fileOrDirectory) {
				display.Text += entry + Environment.NewLine;
			}
		}

		public static void ReadXml(UITextView display)
		{
			display.Text = "";

			// Sample code from the article (doesn't output any values)
			using (TextReader reader = new StreamReader("TestData/Test.xml")) {
				XmlSerializer serializer = new XmlSerializer(typeof(TestXml));
				var xml = (TestXml)serializer.Deserialize(reader);
			}

			// Output to app UITextView
			using (TextReader reader = new StreamReader("TestData/Test.xml")) {
				XmlSerializer serializer = new XmlSerializer(typeof(TestXml));
				var xml = (TestXml)serializer.Deserialize(reader);

				display.Text = "XML was deserialized." + Environment.NewLine
						+ "-----------------" + Environment.NewLine
						+ "Title: " + xml.Title + Environment.NewLine
						+ "Description: " + xml.Description;
			}
		}

		public static void WriteFile(UITextView display)
		{
			// Sample code from the article
			var documents =
				Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var filename = Path.Combine (documents, "Write.txt");
			File.WriteAllText(filename, "Write this text into a file!");
			
			// Output to app UITextView
			display.Text = "Text was written to a file." + Environment.NewLine
						+ "-----------------" + Environment.NewLine
						+ System.IO.File.ReadAllText(filename);
		}

		public static void CreateDirectory(UITextView display)
		{
			// Sample code from the article
			var documents =
				Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var directoryname = Path.Combine (documents, "NewDirectory");
			Directory.CreateDirectory(directoryname);

			// Output to app UITextView
			display.Text = "A directory was created." + Environment.NewLine
						+ "-----------------" + Environment.NewLine
						+ directoryname;
		}
	}
}