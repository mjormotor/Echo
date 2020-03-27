using System;
using System.Collections.Generic;

namespace Echo.PWalkService.Core
{
	internal class SystemData
	{
		public IList<Type> TargetTypes { get; } = new List<Type>();

		public IList<Type> MarkTypes { get; } = new List<Type>();

		public IList<Type> InvokeMarkTypes { get; } = new List<Type>();

		public object[] Parameters { get; set; }

		public IList<Type> KeepOutTypes { get; set; }

		public Func<Type, object, string> SolveDictionaryItemNodeNameDelegate { get; set; }
	}
}
