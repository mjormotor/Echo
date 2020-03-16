using System;
using System.ComponentModel;

namespace Echo.Data
{
	/// <summary>
	/// データプロパティ変更前
	/// </summary>
	public abstract class DataPropertyChangingEventArgs : CancelEventArgs
	{
		/// <summary>
		/// データプロパティ名
		/// </summary>
		public string DataPropertyName
		{
			get { return PrepareDataPropertyName(); }
		}

		#region protected members
		protected DataPropertyChangingEventArgs(IDataPropertyCore dataProperty)
		{
			this.dataProperty = new WeakReference(dataProperty);
		}

		/// <summary>
		/// データプロパティ
		/// </summary>
		protected IDataPropertyCore DataProperty
		{
			get
			{
				return (IDataPropertyCore)this.dataProperty.Target;
			}
		}
		#endregion // protected members

		#region private members
		private WeakReference dataProperty;
		private string dataPropertyName;

		private string PrepareDataPropertyName()
		{
			if (this.dataPropertyName == null)
			{
				var dataProperty = DataProperty;
				if (dataProperty != null)
				{
					this.dataPropertyName = dataProperty.Name;
				}
			}

			return this.dataPropertyName;
		}
		#endregion // private members
	}
}
