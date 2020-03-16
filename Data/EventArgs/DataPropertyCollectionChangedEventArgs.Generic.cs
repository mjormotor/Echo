using System.Collections.Generic;

namespace Echo.Data
{
	/// <summary>
	/// データプロパティ配列変更後
	/// </summary>
	public class DataPropertyCollectionChangedEventArgs<T> : DataPropertyCollectionChangedEventArgs
	{
		/// <summary>
		/// 操作
		/// </summary>
		public IReadOnlyList<IReadOnlyDataPropertyCollectionChangeOperation<T>> Operations { get; }

		/// <summary>
		/// 入力時の操作
		/// </summary>
		public IReadOnlyList<IReadOnlyDataPropertyCollectionChangeOperation<T>> InputOperations { get; }

		#region protected internal members
		protected internal DataPropertyCollectionChangedEventArgs(IDataPropertyCollection dataProperty, IEnumerable<IReadOnlyDataPropertyCollectionChangeOperation<T>> operations, IEnumerable<IReadOnlyDataPropertyCollectionChangeOperation<T>> inputOperations)
			: base(dataProperty)
		{
			Operations = new List<IReadOnlyDataPropertyCollectionChangeOperation<T>>(operations);
			InputOperations = new List<IReadOnlyDataPropertyCollectionChangeOperation<T>>(inputOperations);
		}
		#endregion // protected internal members
	}
}
