namespace Echo.Data
{
	/// <summary>
	/// データプロパティ所有
	/// </summary>
	[DataProperty]
	public interface IDataPropertyShell
	{
		/// <summary>
		/// データプロパティ
		/// </summary>
		IDataProperty Core { get; }
	}
}
