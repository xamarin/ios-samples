// <copyright file="String.cs" company="takoyaki.ch">
// Copyright (c) 2013, All Right Reserved, http://takoyaki.ch
// </copyright>
// <author>Urs Keller</author>
// <email><urs@takoyaki.ch</email>
// <date>07/20/2013</date>
// <summary>Substring on C# and Java don't have the same semantics</summary>
//
using System;

namespace ElizaCore
{
	public class String
	{
		public String ()
		{
		}

		public static string Sub(string s, int beginIndex, int endIndex) {
			return s.Substring(beginIndex, endIndex - beginIndex);
		}
	}
}

