using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Echo.Data
{
	/// <summary>
	/// データプロパティ辞書
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[System.Diagnostics.DebuggerTypeProxy(typeof(DataPropertyDictionary<,>.DebugView))]
	public class DataPropertyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, DataProperty<TValue>>, IReadOnlyCollection<DataPropertyShell<TKey, TValue>>, IReadOnlyIndexer<TKey, DataProperty<TValue>>, IDataPropertyDictionary
	{
		#region class KeyCollection
		[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
		[System.Diagnostics.DebuggerTypeProxy(typeof(DataPropertyDictionary<,>.KeyCollection.DebugView))]
		[Serializable]
		public sealed class KeyCollection : ICollection<TKey>, ICollection, IReadOnlyCollection<TKey>
		{
			public KeyCollection(DataPropertyDictionary<TKey, TValue> owner)
			{
				if (owner == null)
				{
					throw new ArgumentNullException(nameof(owner));
				}

				this.owner = owner;
			}

			#region ICollection<TKey> interface support
			#region IEnumerable<TKey> interface support
			#region IEnumerable interface support
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
			#endregion  // IEnumerable interface support

			public IEnumerator<TKey> GetEnumerator()
			{
				foreach (var shell in this.owner.Shells)
				{
					yield return shell.Key;
				}
			}
			#endregion  // IEnumerable<TKey> interface support

			public int Count => this.owner.Count;

			bool ICollection<TKey>.IsReadOnly => true;

			void ICollection<TKey>.Add(TKey item)
			{
				throw new NotSupportedException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_NOT_SUPPORTED_MUTATE_ITEM_COLLECTION_FORMAT, nameof(KeyCollection), nameof(DataPropertyDictionary<TKey, TValue>)));
			}

			void ICollection<TKey>.Clear()
			{
				throw new NotSupportedException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_NOT_SUPPORTED_MUTATE_ITEM_COLLECTION_FORMAT, nameof(KeyCollection), nameof(DataPropertyDictionary<TKey, TValue>)));
			}

			bool ICollection<TKey>.Contains(TKey item)
			{
				return this.owner.ContainsKey(item);
			}

			public void CopyTo(TKey[] array, int arrayIndex)
			{
				if (array == null)
				{
					throw new ArgumentNullException(nameof(array));
				}

				if (arrayIndex < 0 || arrayIndex > array.Length)
				{
					throw new ArgumentOutOfRangeException(nameof(arrayIndex), Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_OUT_OF_RANGE_NEGATIVE_NUMBER);
				}

				if (array.Length - arrayIndex < this.owner.Count)
				{
					throw new ArgumentException(Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_SHORT_LENGTH_ARRAY);
				}

				foreach (var key in this)
				{
					array[arrayIndex++] = key;
				}
			}

			bool ICollection<TKey>.Remove(TKey item)
			{
				throw new NotSupportedException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_NOT_SUPPORTED_MUTATE_ITEM_COLLECTION_FORMAT, nameof(KeyCollection), nameof(DataPropertyDictionary<TKey, TValue>)));
			}
			#endregion  // ICollection<TKey> interface support

			#region ICollection interface support
			bool ICollection.IsSynchronized => false;

			object ICollection.SyncRoot => ((ICollection)this.owner).SyncRoot;

			void ICollection.CopyTo(Array array, int arrayIndex)
			{
				if (array == null)
				{
					throw new ArgumentNullException(nameof(array));
				}

				if (array.Rank != 1)
				{
					throw new ArgumentException(Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_MULTI_DIMENSION_ARRAY);
				}

				if (array.GetLowerBound(0) != 0)
				{
					throw new ArgumentException(Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_NON_ZERO_LOWER_BOUND_OF_ARRAY);
				}

				if (arrayIndex < 0 || arrayIndex > array.Length)
				{
					throw new ArgumentOutOfRangeException(nameof(arrayIndex), Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_OUT_OF_RANGE_NEGATIVE_NUMBER);
				}

				if (array.Length - arrayIndex < this.owner.Count)
				{
					throw new ArgumentException(Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_SHORT_LENGTH_ARRAY);
				}

				if (array is TKey[] keys)
				{
					CopyTo(keys, arrayIndex);
				}
				else
				{
					var objects = array as object[];
					if (objects == null)
					{
						throw new ArgumentException(Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_INVALID_TYPED_ARRAY);
					}

					try
					{
						foreach (var key in this)
						{
							objects[arrayIndex++] = key;
						}
					}
					catch (ArrayTypeMismatchException)
					{
						throw new ArgumentException(Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_INVALID_TYPED_ARRAY);
					}
				}
			}
			#endregion  // ICollection interface support

			#region private members
			#region class DebugView
			private class DebugView
			{
				public DebugView(KeyCollection target)
				{
					this.target = target;
				}

				[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
				public TKey[] Items
				{
					get
					{
						var ret = new TKey[this.target.Count];
						this.target.CopyTo(ret, 0);
						return ret;
					}
				}

				#region private members
				private KeyCollection target;
				#endregion // private members
			}
			#endregion // class DebugView

			private DataPropertyDictionary<TKey, TValue> owner;
			#endregion // private members
		}
		#endregion // class KeyCollection

		#region class ItemCollection
		[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
		[System.Diagnostics.DebuggerTypeProxy(typeof(DataPropertyDictionary<,>.ItemCollection.DebugView))]
		[Serializable]
		public sealed class ItemCollection : ICollection<DataProperty<TValue>>, ICollection, IReadOnlyCollection<DataProperty<TValue>>
		{
			public ItemCollection(DataPropertyDictionary<TKey, TValue> owner)
			{
				if (owner == null)
				{
					throw new ArgumentNullException(nameof(owner));
				}

				this.owner = owner;
			}

			#region ICollection<DataProperty<TValue>> interface support
			#region IEnumerable<DataProperty<TValue>> interface support
			#region IEnumerable interface support
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
			#endregion  // IEnumerable interface support

			public IEnumerator<DataProperty<TValue>> GetEnumerator()
			{
				foreach (var shell in this.owner.Shells)
				{
					yield return shell.Core;
				}
			}
			#endregion  // IEnumerable<DataProperty<TValue>> interface support

			public int Count => this.owner.Count;

			bool ICollection<DataProperty<TValue>>.IsReadOnly => true;

			void ICollection<DataProperty<TValue>>.Add(DataProperty<TValue> item)
			{
				throw new NotSupportedException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_NOT_SUPPORTED_MUTATE_ITEM_COLLECTION_FORMAT, nameof(ItemCollection), nameof(DataPropertyDictionary<TKey, TValue>)));
			}

			void ICollection<DataProperty<TValue>>.Clear()
			{
				throw new NotSupportedException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_NOT_SUPPORTED_MUTATE_ITEM_COLLECTION_FORMAT, nameof(ItemCollection), nameof(DataPropertyDictionary<TKey, TValue>)));
			}

			bool ICollection<DataProperty<TValue>>.Contains(DataProperty<TValue> item)
			{
				return this.owner.Shells.Any(_ => _.Core == item);
			}

			public void CopyTo(DataProperty<TValue>[] array, int arrayIndex)
			{
				if (array == null)
				{
					throw new ArgumentNullException(nameof(array));
				}

				if (arrayIndex < 0 || arrayIndex > array.Length)
				{
					throw new ArgumentOutOfRangeException(nameof(arrayIndex), Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_OUT_OF_RANGE_NEGATIVE_NUMBER);
				}

				if (array.Length - arrayIndex < this.owner.Count)
				{
					throw new ArgumentException(Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_SHORT_LENGTH_ARRAY);
				}

				foreach (var item in this)
				{
					array[arrayIndex++] = item;
				}
			}

			bool ICollection<DataProperty<TValue>>.Remove(DataProperty<TValue> item)
			{
				throw new NotSupportedException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_NOT_SUPPORTED_MUTATE_ITEM_COLLECTION_FORMAT, nameof(ItemCollection), nameof(DataPropertyDictionary<TKey, TValue>)));
			}
			#endregion  // ICollection<DataProperty<TValue>> interface support

			#region ICollection interface support
			bool ICollection.IsSynchronized => false;

			object ICollection.SyncRoot => ((ICollection)this.owner).SyncRoot;

			void ICollection.CopyTo(Array array, int arrayIndex)
			{
				if (array == null)
				{
					throw new ArgumentNullException(nameof(array));
				}

				if (array.Rank != 1)
				{
					throw new ArgumentException(Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_MULTI_DIMENSION_ARRAY);
				}

				if (array.GetLowerBound(0) != 0)
				{
					throw new ArgumentException(Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_NON_ZERO_LOWER_BOUND_OF_ARRAY);
				}

				if (arrayIndex < 0 || arrayIndex > array.Length)
				{
					throw new ArgumentOutOfRangeException(nameof(arrayIndex), Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_OUT_OF_RANGE_NEGATIVE_NUMBER);
				}

				if (array.Length - arrayIndex < this.owner.Count)
				{
					throw new ArgumentException(Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_SHORT_LENGTH_ARRAY);
				}

				var values = array as DataProperty<TValue>[];
				if (values != null)
				{
					CopyTo(values, arrayIndex);
				}
				else
				{
					var objects = array as object[];
					if (objects == null)
					{
						throw new ArgumentException(Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_INVALID_TYPED_ARRAY);
					}

					try
					{
						foreach (var item in this)
						{
							objects[arrayIndex++] = item;
						}
					}
					catch (ArrayTypeMismatchException)
					{
						throw new ArgumentException(Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_INVALID_TYPED_ARRAY);
					}
				}
			}
			#endregion  // ICollection interface support

			#region private members
			#region class DebugView
			private class DebugView
			{
				public DebugView(ItemCollection target)
				{
					this.target = target;
				}

				[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
				public DataProperty<TValue>[] Items
				{
					get
					{
						var ret = new DataProperty<TValue>[this.target.Count];
						this.target.CopyTo(ret, 0);
						return ret;
					}
				}

				#region private members
				private ItemCollection target;
				#endregion // private members
			}
			#endregion // class DebugView

			private DataPropertyDictionary<TKey, TValue> owner;
			#endregion // private members
		}
		#endregion // class ItemCollection

		public DataPropertyDictionary([CallerMemberName] string name = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name));
			}

			Name = name;
		}

		public DataPropertyDictionary(IEqualityComparer<TKey> comparer, [CallerMemberName] string name = null)
			: this(name)
		{
			Comparer = comparer ?? EqualityComparer<TKey>.Default;
		}

		public DataPropertyDictionary(IEnumerable<KeyValuePair<TKey, TValue>> dictionary, [CallerMemberName] string name = null)
			: this(name)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}

			foreach (var item in dictionary)
			{
				Add(item.Key, item.Value);
			}
		}

		public DataPropertyDictionary(IEnumerable<KeyValuePair<TKey, TValue>> dictionary, IEqualityComparer<TKey> comparer, [CallerMemberName] string name = null)
			: this(dictionary, name)
		{
			Comparer = comparer ?? EqualityComparer<TKey>.Default;
		}

		/// <summary>
		/// プロパティ配列変更前
		/// </summary>
		public event EventHandler<DataPropertyDictionaryChangingEventArgs<TKey, TValue>> DataPropertyDictionaryChanging;

		/// <summary>
		/// プロパティ配列変更後
		/// </summary>
		public event EventHandler<DataPropertyDictionaryChangedEventArgs<TKey, TValue>> DataPropertyDictionaryChanged;

		/// <summary>
		/// キー列
		/// </summary>
		public KeyCollection Keys => PrepareKeys();

		/// <summary>
		/// 要素列
		/// </summary>
		public ItemCollection Items => PrepareItems();

		/// <summary>
		/// 比較方式
		/// </summary>
		public IEqualityComparer<TKey> Comparer { get; }

		/// <summary>
		/// 値の追加
		/// </summary>
		public void Add(TKey key, TValue value) => InsertItem(Count, key, value);

		/// <summary>
		/// 値の連続追加
		/// </summary>
		public void AddRange(IEnumerable<TKey> keys, IEnumerable<TValue> values) => InsertItems(Count, keys, values);

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
		public void CopyTo(TValue[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException(nameof(array));
			}

			if (arrayIndex < 0 || arrayIndex > array.Length)
			{
				throw new ArgumentOutOfRangeException(nameof(arrayIndex), Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_OUT_OF_RANGE_NEGATIVE_NUMBER);
			}

			if (array.Length - arrayIndex < Count)
			{
				throw new ArgumentException(Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_SHORT_LENGTH_ARRAY);
			}

			foreach (var shell in Shells)
			{
				array[arrayIndex++] = shell.Core.Value;
			}
		}

		/// <summary>
		/// 値のコピー
		/// </summary>
		public void CopyTo(DataProperty<TValue>[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException(nameof(array));
			}

			if (arrayIndex < 0 || arrayIndex > array.Length)
			{
				throw new ArgumentOutOfRangeException(nameof(arrayIndex), Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_OUT_OF_RANGE_NEGATIVE_NUMBER);
			}

			if (array.Length - arrayIndex < Count)
			{
				throw new ArgumentException(Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_SHORT_LENGTH_ARRAY);
			}

			foreach (var shell in Shells)
			{
				array[arrayIndex++] = shell.Core;
			}
		}

		/// <summary>
		/// 挿入操作の作成
		/// </summary>
		public DataPropertyDictionaryChangeOperation<TKey, TValue> GenerateInsertOperation(int index, TKey key, TValue value)
		{
			return DataPropertyDictionaryChangeOperation<TKey, TValue>.Insert(index, key, new DataProperty<TValue>(ItemPropertyName) { Value = value, });
		}

		/// <summary>
		/// 移動操作の作成
		/// </summary>
		public DataPropertyDictionaryChangeOperation<TKey, TValue> GenerateMoveOperation(int oldIndex, TKey oldKey, int newIndex, TKey newKey)
		{
			return DataPropertyDictionaryChangeOperation<TKey, TValue>.Move(oldIndex, oldKey, newIndex, newKey);
		}

		/// <summary>
		/// 削除操作の作成
		/// </summary>
		public DataPropertyDictionaryChangeOperation<TKey, TValue> GenerateRemoveOperation(int index, TKey key)
		{
			return DataPropertyDictionaryChangeOperation<TKey, TValue>.Remove(index, key);
		}

		/// <summary>
		/// 交換操作の作成
		/// </summary>
		public DataPropertyDictionaryChangeOperation<TKey, TValue> GenerateSwapOperation(int index0, TKey key0, int index1, TKey key1)
		{
			return DataPropertyDictionaryChangeOperation<TKey, TValue>.Swap(index0, key0, index1, key1);
		}

		/// <summary>
		/// キーの番号の検索
		/// </summary>
		public int IndexOf(TKey key)
		{
			for (var index = 0; index < Count; ++index)
			{
				var shell = Shells[index];
				if (Equals(shell.Key, key))
				{
					return index;
				}
			}

			return -1;
		}

		/// <summary>
		/// 値の番号の検索
		/// </summary>
		public int IndexOf(TValue value)
		{
			for (var index = 0; index < Count; ++index)
			{
				var shell = Shells[index];
				if (Equals(shell.Core.Value, value))
				{
					return index;
				}
			}

			return -1;
		}

		/// <summary>
		/// 値の挿入
		/// </summary>
		public void Insert(int index, TKey key, TValue value) => InsertItem(index, key, value);

		/// <summary>
		/// 値の連続挿入
		/// </summary>
		public void InsertRange(int index, IEnumerable<TKey> keys, IEnumerable<TValue> values) => InsertItems(index, keys, values);

		/// <summary>
		/// キーの削除
		/// </summary>
		public bool Remove(TKey key)
		{
			var index = IndexOf(key);
			if (index == -1)
			{
				return false;
			}

			RemoveItem(index);
			return true;
		}

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
		public void ResetRange(IEnumerable<TKey> keys, IEnumerable<TValue> values) => ResetItems(keys, values);

		/// <summary>
		/// 要素の取得の試行
		/// </summary>
		public bool TryGetItem(TKey key, out DataProperty<TValue> item)
		{
			var ret = false;
			if (Shells.TryGetValue(key, out var shell))
			{
				item = shell.Core;
				ret = true;
			}
			else
			{
				item = null;
			}

			return ret;
		}

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

		public int Count => Shells.Count;
		#endregion  // IReadOnlyCollection<KeyValuePair<TKey, DataProperty<TValue>>> interface support

		public DataProperty<TValue> this[TKey key] => Shells[key].Core;

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		IEnumerable<TKey> IReadOnlyDictionary<TKey, DataProperty<TValue>>.Keys => Keys;

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		IEnumerable<DataProperty<TValue>> IReadOnlyDictionary<TKey, DataProperty<TValue>>.Values => Items;

		bool IReadOnlyDictionary<TKey, DataProperty<TValue>>.TryGetValue(TKey key, out DataProperty<TValue> value) => TryGetItem(key, out value);
		#endregion  // IReadOnlyDictionary<TKey, DataProperty<TValue>> interface support

		#region IReadOnlyCollection<DataPropertyShell<TKey, TValue>> interface support
		#region IEnumerable<DataPropertyShell<TKey, TValue>> interface support
		#region IEnumerable interface support
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		#endregion  // IEnumerable interface support

		public IEnumerator<DataPropertyShell<TKey, TValue>> GetEnumerator() => Shells.GetEnumerator();
		#endregion  // IEnumerable<DataPropertyShell<TKey, TValue>> interface support
		#endregion  // IReadOnlyCollection<DataPropertyShell<TKey, TValue>> interface support

		#region IDataPropertyDictionary interface support
		#region IReadOnlyList<IDataPropertyShell> interface support
		#region IEnumerable<IDataPropertyShell> interface support
		IEnumerator<IDataPropertyShell> IEnumerable<IDataPropertyShell>.GetEnumerator() => GetEnumerator();
		#endregion  // IEnumerable<IDataPropertyShell> interface support

		IDataPropertyShell IReadOnlyList<IDataPropertyShell>.this[int index] => Shells[index];
		#endregion  // IReadOnlyList<IDataPropertyShell> interface support

		#region IDictionary interface support
		#region ICollection interface support
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		bool ICollection.IsSynchronized => ((ICollection)Shells).IsSynchronized;

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		object ICollection.SyncRoot => ((ICollection)Shells).SyncRoot;

		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException(nameof(array));
			}

			if (array.Rank != 1)
			{
				throw new ArgumentException(Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_MULTI_DIMENSION_ARRAY);
			}

			if (array.GetLowerBound(0) != 0)
			{
				throw new ArgumentException(Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_NON_ZERO_LOWER_BOUND_OF_ARRAY);
			}

			if (arrayIndex < 0 || arrayIndex > array.Length)
			{
				throw new ArgumentOutOfRangeException(nameof(arrayIndex), Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_OUT_OF_RANGE_NEGATIVE_NUMBER);
			}

			if (array.Length - arrayIndex < Count)
			{
				throw new ArgumentException(Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_SHORT_LENGTH_ARRAY);
			}

			if (array is TValue[] values)
			{
				CopyTo(values, arrayIndex);
			}
			else if (array is DataProperty<TValue>[] items)
			{
				CopyTo(items, arrayIndex);
			}
			else if (array is DictionaryEntry[] dictionaryEntries)
			{
				foreach (var shell in this)
				{
					dictionaryEntries[arrayIndex++] = new DictionaryEntry(shell.Key, shell.Core);
				}
			}
			else if (array is KeyValuePair<TKey, DataProperty<TValue>>[] keyValuePairs)
			{
				foreach (var shell in this)
				{
					keyValuePairs[arrayIndex++] = shell.KeyValuePair;
				}
			}
			else if (array is DataPropertyShell<TKey, TValue>[] shells)
			{
				Shells.CopyTo(shells, arrayIndex);
			}
			else
			{
				var objects = array as object[];
				if (objects == null)
				{
					throw new ArgumentException(Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_INVALID_TYPED_ARRAY);
				}

				try
				{
					foreach (var shell in this)
					{
						objects[arrayIndex++] = shell;
					}
				}
				catch (ArrayTypeMismatchException)
				{
					throw new ArgumentException(Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_INVALID_TYPED_ARRAY);
				}
			}
		}
		#endregion  // ICollection interface support

		object IDictionary.this[object key]
		{
			get { return this[(TKey)key]; }
			set { throw new NotSupportedException(string.Format(Echo.Properties.Resources.MESSAGE_EXCEPTION_NOT_SUPPORTED_MUTATE_BY_INTERFACE_FORMAT, nameof(DataPropertyDictionary<TKey, TValue>), nameof(IDictionary))); }
		}

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		bool IDictionary.IsFixedSize => false;

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		bool IDictionary.IsReadOnly => false;

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		ICollection IDictionary.Keys => Keys;

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		ICollection IDictionary.Values => Items;

		void IDictionary.Add(object key, object value) => new NotSupportedException(string.Format(Echo.Properties.Resources.MESSAGE_EXCEPTION_NOT_SUPPORTED_MUTATE_BY_INTERFACE_FORMAT, nameof(DataPropertyDictionary<TKey, TValue>), nameof(IDictionary)));

		bool IDictionary.Contains(object key) => ContainsKey((TKey)key);

		IDictionaryEnumerator IDictionary.GetEnumerator() => new DictionaryEnumerator(GetEnumerator());

		void IDictionary.Remove(object key) => Remove((TKey)key);
		#endregion  // IDictionary interface support

		#region IReadOnlyIndexer<Iint, IDataPropertyShell> interface support
		IDataPropertyShell IReadOnlyIndexer<int, IDataPropertyShell>.this[int index] => Shells[index];
		#endregion  // IReadOnlyIndexer<int, IDataPropertyShell> interface support

		#region IDataPropertyCore interface support
		public string Name { get; }

		public Type PropertyType => typeof(TValue);
		#endregion  // IDataPropertyCore interface support

		event EventHandler<DataPropertyDictionaryChangingEventArgs> IDataPropertyDictionary.DataPropertyDictionaryChanging
		{
			add { _DataPropertyDictionaryChanging += value; }
			remove { _DataPropertyDictionaryChanging -= value; }
		}

		event EventHandler<DataPropertyDictionaryChangedEventArgs> IDataPropertyDictionary.DataPropertyDictionaryChanged
		{
			add { _DataPropertyDictionaryChanged += value; }
			remove { _DataPropertyDictionaryChanged -= value; }
		}

		public Type KeyType => typeof(TKey);

		void IDataPropertyDictionary.Extend(object key) => Add((TKey)key, default(TValue));

		void IDataPropertyDictionary.Reduce(object key) => Remove((TKey)key);
		#endregion  // IDataPropertyDictionary interface support

		#region protected members
		protected const string ItemPropertyName = "Item";

		#region protected virtual members
		/// <summary>
		///	プロパティ名の選定
		/// </summary>
		protected virtual string EvaluatePropertyName(TKey key)
		{
			if (typeof(TKey).IsValueType)
			{
				return key.ToString();
			}

			return ItemPropertyName;
		}

		/// <summary>
		/// 操作の実行
		/// </summary>
		protected virtual bool Execute(IEnumerable<DataPropertyDictionaryChangeOperation<TKey, TValue>> operations)
		{
			if (!operations.Any())
			{
				return false;
			}

			foreach (var operation in operations)
			{
				switch (operation.Action)
				{
					case DataPropertyCollectionChangeAction.Insert:
						var property = operation.SettingValue.Property;
						property.Value = operation.SettingValue.NewValue;
						property.NameCore = EvaluatePropertyName(operation.Key);
						Shells.Insert(operation.Index, new DataPropertyShell<TKey, TValue>(operation.Key, property));
						break;

					case DataPropertyCollectionChangeAction.Remove:
						if (Equals(Shells[operation.Index].Key, operation.Key))
						{
							Shells.RemoveAt(operation.Index);
						}

						break;

					case DataPropertyCollectionChangeAction.Move:
						var shell = Shells[operation.Index];
						if (!Equals(operation.MovingKey, operation.Key))
						{
							shell = new DataPropertyShell<TKey, TValue>(operation.MovingKey, shell.Core);
						}

						Shells.RemoveAt(operation.Index);
						Shells.Insert(operation.MovingIndex, shell);
						break;

					case DataPropertyCollectionChangeAction.Swap:
						var shell0 = Shells[operation.Index];
						var shell1 = Shells[operation.MovingIndex];
						if (!Equals(operation.MovingKey, operation.Key))
						{
							shell0 = new DataPropertyShell<TKey, TValue>(operation.MovingKey, shell0.Core);
						}

						Shells.RemoveAt(operation.Index);
						Shells.Insert(operation.MovingIndex, shell0);
						Shells.Remove(shell1);
						if (!Equals(operation.MovingKey, operation.Key))
						{
							shell1 = new DataPropertyShell<TKey, TValue>(operation.Key, shell1.Core);
						}

						Shells.Insert(operation.Index, shell1);
						break;
				}
			}

			return true;
		}

		/// <summary>
		/// 消去の操作
		/// </summary>
		protected virtual void ClearItems()
		{
			if (Count > 0)
			{
				CheckReentrancy();

				var operations = new List<DataPropertyDictionaryChangeOperation<TKey, TValue>>(Enumerable.Range(0, Count).Reverse().Select(_ => GenerateRemoveOperation(_, Shells[_].Key)));

				var e = new DataPropertyDictionaryChangingEventArgs<TKey, TValue>(this, operations);
				OnDataPropertyDictionaryChanging(e);
				if (!e.Cancel)
				{
					if (Execute(e.Operations))
					{
						OnDataPropertyDictionaryChanged(new DataPropertyDictionaryChangedEventArgs<TKey, TValue>(this, e.Operations, e.InputOperations));
					}
				}
			}
		}

		/// <summary>
		/// 挿入の操作
		/// </summary>
		protected virtual void InsertItem(int index, TKey key, TValue value)
		{
			CheckReentrancy();
			var operation = GenerateInsertOperation(index, key, value);

			var e = new DataPropertyDictionaryChangingEventArgs<TKey, TValue>(this, operation);
			OnDataPropertyDictionaryChanging(e);
			if (!e.Cancel)
			{
				if (Execute(e.Operations))
				{
					OnDataPropertyDictionaryChanged(new DataPropertyDictionaryChangedEventArgs<TKey, TValue>(this, e.Operations, e.InputOperations));
				}
			}
		}

		/// <summary>
		/// 連続挿入の操作
		/// </summary>
		protected virtual void InsertItems(int index, IEnumerable<TKey> keys, IEnumerable<TValue> values)
		{
			if (shells.Any())
			{
				CheckReentrancy();

				var operations = new List<DataPropertyDictionaryChangeOperation<TKey, TValue>>();
				foreach ((var key, var value) in Enumerable.Zip(keys, values, (key, value) => (key, value)))
				{
					operations.Add(GenerateInsertOperation(index++, key, value));
				}

				var e = new DataPropertyDictionaryChangingEventArgs<TKey, TValue>(this, operations);
				OnDataPropertyDictionaryChanging(e);
				if (!e.Cancel)
				{
					if (Execute(e.Operations))
					{
						OnDataPropertyDictionaryChanged(new DataPropertyDictionaryChangedEventArgs<TKey, TValue>(this, e.Operations, e.InputOperations));
					}
				}
			}
		}

		/// <summary>
		/// 移動の操作
		/// </summary>
		protected virtual void MoveItem(int oldIndex, TKey oldKey, int newIndex, TKey newKey)
		{
			CheckReentrancy();
			var operation = GenerateMoveOperation(oldIndex, oldKey, newIndex, newKey);

			var e = new DataPropertyDictionaryChangingEventArgs<TKey, TValue>(this, operation);
			OnDataPropertyDictionaryChanging(e);
			if (!e.Cancel)
			{
				if (Execute(e.Operations))
				{
					OnDataPropertyDictionaryChanged(new DataPropertyDictionaryChangedEventArgs<TKey, TValue>(this, e.Operations, e.InputOperations));
				}
			}
		}

		/// <summary>
		/// 削除の操作
		/// </summary>
		protected virtual void RemoveItem(int index)
		{
			CheckReentrancy();
			var operation = GenerateRemoveOperation(index, Shells[index].Key);

			var e = new DataPropertyDictionaryChangingEventArgs<TKey, TValue>(this, operation);
			OnDataPropertyDictionaryChanging(e);
			if (!e.Cancel)
			{
				if (Execute(e.Operations))
				{
					OnDataPropertyDictionaryChanged(new DataPropertyDictionaryChangedEventArgs<TKey, TValue>(this, e.Operations, e.InputOperations));
				}
			}
		}

		/// <summary>
		/// 連続削除の操作
		/// </summary>
		protected virtual void RemoveItems(int index, int count)
		{
			if (shells.Any())
			{
				CheckReentrancy();

				var operations = new List<DataPropertyDictionaryChangeOperation<TKey, TValue>>(Enumerable.Range(index, count).Reverse().Select(_ => GenerateRemoveOperation(_, Shells[_].Key)));

				var e = new DataPropertyDictionaryChangingEventArgs<TKey, TValue>(this, operations);
				OnDataPropertyDictionaryChanging(e);
				if (!e.Cancel)
				{
					if (Execute(e.Operations))
					{
						OnDataPropertyDictionaryChanged(new DataPropertyDictionaryChangedEventArgs<TKey, TValue>(this, e.Operations, e.InputOperations));
					}
				}
			}
		}

		/// <summary>
		/// 再設定の操作
		/// </summary>
		protected virtual void ResetItems(IEnumerable<TKey> keys, IEnumerable<TValue> values)
		{
			ClearItems();
			InsertItems(0, keys, values);
		}

		/// <summary>
		/// プロパティ辞書変更前の処理
		/// </summary>
		protected virtual void OnDataPropertyDictionaryChanging(DataPropertyDictionaryChangingEventArgs<TKey, TValue> e)
		{
			if (DataPropertyDictionaryChanging != null || _DataPropertyDictionaryChanging != null)
			{
				using (BlockReentrancy())
				{
					DataPropertyDictionaryChanging?.Invoke(this, e);
					_DataPropertyDictionaryChanging?.Invoke(this, e);
				}
			}
		}

		/// <summary>
		/// プロパティ辞書変更後の処理
		/// </summary>
		protected virtual void OnDataPropertyDictionaryChanged(DataPropertyDictionaryChangedEventArgs<TKey, TValue> e)
		{
			_DataPropertyDictionaryChanged?.Invoke(this, e);
			DataPropertyDictionaryChanged?.Invoke(this, e);
		}
		#endregion // protected virtual members

		/// <summary>
		/// 要素
		/// </summary>
		protected internal KeyedCollection<TKey, DataPropertyShell<TKey, TValue>> Shells => this.shells;

		/// <summary>
		/// 再操作の防止
		/// </summary>
		protected IDisposable BlockReentrancy()
		{
			return this.watcher.Scope();
		}

		/// <summary>
		/// 再操作の確認
		/// </summary>
		protected void CheckReentrancy()
		{
			if (this.watcher.IsInScope)
			{
				var invocationCount = 0;
				if (DataPropertyDictionaryChanging != null)
				{
					invocationCount += DataPropertyDictionaryChanging.GetInvocationList().Length;
				}

				if (_DataPropertyDictionaryChanging != null)
				{
					invocationCount += _DataPropertyDictionaryChanging.GetInvocationList().Length;
				}

				if (invocationCount > 1)
				{
					throw new InvalidOperationException(string.Format(Echo.Properties.Resources.MESSAGE_EXCEPTION_INVALID_OPERATION_REENTRANCY_FORMAT, nameof(DataPropertyDictionary<TKey, TValue>), nameof(DataPropertyDictionaryChanging)));
				}
			}
		}
		#endregion // protected members

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

		#region class DataPropertyShellCollection
		private class DataPropertyShellCollection : KeyedCollection<TKey, DataPropertyShell<TKey, TValue>>
		{
			#region protected members
			#region KeyedCollection<TKey, DataPropertyShell<TKey, TValue>> implement
			protected override TKey GetKeyForItem(DataPropertyShell<TKey, TValue> item) => item.Key;
			#endregion // KeyedCollection<TKey, DataPropertyShell<TKey, TValue>> implement
			#endregion // protected members
		}
		#endregion // class DataPropertyShellCollection

		#region struct DictionaryEnumerator
		private struct DictionaryEnumerator : IDictionaryEnumerator, IDisposable
		{
			public DictionaryEnumerator(IEnumerator<DataPropertyShell<TKey, TValue>> core)
			{
				if (core == null)
				{
					throw new ArgumentNullException(nameof(core));
				}

				this.core = core;
				this.needsToCreate = true;
			}

			#region IDictionaryEnumerator interface support
			#region IEnumerator interface support
			public object Current => Entry;

			public bool MoveNext()
			{
				var ret = this.core.MoveNext();
				this.needsToCreate = true;

				return ret;
			}

			public void Reset()
			{
				this.core.Reset();
				this.needsToCreate = true;
			}
			#endregion  // IEnumerator interface support

			public DictionaryEntry Entry => PrepareEntry();

			public object Key => Entry.Key;

			public object Value => Entry.Value;
			#endregion  // IDictionaryEnumerator interface support

			#region IDisposable interface support
			public void Dispose()
			{
				if (this.core != null)
				{
					this.core.Dispose();
					this.core = null;
				}
			}
			#endregion  // IDisposable interface support

			#region private members
			private static readonly DictionaryEntry EmptyDictionaryEntry = new DictionaryEntry();
			private volatile IEnumerator<DataPropertyShell<TKey, TValue>> core;
			private DictionaryEntry entry;
			private bool needsToCreate;

			private DictionaryEntry PrepareEntry()
			{
				if (this.needsToCreate)
				{
					var shell = this.core.Current;
					this.entry = new DictionaryEntry(shell.Key, shell.Core);
					this.needsToCreate = false;
				}

				return this.entry;
			}
			#endregion // private members
		}
		#endregion // struct Enumerator

		private DataPropertyShellCollection shells = new DataPropertyShellCollection();
		private KeyCollection keys;
		private ItemCollection items;
		private ScopeWatcher watcher = new ScopeWatcher();
		private EventHandler<DataPropertyDictionaryChangingEventArgs> _DataPropertyDictionaryChanging;
		private EventHandler<DataPropertyDictionaryChangedEventArgs> _DataPropertyDictionaryChanged;

		private KeyCollection PrepareKeys()
		{
			if (this.keys == null)
			{
				this.keys = new KeyCollection(this);
			}

			return this.keys;
		}

		private ItemCollection PrepareItems()
		{
			if (this.items == null)
			{
				this.items = new ItemCollection(this);
			}

			return this.items;
		}
		#endregion // private members
	}
}
