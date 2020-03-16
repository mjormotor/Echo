using System.Collections.Generic;

namespace Echo.Data
{
	/// <summary>
	/// データプロパティ辞書変更後
	/// </summary>
	public class DataPropertyDictionaryChangedEventArgs<TKey, TValue> : DataPropertyDictionaryChangedEventArgs
	{
		/// <summary>
		/// 操作
		/// </summary>
		public IReadOnlyList<IReadOnlyDataPropertyDictionaryChangeOperation<TKey, TValue>> Operations { get; }

		/// <summary>
		/// 入力時の操作
		/// </summary>
		public IReadOnlyList<IReadOnlyDataPropertyDictionaryChangeOperation<TKey, TValue>> InputOperations { get; }

		#region protected internal members
		protected internal DataPropertyDictionaryChangedEventArgs(IDataPropertyDictionary dataProperty, IEnumerable<IReadOnlyDataPropertyDictionaryChangeOperation<TKey, TValue>> operations, IEnumerable<IReadOnlyDataPropertyDictionaryChangeOperation<TKey, TValue>> inputOperations)
			: base(dataProperty)
		{
			Operations = new List<IReadOnlyDataPropertyDictionaryChangeOperation<TKey, TValue>>(operations);
			InputOperations = new List<IReadOnlyDataPropertyDictionaryChangeOperation<TKey, TValue>>(inputOperations);
		}
		#endregion // protected internal members
	}
}
