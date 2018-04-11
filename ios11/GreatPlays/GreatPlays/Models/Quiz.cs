using System;
using System.Collections.Generic;
using System.Linq;

namespace GreatPlays {
	public partial class Quiz {

		public Scene Scene { get; set; }
		public string Title { get; set; }
		public List<Question> Questions { get; set; }
		public int QuestionIndex { get; set; } = 0;

		public Quiz (string title, List<Question> questions, Scene scene)
		{
			Title = title;
			Questions = questions;
			Scene = scene;
		}

		public void Reset ()
		{
			Questions.ForEach (q => q.Reset ());
			QuestionIndex = 0;
		}

		public int CorrectCount => Questions.Count (q => q.IsCorrect);
		public double Score => (double) CorrectCount / (double) Questions.Count;
		public int Hints => Questions.Sum (q => q.Hints);
		public void Start () => this.BeginActivity (true);
		public Question CurrentQuestion => Questions [QuestionIndex];
		public bool IsOver => QuestionIndex > Questions.Count - 1;

		public void SetAnswer (int index)
		{
			CurrentQuestion.ResponseIndex = index;
			QuestionIndex += 1;

			this.Update ((double) QuestionIndex / (double) Questions.Count);
		}

		public void Record ()
		{
			this.AddScore (Score, "Score", true);
			this.AddQuantity (Hints, "Hints");
			this.EndActivity ();
		}
	}
}