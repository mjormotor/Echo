using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Echo.Data
{
	/// <summary>
	/// データプロパティ系譜
	/// </summary>
	[System.Diagnostics.DebuggerTypeProxy(typeof(DataPropertyPedigree<>.DebugView))]
	public class DataPropertyPedigree<T> : DataPropertyDictionaryBase<T, T>
		where T : class
	{
		#region class TreeNode
		/// <summary>
		/// 樹形図のノード
		/// </summary>
		[System.Diagnostics.DebuggerDisplay("{Core}")]
		[System.Diagnostics.DebuggerTypeProxy(typeof(DataPropertyPedigree<>.TreeNode.DebugView))]
		public sealed class TreeNode : IReadOnlyShell<T>
		{
			#region class ChildTreeNodeCollection
			[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
			[System.Diagnostics.DebuggerTypeProxy(typeof(DataPropertyPedigree<>.TreeNode.ChildTreeNodeCollection.DebugView))]
			public sealed class ChildTreeNodeCollection : IReadOnlyList<TreeNode>
			{
				public ChildTreeNodeCollection(TreeNode owner)
				{
					this.owner = owner;
				}

				/// <summary>
				/// 番号の検索
				/// </summary>
				public int IndexOf(T value)
				{
					for (var index = 0; index < Count; ++index)
					{
						var sample = this[index];
						if (Equals(sample.Core, value))
						{
							return index;
						}
					}

					return -1;
				}

				#region IReadOnlyList<TreeNode> interface support
				#region IReadOnlyCollection<TreeNode> interface support
				#region IEnumerable<TreeNode> interface support
				#region IEnumerable interface support
				IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
				#endregion  // IEnumerable interface support

				public IEnumerator<TreeNode> GetEnumerator()
				{
					for (var index = 0; index < Count; ++index)
					{
						yield return this[index];
					}
				}
				#endregion  // IEnumerable<TreeNode> interface support

				public int Count
				{
					get
					{
						var indices = PrepareIndices();
						return indices.Length;
					}
				}
				#endregion  // IReadOnlyCollection<TreeNode> interface support

				public TreeNode this[int index]
				{
					get
					{
						var indices = PrepareIndices();
						return this.owner.owner.Tree.Nodes[indices[index]];
					}
				}
				#endregion  // IReadOnlyList<TreeNode> interface support

				#region private members
				#region class DebugView
				private class DebugView
				{
					public DebugView(ChildTreeNodeCollection target)
					{
						this.target = target;
					}

					[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
					public TreeNode[] Items => this.target.ToArray();

					#region private members
					private ChildTreeNodeCollection target;
					#endregion // private members
				}
				#endregion // class DebugView

				private TreeNode owner;
				private int[] indices;

				private int[] PrepareIndices()
				{
					if (!this.owner.IsValid())
					{
						throw new InvalidOperationException(Properties.Resources.MESSAGE_EXCEPTION_INVALID_OPERATION_KEEP_ENUMERATION_WITH_MODIFIED_ORGANIZATION);
					}

					if (this.indices == null)
					{
						var item = this.owner.Core;
						if (item != null)
						{
							this.indices = this.owner.owner.Shells
								.Select((_, index) => (_, index))
								.Skip(this.owner.index + 1)
								.TakeWhile(_ => this.owner.owner.EvaluateDescendant(_._.Key, item))
								.Where(_ => _._.Core.Value == item)
								.Select(_ => _.index)
								.ToArray();
						}
						else
						{
							this.indices = this.owner.owner.Shells
								.Select((_, index) => (_, index))
								.Where(_ => _._.Core.Value == null)
								.Select(_ => _.index)
								.ToArray();
						}
					}

					return this.indices;
				}
				#endregion // private members
			}
			#endregion // class ChildTreeNodeCollection

			/// <summary>
			/// 子ノード
			/// </summary>
			public ChildTreeNodeCollection Children { get; }

			#region IReadOnlyShell<T> interface support
			public T Core { get; }
			#endregion  // IReadOnlyShell<T> interface support

			#region internal members
			internal TreeNode(DataPropertyPedigree<T> owner, int index)
			{
				if (owner == null)
				{
					throw new ArgumentNullException(nameof(owner));
				}

				this.owner = owner;
				this.index = index;

				Children = new ChildTreeNodeCollection(this);
				Core = this.owner.Shells[this.index].Key;
			}

			internal TreeNode(DataPropertyPedigree<T> owner)
			{
				if (owner == null)
				{
					throw new ArgumentNullException(nameof(owner));
				}

				this.owner = owner;

				Children = new ChildTreeNodeCollection(this);
			}

			internal bool IsValid()
			{
				if (Core != null)
				{
					return this.owner.Tree.Nodes[this.index] == this;
				}

				return this.owner.Tree.Root == this;
			}
			#endregion // internal members

			#region private members
			#region class DebugView
			private class DebugView
			{
				#region class Core
				[System.Diagnostics.DebuggerDisplay("{core}", Name = nameof(Core))]
				public class Core
				{
					public Core(T core)
					{
						this.core = core;
					}

					#region private members
					[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
					private T core;
					#endregion // private members
				}
				#endregion // class Core

				#region class Children
				[System.Diagnostics.DebuggerDisplay("Count = {core.Count}", Name = nameof(Children))]
				public class Children
				{
					public Children(ChildTreeNodeCollection core)
					{
						this.core = core;
					}

					#region private members
					[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
					private ChildTreeNodeCollection core;
					#endregion // private members
				}
				#endregion // class Children

				public DebugView(TreeNode target)
				{
					core = new Core(target.Core);
					items = new Children(target.Children);
				}

				public Core core { get; }

				public Children items { get; }
			}
			#endregion // class DebugView

			private DataPropertyPedigree<T> owner;
			private int index;
			#endregion // private members
		}
		#endregion // class TreeNode

		#region class TreeBuilder
		/// <summary>
		/// 樹形図の構築
		/// </summary>
		[System.Diagnostics.DebuggerTypeProxy(typeof(DataPropertyPedigree<>.TreeBuilder.DebugView))]
		public sealed class TreeBuilder
		{
			public TreeBuilder(DataPropertyPedigree<T> owner)
			{
				if (owner == null)
				{
					throw new ArgumentNullException(nameof(owner));
				}

				this.owner = owner;

				Nodes = new TreeNodeCache(this);
			}

			public TreeNode Root => Nodes.Root;

			#region internal members
			#region class TreeNodeCache
			internal class TreeNodeCache : IReadOnlyIndexer<int, TreeNode>
			{
				public TreeNodeCache(TreeBuilder owner)
				{
					this.owner = owner;
				}

				public TreeNode Root
				{
					get
					{
						var items = PrepareItems();
						var ret = items[0];
						if (ret == null)
						{
							ret = new TreeNode(this.owner.owner);
							items[0] = ret;
						}

						return ret;
					}
				}

				public TreeNode this[int index]
				{
					get
					{
						var items = PrepareItems();
						var ret = items[index + 1];
						if (ret == null)
						{
							ret = new TreeNode(this.owner.owner, index);
							items[index + 1] = ret;
						}

						return ret;
					}
				}

				public void Reset()
				{
					this.items = null;
				}

				#region private members
				private TreeBuilder owner;
				private TreeNode[] items;

				private TreeNode[] PrepareItems()
				{
					if (this.items == null)
					{
						this.items = new TreeNode[this.owner.owner.Count + 1];
					}

					return this.items;
				}
				#endregion // private members
			}
			#endregion // class TreeNodeCache

			internal TreeNodeCache Nodes { get; }
			#endregion // internal members

			#region private members
			#region class DebugView
			private class DebugView
			{
				public DebugView(TreeBuilder target)
				{
					this.target = target;
				}

				[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
				public TreeNode[] Items
				{
					get
					{
						return this.target.Root.Children.ToArray();
					}
				}

				#region private members
				private TreeBuilder target;
				#endregion // private members
			}
			#endregion // class DebugView

			private DataPropertyPedigree<T> owner;
			#endregion // private members
		}
		#endregion // class TreeBuilder

		public DataPropertyPedigree([CallerMemberName] string name = null)
			: base(name)
		{
		}

		public DataPropertyPedigree(IEqualityComparer<T> comparer, [CallerMemberName] string name = null)
			: base(comparer, name)
		{
		}

		/// <summary>
		/// 樹形図
		/// </summary>
		public TreeBuilder Tree => PrepareTree();

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

		private TreeBuilder tree;

		private TreeBuilder PrepareTree()
		{
			if (this.tree == null)
			{
				this.tree = new TreeBuilder(this);
			}

			return this.tree;
		}

		private bool EvaluateDescendant(T item, T parent)
		{
			var ancestor = item;
			do
			{
				ancestor = this[ancestor].Value;
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
