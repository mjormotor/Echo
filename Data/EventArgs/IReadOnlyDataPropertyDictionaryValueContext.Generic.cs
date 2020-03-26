namespace Echo.Data
{
	/// <summary>
	/// データプロパティ辞書値コンテキスト
	/// </summary>
	public interface IReadOnlyDataPropertyDictionaryValueContext<out TKey, out TValue> : IReadOnlyDataPropertyCollectionValueContext<TValue>
	{
		/// <summary>
		/// 変更後のキー
		/// </summary>
		TKey NewKey { get; }

		/// <summary>
		/// 入力時のキー
		/// </summary>
		TKey InputKey { get; }

		/// <summary>
		/// 変更前のキー
		/// </summary>
		TKey OldKey { get; }
	}
}
