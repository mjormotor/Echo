namespace Echo
{
	/// <summary>
	///	読み取り専用の格納
	/// </summary>
	public interface IReadOnlyShell<out T>
	{
		/// <summary>
		/// 格納値
		/// </summary>
		T Core { get; }
	}
}
