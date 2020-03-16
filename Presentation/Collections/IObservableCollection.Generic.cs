using System.Collections;
using System.Collections.Generic;

namespace Echo.Presentation
{
	/// <summary>
	/// 監視可能コレクション
	/// </summary>
	public interface IObservableCollection<T> : IList<T>, IList, IIndexer<int, T>, IReadOnlyObservableCollection<T>
	{
		/// <summary>
		/// インデクサ
		/// </summary>
		new T this[int index] { get; set; }

		/// <summary>
		/// 要素数
		/// </summary>
		new int Count { get; }

		/// <summary>
		/// 要素の移動
		/// </summary>
		void Move(int oldIndex, int newIndex);
	}
}
