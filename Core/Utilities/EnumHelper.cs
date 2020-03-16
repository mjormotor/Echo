using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Echo
{
	public static class EnumHelper
	{
		/// <summary>
		/// キャッシュの有効化
		/// </summary>
		/// <remarks>
		/// 型解析の情報をキャッシュすることで、次回の処理を高速化できます。
		/// その代わり、メモリを使用します。
		/// </remarks>
		public static bool EnableUsingCache
		{
			get
			{
				return enumProfileCache != null;
			}

			set
			{
				if (value != EnableUsingCache)
				{
					enumProfileCache = value ? new EnumProfileCache() : null;
				}
			}
		}

		/// <summary>
		/// 第一の定義名を取得
		/// </summary>
		public static string ToPrimalName(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			var type = value.GetType();
			if (!type.IsEnum)
			{
				throw new ArgumentException(Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_NON_ENUM_TYPE);
			}

			var profile = ProfileEnum(type);

			var code = value.GetHashCode();
			if (profile.PrimalNames.TryGetValue(code, out var ret))
			{
				return ret;
			}

			return value.ToString();
		}

		/// <summary>
		/// 各フラグに分解
		/// </summary>
		public static IEnumerable<object> ToFlags(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			var type = value.GetType();
			if (!type.IsEnum)
			{
				throw new ArgumentException(Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_NON_ENUM_TYPE);
			}

			var profile = ProfileEnum(type);

			var result = new List<object>();
			var data = value.GetHashCode();
			if (data == 0x00000000)
			{
				yield return value;
				yield break;
			}

			foreach (var part in profile.Parts)
			{
				var code = part.GetHashCode();
				if ((data & code) == code)
				{
					result.Add(part);
					data &= ~code;
				}
			}

			if (data == 0x00000000)
			{
				result.Reverse();
				foreach (var ret in result)
				{
					yield return ret;
				}

				yield break;
			}

			yield return value;
		}

		#region private members
		#region class EnumProfile
		private class EnumProfile
		{
			public EnumProfile(Type type)
			{
				SetupPrimalNames(type);

				var isFlag = type.IsDefined(typeof(FlagsAttribute), false);
				if (isFlag)
				{
					SetupParts(type);
				}
			}

			public IReadOnlyList<object> Parts => this.parts;

			public IReadOnlyDictionary<int, string> PrimalNames => this.primalNames;

			#region private members
			private List<object> parts = new List<object>();
			private Dictionary<int, string> primalNames = new Dictionary<int, string>();

			private void SetupPrimalNames(Type type)
			{
				foreach (var name in Enum.GetNames(type))
				{
					var value = Enum.Parse(type, name);
					var code = value.GetHashCode();
					if (!this.primalNames.ContainsKey(code))
					{
						this.primalNames.Add(code, name);
					}
				}
			}

			private void SetupParts(Type type)
			{
				var unions = new List<object>();
				var coveredBitsMask = 0x00000000;
				foreach (var value in Enum.GetValues(type))
				{
					var code = value.GetHashCode();
					if (code.IsSingleBit())
					{
						if (!this.parts.Any(_ => _.GetHashCode() == code))
						{
							this.parts.Add(value);
							coveredBitsMask |= code;
						}
					}
					else
					{
						if (!unions.Any(_ => _.GetHashCode() == code))
						{
							unions.Add(value);
						}
					}
				}

				this.parts.AddRange(unions.Where(_ => (_.GetHashCode() & ~coveredBitsMask) != 0));
				this.parts.Sort((lhs, rhs) => rhs.GetHashCode() - lhs.GetHashCode());
			}
			#endregion // private members
		}
		#endregion // class EnumProfile

		#region class EnumProfileCache
		private class EnumProfileCache : ConcurrentDictionary<Type, EnumProfile>
		{
			public new EnumProfile this[Type key]
			{
				get
				{
					return GetOrAdd(key, _ => new EnumProfile(_));
				}
			}
		}
		#endregion // class EnumProfileCache

		private static EnumProfileCache enumProfileCache = new EnumProfileCache();

		private static EnumProfile ProfileEnum(Type type)
		{
			if (enumProfileCache != null)
			{
				return enumProfileCache[type];
			}

			return new EnumProfile(type);
		}
		#endregion // private members
	}
}
