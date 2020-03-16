namespace Echo.Data
{
	/// <summary>
	/// 読み取り専用のデータプロパティ辞書変更操作
	/// </summary>
	public interface IReadOnlyDataPropertyDictionaryChangeOperation<out T>
	{
		/// <summary>
		/// 操作の種別
		/// </summary>
		DataPropertyCollectionChangeAction Action { get; }

		/// <summary>
		/// キー
		/// </summary>
		T Key { get; }

		/// <summary>
		/// 移動先のキー
		/// </summary>
		T MovingKey { get; }
	}
}
