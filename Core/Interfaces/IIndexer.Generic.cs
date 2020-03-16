namespace Echo
{
	/// <summary>
	///	インデクサ
	/// </summary>
	public interface IIndexer<in TKey, TValue> : IReadOnlyIndexer<TKey, TValue>
	{
		/// <summary>
		/// インデクサ
		/// </summary>
		new TValue this[TKey key] { get; set; }
	}
}
