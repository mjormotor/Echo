using System;
using System.Collections.Concurrent;

namespace Echo.PWalkService.Core
{
	internal static class TypeProfileHelper
	{
		public static bool EnableUsingCache
		{
			get
			{
				return cache != null;
			}

			set
			{
				if (value != EnableUsingCache)
				{
					cache = value ? new Cache() : null;
				}
			}
		}

		public static TypeProfile Profile(this Type type)
		{
			if (cache != null)
			{
				return cache[type];
			}

			return new TypeProfile(type);
		}

		#region private members
		#region class Cache
		private class Cache : ConcurrentDictionary<Type, TypeProfile>
		{
			public new TypeProfile this[Type key]
			{
				get
				{
					return GetOrAdd(key, _ => new TypeProfile(_));
				}
			}
		}
		#endregion // class Cache

		private static Cache cache = new Cache();
		#endregion // private members
	}
}
