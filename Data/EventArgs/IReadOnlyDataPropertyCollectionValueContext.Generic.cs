namespace Echo.Data
{
	/// <summary>
	/// データプロパティ配列値コンテキスト
	/// </summary>
	public interface IReadOnlyDataPropertyCollectionValueContext<out T>
	{
		/// <summary>
		/// 変更後の値
		/// </summary>
		T NewValue { get; }

		/// <summary>
		/// 入力時の値
		/// </summary>
		T InputValue { get; }

		/// <summary>
		/// 変更前の値
		/// </summary>
		T OldValue { get; }
	}
}
