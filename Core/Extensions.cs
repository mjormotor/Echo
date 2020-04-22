using System;
using System.Collections.Generic;
using System.Linq;

namespace Echo
{
	public static class Extensions
	{
		/// <summary>
		/// 単ビットまたはゼロ判定
		/// </summary>
		public static bool IsSingleBitOrZero(this int self)
		{
			return (self & (self - 0x00000001)) == 0x00000000;
		}

		/// <summary>
		/// 単ビット判定
		/// </summary>
		public static bool IsSingleBit(this int self)
		{
			return self != 0x00000000 && IsSingleBitOrZero(self);
		}

		/// <summary>
		/// 第一の定義名を取得
		/// </summary>
		public static string ToPrimalName<T>(this T self)
			where T : Enum
		{
			return EnumHelper.ToPrimalName(self);
		}

		/// <summary>
		/// 各フラグに分解
		/// </summary>
		public static IEnumerable<T> ToFlags<T>(this T self)
			where T : Enum
		{
			return EnumHelper.ToFlags(self).Cast<T>();
		}

		/// <summary>
		/// null 許容型判定
		/// </summary>
		public static bool IsNullable(this Type self)
		{
			return self.IsGenericType && self.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		/// <summary>
		/// 継承距離の算出
		/// </summary>
		public static int EvaluateInheritanceDistance(this Type self, Type target)
		{
			if (target == null)
			{
				throw new ArgumentNullException(nameof(target));
			}

			var ret = -1;
			if (target.IsAssignableFrom(self))
			{
				if (target == self)
				{
					ret = 0;
				}
				else
				{
					ret = 1;
					if (target.IsInterface)
					{
						if (!self.IsInterface)
						{
							var type = self.BaseType;
							while (type != null)
							{
								if (!type.GetInterfaces().Contains(target))
								{
									break;
								}

								++ret;
								type = type.BaseType;
							}
						}
					}
					else
					{
						var type = self.BaseType;
						while (type != null)
						{
							if (type == target)
							{
								break;
							}

							++ret;
							type = type.BaseType;
						}
					}
				}
			}

			return ret;
		}

		/// <summary>
		/// 羅列
		/// </summary>
		public static IEnumerable<T> Enumerate<T>(this T self)
		{
			yield return self;
		}

		/// <summary>
		/// 羅列
		/// </summary>
		public static IEnumerable<T> EnumerateWith<T>(this T self, params T[] succeeded)
		{
			yield return self;
			foreach (var item in succeeded)
			{
				yield return item;
			}
		}

		/// <summary>
		/// 網羅
		/// </summary>
		public static IEnumerable<T> Align<T>(this IEnumerable<IEnumerable<T>> self)
		{
			return self.Aggregate((accumulate, _) => accumulate.Concat(_));
		}

		/// <summary>
		/// 網羅
		/// </summary>
		public static IEnumerable<T> Align<T>(this T self, Func<T, IEnumerable<T>> vary)
		{
			return vary(self);
		}

		/// <summary>
		/// 網羅
		/// </summary>
		public static IEnumerable<T> Align<T>(this IEnumerable<T> self, Func<T, IEnumerable<T>> vary)
		{
			return self.Select(_ => vary(_)).Align();
		}

		/// <summary>
		/// 番号の賦与
		/// </summary>
		public static IEnumerable<(T, int)> WithIndices<T>(this IEnumerable<T> self)
		{
			return self.Select((_, index) => (_, index));
		}

		/// <summary>
		/// 該当するキーの収集
		/// </summary>
		public static IEnumerable<TKey> FetchKeys<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> self, TValue value)
		{
			foreach (var item in self)
			{
				if (Equals(item.Value, value))
				{
					yield return item.Key;
				}
			}
		}
	}
}
