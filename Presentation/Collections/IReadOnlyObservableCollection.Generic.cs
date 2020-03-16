using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Echo.Presentation
{
	/// <summary>
	/// 読み取り専用の監視可能コレクション
	/// </summary>
	public interface IReadOnlyObservableCollection<out T> : IReadOnlyIndexer<int, T>, IReadOnlyList<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
	}
}
