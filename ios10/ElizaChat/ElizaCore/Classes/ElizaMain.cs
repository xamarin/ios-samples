// class translated from Java
// Credit goes to Charles Hayden http://www.chayden.net/eliza/Eliza.html

using System;

namespace ElizaCore {
	/// <summary>Eliza main class.</summary>
	/// <remarks>
	/// Eliza main class. Stores the processed script. Does the input
	/// transformations.
	/// </remarks>
	public class ElizaMain {
		internal readonly bool echoInput = false;

		internal readonly bool printData = false;

		internal readonly bool printKeys = false;

		internal readonly bool printSyns = false;

		internal readonly bool printPrePost = false;

		internal readonly bool printInitialFinal = false;

		/// <summary>The key list</summary>
		internal KeyList keys = new KeyList ();

		/// <summary>The syn list</summary>
		internal SynList syns = new SynList ();

		/// <summary>The pre list</summary>
		internal PrePostList pre = new PrePostList ();

		/// <summary>The post list</summary>
		internal PrePostList post = new PrePostList ();

		/// <summary>Initial string</summary>
		internal string initial = "Hello.";

		/// <summary>Final string</summary>
		internal string finl = "Goodbye.";

		/// <summary>Quit list</summary>
		internal WordList quit = new WordList ();

		/// <summary>Key stack</summary>
		internal KeyStack keyStack = new KeyStack ();

		/// <summary>Memory</summary>
		internal Mem mem = new Mem ();

		internal DecompList lastDecomp;

		internal ReasembList lastReasemb;

		internal bool finished = false;

		internal const int success = 0;

		internal const int failure = 1;

		internal const int gotoRule = 2;

		public ElizaMain () : this (new LineSourceFromAssembly ("ElizaScript.txt"))
		{

		}

		public ElizaMain (ILineSource linesource)
		{
			ReadScript (linesource);
		}

		public virtual bool IsFinished ()
		{
			return finished;
		}

		/// <summary>Process a line of script input.</summary>
		/// <remarks>Process a line of script input.</remarks>
		private void Collect (string s)
		{
			string [] lines = new string [4];
			if (EString.Match (s, "*reasmb: *", lines)) {
				if (lastReasemb == null) {
					ConsoleSurrogate.WriteLine ("Error: no last reasemb");
					return;
				}
				lastReasemb.Add (lines [1]);
			} else {
				if (EString.Match (s, "*decomp: *", lines)) {
					if (lastDecomp == null) {
						ConsoleSurrogate.WriteLine ("Error: no last decomp");
						return;
					}
					lastReasemb = new ReasembList ();
					string temp = lines [1];
					if (EString.Match (temp, "$ *", lines)) {
						lastDecomp.Add (lines [0], true, lastReasemb);
					} else {
						lastDecomp.Add (temp, false, lastReasemb);
					}
				} else {
					if (EString.Match (s, "*key: * #*", lines)) {
						lastDecomp = new DecompList ();
						lastReasemb = null;
						int n = 0;
						if (lines [2].Length != 0) {
							try {
								n = int.Parse (lines [2]);
							} catch (FormatException) {
								ConsoleSurrogate.WriteLine ("Number is wrong in key: " + lines [2]);
							}
						}
						keys.Add (lines [1], n, lastDecomp);
					} else {
						if (EString.Match (s, "*key: *", lines)) {
							lastDecomp = new DecompList ();
							lastReasemb = null;
							keys.Add (lines [1], 0, lastDecomp);
						} else {
							if (EString.Match (s, "*synon: * *", lines)) {
								WordList words = new WordList ();
								words.Add (lines [1]);
								s = lines [2];
								while (EString.Match (s, "* *", lines)) {
									words.Add (lines [0]);
									s = lines [1];
								}
								words.Add (s);
								syns.Add (words);
							} else {
								if (EString.Match (s, "*pre: * *", lines)) {
									pre.Add (lines [1], lines [2]);
								} else {
									if (EString.Match (s, "*post: * *", lines)) {
										post.Add (lines [1], lines [2]);
									} else {
										if (EString.Match (s, "*initial: *", lines)) {
											initial = lines [1];
										} else {
											if (EString.Match (s, "*final: *", lines)) {
												finl = lines [1];
											} else {
												if (EString.Match (s, "*quit: *", lines)) {
													quit.Add (" " + lines [1] + " ");
												} else {
													ConsoleSurrogate.WriteLine ("Unrecognized input: " + s);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		/// <summary>Process a line of input.</summary>
		/// <remarks>Process a line of input.</remarks>
		public virtual string ProcessInput (string s)
		{
			string reply;
			// Do some input transformations first.
			s = EString.Translate (s, "ABCDEFGHIJKLMNOPQRSTUVWXYZ", "abcdefghijklmnopqrstuvwxyz"
				);
			s = EString.Translate (s, "@#$%^&*()_-+=~`{[}]|:;<>\\\"", "                          "
				);
			s = EString.Translate (s, ",?!", "...");
			// Compress out multiple speace.
			s = EString.Compress (s);
			string [] lines = new string [2];
			// Break apart sentences, and do each separately.
			while (EString.Match (s, "*.*", lines)) {
				reply = Sentence (lines [0]);
				if (reply != null) {
					return reply;
				}
				s = EString.Trim (lines [1]);
			}
			if (s.Length != 0) {
				reply = Sentence (s);
				if (reply != null) {
					return reply;
				}
			}
			// Nothing matched, so try memory.
			string m = mem.Get ();
			if (m != null) {
				return m;
			}
			// No memory, reply with xnone.
			Key key = keys.GetKey ("xnone");
			if (key != null) {
				Key dummy = null;
				reply = Decompose (key, s, dummy);
				if (reply != null) {
					return reply;
				}
			}
			// No xnone, just say anything.
			return "I am at a loss for words.";
		}

		/// <summary>Process a sentence.</summary>
		/// <remarks>
		/// Process a sentence. (1) Make pre transformations. (2) Check for quit
		/// word. (3) Scan sentence for keys, build key stack. (4) Try decompositions
		/// for each key.
		/// </remarks>
		private string Sentence (string s)
		{
			s = pre.Translate (s);
			s = EString.Pad (s);
			if (quit.Find (s)) {
				finished = true;
				return finl;
			}
			keys.BuildKeyStack (keyStack, s);
			for (int i = 0; i < keyStack.KeyTop (); i++) {
				Key gotoKey = new Key ();
				string reply = Decompose (keyStack.Key (i), s, gotoKey);
				if (reply != null) {
					return reply;
				}
				// If decomposition returned gotoKey, try it
				while (gotoKey.GetKey () != null) {
					reply = Decompose (gotoKey, s, gotoKey);
					if (reply != null) {
						return reply;
					}
				}
			}
			return null;
		}

		/// <summary>Decompose a string according to the given key.</summary>
		/// <remarks>
		/// Decompose a string according to the given key. Try each decomposition
		/// rule in order. If it matches, assemble a reply and return it. If assembly
		/// fails, try another decomposition rule. If assembly is a goto rule, return
		/// null and give the key. If assembly succeeds, return the reply;
		/// </remarks>
		private string Decompose (Key key, string s, Key gotoKey)
		{
			string [] reply = new string [10];
			for (int i = 0; i < key.Decomp ().Count; i++) {
				Decomp d = (Decomp) key.Decomp () [i];
				string pat = d.Pattern ();
				if (syns.MatchDecomp (s, pat, reply)) {
					string rep = Assemble (d, reply, gotoKey);
					if (rep != null) {
						return rep;
					}
					if (gotoKey.GetKey () != null) {
						return null;
					}
				}
			}
			return null;
		}

		/// <summary>Assembly a reply from a decomp rule and the input.</summary>
		/// <remarks>
		/// Assembly a reply from a decomp rule and the input. If the reassembly rule
		/// is goto, return null and give the gotoKey to use. Otherwise return the
		/// response.
		/// </remarks>
		private string Assemble (Decomp d, string [] reply, Key gotoKey)
		{
			string [] lines = new string [3];
			d.StepRule ();
			string rule = d.NextRule ();
			if (EString.Match (rule, "goto *", lines)) {
				// goto rule -- set gotoKey and return false.
				gotoKey.Copy (keys.GetKey (lines [0]));
				if (gotoKey.GetKey () != null) {
					return null;
				}
				ConsoleSurrogate.WriteLine ("Goto rule did not match key: " + lines [0]);
				return null;
			}
			string work = string.Empty;
			while (EString.Match (rule, "* (#)*", lines)) {
				// reassembly rule with number substitution
				rule = lines [2];
				// there might be more
				int n = 0;
				try {
					n = int.Parse (lines [1]) - 1;
				} catch (FormatException) {
					ConsoleSurrogate.WriteLine ("Number is wrong in reassembly rule " + lines [1]);
				}
				if (n < 0 || n >= reply.Length) {
					ConsoleSurrogate.WriteLine ("Substitution number is bad " + lines [1]);
					return null;
				}
				reply [n] = post.Translate (reply [n]);
				work += lines [0] + " " + reply [n];
			}
			work += rule;
			if (d.Mem ()) {
				mem.Save (work);
				return null;
			}
			return work;
		}

		private int ReadScript (ILineSource linesource)
		{
			try {
				while (true) {
					string s;
					s = linesource.ReadLine ();
					if (s == null) {
						break;
					}
					Collect (s);
				}
			} catch (Exception) {
				ConsoleSurrogate.WriteLine ("There was a problem reading the script file.");
				ConsoleSurrogate.WriteLine ("Tried " + linesource.ToString ());
				return 1;
			}
			return 0;
		}
	}
}
