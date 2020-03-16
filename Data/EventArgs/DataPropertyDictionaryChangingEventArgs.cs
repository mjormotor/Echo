namespace Echo.Data
{
	/// <summary>
	/// データプロパティ辞書変更前
	/// </summary>
	public abstract class DataPropertyDictionaryChangingEventArgs : DataPropertyChangingEventArgs
	{
		#region protected members
		protected DataPropertyDictionaryChangingEventArgs(IDataPropertyDictionary dataProperty)
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
