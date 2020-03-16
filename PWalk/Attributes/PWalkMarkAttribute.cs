using System;

namespace Echo.PWalkService
{
	/// <summary>
	/// 探索標識属性
	/// </summary>
	[AttributeUsage(AttributeTargetsDefinition, Inherited = true)]
	public abstract class PWalkMarkAttribute : Attribute
	{
		#region private members
		private const AttributeTargets AttributeTargetsDefinition = AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface;
		#endregion // private members
	}
}
