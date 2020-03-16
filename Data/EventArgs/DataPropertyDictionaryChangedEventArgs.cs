namespace Echo.Data
{
	/// <summary>
	/// データプロパティ辞書変更後
	/// </summary>
	public abstract class DataPropertyDictionaryChangedEventArgs : DataPropertyChangedEventArgs
	{
		#region protected members
		protected DataPropertyDictionaryChangedEventArgs(IDataPropertyDictionary dataProperty)
			: base(dataProperty)
		{
		}

		/// <summary>
		/// データプロパティ辞書
		/// </summary>
		protected IDataPropertyDictionary DataPropertyDictionary
		{
			get
			{
				return (IDataPropertyDictionary)base.DataProperty;
			}
		}
		#endregion // protected members
	}
}
