using System;

namespace Echo.PWalkService
{
	/// <summary>
	/// 探索の添付データ属性
	/// </summary>
	[AttributeUsage(AttributeTargetsDefinition, Inherited = true, AllowMultiple = true)]
	public abstract class PWalkOptionDataAttribute : Attribute
	{
		#region protected members
		protected PWalkOptionDataAttribute()
		{
		}
		#endregion // protected members

		#region private members
		private const AttributeTargets AttributeTargetsDefinition = AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface;
		#endregion // private members
	}
}
