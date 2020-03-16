namespace Echo.Data
{
	/// <summary>
	/// データプロパティ変更後
	/// </summary>
	public class DataPropertyChangedEventArgs<T> : DataPropertyChangedEventArgs
	{
		/// <summary>
		/// 変更後の値
		/// </summary>
		public T NewValue { get; }

		/// <summary>
		/// 入力時の値
		/// </summary>
		public T InputValue { get; }

		/// <summary>
		/// 変更前の値
		/// </summary>
		public T OldValue { get; }

		#region protected internal members
		protected internal DataPropertyChangedEventArgs(IDataPropertyCore dataProperty, T newValue, T inputValue, T oldValue)
			: base(dataProperty)
		{
			NewValue = newValue;
			InputValue = inputValue;
			OldValue = oldValue;
		}
		#endregion // protected internal members
	}
}
