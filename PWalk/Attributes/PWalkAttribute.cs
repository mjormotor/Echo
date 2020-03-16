using System;

namespace Echo.PWalkService
{
	/// <summary>
	/// 探索時処理属性
	/// </summary>
	[AttributeUsage(AttributeTargetsDefinition, Inherited = true, AllowMultiple = true)]
	public abstract class PWalkAttribute : Attribute
	{
		#region internal members
		#region internal abstract members
		internal virtual void Visit(object target, PWalkContext context)
		{
			OnVisit(target, context);
		}
		#endregion // internal abstract members
		#endregion // internal members

		#region protected members
		protected PWalkAttribute()
		{
		}

		#region protected abstract members
		/// <summary>
		/// 探索時処理
		/// </summary>
		protected abstract void OnVisit(object target, PWalkContext context);
		#endregion // protected abstract members
		#endregion // protected members

		#region private members
		private const AttributeTargets AttributeTargetsDefinition = AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface;
		#endregion // private members
	}
}
