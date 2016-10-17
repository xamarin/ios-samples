// <copyright file="ConsoleSurrogate.cs" company="takoyaki.ch">
// Copyright (c) 2013, All Right Reserved, http://takoyaki.ch
// </copyright>
// <author>Urs Keller</author>
// <email><urs@takoyaki.ch</email>
// <date>07/20/2013</date>
// <summary></summary>
//
using System;

namespace ElizaCore
{
	public static class ConsoleSurrogate
	{
		public static void WriteLine() {
			Console.WriteLine ("\n");
		}

		public static void WriteLine(string s) {
			Console.WriteLine (s);
		}

		public static void Write(string s) {
			Console.Write (s);
		}


	}
}

