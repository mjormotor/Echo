namespace Echo.Data
{
	/// <summary>
	/// データプロパティ値コンテキスト
	/// </summary>
	public class DataPropertyValueContext<T> : IReadOnlyDataPropertyValueContext<T>
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
		internal DataPropertyValueContext(DataProperty<T> property)
		{
			NewValue = property.Value;
			InputValue = property.Value;
			OldValue = default(T);
			Property = property;
		}

		internal DataPropertyValueContext(T value, DataProperty<T> property)
		{
			NewValue = value;
			InputValue = value;
			OldValue = property.Value;
			Property = property;
		}

		internal DataProperty<T> Property { get; }
		#endregion // internal members
	}
}
