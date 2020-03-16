namespace Echo.Data
{
	/// <summary>
	/// マップ値格納
	/// </summary>
	public interface IKeyedValueShell<TKey, out TValue>
	{
		/// <summary>
		/// キー
		/// </summary>
		TKey Key { get; }

		/// <summary>
		/// 内包値
		/// </summary>
		TValue Core { get; }
	}
}
