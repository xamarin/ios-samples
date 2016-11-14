// class translated from Java
// Credit goes to Charles Hayden http://www.chayden.net/eliza/Eliza.html

namespace ElizaCore
{
	/// <summary>Eliza string functions.</summary>
	/// <remarks>Eliza string functions.</remarks>
	public class EString
	{
		/// <summary>The digits.</summary>
		/// <remarks>The digits.</remarks>
		internal static readonly string num = "0123456789";

		/// <summary>Look for a match between the string and the pattern.</summary>
		/// <remarks>
		/// Look for a match between the string and the pattern.
		/// Return count of maching characters before * or #.
		/// Return -1 if strings do not match.
		/// </remarks>
		public static int Amatch(string str, string pat)
		{
			int count = 0;
			int i = 0;
			// march through str
			int j = 0;
			// march through pat
			while (i < str.Length && j < pat.Length)
			{
				char p = pat[j];
				// stop if pattern is * or #
				if (p == '*' || p == '#')
				{
					return count;
				}
				if (str[i] != p)
				{
					return -1;
				}
				// they are still equal
				i++;
				j++;
				count++;
			}
			return count;
		}

		/// <summary>
		/// Search in successive positions of the string,
		/// looking for a match to the pattern.
		/// </summary>
		/// <remarks>
		/// Search in successive positions of the string,
		/// looking for a match to the pattern.
		/// Return the string position in str of the match,
		/// or -1 for no match.
		/// </remarks>
		public static int FindPat(string str, string pat)
		{
			int count = 0;
			for (int i = 0; i < str.Length; i++)
			{
				if (Amatch(str.Substring( i), pat) >= 0)
				{
					return count;
				}
				count++;
			}
			return -1;
		}

		/// <summary>Look for a number in the string.</summary>
		/// <remarks>
		/// Look for a number in the string.
		/// Return the number of digits at the beginning.
		/// </remarks>
		public static int FindNum(string str)
		{
			int count = 0;
			for (int i = 0; i < str.Length; i++)
			{
				if (num.IndexOf(str[i]) == -1)
				{
					return count;
				}
				count++;
			}
			return count;
		}

		/// <summary>
		/// Match the string against a pattern and fills in
		/// matches array with the pieces that matched * and #
		/// </summary>
		internal static bool MatchA(string str, string pat, string[] matches)
		{
			int i = 0;
			//  move through str
			int j = 0;
			//  move through matches
			int pos = 0;
			//  move through pat
			while (pos < pat.Length && j < matches.Length)
			{
				char p = pat[pos];
				if (p == '*')
				{
					int n;
					if (pos + 1 == pat.Length)
					{
						//  * is the last thing in pat
						//  n is remaining string length
						n = str.Length - i;
					}
					else
					{
						//  * is not last in pat
						//  find using remaining pat
						n = FindPat(str.Substring( i), pat.Substring( pos + 1));
					}
					if (n < 0)
					{
						return false;
					}
					matches[j++] = String.Sub(str, i, i + n);
					i += n;
					pos++;
				}
				else
				{
					if (p == '#')
					{
						int n = FindNum(str.Substring( i));
						matches[j++] = String.Sub(str, i, i + n);
						i += n;
						pos++;
					}
					else
					{
						int n = Amatch(str.Substring( i), pat.Substring( pos));
						if (n <= 0)
						{
							return false;
						}
						i += n;
						pos += n;
					}
				}
			}
			if (i >= str.Length && pos >= pat.Length)
			{
				return true;
			}
			return false;
		}

		internal static bool MatchB(string strIn, string patIn, string[] matches)
		{
			string str = strIn;
			string pat = patIn;
			int j = 0;
			//  move through matches
			while (pat.Length > 0 && str.Length >= 0 && j < matches.Length)
			{
				char p = pat[0];
				if (p == '*')
				{
					int n;
					if (pat.Length == 1)
					{
						//  * is the last thing in pat
						//  n is remaining string length
						n = str.Length;
					}
					else
					{
						//  * is not last in pat
						//  find using remaining pat
						n = FindPat(str, pat.Substring( 1));
					}
					if (n < 0)
					{
						return false;
					}
					matches[j++] = String.Sub(str, 0, n);
					str = str.Substring( n);
					pat = pat.Substring( 1);
				}
				else
				{
					if (p == '#')
					{
						int n = FindNum(str);
						matches[j++] = String.Sub(str, 0, n);
						str = str.Substring( n);
						pat = pat.Substring( 1);
					}
					else
					{
						//           } else if (p == ' ' && str.length() > 0 && str.charAt(0) != ' ') {
						//               pat = pat.substring(1);
						int n = Amatch(str, pat);
						if (n <= 0)
						{
							return false;
						}
						str = str.Substring( n);
						pat = pat.Substring( n);
					}
				}
			}
			if (str.Length == 0 && pat.Length == 0)
			{
				return true;
			}
			return false;
		}

		public static bool Match(string str, string pat, string[] matches)
		{
			return MatchA(str, pat, matches);
		}

		public static string Translate(string str, string src, string dest)
		{
			if (src.Length != dest.Length)
			{
			}
			// impossible error
			for (int i = 0; i < src.Length; i++)
			{
				str = str.Replace(src[i], dest[i]);
			}
			return str;
		}

		/// <summary>
		/// Compresses its input by:
		/// dropping space before space, comma, and period;
		/// adding space before question, if char before is not a space; and
		/// copying all others
		/// </summary>
		public static string Compress(string s)
		{
			string dest = string.Empty;
			if (s.Length == 0)
			{
				return s;
			}
			char c = s[0];
			for (int i = 1; i < s.Length; i++)
			{
				if (c == ' ' && ((s[i] == ' ') || (s[i] == ',') || (s[i] == '.')))
				{
				}
				else
				{
					// nothing
					if (c != ' ' && s[i] == '?')
					{
						dest += c + " ";
					}
					else
					{
						dest += c;
					}
				}
				c = s[i];
			}
			dest += c;
			return dest;
		}

		/// <summary>Trim off leading space</summary>
		public static string Trim(string s)
		{
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] != ' ')
				{
					return s.Substring( i);
				}
			}
			return string.Empty;
		}

		/// <summary>Pad by ensuring there are spaces before and after the sentence.</summary>
		/// <remarks>Pad by ensuring there are spaces before and after the sentence.</remarks>
		public static string Pad(string s)
		{
			if (s.Length == 0)
			{
				return " ";
			}
			char first = s[0];
			char last = s[s.Length - 1];
			if (first == ' ' && last == ' ')
			{
				return s;
			}
			if (first == ' ' && last != ' ')
			{
				return s + " ";
			}
			if (first != ' ' && last == ' ')
			{
				return " " + s;
			}
			if (first != ' ' && last != ' ')
			{
				return " " + s + " ";
			}
			// impossible
			return s;
		}

		/// <summary>Count number of occurrances of c in str</summary>
		public static int Count(string s, char c)
		{
			int count = 0;
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] == c)
				{
					count++;
				}
			}
			return count;
		}
	}
}
