namespace Echo.Data
{
	/// <summary>
	/// データプロパティ配列値コンテキスト
	/// </summary>
	public class DataPropertyCollectionValueContext<T> : IReadOnlyDataPropertyCollectionValueContext<T>
	{
		/// <summary>
		/// 変更後の値
		/// </summary>
		public T NewValue { get; set; }

		/// <summary>
		/// 入力時の値
		/// </summary>
		public T InputValue { get; }

		/// <summary>
		/// 変更前の値
		/// </summary>
		public T OldValue { get; }

		#region internal members
		internal DataPropertyCollectionValueContext(DataProperty<T> property)
		{
			NewValue = property.Value;
			InputValue = property.Value;
			OldValue = default;
			Property = property;
		}

		internal DataPropertyCollectionValueContext(T oldValue, T inputValue, DataProperty<T> property)
		{
			NewValue = inputValue;
			InputValue = inputValue;
			OldValue = oldValue;
			Property = property;
		}

		internal DataProperty<T> Property { get; }
		#endregion // internal members
	}
}
