namespace Echo.Data
{
	/// <summary>
	/// データプロパティ配列変更後
	/// </summary>
	public abstract class DataPropertyCollectionChangedEventArgs : DataPropertyChangedEventArgs
	{
		#region protected members
		protected DataPropertyCollectionChangedEventArgs(IDataPropertyCollection dataProperty)
			: base(dataProperty)
		{
		}

		/// <summary>
		/// データプロパティ配列
		/// </summary>
		protected IDataPropertyCollection DataPropertyCollection
		{
			get
			{
				return (IDataPropertyCollection)base.DataProperty;
			}
		}
		#endregion // protected members
	}
}
