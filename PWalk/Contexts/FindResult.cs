namespace Echo.PWalkService
{
	/// <summary>
	/// プロパティ探索結果
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class FindResult
	{
		public static bool operator true(FindResult value) { return value.IsSucceeded; }

		public static bool operator false(FindResult value) { return value.IsFailed; }

		public static bool operator !(FindResult value) { return value.IsFailed; }

		/// <summary>
		/// 成功判定
		/// </summary>
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		public bool IsSucceeded => this.isSucceeded;

		/// <summary>
		/// 失敗判定
		/// </summary>
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		public bool IsFailed => !IsSucceeded;

		/// <summary>
		/// コンテキスト
		/// </summary>
		public PWalkContext Context { get; }

		#region internal members
		internal FindResult(bool isSucceeded, PWalkContext context)
		{
			this.isSucceeded = isSucceeded;
			Context = context;
		}
		#endregion // internal members

		#region private members
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private bool isSucceeded;

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private string DebuggerDisplay => this.isSucceeded ? "Succeeded" : "Failed";
		#endregion // private members
	}
}
