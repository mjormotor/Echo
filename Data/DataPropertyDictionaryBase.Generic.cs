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
	[System.Diagnostics.DebuggerTypeProxy(typeof(DataPropertyDictionaryBase<,>.DebugView))]
	public abstract partial class DataPropertyDictionaryBase<TKey, TValue> : IReadOnlyCollection<DataPropertyShell<TKey, TValue>>, IDataPropertyDictionary
	{
		/// <summary>
		/// プロパティ配列変更前
		/// </summary>
		public event EventHandler<DataPropertyDictionaryChangingEventArgs<TKey, TValue>> DataPropertyDictionaryChanging;

		/// <summary>
		/// プロパティ配列変更後
		/// </summary>
		public event EventHandler<DataPropertyDictionaryChangedEventArgs<TKey, TValue>> DataPropertyDictionaryChanged;

		/// <summary>
		/// 比較方式
		/// </summary>
		public IEqualityComparer<TKey> Comparer { get; }

		/// <summary>
		/// 挿入操作の作成
		/// </summary>
		public DataPropertyDictionaryChangeOperation<TKey, TValue> GenerateInsertOperation(int index, TKey key, TValue value)
		{
			return DataPropertyDictionaryChangeOperation<TKey, TValue>.Insert(index, key, new DataProperty<TValue>(EvaluatePropertyName(key)) { Value = value, });
		}

		/// <summary>
		/// 移動操作の作成
		/// </summary>
		public DataPropertyDictionaryChangeOperation<TKey, TValue> GenerateMoveOperation(int oldIndex, int newIndex)
		{
			return DataPropertyDictionaryChangeOperation<TKey, TValue>.Move(oldIndex, newIndex);
		}

		/// <summary>
		/// 削除操作の作成
		/// </summary>
		public DataPropertyDictionaryChangeOperation<TKey, TValue> GenerateRemoveOperation(int index)
		{
			return DataPropertyDictionaryChangeOperation<TKey, TValue>.Remove(index);
		}

		/// <summary>
		/// 設定操作の作成
		/// </summary>
		public DataPropertyDictionaryChangeOperation<TKey, TValue> GenerateSetOperation(int index, TKey key, TValue value)
		{
			var shell = Shells[index];
			return DataPropertyDictionaryChangeOperation<TKey, TValue>.Set(index, shell.Key, shell.Core.Value, key, value, shell.Core);
		}

		/// <summary>
		/// 交換操作の作成
		/// </summary>
		public DataPropertyDictionaryChangeOperation<TKey, TValue> GenerateSwapOperation(int index0, int index1)
		{
			return DataPropertyDictionaryChangeOperation<TKey, TValue>.Swap(index0, index1);
		}

		#region IReadOnlyCollection<DataPropertyShell<TKey, TValue>> interface support
		#region IEnumerable<DataPropertyShell<TKey, TValue>> interface support
		#region IEnumerable interface support
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		#endregion  // IEnumerable interface support

		public IEnumerator<DataPropertyShell<TKey, TValue>> GetEnumerator() => Shells.GetEnumerator();
		#endregion  // IEnumerable<DataPropertyShell<TKey, TValue>> interface support

		public int Count => Shells.Count;
		#endregion  // IReadOnlyCollection<DataPropertyShell<TKey, TValue>> interface support

		#region IDataPropertyDictionary interface support
		#region IReadOnlyList<IReadOnlyShell<IDataProperty>> interface support
		#region IEnumerable<IReadOnlyShell<IDataProperty>> interface support
		IEnumerator<IReadOnlyShell<IDataProperty>> IEnumerable<IReadOnlyShell<IDataProperty>>.GetEnumerator() => GetEnumerator();
		#endregion  // IEnumerable<IReadOnlyShell<IDataProperty>> interface support

		IReadOnlyShell<IDataProperty> IReadOnlyList<IReadOnlyShell<IDataProperty>>.this[int index] => Shells[index];
		#endregion  // IReadOnlyList<IReadOnlyShell<IDataProperty>> interface support

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
			get { return Shells[(TKey)key].Core; }
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

		void IDictionary.Clear() => ClearItems();

		bool IDictionary.Contains(object key) => Shells.Contains((TKey)key);

		IDictionaryEnumerator IDictionary.GetEnumerator() => new DictionaryEnumerator(GetEnumerator());

		void IDictionary.Remove(object key) => RemoveItem((TKey)key);
		#endregion  // IDictionary interface support

		#region IReadOnlyIndexer<int, IReadOnlyShell<IDataProperty>> interface support
		IReadOnlyShell<IDataProperty> IReadOnlyIndexer<int, IReadOnlyShell<IDataProperty>>.this[int index] => Shells[index];
		#endregion  // IReadOnlyIndexer<int, IReadOnlyShell<IDataProperty>> interface support

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

		void IDataPropertyDictionary.Extend(object key) => InsertItem(Count, (TKey)key, default);

		void IDataPropertyDictionary.Reduce(object key) => RemoveItem((TKey)key);
		#endregion  // IDataPropertyDictionary interface support

		#region protected members
		protected const string ItemPropertyName = "Item";

		protected DataPropertyDictionaryBase([CallerMemberName] string name = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name));
			}

			this.keys = new Lazy<KeyCollection>(() => new KeyCollection(this));
			this.items = new Lazy<ItemCollection>(() => new ItemCollection(this));

			Name = name;
		}

		protected DataPropertyDictionaryBase(IEqualityComparer<TKey> comparer, [CallerMemberName] string name = null)
			: this(name)
		{
			Comparer = comparer ?? EqualityComparer<TKey>.Default;
		}

		protected DataPropertyDictionaryBase(IEnumerable<KeyValuePair<TKey, TValue>> dictionary, [CallerMemberName] string name = null)
			: this(name)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}

			InsertItems(0, dictionary.Select(_ => _.Key), dictionary.Select(_ => _.Value));
		}

		protected DataPropertyDictionaryBase(IEnumerable<KeyValuePair<TKey, TValue>> dictionary, IEqualityComparer<TKey> comparer, [CallerMemberName] string name = null)
			: this(dictionary, name)
		{
			Comparer = comparer ?? EqualityComparer<TKey>.Default;
		}

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
					case DataPropertyCollectionChangeAction.Set:
						operation.SettingValue.Property.Value = operation.SettingValue.NewValue;
						break;

					case DataPropertyCollectionChangeAction.Insert:
						var property = operation.SettingValue.Property;
						property.Value = operation.SettingValue.NewValue;
						property.NameCore = EvaluatePropertyName(operation.SettingValue.NewKey);
						Shells.Insert(operation.Index, new DataPropertyShell<TKey, TValue>(operation.SettingValue.NewKey, property));
						break;

					case DataPropertyCollectionChangeAction.Remove:
						Shells.RemoveAt(operation.Index);
						break;

					case DataPropertyCollectionChangeAction.Move:
						var shell = Shells[operation.Index];
						Shells.RemoveAt(operation.Index);
						Shells.Insert(operation.MovingIndex, shell);
						break;

					case DataPropertyCollectionChangeAction.Swap:
						var shell0 = Shells[operation.Index];
						var shell1 = Shells[operation.MovingIndex];
						Shells.RemoveAt(operation.Index);
						Shells.Insert(operation.MovingIndex, shell0);
						Shells.Remove(shell1);
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

				var operations = new List<DataPropertyDictionaryChangeOperation<TKey, TValue>>(Enumerable.Range(0, Count).Reverse().Select(_ => GenerateRemoveOperation(_)));

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
			if (values.Any())
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
		protected virtual void MoveItem(int oldIndex, int newIndex)
		{
			CheckReentrancy();
			var operation = GenerateMoveOperation(oldIndex, newIndex);

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
			var operation = GenerateRemoveOperation(index);

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
			if (count > 0)
			{
				CheckReentrancy();

				var operations = new List<DataPropertyDictionaryChangeOperation<TKey, TValue>>(Enumerable.Range(index, count).Reverse().Select(_ => GenerateRemoveOperation(_)));

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
		/// 設定の操作
		/// </summary>
		protected virtual void SetItem(int index, TKey key, TValue value)
		{
			CheckReentrancy();
			var operation = GenerateSetOperation(index, key, value);

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
		/// 連続設定の操作
		/// </summary>
		protected virtual void SetItems(int index, IEnumerable<TKey> keys, IEnumerable<TValue> values)
		{
			if (values.Any())
			{
				CheckReentrancy();

				var operations = new List<DataPropertyDictionaryChangeOperation<TKey, TValue>>();
				foreach ((var key, var value) in Enumerable.Zip(keys, values, (key, value) => (key, value)))
				{
					operations.Add(GenerateSetOperation(index++, key, value));
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
		/// 交換の操作
		/// </summary>
		protected virtual void SwapItem(int index0, int index1)
		{
			CheckReentrancy();
			var operation = GenerateSwapOperation(index0, index1);

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
		/// 格納列
		/// </summary>
		protected KeyedCollection<TKey, DataPropertyShell<TKey, TValue>> Shells => this.shells;

		/// <summary>
		/// キー列
		/// </summary>
		protected KeyCollection Keys => this.keys.Value;

		/// <summary>
		/// 要素列
		/// </summary>
		protected ItemCollection Items => this.items.Value;

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

		/// <summary>
		/// 値のコピー
		/// </summary>
		protected void CopyTo(TValue[] array, int arrayIndex)
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
		protected void CopyTo(DataProperty<TValue>[] array, int arrayIndex)
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
		/// 削除の操作
		/// </summary>
		protected bool RemoveItem(TKey key)
		{
			var index = Keys.IndexOf(key);
			if (index == -1)
			{
				return false;
			}

			RemoveItem(index);
			return true;
		}

		/// <summary>
		/// 要素の取得の試行
		/// </summary>
		protected bool TryGetItem(TKey key, out DataProperty<TValue> item)
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
		private Lazy<KeyCollection> keys;
		private Lazy<ItemCollection> items;
		private ScopeWatcher watcher = new ScopeWatcher();
		private EventHandler<DataPropertyDictionaryChangingEventArgs> _DataPropertyDictionaryChanging;
		private EventHandler<DataPropertyDictionaryChangedEventArgs> _DataPropertyDictionaryChanged;
		#endregion // private members
	}
}
