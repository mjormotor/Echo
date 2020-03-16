using System;

namespace Echo.Data
{
	/// <summary>
	/// データプロパティ
	/// </summary>
	[DataProperty]
	public interface IDataPropertyCore
	{
		/// <summary>
		/// 名前
		/// </summary>
		string Name { get; }

		/// <summary>
		/// プロパティ型
		/// </summary>
		Type PropertyType { get; }
	}
}
