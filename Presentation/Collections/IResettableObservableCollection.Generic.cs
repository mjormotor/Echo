using System.Collections.Generic;

namespace Echo.Presentation
{
	/// <summary>
	/// 再利用可能な監視可能コレクション
	/// </summary>
	public interface IResettableObservableCollection<T> : IObservableCollection<T>
	{
		/// <summary>
		/// 要素の再設定
		/// </summary>
		/// <remarks>
		/// 既存の要素をすべて削除して、入力された要素を設定します。
		/// </remarks>
		void Reset(IEnumerable<T> collection);

		/// <summary>
		/// 要素の連続追加
		/// </summary>
		void AddRange(IEnumerable<T> collection);

		/// <summary>
		/// 要素の連続挿入
		/// </summary>
		void InsertRange(int index, IEnumerable<T> collection);

		/// <summary>
		/// 要素の連続削除
		/// </summary>
		void RemoveRange(int index, int count);
	}
}
