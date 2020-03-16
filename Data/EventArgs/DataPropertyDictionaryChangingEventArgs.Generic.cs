using System.Collections.Generic;

namespace Echo.Data
{
	/// <summary>
	/// データプロパティ辞書変更前
	/// </summary>
	public class DataPropertyDictionaryChangingEventArgs<TKey, TValue> : DataPropertyDictionaryChangingEventArgs
	{
		/// <summary>
		/// 操作
		/// </summary>
		public IList<DataPropertyDictionaryChangeOperation<TKey, TValue>> Operations { get; }

		/// <summary>
		/// 入力時の操作
		/// </summary>
		public IReadOnlyList<IReadOnlyDataPropertyDictionaryChangeOperation<TKey, TValue>> InputOperations { get; }

		#region protected internal members
		protected internal DataPropertyDictionaryChangingEventArgs(IDataPropertyDictionary dataProperty, IEnumerable<DataPropertyDictionaryChangeOperation<TKey, TValue>> operations)
			: base(dataProperty)
		{
			Operations = new List<DataPropertyDictionaryChangeOperation<TKey, TValue>>(operations);
			InputOperations = new List<DataPropertyDictionaryChangeOperation<TKey, TValue>>(operations);
		}

		protected internal DataPropertyDictionaryChangingEventArgs(IDataPropertyDictionary dataProperty, DataPropertyDictionaryChangeOperation<TKey, TValue> operation)
			: this(dataProperty, operation.Enumerate())
		{
		}
		#endregion // protected internal members
	}
}
