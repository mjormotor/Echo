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
	public abstract class DataPropertyDictionaryBase<TKey, TValue> : IReadOnlyCollection<DataPropertyShell<TKey, TValue>>, IDataPropertyDictionary
	{
		#region class KeyCollection
		/// <summary>
		/// キー列
		/// </summary>
		[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
		[System.Diagnostics.DebuggerTypeProxy(typeof(DataPropertyDictionaryBase<,>.KeyCollection.DebugView))]
		[Serializable]
		public sealed class KeyCollection : ICollection<TKey>, ICollection, IReadOnlyList<TKey>
		{
			/// <summary>
			/// 番号の検索
			/// </summary>
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

			#region internal members
			internal KeyCollection(DataPropertyDictionaryBase<TKey, TValue> owner)
			{
				this.owner = owner;
			}
			#endregion // internal members

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

			private DataPropertyDictionaryBase<TKey, TValue> owner;
			#endregion // private members
		}
		#endregion // class KeyCollection

		#region class ItemCollection
		/// <summary>
		/// 要素列
		/// </summary>
		[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
		[System.Diagnostics.DebuggerTypeProxy(typeof(DataPropertyDictionaryBase<,>.ItemCollection.DebugView))]
		[Serializable]
		public sealed class ItemCollection : ICollection<DataProperty<TValue>>, ICollection, IReadOnlyList<DataProperty<TValue>>
		{
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

				var items = array as DataProperty<TValue>[];
				if (items != null)
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

			#region IReadOnlyList<DataProperty<TValue>> interface support
			public DataProperty<TValue> this[int index] => this.owner.Shells[index].Core;
			#endregion  // IReadOnlyList<DataProperty<TValue>> interface support

			#region internal members
			internal ItemCollection(DataPropertyDictionaryBase<TKey, TValue> owner)
			{
				this.owner = owner;
			}
			#endregion // internal members

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

			private DataPropertyDictionaryBase<TKey, TValue> owner;
			#endregion // private members
		}
		#endregion // class ItemCollection

		#region class DictionaryOperatorBase
		/// <summary>
		/// 辞書操作
		/// </summary>
		[System.Diagnostics.DebuggerDisplay("Count = {Shells.Count}")]
		[System.Diagnostics.DebuggerTypeProxy(typeof(DataPropertyDictionary<,>.DictionaryOperatorBase.DebugView))]
		public abstract class DictionaryOperatorBase : IDisposable
		{
			#region class KeyCollection
			[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
			[System.Diagnostics.DebuggerTypeProxy(typeof(DataPropertyDictionary<,>.DictionaryOperatorBase.KeyCollection.DebugView))]
			[Serializable]
			public sealed class KeyCollection : ICollection<TKey>, ICollection, IReadOnlyList<TKey>
			{
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

				public int Count => this.owner.Shells.Count;

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

					if (array.Length - arrayIndex < Count)
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

					if (array.Length - arrayIndex < Count)
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

				#region internal members
				internal KeyCollection(DictionaryOperatorBase owner)
				{
					this.owner = owner;
				}
				#endregion // internal members

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

				private DictionaryOperatorBase owner;
				#endregion // private members
			}
			#endregion // class KeyCollection

			#region class ValueCollection
			[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
			[System.Diagnostics.DebuggerTypeProxy(typeof(DataPropertyDictionary<,>.DictionaryOperatorBase.ValueCollection.DebugView))]
			[Serializable]
			public sealed class ValueCollection : ICollection<TValue>, ICollection, IReadOnlyList<TValue>
			{
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

				public int Count => this.owner.Shells.Count;

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

					if (array.Length - arrayIndex < Count)
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

					if (array.Length - arrayIndex < Count)
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

				#region internal members
				public ValueCollection(DictionaryOperatorBase owner)
				{
					this.owner = owner;
				}
				#endregion // internal members

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

				private DictionaryOperatorBase owner;
				#endregion // private members
			}
			#endregion // class ValueCollection

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

			protected DictionaryOperatorBase(DataPropertyDictionaryBase<TKey, TValue> owner)
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

			#region protected virtual members
			protected virtual void ClearItems()
			{
				if (Shells.Count > 0)
				{
					var lastOperationCount = this.operations.Count;
					Shells.Clear();
					this.operations.AddRange(Enumerable.Range(0, Shells.Count).Reverse().Select(_ => DataPropertyDictionaryChangeOperation<TKey, TValue>.Remove(_)));

					OnOperated(this.operations.Skip(lastOperationCount));
				}
			}

			protected virtual void InsertItem(int index, TKey key, TValue value)
			{
				var lastOperationCount = this.operations.Count;
				var item = new DataProperty<TValue>(this.owner.EvaluatePropertyName(key)) { Value = value, };
				Shells.Insert(index, new Shell(key, item));
				this.operations.Add(DataPropertyDictionaryChangeOperation<TKey, TValue>.Insert(index, key, item));

				OnOperated(this.operations.Skip(lastOperationCount));
			}

			protected virtual void InsertItems(int index, IEnumerable<TKey> keys, IEnumerable<TValue> values)
			{
				if (values.Any())
				{
					var lastOperationCount = this.operations.Count;
					foreach ((var key, var value) in Enumerable.Zip(keys, values, (key, value) => (key, value)))
					{
						var item = new DataProperty<TValue>(this.owner.EvaluatePropertyName(key)) { Value = value, };
						Shells.Insert(index, new Shell(key, item));
						this.operations.Add(DataPropertyDictionaryChangeOperation<TKey, TValue>.Insert(index, key, item));
						++index;
					}

					OnOperated(this.operations.Skip(lastOperationCount));
				}
			}

			protected virtual void MoveItem(int oldIndex, int newIndex)
			{
				var lastOperationCount = this.operations.Count;
				var shell = Shells[oldIndex];
				Shells.RemoveAt(oldIndex);
				Shells.Insert(newIndex, shell);
				this.operations.Add(DataPropertyDictionaryChangeOperation<TKey, TValue>.Move(oldIndex, newIndex));

				OnOperated(this.operations.Skip(lastOperationCount));
			}

			protected virtual void RemoveItem(int index)
			{
				var lastOperationCount = this.operations.Count;
				Shells.RemoveAt(index);
				this.operations.Add(DataPropertyDictionaryChangeOperation<TKey, TValue>.Remove(index));

				OnOperated(this.operations.Skip(lastOperationCount));
			}

			protected virtual void RemoveItems(int index, int count)
			{
				if (count != 0)
				{
					var lastOperationCount = this.operations.Count;
					foreach (var value in Enumerable.Range(index, count).Reverse())
					{
						Shells.RemoveAt(value);
						this.operations.Add(DataPropertyDictionaryChangeOperation<TKey, TValue>.Remove(value));
					}

					OnOperated(this.operations.Skip(lastOperationCount));
				}
			}

			protected virtual void SetItem(int index, TKey key, TValue value)
			{
				var lastOperationCount = this.operations.Count;
				var shell = Shells[index];
				var oldKey = shell.Key;
				var oldValue = shell.Value;
				shell.Key = key;
				shell.Value = value;
				this.operations.Add(DataPropertyDictionaryChangeOperation<TKey, TValue>.Set(index, oldKey, oldValue, key, value, shell.Property));

				OnOperated(this.operations.Skip(lastOperationCount));
			}

			protected virtual void SetItems(int index, IEnumerable<TKey> keys, IEnumerable<TValue> values)
			{
				if (values.Any())
				{
					var lastOperationCount = this.operations.Count;
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

					OnOperated(this.operations.Skip(lastOperationCount));
				}
			}

			protected virtual void SwapItem(int index0, int index1)
			{
				var lastOperationCount = this.operations.Count;
				var shell0 = Shells[index0];
				var shell1 = Shells[index1];
				Shells.RemoveAt(index0);
				Shells.Insert(index1, shell0);
				Shells.Remove(shell1);
				Shells.Insert(index0, shell1);
				this.operations.Add(DataPropertyDictionaryChangeOperation<TKey, TValue>.Swap(index0, index1));

				OnOperated(this.operations.Skip(lastOperationCount));
			}

			protected virtual void OnOperated(IEnumerable<DataPropertyDictionaryChangeOperation<TKey, TValue>> operations)
			{
			}
			#endregion // protected virtual members

			protected KeyedCollection<TKey, Shell> Shells => this.shells;

			protected KeyCollection Keys => PrepareKeys();

			protected ValueCollection Values => PrepareValues();

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
				public DebugView(DictionaryOperatorBase target)
				{
					this.target = target;
				}

				[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
				public KeyValuePair<TKey, TValue>[] Items
				{
					get
					{
						var ret = new KeyValuePair<TKey, TValue>[this.target.Shells.Count];
						foreach ((var item, var index) in this.target.Shells.Select((_, index) => (new KeyValuePair<TKey, TValue>(_.Key, _.Value), index)))
						{
							ret[index] = item;
						}

						return ret;
					}
				}

				#region private members
				private DictionaryOperatorBase target;
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

			private DataPropertyDictionaryBase<TKey, TValue> owner;
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
		#endregion // class DictionaryOperatorBase

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
		protected KeyCollection Keys => PrepareKeys();

		/// <summary>
		/// 要素列
		/// </summary>
		protected ItemCollection Items => PrepareItems();

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
