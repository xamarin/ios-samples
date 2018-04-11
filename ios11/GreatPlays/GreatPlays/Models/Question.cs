using System;
using System.Collections.Generic;

namespace GreatPlays {
	public class Question {

		public string Text { get; set; }
		public List<string> Answers { get; set; }
		public int CorrectAnswerIndex { get; set; }

		public int? ResponseIndex { get; set; }
		public int Hints { get; set; }

		public Question (string text, List<string> answers, int correctAnswerIndex)
		{
			Text = text;
			Answers = answers;
			CorrectAnswerIndex = correctAnswerIndex;
		}

		public void Reset ()
		{
			ResponseIndex = null;
			Hints = 0;
		}

		public bool IsCorrect => CorrectAnswerIndex == ResponseIndex;
		public string Response => ResponseIndex != null && ResponseIndex.HasValue ? Answers [ResponseIndex.Value] : null;

	}
}