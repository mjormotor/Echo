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
	public class DataPropertyDictionary<TKey, TValue> : DataPropertyDictionaryBase<TKey, TValue>
	{
		#region class DictionaryOperator
		/// <summary>
		/// 辞書操作
		/// </summary>
		[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
		[System.Diagnostics.DebuggerTypeProxy(typeof(DataPropertyDictionary<,>.DictionaryOperator.DebugView))]
		public class DictionaryOperator : IReadOnlyDictionary<TKey, TValue>, IDisposable
		{
			#region class KeyCollection
			[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
			[System.Diagnostics.DebuggerTypeProxy(typeof(DataPropertyDictionary<,>.DictionaryOperator.KeyCollection.DebugView))]
			[Serializable]
			public sealed class KeyCollection : ICollection<TKey>, ICollection, IReadOnlyList<TKey>
			{
				public KeyCollection(DictionaryOperator owner)
				{
					if (owner == null)
					{
						throw new ArgumentNullException(nameof(owner));
					}

					this.owner = owner;
				}

				public int IndexOf(TKey key)
				{
					for (var index = 0; index < Count; ++index)
					{
						var sample = this[index];
						if (Equals(sample, key))
						{
							return index;
						}
					}

					return -1;
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
					return this.owner.Shells.Contains(item);
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

				#region IReadOnlyList<TKey> interface support
				public TKey this[int index] => this.owner.Shells[index].Key;
				#endregion  // IReadOnlyList<TKey> interface support

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

				private DictionaryOperator owner;
				#endregion // private members
			}
			#endregion // class KeyCollection

			#region class ValueCollection
			[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
			[System.Diagnostics.DebuggerTypeProxy(typeof(DataPropertyDictionary<,>.DictionaryOperator.ValueCollection.DebugView))]
			[Serializable]
			public sealed class ValueCollection : ICollection<TValue>, ICollection, IReadOnlyList<TValue>
			{
				public ValueCollection(DictionaryOperator owner)
				{
					if (owner == null)
					{
						throw new ArgumentNullException(nameof(owner));
					}

					this.owner = owner;
				}

				#region ICollection<TValue> interface support
				#region IEnumerable<TValue> interface support
				#region IEnumerable interface support
				IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
				#endregion  // IEnumerable interface support

				public IEnumerator<TValue> GetEnumerator()
				{
					foreach (var shell in this.owner.Shells)
					{
						yield return shell.Value;
					}
				}
				#endregion  // IEnumerable<TValue> interface support

				public int Count => this.owner.Count;

				bool ICollection<TValue>.IsReadOnly => true;

				void ICollection<TValue>.Add(TValue value)
				{
					throw new NotSupportedException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_NOT_SUPPORTED_MUTATE_ITEM_COLLECTION_FORMAT, nameof(ItemCollection), nameof(DataPropertyDictionary<TKey, TValue>)));
				}

				void ICollection<TValue>.Clear()
				{
					throw new NotSupportedException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_NOT_SUPPORTED_MUTATE_ITEM_COLLECTION_FORMAT, nameof(ItemCollection), nameof(DataPropertyDictionary<TKey, TValue>)));
				}

				bool ICollection<TValue>.Contains(TValue value)
				{
					return this.owner.Shells.Any(_ => Equals(_.Value, value));
				}

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

					if (array.Length - arrayIndex < this.owner.Count)
					{
						throw new ArgumentException(Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_SHORT_LENGTH_ARRAY);
					}

					foreach (var value in this)
					{
						array[arrayIndex++] = value;
					}
				}

				bool ICollection<TValue>.Remove(TValue value)
				{
					throw new NotSupportedException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_NOT_SUPPORTED_MUTATE_ITEM_COLLECTION_FORMAT, nameof(ItemCollection), nameof(DataPropertyDictionary<TKey, TValue>)));
				}
				#endregion  // ICollection<TValue> interface support

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

					var values = array as TValue[];
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

				#region IReadOnlyList<TValue> interface support
				public TValue this[int index] => this.owner.Shells[index].Value;
				#endregion  // IReadOnlyList<TValue> interface support

				#region private members
				#region class DebugView
				private class DebugView
				{
					public DebugView(ValueCollection target)
					{
						this.target = target;
					}

					[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
					public TValue[] Items
					{
						get
						{
							var ret = new TValue[this.target.Count];
							this.target.CopyTo(ret, 0);
							return ret;
						}
					}

					#region private members
					private ValueCollection target;
					#endregion // private members
				}
				#endregion // class DebugView

				private DictionaryOperator owner;
				#endregion // private members
			}
			#endregion // class ValueCollection

			public DictionaryOperator(DataPropertyDictionary<TKey, TValue> owner)
			{
				if (owner == null)
				{
					throw new ArgumentNullException(nameof(owner));
				}

				this.owner = owner;
				foreach (var item in owner)
				{
					this.shells.Add(new Shell(item.Key, item.Core));
				}

				this.block = this.owner.BlockReentrancy();
			}

			public KeyCollection Keys => PrepareKeys();

			public ValueCollection Values => PrepareValues();

			public void Add(TKey key, TValue value) => InsertItem(Count, key, value);

			public void AddRange(IEnumerable<TKey> keys, IEnumerable<TValue> values) => InsertItems(Count, keys, values);

			public void ChangeKey(TKey oldKey, TKey newKey)
			{
				var index = Keys.IndexOf(oldKey);
				SetItem(index, newKey, Shells[index].Value);
			}

			public void Clear() => ClearItems();

			public bool ContainsKey(TKey key) => Shells.Contains(key);

			public void ContainsValue(TValue value) => Shells.Any(_ => Equals(_.Value, value));

			public void CopyTo(TValue[] array, int arrayIndex) => Values.CopyTo(array, arrayIndex);

			public void Insert(int index, TKey key, TValue value) => InsertItem(index, key, value);

			public void InsertRange(int index, IEnumerable<TKey> keys, IEnumerable<TValue> values) => InsertItems(index, keys, values);

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

			public bool Remove(TKey key) => RemoveItem(key);

			public void RemoveAt(int index) => RemoveItem(index);

			public void RemoveRange(int index, int count) => RemoveItems(index, count);

			public void Reset(IEnumerable<TKey> keys, IEnumerable<TValue> values)
			{
				ClearItems();
				InsertItems(0, keys, values);
			}

			public void Set(int index, TKey key, TValue value) => SetItem(index, key, value);

			public void SetRange(int index, IEnumerable<TKey> keys, IEnumerable<TValue> values) => SetItems(index, keys, values);

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

			public bool TryGetValue(TKey key, out TValue value)
			{
				var ret = false;
				if (Shells.TryGetValue(key, out var shell))
				{
					value = shell.Value;
					ret = true;
				}
				else
				{
					value = default;
				}

				return ret;
			}

			#region IReadOnlyDictionary<TKey, Value> interface support
			#region IReadOnlyCollection<KeyValuePair<TKey, TValue>> interface support
			#region IEnumerable<KeyValuePair<TKey, TValue>> interface support
			#region IEnumerable interface support
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
			#endregion  // IEnumerable interface support

			public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
			{
				foreach (var shell in Shells)
				{
					yield return new KeyValuePair<TKey, TValue>(shell.Key, shell.Value);
				}
			}
			#endregion  // IEnumerable<KeyValuePair<TKey, TValue>> interface support

			public int Count => Shells.Count;
			#endregion  // IReadOnlyCollection<KeyValuePair<TKey, TValue>> interface support

			public TValue this[TKey key] => Shells[key].Value;

			[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
			IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

			[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
			IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
			#endregion  // IReadOnlyDictionary<TKey, TValue> interface support

			#region IDisposable interface support
			public void Dispose()
			{
				if (this.block != null)
				{
					this.block.Dispose();
					this.block = null;

					var e = new DataPropertyDictionaryChangingEventArgs<TKey, TValue>(this.owner, this.operations);
					this.owner.OnDataPropertyDictionaryChanging(e);
					if (!e.Cancel)
					{
						if (this.owner.Execute(e.Operations))
						{
							this.owner.OnDataPropertyDictionaryChanged(new DataPropertyDictionaryChangedEventArgs<TKey, TValue>(this.owner, e.Operations, e.InputOperations));
						}
					}
				}
			}
			#endregion  // IDisposable interface support

			#region protected members
			#region class Shell
			protected class Shell
			{
				public Shell(TKey key, DataProperty<TValue> property)
				{
					Property = property;
					Key = key;
					Value = Property.Value;
				}

				public DataProperty<TValue> Property { get; }

				public TKey Key { get; set; }

				public TValue Value { get; set; }
			}
			#endregion // class Shell

			#region protected virtual members
			protected virtual void ClearItems()
			{
				if (Count > 0)
				{
					Shells.Clear();
					this.operations.AddRange(Enumerable.Range(0, Count).Reverse().Select(_ => DataPropertyDictionaryChangeOperation<TKey, TValue>.Remove(_)));
				}
			}

			protected virtual void InsertItem(int index, TKey key, TValue value)
			{
				var item = new DataProperty<TValue>(this.owner.EvaluatePropertyName(key)) { Value = value, };
				Shells.Insert(index, new Shell(key, item));
				this.operations.Add(DataPropertyDictionaryChangeOperation<TKey, TValue>.Insert(index, key, item));
			}

			protected virtual void InsertItems(int index, IEnumerable<TKey> keys, IEnumerable<TValue> values)
			{
				if (values.Any())
				{
					foreach ((var key, var value) in Enumerable.Zip(keys, values, (key, value) => (key, value)))
					{
						var item = new DataProperty<TValue>(this.owner.EvaluatePropertyName(key)) { Value = value, };
						Shells.Insert(index, new Shell(key, item));
						this.operations.Add(DataPropertyDictionaryChangeOperation<TKey, TValue>.Insert(index, key, item));
						++index;
					}
				}
			}

			protected virtual void MoveItem(int oldIndex, int newIndex)
			{
				var shell = Shells[oldIndex];
				Shells.RemoveAt(oldIndex);
				Shells.Insert(newIndex, shell);
				this.operations.Add(DataPropertyDictionaryChangeOperation<TKey, TValue>.Move(oldIndex, newIndex));
			}

			protected virtual void RemoveItem(int index)
			{
				Shells.RemoveAt(index);
				this.operations.Add(DataPropertyDictionaryChangeOperation<TKey, TValue>.Remove(index));
			}

			protected virtual void RemoveItems(int index, int count)
			{
				foreach (var value in Enumerable.Range(index, count).Reverse())
				{
					Shells.RemoveAt(value);
					this.operations.Add(DataPropertyDictionaryChangeOperation<TKey, TValue>.Remove(value));
				}
			}

			protected virtual void SetItem(int index, TKey key, TValue value)
			{
				var shell = Shells[index];
				var oldKey = shell.Key;
				var oldValue = shell.Value;
				shell.Key = key;
				shell.Value = value;
				this.operations.Add(DataPropertyDictionaryChangeOperation<TKey, TValue>.Set(index, oldKey, oldValue, key, value, shell.Property));
			}

			protected virtual void SetItems(int index, IEnumerable<TKey> keys, IEnumerable<TValue> values)
			{
				if (values.Any())
				{
					foreach ((var key, var value) in Enumerable.Zip(keys, values, (key, value) => (key, value)))
					{
						var shell = Shells[index];
						var oldKey = shell.Key;
						var oldValue = shell.Value;
						shell.Key = key;
						shell.Value = value;
						this.operations.Add(DataPropertyDictionaryChangeOperation<TKey, TValue>.Set(index, oldKey, oldValue, key, value, shell.Property));
						++index;
					}
				}
			}

			protected virtual void SwapItem(int index0, int index1)
			{
				var shell0 = Shells[index0];
				var shell1 = Shells[index1];
				Shells.RemoveAt(index0);
				Shells.Insert(index1, shell0);
				Shells.Remove(shell1);
				Shells.Insert(index0, shell1);
				this.operations.Add(DataPropertyDictionaryChangeOperation<TKey, TValue>.Swap(index0, index1));
			}
			#endregion // protected virtual members

			protected KeyedCollection<TKey, Shell> Shells => this.shells;

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
			#endregion // protected members

			#region private members
			#region class DebugView
			private class DebugView
			{
				public DebugView(DictionaryOperator target)
				{
					this.target = target;
				}

				[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
				public KeyValuePair<TKey, TValue>[] Items => this.target.ToArray();

				#region private members
				private DictionaryOperator target;
				#endregion // private members
			}
			#endregion // class DebugView

			#region class ShellCollection
			private class ShellCollection : KeyedCollection<TKey, Shell>
			{
				#region protected members
				#region KeyedCollection<TKey, Shell> implement
				protected override TKey GetKeyForItem(Shell item) => item.Key;
				#endregion // KeyedCollection<TKey, Shell> implement
				#endregion // protected members
			}
			#endregion // class ShellCollection

			private DataPropertyDictionary<TKey, TValue> owner;
			private ShellCollection shells = new ShellCollection();
			private List<DataPropertyDictionaryChangeOperation<TKey, TValue>> operations = new List<DataPropertyDictionaryChangeOperation<TKey, TValue>>();
			private KeyCollection keys;
			private ValueCollection values;
			private volatile IDisposable block;

			private KeyCollection PrepareKeys()
			{
				if (this.keys == null)
				{
					this.keys = new KeyCollection(this);
				}

				return this.keys;
			}

			private ValueCollection PrepareValues()
			{
				if (this.values == null)
				{
					this.values = new ValueCollection(this);
				}

				return this.values;
			}
			#endregion // private members
		}
		#endregion // class DictionaryOperator

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
