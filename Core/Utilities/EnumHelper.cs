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
		/// 組み合わせを列挙
		/// </summary>
		public static IEnumerable<T> EnumerateCombinations<T>()
			where T : Enum
		{
			var profile = ProfileEnum(typeof(T));
			return profile.Combinations.Cast<T>();
		}

		/// <summary>
		/// 組み合わせを列挙
		/// </summary>
		public static IList<T> EnumerateCombinations<T>(IEnumerable<T> values)
			where T : Enum
		{
			var ret = new List<T>();

			var type = typeof(T);
			var isFlag = type.IsDefined(typeof(FlagsAttribute), false);
			if (isFlag)
			{
				var zero = (T)Enum.ToObject(type, 0x00000000);
				if (Enum.IsDefined(type, 0x00000000))
				{
					ret.Add(zero);
				}

				foreach (var value in values.Aggregate(zero.Enumerate(), (accumlate, _) => Combine(accumlate, _)))
				{
					var code = value.GetHashCode();
					if (code != 0x00000000 && !ret.Any(_ => _.GetHashCode() == code))
					{
						ret.Add(value);
					}
				}
			}
			else
			{
				foreach (var value in values)
				{
					var code = value.GetHashCode();
					if (!ret.Any(_ => _.GetHashCode() == code))
					{
						ret.Add(value);
					}
				}
			}

			return ret;

			#region method Combine
			IEnumerable<T> Combine(IEnumerable<T> accumlate, object part)
			{
				var code = part.GetHashCode();
				return accumlate.Concat(accumlate.Select(_ => (T)Enum.ToObject(type, _.GetHashCode() + code)));
			}
			#endregion // method Combine
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

			foreach (var part in profile.Parts.Reverse())
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
				Type = type;
				IsFlag = Type.IsDefined(typeof(FlagsAttribute), false);

				this.parts = new Lazy<List<object>>(GenerateParts);
				this.combinations = new Lazy<List<object>>(GenerateCombinations);
				this.primalNames = new Lazy<Dictionary<int, string>>(GeneratePrimalNames);
			}

			public Type Type { get; }

			public bool IsFlag { get; }

			public IReadOnlyList<object> Parts => this.parts.Value;

			public IReadOnlyList<object> Combinations => this.combinations.Value;

			public IReadOnlyDictionary<int, string> PrimalNames => this.primalNames.Value;

			#region private members
			private Lazy<List<object>> parts;
			private Lazy<List<object>> combinations;
			private Lazy<Dictionary<int, string>> primalNames;

			private List<object> GenerateParts()
			{
				var ret = new List<object>();

				if (IsFlag)
				{
					var unions = new List<object>();
					var coveredBitsMask = 0x00000000;
					foreach (var value in Enum.GetValues(Type))
					{
						var code = value.GetHashCode();
						if (code.IsSingleBit())
						{
							if (!ret.Any(_ => _.GetHashCode() == code))
							{
								ret.Add(value);
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

					ret.AddRange(unions.Where(_ => (_.GetHashCode() & ~coveredBitsMask) != 0));
					ret.Sort((lhs, rhs) => lhs.GetHashCode() - rhs.GetHashCode());
				}

				return ret;
			}

			private List<object> GenerateCombinations()
			{
				var ret = new List<object>();

				if (IsFlag)
				{
					var zero = Enum.ToObject(Type, 0x00000000);
					if (Enum.IsDefined(Type, 0x00000000))
					{
						ret.Add(zero);
					}

					foreach (var value in Parts.Aggregate(zero.Enumerate(), (accumlate, _) => Combine(accumlate, _)))
					{
						var code = value.GetHashCode();
						if (code != 0x00000000 && !ret.Any(_ => _.GetHashCode() == code))
						{
							ret.Add(value);
						}
					}
				}
				else
				{
					foreach (var value in Enum.GetValues(Type))
					{
						var code = value.GetHashCode();
						if (!ret.Any(_ => _.GetHashCode() == code))
						{
							ret.Add(value);
						}
					}
				}

				return ret;

				#region method Combine
				IEnumerable<object> Combine(IEnumerable<object> accumlate, object part)
				{
					var code = part.GetHashCode();
					return accumlate.Concat(accumlate.Select(_ => Enum.ToObject(Type, _.GetHashCode() + code)));
				}
				#endregion // method Combine
			}

			private Dictionary<int, string> GeneratePrimalNames()
			{
				var ret = new Dictionary<int, string>();
				foreach (var name in Enum.GetNames(Type))
				{
					var value = Enum.Parse(Type, name);
					var code = value.GetHashCode();
					if (!ret.ContainsKey(code))
					{
						ret.Add(code, name);
					}
				}

				return ret;
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
