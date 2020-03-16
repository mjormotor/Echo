namespace Echo.Data
{
	/// <summary>
	/// データプロパティ変更前
	/// </summary>
	public class DataPropertyChangingEventArgs<T> : DataPropertyChangingEventArgs
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

		#region protected internal members
		protected internal DataPropertyChangingEventArgs(IDataPropertyCore dataProperty, T inputValue, T oldValue)
			: base(dataProperty)
		{
			NewValue = inputValue;
			InputValue = inputValue;
			OldValue = oldValue;
		}
		#endregion // protected internal members
	}
}
