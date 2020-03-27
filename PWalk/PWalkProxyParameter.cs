namespace Echo.PWalkService
{
	/// <summary>
	/// プロパティ探索の代替引数
	/// </summary>
	/// <remarks>
	/// InvokeMarkedWith メソッドの parameters としてこれらを入力すると、
	/// 探索中の動的なインスタンスに置換されて渡されます。
	/// </remarks>
	public enum PWalkProxyParameter
	{
		/// <summary>
		/// 現在値
		/// </summary>
		Current,

		/// <summary>
		/// コンテキスト
		/// </summary>
		Context,

		/// <summary>
		/// 標識
		/// </summary>
		Mark,
	}
}
