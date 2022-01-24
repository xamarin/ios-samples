namespace StoryboardTable;

/// <summary>
/// Represents a Chore.
/// </summary>
///
public class SecretItem {

	public SecretItem ()
	{
	}

	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Notes { get; set; } = string.Empty;
	public bool Done { get; set; }
}
