using System;
using System.Collections;
using System.Collections.Generic;

namespace Echo.Data
{
	/// <summary>
	/// データプロパティ辞書
	/// </summary>
	public interface IDataPropertyDictionary : IReadOnlyList<IReadOnlyShell<IDataProperty>>, IDictionary, IReadOnlyIndexer<int, IReadOnlyShell<IDataProperty>>, IDataPropertyCore
	{
		/// <summary>
		/// プロパティ辞書変更前
		/// </summary>
		event EventHandler<DataPropertyDictionaryChangingEventArgs> DataPropertyDictionaryChanging;

		/// <summary>
		/// プロパティ辞書変更後
		/// </summary>
		event EventHandler<DataPropertyDictionaryChangedEventArgs> DataPropertyDictionaryChanged;

		/// <summary>
		/// キー型
		/// </summary>
		Type KeyType { get; }

		/// <summary>
		/// 延長
		/// </summary>
		void Extend(object key);

		/// <summary>
		/// 短縮
		/// </summary>
		void Reduce(object key);
	}
}
