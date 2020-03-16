using System.Collections.Generic;

namespace Echo.Data
{
	/// <summary>
	/// データプロパティ配列変更前
	/// </summary>
	public class DataPropertyCollectionChangingEventArgs<T> : DataPropertyCollectionChangingEventArgs
	{
		/// <summary>
		/// 操作
		/// </summary>
		public IList<DataPropertyCollectionChangeOperation<T>> Operations { get; }

		/// <summary>
		/// 入力時の操作
		/// </summary>
		public IReadOnlyList<IReadOnlyDataPropertyCollectionChangeOperation<T>> InputOperations { get; }

		#region protected internal members
		protected internal DataPropertyCollectionChangingEventArgs(IDataPropertyCollection dataProperty, IEnumerable<DataPropertyCollectionChangeOperation<T>> operations)
			: base(dataProperty)
		{
			Operations = new List<DataPropertyCollectionChangeOperation<T>>(operations);
			InputOperations = new List<DataPropertyCollectionChangeOperation<T>>(operations);
		}

		protected internal DataPropertyCollectionChangingEventArgs(IDataPropertyCollection dataProperty, DataPropertyCollectionChangeOperation<T> operation)
			: this(dataProperty, operation.Enumerate())
		{
		}
		#endregion // protected internal members
	}
}
