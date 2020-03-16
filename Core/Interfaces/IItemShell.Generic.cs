namespace Echo
{
	/// <summary>
	///	要素
	/// </summary>
	public interface IItemShell<T> : IReadOnlyItemShell<T>
	{
		/// <summary>
		/// 要素
		/// </summary>
		new T Item { get; set; }
	}
}
