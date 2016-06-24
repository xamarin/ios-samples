using System;
using System.Collections.Generic;

using UIKit;
using Foundation;
using CoreLocation;
using System.Threading.Tasks;

namespace CloudKitAtlas
{
	public enum Change
	{
		Modified,
		Deleted,
		Added
	}

	public class Attribute
	{
		public string Key { get; set; }
		public string Value { get; set; }
		public bool IsNested { get; set; } = false;

		public UIImage Image { get; set; }

		public Attribute (string key)
		{
			Key = key;
		}

		public Attribute (string key, string value)
			: this (key)
		{
			Value = value;
		}

		public Attribute (string key, string value, bool isNested)
			: this (key, value)
		{
			IsNested = isNested;
		}

		public Attribute (string key, string value, UIImage image)
			: this (key, value)
		{
			Image = image;
		}
	}

	public class AttributeGroup
	{
		public string Title { get; }

		public List<Attribute> Attributes { get; set; } = new List<Attribute> ();

		public AttributeGroup (string title)
		{
			Title = title;
		}

		public AttributeGroup (string title, IEnumerable<Attribute> attributes)
			: this (title)
		{
			Attributes.AddRange (attributes);
		}
	}

	public class Input
	{
		public string Label { get; }

		public virtual bool IsValid {
			get {
				return false;
			}
		}

		public bool IsRequired { get; protected set; } = false;

		public bool IsHidden { get; set; } = false;

		public List<int> ToggleIndexes { get; } = new List<int> ();

		public Input (string label)
		{
			Label = label;

		}

		public Input (string label, bool isRequired)
			: this (label)
		{
			IsRequired = isRequired;
		}

		public Input (string label, int [] toggleIndexes)
			: this (label)
		{
			ToggleIndexes.AddRange (toggleIndexes);
		}
	}

	public class SelectionInput : Input
	{
		public List<Input> Items { get; set; } = new List<Input> ();

		public override bool IsValid {
			get {
				return true;
			}
		}

		public int? Value { get; set; }

		public SelectionInput (string label, Input [] items)
			: base (label)
		{
			Items.AddRange (items);

			if (Items.Count > 0)
				Value = 0;
		}
	}

	public enum TextInputType
	{
		Text,
		Email
	}

	public class LocationInput : Input
	{
		public int? Longitude { get; set; }
		public int? Latitude { get; set; }

		public override bool IsValid {
			get {
				if (!Latitude.HasValue)
					return !Longitude.HasValue && !IsRequired;
				if (!Longitude.HasValue)
					return false;

				var lat = Latitude.Value;
				var lon = Longitude.Value;

				return -90 <= lat && lat <= 90 && -180 <= lon && lon <= 180;
			}
		}

		public LocationInput (string label, bool isRequired)
			: base (label, isRequired)
		{
		}
	}

	public class ImageInput : Input
	{
		public NSUrl Value { get; set; }

		public override bool IsValid {
			get {
				return !IsRequired || Value != null;
			}
		}

		public ImageInput (string label)
			: base (label)
		{
		}
	}

	public class BooleanInput : Input
	{
		public bool Value { get; set; }

		public override bool IsValid {
			get {
				return true;
			}
		}

		public BooleanInput (string label, bool value)
				: base (label)
		{
			Value = value;
		}

		public BooleanInput (string label, bool value, bool isHidden)
			: base (label, value)
		{
			IsHidden = isHidden;
		}
	}

	public class TextInput : Input
	{
		public TextInputType Type { get; private set; } = TextInputType.Text;

		public string Value { get; set; } = string.Empty;

		public override bool IsValid {
			get {
				return !IsRequired || !string.IsNullOrWhiteSpace (Value);
			}
		}

		public TextInput (string label, string value)
				: base (label)
		{
			Value = value;
		}

		public TextInput (string label, string value, bool isRequired, bool isHidden = false)
			: this (label, value)
		{
			IsHidden = isHidden;
			IsRequired = isRequired;
		}

		public TextInput (string label, string value, bool isRequired, TextInputType type)
				: this (label, value, isRequired)
		{
			Type = type;
		}
	}

	public interface IResult
	{
		string SummaryField { get; }
		List<AttributeGroup> AttributeList { get; }
	}

	public class NoResults : IResult
	{
		public string SummaryField {
			get {
				return null;
			}
		}

		List<AttributeGroup> attributeList;
		public List<AttributeGroup> AttributeList {
			get {
				return attributeList = attributeList ?? new List<AttributeGroup> { new AttributeGroup ("No Result") };
			}
		}
	}

	public class Results
	{
		public List<IResult> Items { get; } = new List<IResult> ();

		public bool AlwaysShowAsList { get; }

		public bool MoreComing { get; private set; } = false;

		public HashSet<int> Added { get; } = new HashSet<int> ();
		public HashSet<int> Modified { get; } = new HashSet<int> ();
		public HashSet<int> Deleted { get; } = new HashSet<int> ();

		public Results (IResult [] items = null, bool alwaysShowAsList = false)
		{
			if (items != null)
				Items.AddRange (items);

			AlwaysShowAsList = alwaysShowAsList;
		}

		public bool ShowAsList {
			get {
				return Items.Count > 1 || AlwaysShowAsList;
			}
		}

		public void Reset ()
		{
			MoreComing = false;

			Items.Clear ();
			Added.Clear ();
			Modified.Clear ();
			Deleted.Clear ();
		}
	}

	public abstract class CodeSample
	{
		public string Title { get; }
		public string ClassName { get; }
		public string MethodName { get; }
		public string Description { get; }

		public List<Input> Inputs { get; } = new List<Input> ();

		public string ListHeading { get; set; }

		public string Error { get; private set; }

		// TODO: remove default ctor
		public CodeSample ()
		{
		}

		public CodeSample (string title, string className, string methodName, string descriptionKey, Input [] inputs = null)
		{
			Title = title;
			ClassName = className;
			MethodName = methodName;
			Description = "Code Sample Description";

			if (inputs != null)
				Inputs.AddRange (inputs);
		}

		// TODO: create safe type container instead of Data
		public Dictionary<string, object> Data {
			get {
				var data = new Dictionary<string, object> ();
				foreach (var input in Inputs) {
					var textInput = input as TextInput;
					if (textInput != null)
						data [textInput.Label] = textInput.Value;

					var location = input as LocationInput;
					if (location != null && location.Latitude.HasValue && location.Longitude.HasValue)
						data [location.Label] = new CLLocation (location.Latitude.Value, location.Longitude.Value);

					var image = input as ImageInput;
					if (image != null && image.Value != null)
						data [image.Label] = image.Value;

					var boolean = input as BooleanInput;
					if (boolean != null)
						data [boolean.Label] = boolean.Value;

					var selection = input as SelectionInput;
					if (selection != null && selection.Value.HasValue)
						data [selection.Label] = selection.Items [selection.Value.Value].Label;
				}
				return data;
			}
		}

		protected bool TryGetString (string key, out string value)
		{
			return TryGet (key, out value);
		}

		protected bool TryGetLocation (string key, out CLLocation location)
		{
			return TryGet (key, out location);
		}

		protected bool TryGetUrl (string key, out NSUrl url)
		{
			return TryGet (key, out url);
		}

		bool TryGet<T> (string key, out T value) where T : class
		{
			object obj;
			Data.TryGetValue (key, out obj);
			value = obj as T;

			return value != null;
		}


		public abstract Task<Results> Run ();
	}
}