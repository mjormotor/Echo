namespace Echo.Data
{
	/// <summary>
	/// 読み取り専用のデータプロパティ配列変更操作
	/// </summary>
	public interface IReadOnlyDataPropertyCollectionChangeOperation<out T> : IReadOnlyDataPropertyCollectionChangeOperation
	{
		/// <summary>
		/// 設定値
		/// </summary>
		IReadOnlyDataPropertyCollectionValueContext<T> SettingValue { get; }
	}
}
