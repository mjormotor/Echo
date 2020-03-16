namespace Echo.Data
{
	/// <summary>
	/// データプロパティ値コンテキスト
	/// </summary>
	public interface IReadOnlyDataPropertyValueContext<out T>
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
