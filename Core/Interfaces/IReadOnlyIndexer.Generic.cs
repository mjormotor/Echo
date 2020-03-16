namespace Echo
{
	/// <summary>
	///	読み取り専用のインデクサ
	/// </summary>
	public interface IReadOnlyIndexer<in TKey, out TValue>
	{
		/// <summary>
		/// インデクサ
		/// </summary>
		TValue this[TKey key] { get; }
	}
}
