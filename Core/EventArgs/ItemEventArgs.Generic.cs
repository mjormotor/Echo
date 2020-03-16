using System;

namespace Echo
{
	/// <summary>
	///	データを持つイベント引数
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("{Item}")]
	public class ItemEventArgs<T> : EventArgs, IReadOnlyItemShell<T>
	{
		public ItemEventArgs(T item)
		{
			Item = item;
		}

		#region IReadOnlyItemShell<T> interface support
		public T Item { get; }
		#endregion  // IReadOnlyItemShell<T> interface support
	}
}
