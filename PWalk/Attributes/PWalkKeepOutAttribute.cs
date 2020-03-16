using System;
using System.Collections.Generic;
using System.Linq;

namespace Echo.PWalkService
{
	/// <summary>
	/// 探索禁止属性
	/// </summary>
	[AttributeUsage(AttributeTargetsDefinition, Inherited = true, AllowMultiple = true)]
	public class PWalkKeepOutAttribute : PWalkAttribute
	{
		public PWalkKeepOutAttribute(params Type[] targetTypes)
		{
			TargetTypes = targetTypes;
		}

		/// <summary>
		/// 探索対象の型
		/// </summary>
		public IReadOnlyList<Type> TargetTypes { get; }

		#region internal members
		#region PWalkAttribute override
		internal override void Visit(object target, PWalkContext context)
		{
			if (context.Core.SystemData.TargetTypes.Any(_ => TargetTypes.Any(type => _.IsAssignableFrom(type))))
			{
				OnVisit(target, context);
			}
		}
		#endregion // PWalkAttribute members
		#endregion // internal members

		#region protected members
		#region PWalkAttribute implement
		protected override void OnVisit(object target, PWalkContext context)
		{
			context.NextStep = 0;
		}
		#endregion // PWalkAttribute implement
		#endregion // protected members

		#region private members
		private const AttributeTargets AttributeTargetsDefinition = AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface;
		#endregion // private members
	}
}
