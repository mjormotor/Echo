using System;

namespace Echo
{
	/// <summary>
	///	データを持つイベント引数
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("{Item}")]
	public class ItemEventArgs<T> : EventArgs
	{
		public ItemEventArgs(T item)
		{
			Item = item;
		}

		/// <summary>
		/// 要素
		/// </summary>
		public T Item { get; }
	}
}
