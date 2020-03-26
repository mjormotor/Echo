namespace Echo
{
	/// <summary>
	///	格納
	/// </summary>
	public interface IShell<T> : IReadOnlyShell<T>
	{
		/// <summary>
		/// 格納値
		/// </summary>
		new T Core { get; set; }
	}
}
