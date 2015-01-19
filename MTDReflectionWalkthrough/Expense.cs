using System;
using MonoTouch.Dialog;

namespace MTDReflectionWalkthrough
{
	public enum Category
	{
		Travel,
		Lodging,
		Books
	}

	public class Expense
	{
		[Section("Expense Entry")]

		[Entry("Enter expense name")]
		public string Name;
		[Section("Expense Details")]

		[Caption("Description")]
		[Entry]
		public string Details;
		[Checkbox]
		public bool IsApproved = true;
		[Caption("Category")]
		public Category ExpenseCategory;
	}
}

