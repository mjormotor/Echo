using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Echo.Data
{
	/// <summary>
	/// データプロパティ系譜
	/// </summary>
	[System.Diagnostics.DebuggerTypeProxy(typeof(DataPropertyPedigree<>.DebugView))]
	public partial class DataPropertyPedigree<T> : DataPropertyDictionaryBase<T, T>
		where T : class
	{
		public DataPropertyPedigree([CallerMemberName] string name = null)
			: base(name)
		{
			this.tree = new Lazy<TreeBuilder>(() => new TreeBuilder(this));
		}

		public DataPropertyPedigree(IEqualityComparer<T> comparer, [CallerMemberName] string name = null)
			: base(comparer, name)
		{
			this.tree = new Lazy<TreeBuilder>(() => new TreeBuilder(this));
		}

		/// <summary>
		/// 樹形図
		/// </summary>
		public TreeBuilder Tree => this.tree.Value;

		/// <summary>
		/// 値の追加
		/// </summary>
		public void Add(T item, T parent = null)
		{
			var index = Count;
			if (parent != null)
			{
				index = Keys.IndexOf(parent);
				if  (index == -1)
				{
					throw new ArgumentException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_TARGET_UNCONTAINED_ITEM_FORMAT, nameof(parent)));
				}

				++index;
				while (index < Count)
				{
					if (!EvaluateDescendant(Shells[index].Key, parent))
					{
						break;
					}

					++index;
				}
			}

			InsertItem(index, item, parent);
		}

		/// <summary>
		/// 値の消去
		/// </summary>
		public void Clear() => ClearItems();

		/// <summary>
		/// 包含判定
		/// </summary>
		public bool Contains(T item) => Shells.Contains(item);

		/// <summary>
		/// 値のコピー
		/// </summary>
		public new void CopyTo(T[] array, int arrayIndex) => Keys.CopyTo(array, arrayIndex);

		/// <summary>
		/// 値の挿入
		/// </summary>
		public void Insert(T target, T item)
		{
			var index = Keys.IndexOf(target);
			if (index == -1)
			{
				throw new ArgumentException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_TARGET_UNCONTAINED_ITEM_FORMAT, nameof(target)));
			}

			var parent = Shells[index].Core.Value;
			InsertItem(index, item, parent);
		}

		/// <summary>
		/// 親子づけの設定
		/// </summary>
		public void Parent(T item, T parent)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (!Shells.TryGetValue(item, out var property))
			{
				throw new ArgumentException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_TARGET_UNCONTAINED_ITEM_FORMAT, nameof(item)));
			}

			var currentParent = property.Core.Value;
			if (parent == currentParent)
			{
				return;
			}

			if (EvaluateDescendant(parent, item))
			{
				throw new ArgumentException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_PARENT_TO_DESCENDANT_FORMAT, nameof(parent), nameof(item)));
			}

			const int SearchIndex = 0x00000001 << 0;
			const int SearchCount = 0x00000001 << 1;
			const int SearchParent = 0x00000001 << 2;
			const int SearchTargetIndex = 0x00000001 << 3;
			var index = -1;
			var count = 0;
			var targetIndex = -1;
			var searchMode = SearchIndex;
			if (parent != null)
			{
				if (parent == item)
				{
					throw new ArgumentException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_SAME_TARGET_FORMAT, nameof(parent), nameof(item)));
				}

				searchMode |= SearchParent;
				for (var sampleIndex = 0; sampleIndex < Count; ++sampleIndex)
				{
					var sample = Shells[sampleIndex];
					if ((searchMode & SearchCount) == SearchCount)
					{
						if (!EvaluateDescendant(sample.Key, item))
						{
							count = sampleIndex - index;
							searchMode ^= SearchCount;
						}
					}
					else if ((searchMode & SearchIndex) == SearchIndex)
					{
						if (Equals(sample.Key, item))
						{
							index = sampleIndex;
							count = 1;
							searchMode ^= SearchIndex;
							searchMode |= SearchCount;
						}
					}

					if ((searchMode & SearchTargetIndex) == SearchTargetIndex)
					{
						if (!EvaluateDescendant(sample.Key, parent))
						{
							searchMode ^= SearchTargetIndex;
						}
						else
						{
							targetIndex = sampleIndex;
						}
					}
					else if ((searchMode & SearchParent) == SearchParent)
					{
						if (Equals(sample.Key, parent))
						{
							targetIndex = sampleIndex;
							searchMode ^= SearchParent;
							searchMode |= SearchTargetIndex;
						}
					}

					if (searchMode == 0x00000000)
					{
						break;
					}
				}

				if (targetIndex == -1)
				{
					throw new ArgumentException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_TARGET_UNCONTAINED_ITEM_FORMAT, nameof(parent)));
				}
			}
			else
			{
				searchMode |= SearchParent;
				for (var sampleIndex = 0; sampleIndex < Count; ++sampleIndex)
				{
					var sample = Shells[sampleIndex];
					if ((searchMode & SearchCount) == SearchCount)
					{
						if (!EvaluateDescendant(sample.Key, item))
						{
							count = sampleIndex - index;
							searchMode ^= SearchCount;
						}
					}
					else if ((searchMode & SearchIndex) == SearchIndex)
					{
						if (Equals(sample.Key, item))
						{
							index = sampleIndex;
							count = 1;
							searchMode ^= SearchIndex;
							searchMode |= SearchCount;
						}
					}

					if ((searchMode & SearchTargetIndex) == SearchTargetIndex)
					{
						if (!EvaluateDescendant(sample.Key, currentParent))
						{
							searchMode ^= SearchTargetIndex;
						}
						else
						{
							targetIndex = sampleIndex;
						}
					}
					else if ((searchMode & SearchParent) == SearchParent)
					{
						if (Equals(sample.Key, currentParent))
						{
							targetIndex = sampleIndex;
							searchMode ^= SearchParent;
							searchMode |= SearchTargetIndex;
						}
					}

					if (searchMode == 0x00000000)
					{
						break;
					}
				}
			}

			SetItem(index, Shells[index].Key, parent);
			if (targetIndex < index)
			{
				var movingIndex = targetIndex + 1;
				for (var i = 0; i < count; ++i)
				{
					MoveItem(index + i, movingIndex + i);
				}
			}
			else
			{
				var movingIndex = targetIndex;
				for (var i = 0; i < count; ++i)
				{
					MoveItem(index, movingIndex);
				}
			}
		}

		/// <summary>
		/// 値の削除
		/// </summary>
		public bool Remove(T item)
		{
			var target = Keys.IndexOf(item);
			if (target == -1)
			{
				return false;
			}

			var parent = Shells[target].Core.Value;
			for (var index = 0; index < Count; ++index)
			{
				if (Shells[index].Core.Value == item)
				{
					SetItem(index, Shells[index].Key, parent);
				}
			}

			RemoveItem(target);
			return true;
		}

		/// <summary>
		/// 親子づけの解除
		/// </summary>
		public bool Unparent(T item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (!Shells.TryGetValue(item, out var property))
			{
				throw new ArgumentException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_TARGET_UNCONTAINED_ITEM_FORMAT, nameof(item)));
			}

			var currentParent = property.Core.Value;
			if (currentParent == null)
			{
				return false;
			}

			T parent = null;

			const int SearchIndex = 0x00000001 << 0;
			const int SearchCount = 0x00000001 << 1;
			const int SearchParent = 0x00000001 << 2;
			const int SearchTargetIndex = 0x00000001 << 3;
			var index = -1;
			var count = 0;
			var targetIndex = -1;
			var searchMode = SearchIndex;
			{
				searchMode |= SearchParent;
				for (var sampleIndex = 0; sampleIndex < Count; ++sampleIndex)
				{
					var sample = Shells[sampleIndex];
					if ((searchMode & SearchCount) == SearchCount)
					{
						if (!EvaluateDescendant(sample.Key, item))
						{
							count = sampleIndex - index;
							searchMode ^= SearchCount;
						}
					}
					else if ((searchMode & SearchIndex) == SearchIndex)
					{
						if (Equals(sample.Key, item))
						{
							index = sampleIndex;
							count = 1;
							searchMode ^= SearchIndex;
							searchMode |= SearchCount;
						}
					}

					if ((searchMode & SearchTargetIndex) == SearchTargetIndex)
					{
						if (!EvaluateDescendant(sample.Key, currentParent))
						{
							searchMode ^= SearchTargetIndex;
						}
						else
						{
							targetIndex = sampleIndex;
						}
					}
					else if ((searchMode & SearchParent) == SearchParent)
					{
						if (Equals(sample.Key, currentParent))
						{
							parent = sample.Core.Value;
							targetIndex = sampleIndex;
							searchMode ^= SearchParent;
							searchMode |= SearchTargetIndex;
						}
					}

					if (searchMode == 0x00000000)
					{
						break;
					}
				}
			}

			SetItem(index, Shells[index].Key, parent);
			if (targetIndex < index)
			{
				var movingIndex = targetIndex + 1;
				for (var i = 0; i < count; ++i)
				{
					MoveItem(index + i, movingIndex + i);
				}
			}
			else
			{
				var movingIndex = targetIndex;
				for (var i = 0; i < count; ++i)
				{
					MoveItem(index, movingIndex);
				}
			}

			return true;
		}

		#region protected members
		#region DataPropertyDictionaryBase<T, T> virtual member override
		protected override bool Execute(IEnumerable<DataPropertyDictionaryChangeOperation<T, T>> operations)
		{
			var ret = base.Execute(operations);
			if (ret)
			{
				Tree.Nodes.Reset();
			}

			return ret;
		}
		#endregion // DataPropertyDictionaryBase<T, T> virtual member override
		#endregion // protected members

		#region private members
		#region class DebugView
		private class DebugView
		{
			public DebugView(DataPropertyPedigree<T> target)
			{
				this.target = target;
			}

			[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
			public TreeNode[] Items
			{
				get
				{
					return this.target.Tree.Root.Children.ToArray();
				}
			}

			#region private members
			private DataPropertyPedigree<T> target;
			#endregion // private members
		}
		#endregion // class DebugView

		private Lazy<TreeBuilder> tree;

		private bool EvaluateDescendant(T item, T parent)
		{
			var ancestor = item;
			do
			{
				ancestor = Shells[ancestor].Core.Value;
				if (ancestor == parent)
				{
					return true;
				}
			}
			while (ancestor != null);

			return false;
		}
		#endregion // private members
	}
}
