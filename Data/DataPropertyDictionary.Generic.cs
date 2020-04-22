using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Echo.Data
{
	/// <summary>
	/// データプロパティ辞書
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[System.Diagnostics.DebuggerTypeProxy(typeof(DataPropertyDictionary<,>.DebugView))]
	public partial class DataPropertyDictionary<TKey, TValue> : DataPropertyDictionaryBase<TKey, TValue>, IReadOnlyDictionary<TKey, DataProperty<TValue>>, IReadOnlyIndexer<TKey, DataProperty<TValue>>
	{
		public DataPropertyDictionary([CallerMemberName] string name = null)
			: base(name)
		{
		}

		public DataPropertyDictionary(IEqualityComparer<TKey> comparer, [CallerMemberName] string name = null)
			: base(comparer, name)
		{
		}

		public DataPropertyDictionary(IEnumerable<KeyValuePair<TKey, TValue>> dictionary, [CallerMemberName] string name = null)
			: base(dictionary, name)
		{
		}

		public DataPropertyDictionary(IEnumerable<KeyValuePair<TKey, TValue>> dictionary, IEqualityComparer<TKey> comparer, [CallerMemberName] string name = null)
			: base(dictionary, comparer, name)
		{
		}

		/// <summary>
		/// キー列
		/// </summary>
		public new KeyCollection Keys => base.Keys;

		/// <summary>
		/// 要素列
		/// </summary>
		public new ItemCollection Items => base.Items;

		/// <summary>
		/// 値の追加
		/// </summary>
		public void Add(TKey key, TValue value) => InsertItem(Count, key, value);

		/// <summary>
		/// 値の連続追加
		/// </summary>
		public void AddRange(IEnumerable<TKey> keys, IEnumerable<TValue> values) => InsertItems(Count, keys, values);

		/// <summary>
		/// キーの変更
		/// </summary>
		public void ChangeKey(TKey oldKey, TKey newKey)
		{
			var index = Keys.IndexOf(oldKey);
			SetItem(index, newKey, Shells[index].Core.Value);
		}

		/// <summary>
		/// 値の消去
		/// </summary>
		public void Clear() => ClearItems();

		/// <summary>
		/// キーの包含判定
		/// </summary>
		public bool ContainsKey(TKey key) => Shells.Contains(key);

		/// <summary>
		/// 値の包含判定
		/// </summary>
		public void ContainsValue(TValue value) => Shells.Any(_ => Equals(_.Core.Value, value));

		/// <summary>
		/// 値のコピー
		/// </summary>
		public new void CopyTo(TValue[] array, int arrayIndex) => base.CopyTo(array, arrayIndex);

		/// <summary>
		/// 値のコピー
		/// </summary>
		public new void CopyTo(DataProperty<TValue>[] array, int arrayIndex) => base.CopyTo(array, arrayIndex);

		/// <summary>
		/// 辞書操作
		/// </summary>
		public DictionaryOperator GetDictionaryOperator() => new DictionaryOperator(this);

		/// <summary>
		/// 値の挿入
		/// </summary>
		public void Insert(int index, TKey key, TValue value) => InsertItem(index, key, value);

		/// <summary>
		/// 値の連続挿入
		/// </summary>
		public void InsertRange(int index, IEnumerable<TKey> keys, IEnumerable<TValue> values) => InsertItems(index, keys, values);

		/// <summary>
		/// 要素の移動
		/// </summary>
		public void Move(int oldIndex, int newIndex)
		{
			if (oldIndex < 0 || oldIndex >= Count)
			{
				throw new ArgumentOutOfRangeException(nameof(oldIndex), Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_OUT_OF_RANGE_NEGATIVE_NUMBER);
			}

			if (newIndex < 0 || newIndex >= Count)
			{
				throw new ArgumentOutOfRangeException(nameof(newIndex), Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_OUT_OF_RANGE_NEGATIVE_NUMBER);
			}

			MoveItem(oldIndex, newIndex);
		}

		/// <summary>
		/// キーの削除
		/// </summary>
		public bool Remove(TKey key) => RemoveItem(key);

		/// <summary>
		/// 番号の削除
		/// </summary>
		public void RemoveAt(int index) => RemoveItem(index);

		/// <summary>
		/// 番号の連続削除
		/// </summary>
		public void RemoveRange(int index, int count) => RemoveItems(index, count);

		/// <summary>
		/// 値の再設定
		/// </summary>
		public void ResetRange(IEnumerable<TKey> keys, IEnumerable<TValue> values)
		{
			ClearItems();
			InsertItems(0, keys, values);
		}

		/// <summary>
		/// キーの設定
		/// </summary>
		public void SetKey(TKey oldKey, TKey newKey)
		{
			var index = Keys.IndexOf(oldKey);
			SetItem(index, newKey, Shells[index].Core.Value);
		}

		/// <summary>
		/// 値の設定
		/// </summary>
		public void SetValue(int index, TValue value) => SetItem(index, Shells[index].Key, value);

		/// <summary>
		/// 値の設定
		/// </summary>
		public void SetValue(TKey key, TValue value) => SetItem(Keys.IndexOf(key), key, value);

		/// <summary>
		/// 値の連続設定
		/// </summary>
		public void SetRange(int index, IEnumerable<TKey> keys, IEnumerable<TValue> values) => SetItems(index, keys, values);

		/// <summary>
		/// 要素の交換
		/// </summary>
		public void Swap(int index0, int index1)
		{
			if (index0 < 0 || index0 >= Count)
			{
				throw new ArgumentOutOfRangeException(nameof(index0), Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_OUT_OF_RANGE_NEGATIVE_NUMBER);
			}

			if (index1 < 0 || index1 >= Count)
			{
				throw new ArgumentOutOfRangeException(nameof(index1), Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_OUT_OF_RANGE_NEGATIVE_NUMBER);
			}

			SwapItem(index0, index1);
		}

		/// <summary>
		/// 要素の取得の試行
		/// </summary>
		public new bool TryGetItem(TKey key, out DataProperty<TValue> item) => base.TryGetItem(key, out item);

		#region IReadOnlyDictionary<TKey, DataProperty<TValue>> interface support
		#region IReadOnlyCollection<KeyValuePair<TKey, DataProperty<TValue>>> interface support
		#region IEnumerable<KeyValuePair<TKey, DataProperty<TValue>>> interface support
		IEnumerator<KeyValuePair<TKey, DataProperty<TValue>>> IEnumerable<KeyValuePair<TKey, DataProperty<TValue>>>.GetEnumerator()
		{
			foreach (var shell in Shells)
			{
				yield return shell.KeyValuePair;
			}
		}
		#endregion  // IEnumerable<KeyValuePair<TKey, DataProperty<TValue>>> interface support
		#endregion  // IReadOnlyCollection<KeyValuePair<TKey, DataProperty<TValue>>> interface support

		public DataProperty<TValue> this[TKey key] => Shells[key].Core;

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		IEnumerable<TKey> IReadOnlyDictionary<TKey, DataProperty<TValue>>.Keys => Keys;

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		IEnumerable<DataProperty<TValue>> IReadOnlyDictionary<TKey, DataProperty<TValue>>.Values => Items;

		bool IReadOnlyDictionary<TKey, DataProperty<TValue>>.ContainsKey(TKey key) => Shells.Contains(key);

		bool IReadOnlyDictionary<TKey, DataProperty<TValue>>.TryGetValue(TKey key, out DataProperty<TValue> value) => TryGetItem(key, out value);
		#endregion  // IReadOnlyDictionary<TKey, DataProperty<TValue>> interface support

		#region private members
		#region class DebugView
		private class DebugView
		{
			public DebugView(DataPropertyDictionary<TKey, TValue> target)
			{
				this.target = target;
			}

			[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
			public DataPropertyShell<TKey, TValue>[] Items
			{
				get
				{
					var ret = new DataPropertyShell<TKey, TValue>[this.target.Count];
					this.target.Shells.CopyTo(ret, 0);
					return ret;
				}
			}

			#region private members
			private DataPropertyDictionary<TKey, TValue> target;
			#endregion // private members
		}
		#endregion // class DebugView
		#endregion // private members
	}
}
