namespace Echo.Data
{
	/// <summary>
	/// 読み取り専用のデータプロパティ配列変更操作
	/// </summary>
	public interface IReadOnlyDataPropertyCollectionChangeOperation
	{
		/// <summary>
		/// 操作の種別
		/// </summary>
		DataPropertyCollectionChangeAction Action { get; }

		/// <summary>
		/// 位置
		/// </summary>
		int Index { get; }

		/// <summary>
		/// 移動先の位置
		/// </summary>
		int MovingIndex { get; }
	}
}
