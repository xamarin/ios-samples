/*
Abstract:
Various JSON keys needed when making calls to the Apple Music API.
*/

using System;
namespace MusicKitSample.Utilities
{
	// Keys related to the `Response Root` JSON object in the Apple Music API.
	public struct ResponseRootJsonKeys
	{
		public static readonly string Data = "data";
		public static readonly string Results = "results";
	}

	// Keys related to the `Resource` JSON object in the Apple Music API.
	public struct ResourceJsonKeys
	{
		public static readonly string Id = "id";
		public static readonly string Attributes = "attributes";
		public static readonly string Type = "type";
	}

	// The various keys needed for parsing a JSON response from the Apple Music Web Service.
	public struct ResourceTypeJsonKeys
	{
		public static readonly string Songs = "songs";
		public static readonly string Albums = "albums";
	}
}
