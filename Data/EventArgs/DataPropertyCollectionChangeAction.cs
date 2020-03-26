namespace Echo.Data
{
	/// <summary>
	/// データプロパティ列変更操作種別
	/// </summary>
	public enum DataPropertyCollectionChangeAction
	{
		/// <summary>
		/// 設定
		/// </summary>
		Set,

		/// <summary>
		/// 挿入
		/// </summary>
		Insert,

		/// <summary>
		/// 削除
		/// </summary>
		Remove,

		/// <summary>
		/// 移動
		/// </summary>
		Move,

		/// <summary>
		/// 交換
		/// </summary>
		Swap,
	}
}
