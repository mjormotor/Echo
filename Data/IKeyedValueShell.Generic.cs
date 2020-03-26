namespace Echo.Data
{
	/// <summary>
	/// キー付き値格納
	/// </summary>
	public interface IKeyedValueShell<TKey, out TValue> : IReadOnlyShell<TValue>
	{
		/// <summary>
		/// キー
		/// </summary>
		TKey Key { get; }
	}
}
