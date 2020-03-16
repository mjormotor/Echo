namespace Echo
{
	/// <summary>
	///	読み取り専用の要素
	/// </summary>
	public interface IReadOnlyItemShell<out T>
	{
		/// <summary>
		/// 要素
		/// </summary>
		T Item { get; }
	}
}
