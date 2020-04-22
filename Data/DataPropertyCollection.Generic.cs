using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Echo.Data
{
	/// <summary>
	/// データプロパティ配列
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[System.Diagnostics.DebuggerTypeProxy(typeof(DataPropertyCollection<>.DebugView))]
	public partial class DataPropertyCollection<T> : IReadOnlyList<DataProperty<T>>, IReadOnlyIndexer<int, DataProperty<T>>, IDataPropertyCollection
	{
		public DataPropertyCollection([CallerMemberName] string name = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name));
			}

			Name = name;
		}

		public DataPropertyCollection(IEnumerable<T> collection, [CallerMemberName] string name = null)
			: this(name)
		{
			foreach (var value in collection)
			{
				Items.Add(new DataProperty<T>(ItemPropertyName) { Value = value, });
			}
		}

		/// <summary>
		/// プロパティ配列変更前
		/// </summary>
		public event EventHandler<DataPropertyCollectionChangingEventArgs<T>> DataPropertyCollectionChanging;

		/// <summary>
		/// プロパティ配列変更後
		/// </summary>
		public event EventHandler<DataPropertyCollectionChangedEventArgs<T>> DataPropertyCollectionChanged;

		/// <summary>
		/// 値の追加
		/// </summary>
		public void Add(T value) => InsertItem(Count, value);

		/// <summary>
		/// 値の連続追加
		/// </summary>
		public void AddRange(IEnumerable<T> values) => InsertItems(Count, values);

		/// <summary>
		/// 値の消去
		/// </summary>
		public void Clear() => ClearItems();

		/// <summary>
		/// 値の包含判定
		/// </summary>
		public bool Contains(T value) => Items.Any(_ => Equals(_.Value, value));

		/// <summary>
		/// 値のコピー
		/// </summary>
		public void CopyTo(T[] array, int arrayIndex)
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

			foreach (var item in Items)
			{
				array[arrayIndex++] = item.Value;
			}
		}

		/// <summary>
		/// 値のコピー
		/// </summary>
		public void CopyTo(DataProperty<T>[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);

		/// <summary>
		/// 挿入操作の作成
		/// </summary>
		public DataPropertyCollectionChangeOperation<T> GenerateInsertOperation(int index, T value)
		{
			return DataPropertyCollectionChangeOperation<T>.Insert(index, new DataProperty<T>(ItemPropertyName) { Value = value, });
		}

		/// <summary>
		/// 移動操作の作成
		/// </summary>
		public DataPropertyCollectionChangeOperation<T> GenerateMoveOperation(int oldIndex, int newIndex)
		{
			return DataPropertyCollectionChangeOperation<T>.Move(oldIndex, newIndex);
		}

		/// <summary>
		/// 削除操作の作成
		/// </summary>
		public DataPropertyCollectionChangeOperation<T> GenerateRemoveOperation(int index)
		{
			return DataPropertyCollectionChangeOperation<T>.Remove(index);
		}

		/// <summary>
		/// 設定操作の作成
		/// </summary>
		public DataPropertyCollectionChangeOperation<T> GenerateSetOperation(int index, T value)
		{
			return DataPropertyCollectionChangeOperation<T>.Set(index, Items[index].Value, value, Items[index]);
		}

		/// <summary>
		/// 交換操作の作成
		/// </summary>
		public DataPropertyCollectionChangeOperation<T> GenerateSwapOperation(int index0, int index1)
		{
			return DataPropertyCollectionChangeOperation<T>.Swap(index0, index1);
		}

		/// <summary>
		/// 配列操作
		/// </summary>
		public CollectionOperator GetCollectionOperator() => new CollectionOperator(this);

		/// <summary>
		/// 値の番号の検索
		/// </summary>
		public int IndexOf(T value)
		{
			for (var index = 0; index < Count; ++index)
			{
				var item = Items[index];
				if (Equals(item.Value, value))
				{
					return index;
				}
			}

			return -1;
		}

		/// <summary>
		/// 値の挿入
		/// </summary>
		public void Insert(int index, T value) => InsertItem(index, value);

		/// <summary>
		/// 値の連続挿入
		/// </summary>
		public void InsertRange(int index, IEnumerable<T> values) => InsertItems(index, values);

		/// <summary>
		/// 値の移動
		/// </summary>
		public void Move(int oldIndex, int newIndex) => MoveItem(oldIndex, newIndex);

		/// <summary>
		/// 値の削除
		/// </summary>
		public bool Remove(T value)
		{
			var index = IndexOf(value);
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
		public void Reset(IEnumerable<T> values)
		{
			ClearItems();
			InsertItems(0, values);
		}

		/// <summary>
		/// 値の交換
		/// </summary>
		public void Swap(int index0, int index1) => SwapItem(index0, index1);

		#region IReadOnlyList<DataProperty<T>> interface support
		#region IReadOnlyCollection<DataProperty<T>> interface support
		#region IEnumerable<DataProperty<T>> interface support
		#region IEnumerable interface support
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		#endregion  // IEnumerable interface support

		public IEnumerator<DataProperty<T>> GetEnumerator() => Items.GetEnumerator();
		#endregion  // IEnumerable<DataProperty<T>> interface support

		public int Count => Items.Count;
		#endregion  // IReadOnlyCollection<DataProperty<T>> interface support

		public DataProperty<T> this[int index] => Items[index];
		#endregion  // IReadOnlyList<DataProperty<T>> interface support

		#region IDataPropertyCollection interface support
		#region IReadOnlyList<IDataProperty> interface support
		#region IEnumerable<IDataProperty> interface support
		IEnumerator<IDataProperty> IEnumerable<IDataProperty>.GetEnumerator() => GetEnumerator();
		#endregion  // IEnumerable<IDataProperty> interface support

		IDataProperty IReadOnlyList<IDataProperty>.this[int index] => this[index];
		#endregion  // IReadOnlyList<IDataProperty> interface support

		#region ICollection interface support
		bool ICollection.IsSynchronized => ((ICollection)Items).IsSynchronized;

		object ICollection.SyncRoot => ((ICollection)Items).SyncRoot;

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

			if (array is T[] values)
			{
				CopyTo(values, arrayIndex);
			}
			else if (array is DataProperty<T>[] items)
			{
				CopyTo(items, arrayIndex);
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

		#region IReadOnlyIndexer<int, IDataProperty> interface support
		IDataProperty IReadOnlyIndexer<int, IDataProperty>.this[int index] => this[index];
		#endregion  // IReadOnlyIndexer<int, IDataProperty> interface support

		#region IDataPropertyCore interface support
		public string Name { get; }

		public Type PropertyType => typeof(T);
		#endregion  // IDataPropertyCore interface support

		event EventHandler<DataPropertyCollectionChangingEventArgs> IDataPropertyCollection.DataPropertyCollectionChanging
		{
			add { _DataPropertyCollectionChanging += value; }
			remove { _DataPropertyCollectionChanging -= value; }
		}

		event EventHandler<DataPropertyCollectionChangedEventArgs> IDataPropertyCollection.DataPropertyCollectionChanged
		{
			add { _DataPropertyCollectionChanged += value; }
			remove { _DataPropertyCollectionChanged -= value; }
		}

		void IDataPropertyCollection.Extend() => Add(default(T));

		void IDataPropertyCollection.Extend(int count) => AddRange(Enumerable.Repeat(default(T), count));

		void IDataPropertyCollection.Reduce() => RemoveAt(Count - 1);

		void IDataPropertyCollection.Reduce(int count) => RemoveRange(count > Count ? 0 : Count - count, count > Count ? Count : count);
		#endregion  // IDataPropertyCollection interface support

		#region protected members
		protected const string ItemPropertyName = "Item";

		#region protected virtual members
		/// <summary>
		/// 操作の実行
		/// </summary>
		protected virtual bool Execute(IEnumerable<DataPropertyCollectionChangeOperation<T>> operations)
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
						Items.Insert(operation.Index, property);
						break;

					case DataPropertyCollectionChangeAction.Remove:
						Items.RemoveAt(operation.Index);
						break;

					case DataPropertyCollectionChangeAction.Move:
						var item = Items[operation.Index];
						Items.RemoveAt(operation.Index);
						Items.Insert(operation.MovingIndex, item);
						break;

					case DataPropertyCollectionChangeAction.Swap:
						var item0 = Items[operation.Index];
						var item1 = Items[operation.MovingIndex];
						Items.RemoveAt(operation.Index);
						Items.Insert(operation.MovingIndex, item0);
						Items.Remove(item1);
						Items.Insert(operation.Index, item1);
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

				var operations = new List<DataPropertyCollectionChangeOperation<T>>(Enumerable.Range(0, Count).Reverse().Select(_ => GenerateRemoveOperation(_)));

				var e = new DataPropertyCollectionChangingEventArgs<T>(this, operations);
				OnDataPropertyCollectionChanging(e);
				if (!e.Cancel)
				{
					if (Execute(e.Operations))
					{
						OnDataPropertyCollectionChanged(new DataPropertyCollectionChangedEventArgs<T>(this, e.Operations, e.InputOperations));
					}
				}
			}
		}

		/// <summary>
		/// 挿入の操作
		/// </summary>
		protected virtual void InsertItem(int index, T value)
		{
			CheckReentrancy();
			var operation = GenerateInsertOperation(index, value);

			var e = new DataPropertyCollectionChangingEventArgs<T>(this, operation);
			OnDataPropertyCollectionChanging(e);
			if (!e.Cancel)
			{
				if (Execute(e.Operations))
				{
					OnDataPropertyCollectionChanged(new DataPropertyCollectionChangedEventArgs<T>(this, e.Operations, e.InputOperations));
				}
			}
		}

		/// <summary>
		/// 連続挿入の操作
		/// </summary>
		protected virtual void InsertItems(int index, IEnumerable<T> values)
		{
			if (items.Any())
			{
				CheckReentrancy();

				var operations = new List<DataPropertyCollectionChangeOperation<T>>();
				foreach (var value in values)
				{
					operations.Add(GenerateInsertOperation(index++, value));
				}

				var e = new DataPropertyCollectionChangingEventArgs<T>(this, operations);
				OnDataPropertyCollectionChanging(e);
				if (!e.Cancel)
				{
					if (Execute(e.Operations))
					{
						OnDataPropertyCollectionChanged(new DataPropertyCollectionChangedEventArgs<T>(this, e.Operations, e.InputOperations));
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

			var e = new DataPropertyCollectionChangingEventArgs<T>(this, operation);
			OnDataPropertyCollectionChanging(e);
			if (!e.Cancel)
			{
				if (Execute(e.Operations))
				{
					OnDataPropertyCollectionChanged(new DataPropertyCollectionChangedEventArgs<T>(this, e.Operations, e.InputOperations));
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

			var e = new DataPropertyCollectionChangingEventArgs<T>(this, operation);
			OnDataPropertyCollectionChanging(e);
			if (!e.Cancel)
			{
				if (Execute(e.Operations))
				{
					OnDataPropertyCollectionChanged(new DataPropertyCollectionChangedEventArgs<T>(this, e.Operations, e.InputOperations));
				}
			}
		}

		/// <summary>
		/// 連続削除の操作
		/// </summary>
		protected virtual void RemoveItems(int index, int count)
		{
			if (items.Any())
			{
				CheckReentrancy();

				var operations = new List<DataPropertyCollectionChangeOperation<T>>(Enumerable.Range(index, count).Reverse().Select(_ => GenerateRemoveOperation(_)));

				var e = new DataPropertyCollectionChangingEventArgs<T>(this, operations);
				OnDataPropertyCollectionChanging(e);
				if (!e.Cancel)
				{
					if (Execute(e.Operations))
					{
						OnDataPropertyCollectionChanged(new DataPropertyCollectionChangedEventArgs<T>(this, e.Operations, e.InputOperations));
					}
				}
			}
		}

		/// <summary>
		/// 設定の操作
		/// </summary>
		protected virtual void SetItem(int index, T value)
		{
			CheckReentrancy();
			var operation = GenerateSetOperation(index, value);

			var e = new DataPropertyCollectionChangingEventArgs<T>(this, operation);
			OnDataPropertyCollectionChanging(e);
			if (!e.Cancel)
			{
				if (Execute(e.Operations))
				{
					OnDataPropertyCollectionChanged(new DataPropertyCollectionChangedEventArgs<T>(this, e.Operations, e.InputOperations));
				}
			}
		}

		/// <summary>
		/// 連続設定の操作
		/// </summary>
		protected virtual void SetItems(int index, IEnumerable<T> values)
		{
			if (items.Any())
			{
				CheckReentrancy();

				var operations = new List<DataPropertyCollectionChangeOperation<T>>();
				foreach (var value in values)
				{
					operations.Add(GenerateSetOperation(index++, value));
				}

				var e = new DataPropertyCollectionChangingEventArgs<T>(this, operations);
				OnDataPropertyCollectionChanging(e);
				if (!e.Cancel)
				{
					if (Execute(e.Operations))
					{
						OnDataPropertyCollectionChanged(new DataPropertyCollectionChangedEventArgs<T>(this, e.Operations, e.InputOperations));
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

			var e = new DataPropertyCollectionChangingEventArgs<T>(this, operation);
			OnDataPropertyCollectionChanging(e);
			if (!e.Cancel)
			{
				if (Execute(e.Operations))
				{
					OnDataPropertyCollectionChanged(new DataPropertyCollectionChangedEventArgs<T>(this, e.Operations, e.InputOperations));
				}
			}
		}

		/// <summary>
		/// プロパティ配列変更前の処理
		/// </summary>
		protected virtual void OnDataPropertyCollectionChanging(DataPropertyCollectionChangingEventArgs<T> e)
		{
			if (DataPropertyCollectionChanging != null || _DataPropertyCollectionChanging != null)
			{
				using (BlockReentrancy())
				{
					DataPropertyCollectionChanging?.Invoke(this, e);
					_DataPropertyCollectionChanging?.Invoke(this, e);
				}
			}
		}

		/// <summary>
		/// プロパティ配列変更後の処理
		/// </summary>
		protected virtual void OnDataPropertyCollectionChanged(DataPropertyCollectionChangedEventArgs<T> e)
		{
			_DataPropertyCollectionChanged?.Invoke(this, e);
			DataPropertyCollectionChanged?.Invoke(this, e);
		}
		#endregion // protected virtual members

		/// <summary>
		/// 要素
		/// </summary>
		protected IList<DataProperty<T>> Items => this.items;

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
				if (DataPropertyCollectionChanging != null)
				{
					invocationCount += DataPropertyCollectionChanging.GetInvocationList().Length;
				}

				if (_DataPropertyCollectionChanging != null)
				{
					invocationCount += _DataPropertyCollectionChanging.GetInvocationList().Length;
				}

				if (invocationCount > 1)
				{
					throw new InvalidOperationException(string.Format(Echo.Properties.Resources.MESSAGE_EXCEPTION_INVALID_OPERATION_REENTRANCY_FORMAT, nameof(DataPropertyCollection<T>), nameof(DataPropertyCollectionChanging)));
				}
			}
		}
		#endregion // protected members

		#region private members
		#region class DebugView
		private class DebugView
		{
			public DebugView(DataPropertyCollection<T> target)
			{
				this.target = target;
			}

			[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
			public DataProperty<T>[] Items
			{
				get
				{
					var ret = new DataProperty<T>[this.target.Count];
					this.target.CopyTo(ret, 0);
					return ret;
				}
			}

			#region private members
			private DataPropertyCollection<T> target;
			#endregion // private members
		}
		#endregion // class DebugView

		private List<DataProperty<T>> items = new List<DataProperty<T>>();
		private ScopeWatcher watcher = new ScopeWatcher();
		private EventHandler<DataPropertyCollectionChangingEventArgs> _DataPropertyCollectionChanging;
		private EventHandler<DataPropertyCollectionChangedEventArgs> _DataPropertyCollectionChanged;
		#endregion // private members
	}
}
