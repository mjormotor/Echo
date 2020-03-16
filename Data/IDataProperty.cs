using System;

namespace Echo.Data
{
	/// <summary>
	/// データプロパティ
	/// </summary>
	[DataProperty]
	public interface IDataProperty : IDataPropertyCore
	{
		/// <summary>
		/// プロパティ変更前
		/// </summary>
		event EventHandler<DataPropertyChangingEventArgs> DataPropertyChanging;

		/// <summary>
		/// プロパティ変更後
		/// </summary>
		event EventHandler<DataPropertyChangedEventArgs> DataPropertyChanged;

		/// <summary>
		/// 名前
		/// </summary>
		new string Name { get; set; }

		/// <summary>
		/// 値
		/// </summary>
		object Value { get; set; }
	}
}
