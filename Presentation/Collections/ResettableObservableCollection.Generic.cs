using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Echo.Presentation
{
	/// <summary>
	/// 再利用可能な監視可能コレクション
	/// </summary>
	/// <remarks>
	///  ObservableCollection と同様に動作し、要素の再設定や連続操作を行えます。
	/// また Clear を呼ばれた場合に削除される要素を通知します。
	/// </remarks>
	[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	public class ResettableObservableCollection<T> : Collection<T>, IResettableObservableCollection<T>
	{
		public ResettableObservableCollection()
			: base()
		{
		}

		public ResettableObservableCollection(List<T> list)
			: base((list != null) ? new List<T>(list.Count) : list)
		{
			CopyFrom(list);
		}

		public ResettableObservableCollection(IEnumerable<T> collection)
			: base()
		{
			if (collection == null)
			{
				throw new ArgumentNullException(nameof(collection));
			}

			CopyFrom(collection);
		}

		#region IResettableObservableCollection<T> interface support
		#region IObservableCollection<T> interface support
		#region IReadOnlyObservableCollection<T> interface support
		#region INotifyCollectionChanged interface support
		public virtual event NotifyCollectionChangedEventHandler CollectionChanged;
		#endregion  // INotifyCollectionChanged interface support

		#region INotifyPropertyChanged interface support
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { PropertyChanged += value; }
			remove { PropertyChanged -= value; }
		}
		#endregion  // INotifyPropertyChanged interface support
		#endregion  // IReadOnlyObservableCollection<T> interface support

		public void Move(int oldIndex, int newIndex)
		{
			MoveItem(oldIndex, newIndex);
		}
		#endregion  // IObservableCollection<T> interface support

		public void Reset(IEnumerable<T> collection)
		{
			ResetItems(collection);
		}

		public void AddRange(IEnumerable<T> collection)
		{
			InsertItems(Count, collection);
		}

		public void InsertRange(int index, IEnumerable<T> collection)
		{
			InsertItems(index, collection);
		}

		public void RemoveRange(int index, int count)
		{
			RemoveItems(index, count);
		}
		#endregion  // IResettableObservableCollection<T> interface support

		#region protected members
		#region protected virtual members
		#region Collection<T> virtual member override
		/// <summary>
		/// 消去の操作
		/// </summary>
		protected override void ClearItems()
		{
			var removedItems = new List<T>(Items);
			CheckReentrancy();
			base.ClearItems();

			if (removedItems.Count > 0)
			{
				OnPropertyChanged(CountChangedEventArgs);
				OnPropertyChanged(IndexerChangedEventArgs);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItems, 0));
			}

			OnCollectionChanged(CollectionResetEventArgs);
		}

		/// <summary>
		/// 挿入の操作
		/// </summary>
		protected override void InsertItem(int index, T item)
		{
			CheckReentrancy();
			base.InsertItem(index, item);

			OnPropertyChanged(CountChangedEventArgs);
			OnPropertyChanged(IndexerChangedEventArgs);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
		}

		/// <summary>
		/// 削除の操作
		/// </summary>
		protected override void RemoveItem(int index)
		{
			CheckReentrancy();
			var removedItem = this[index];

			base.RemoveItem(index);

			OnPropertyChanged(CountChangedEventArgs);
			OnPropertyChanged(IndexerChangedEventArgs);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
		}

		/// <summary>
		/// 設定の操作
		/// </summary>
		protected override void SetItem(int index, T item)
		{
			CheckReentrancy();
			var originalItem = this[index];
			base.SetItem(index, item);

			OnPropertyChanged(IndexerChangedEventArgs);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, originalItem, item, index));
		}
		#endregion  // Collection<T> virtual member override

		#region INotifyPropertyChanged interface support
		protected virtual event PropertyChangedEventHandler PropertyChanged;
		#endregion  // INotifyPropertyChanged interface support

		/// <summary>
		/// 移動の操作
		/// </summary>
		protected virtual void MoveItem(int oldIndex, int newIndex)
		{
			CheckReentrancy();

			if (oldIndex != newIndex)
			{
				var removedItem = this[oldIndex];

				base.RemoveItem(oldIndex);
				base.InsertItem(newIndex, removedItem);

				OnPropertyChanged(IndexerChangedEventArgs);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, removedItem, newIndex, oldIndex));
			}
		}

		/// <summary>
		/// 再設定の操作
		/// </summary>
		protected virtual void ResetItems(IEnumerable<T> items)
		{
			ClearItems();
			InsertItems(0, items);
		}

		/// <summary>
		/// 連続挿入の操作
		/// </summary>
		protected virtual void InsertItems(int index, IEnumerable<T> items)
		{
			CheckReentrancy();

			if (Items is List<T> list)
			{
				list.AddRange(items);
			}
			else
			{
				var insertingIndex = index;
				foreach (var item in items)
				{
					base.InsertItem(insertingIndex, item);
					++insertingIndex;
				}
			}

			if (items.Any())
			{
				OnPropertyChanged(CountChangedEventArgs);
				OnPropertyChanged(IndexerChangedEventArgs);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items), index));
			}
		}

		/// <summary>
		/// 連続削除の操作
		/// </summary>
		protected virtual void RemoveItems(int index, int count)
		{
			CheckReentrancy();

			List<T> removedItems = null;
			if (Items is List<T> list)
			{
				removedItems = list.GetRange(index, count);

				list.RemoveRange(index, count);
			}
			else
			{
				removedItems = new List<T>(count);
				var insertingIndex = index;
				for (var i = 0; i < count; ++i)
				{
					removedItems.Add(this[index]);
					base.RemoveItem(index);
				}
			}

			if (count > 0)
			{
				OnPropertyChanged(CountChangedEventArgs);
				OnPropertyChanged(IndexerChangedEventArgs);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItems, index));
			}
		}

		/// <summary>
		/// 要素変更時の処理
		/// </summary>
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (CollectionChanged != null)
			{
				using (BlockReentrancy())
				{
					CollectionChanged(this, e);
				}
			}
		}

		/// <summary>
		/// プロパティ変更時の処理
		/// </summary>
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, e);
			}
		}
		#endregion // protected virtual members

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
				if ((CollectionChanged != null) && (CollectionChanged.GetInvocationList().Length > 1))
				{
					throw new InvalidOperationException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_INVALID_OPERATION_REENTRANCY_FORMAT, nameof(ResettableObservableCollection<T>), nameof(CollectionChanged)));
				}
			}
		}
		#endregion // protected members

		#region private members
		private const string IndexerName = "Item[]";
		private static readonly PropertyChangedEventArgs CountChangedEventArgs = new PropertyChangedEventArgs(nameof(Count));
		private static readonly PropertyChangedEventArgs IndexerChangedEventArgs = new PropertyChangedEventArgs(IndexerName);
		private static readonly NotifyCollectionChangedEventArgs CollectionResetEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

		private ScopeWatcher watcher = new ScopeWatcher();

		private void CopyFrom(IEnumerable<T> collection)
		{
			using (var enumerator = collection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Items.Add(enumerator.Current);
				}
			}
		}
		#endregion // private members
	}
}
