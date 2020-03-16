using System;
using System.Collections;
using System.Collections.Generic;

namespace Echo.Data
{
	/// <summary>
	/// データプロパティ配列
	/// </summary>
	public interface IDataPropertyCollection : IReadOnlyList<IDataProperty>, ICollection, IReadOnlyIndexer<int, IDataProperty>, IDataPropertyCore
	{
		/// <summary>
		/// プロパティ配列変更前
		/// </summary>
		event EventHandler<DataPropertyCollectionChangingEventArgs> DataPropertyCollectionChanging;

		/// <summary>
		/// プロパティ配列変更後
		/// </summary>
		event EventHandler<DataPropertyCollectionChangedEventArgs> DataPropertyCollectionChanged;

		/// <summary>
		/// 要素数
		/// </summary>
		new int Count { get; }

		/// <summary>
		/// 延長
		/// </summary>
		void Extend();

		/// <summary>
		/// 延長
		/// </summary>
		void Extend(int count);

		/// <summary>
		/// 短縮
		/// </summary>
		void Reduce();

		/// <summary>
		/// 短縮
		/// </summary>
		void Reduce(int count);
	}
}
