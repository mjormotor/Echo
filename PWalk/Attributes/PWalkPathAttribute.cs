using System;
using System.Collections.Generic;
using System.Linq;

namespace Echo.PWalkService
{
	/// <summary>
	/// 探索路属性
	/// </summary>
	[AttributeUsage(AttributeTargetsDefinition, Inherited = true, AllowMultiple = true)]
	public class PWalkPathAttribute : PWalkAttribute
	{
		public PWalkPathAttribute(params Type[] targetTypes)
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
		#endregion // PWalkAttribute override
		#endregion // internal members

		#region protected members
		#region PWalkAttribute implement
		protected override void OnVisit(object target, PWalkContext context)
		{
			context.NextStep = context.CurrentStep + 1;
		}
		#endregion // PWalkAttribute implement
		#endregion // protected members

		#region private members
		private const AttributeTargets AttributeTargetsDefinition = AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface;
		#endregion // private members
	}
}
