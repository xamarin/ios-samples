namespace ElizaCore
{
	public interface ILineSource
	{
		string ReadLine();

		void Close();
	}
}
