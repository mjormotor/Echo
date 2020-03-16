namespace Echo.Data
{
	/// <summary>
	/// データプロパティ配列変更前
	/// </summary>
	public abstract class DataPropertyCollectionChangingEventArgs : DataPropertyChangingEventArgs
	{
		#region protected members
		protected DataPropertyCollectionChangingEventArgs(IDataPropertyCollection dataProperty)
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
