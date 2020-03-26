namespace Echo.Data
{
	/// <summary>
	/// 読み取り専用のデータプロパティ辞書変更操作
	/// </summary>
	public interface IReadOnlyDataPropertyDictionaryChangeOperation<out TKey, out TValue> : IReadOnlyDataPropertyCollectionChangeOperation
	{
		/// <summary>
		/// 設定値
		/// </summary>
		IReadOnlyDataPropertyDictionaryValueContext<TKey, TValue> SettingValue { get; }
	}
}
