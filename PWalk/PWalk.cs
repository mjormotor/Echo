using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Echo.PWalkService
{
	using Core;
    using System.Collections;

    /// <summary>
    /// 属性対象処理
    /// </summary>
    public delegate void TargetAttributeDelegate<T>(object targrt, T attribute, PWalkContext context) where T : Attribute;

	/// <summary>
	/// プロパティ探索
	/// </summary>
	/// <remarks>
	/// 入力されたオブジェクトのプロパティを探索し、それに対する処理を実行します。
	/// </remarks>
	public static class PWalk
	{
		#region enum ProxyParameter
		/// <summary>
		/// 代替引数
		/// </summary>
		/// <remarks>
		/// InvokeMarkedWith メソッドの parameters としてこれらを入力すると、
		/// 探索中の動的なインスタンスに置換されて渡されます。
		/// </remarks>
		public enum ProxyParameter
		{
			/// <summary>
			/// 現在値
			/// </summary>
			Current,

			/// <summary>
			/// コンテキスト
			/// </summary>
			Context,

			/// <summary>
			/// 標識
			/// </summary>
			Mark,
		}
		#endregion // enum ProxyParameter

		#region enum Option
		/// <summary>
		/// 処理のオプション動作
		/// </summary>
		[Flags]
		public enum Option
		{
			/// <summary>
			/// オプション動作なし
			/// </summary>
			None = 0x00000000,

			/// <summary>
			/// 復路でも処理を呼ぶ
			/// </summary>
			CallProcessAlsoOnWayBack = 0x00000001 << 0,

			/// <summary>
			/// ループの再探索は行わない
			/// </summary>
			CheckLoop = 0x00000001 << 1,

			/// <summary>
			/// 一度探索したノードの探索は行わない
			/// </summary>
			CheckVisit = 0x00000001 << 2,
		}
		#endregion // enum Option

		public const int InfiniteStep = -1;

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
				return TypeProfileHelper.EnableUsingCache;
			}

			set
			{
				TypeProfileHelper.EnableUsingCache = value;
			}
		}

		/// <summary>
		/// 探索禁止の型リスト
		/// </summary>
		/// <remarks>
		/// ここに追加されている型の持つプロパティは探索しません。
		/// 
		/// 他の型がここに追加されている型のプロパティを持っている場合、そのプロパティ自体は探索されますが、
		/// そのプロパティ値からさらに中のプロパティは探索されません。
		/// </remarks>
		public static IList<Type> KeepOutTypes => keepOutTypes;

		/// <summary>
		/// 辞書の要素ノード名の解決デリゲート
		/// </summary>
		/// <remarks>
		/// ここに追加されている型の持つプロパティは探索しません。
		/// 
		/// 他の型がここに追加されている型のプロパティを持っている場合、そのプロパティ自体は探索されますが、
		/// そのプロパティ値からさらに中のプロパティは探索されません。
		/// </remarks>
		public static Func<Type, object, string> SolveDictionaryItemNodeNameDelegate { get; set; } = SolveDictionaryItemNodeNameDefault;

		/// <summary>
		/// ノード毎処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードごとに処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNode(Action<object> process, object subject, Option options = Option.None)
		{
			EveryNode(process, subject, null, options);
		}

		/// <summary>
		/// ノード毎処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードごとに処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNode(Action<object> process, object subject, object userData, Option options = Option.None)
		{
			EveryNode(process, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// ノード毎処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードごとに処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNode(Action<object> process, object subject, int step, Option options = Option.None)
		{
			EveryNode(process, subject, null, step, options);
		}

		/// <summary>
		/// ノード毎処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードごとに処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNode(Action<object> process, object subject, object userData, int step, Option options = Option.None)
		{
			EveryNode((current, context) => process(current), subject, userData, step, options);
		}

		/// <summary>
		/// ノード毎処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードごとに処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNode(Action<object, PWalkContext> process, object subject, Option options = Option.None)
		{
			EveryNode(process, subject, null, options);
		}

		/// <summary>
		/// ノード毎処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードごとに処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNode(Action<object, PWalkContext> process, object subject, object userData, Option options = Option.None)
		{
			EveryNode(process, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// ノード毎処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードごとに処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNode(Action<object, PWalkContext> process, object subject, int step, Option options = Option.None)
		{
			EveryNode(process, subject, null, step, options);
		}

		/// <summary>
		/// ノード毎処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードごとに処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNode(Action<object, PWalkContext> process, object subject, object userData, int step, Option options = Option.None)
		{
			EveryNodeIf(() => true, process, subject, userData, step, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<bool> condition, Action<object> process, object subject, Option options = Option.None)
		{
			EveryNodeIf(condition, process, subject, null, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<bool> condition, Action<object> process, object subject, object userData, Option options = Option.None)
		{
			EveryNodeIf(condition, process, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<bool> condition, Action<object> process, object subject, int step, Option options = Option.None)
		{
			EveryNodeIf(condition, process, subject, null, step, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<bool> condition, Action<object> process, object subject, object userData, int step, Option options = Option.None)
		{
			EveryNodeIf(condition, (current, context) => process(current), subject, userData, step, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<bool> condition, Action<object, PWalkContext> process, object subject, Option options = Option.None)
		{
			EveryNodeIf(condition, process, subject, null, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<bool> condition, Action<object, PWalkContext> process, object subject, object userData, Option options = Option.None)
		{
			EveryNodeIf(condition, process, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<bool> condition, Action<object, PWalkContext> process, object subject, int step, Option options = Option.None)
		{
			EveryNodeIf(condition, process, subject, null, step, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<bool> condition, Action<object, PWalkContext> process, object subject, object userData, int step, Option options = Option.None)
		{
			EveryNodeIf(_ => condition(), process, subject, userData, step, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<object, bool> condition, Action<object> process, object subject, Option options = Option.None)
		{
			EveryNodeIf(condition, process, subject, null, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<object, bool> condition, Action<object> process, object subject, object userData, Option options = Option.None)
		{
			EveryNodeIf(condition, process, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<object, bool> condition, Action<object> process, object subject, int step, Option options = Option.None)
		{
			EveryNodeIf(condition, process, subject, null, step, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<object, bool> condition, Action<object> process, object subject, object userData, int step, Option options = Option.None)
		{
			EveryNodeIf(condition, (current, context) => process(current), subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<object, bool> condition, Action<object, PWalkContext> process, object subject, Option options = Option.None)
		{
			EveryNodeIf(condition, process, subject, null, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<object, bool> condition, Action<object, PWalkContext> process, object subject, object userData, Option options = Option.None)
		{
			EveryNodeIf(condition, process, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<object, bool> condition, Action<object, PWalkContext> process, object subject, int step, Option options = Option.None)
		{
			EveryNodeIf(condition, process, subject, null, step, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<object, bool> condition, Action<object, PWalkContext> process, object subject, object userData, int step, Option options = Option.None)
		{
			EveryNodeIf((current, context) => condition(current), process, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object> process, object subject, Option options = Option.None)
		{
			EveryNodeIf(condition, process, subject, null, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object> process, object subject, object userData, Option options = Option.None)
		{
			EveryNodeIf(condition, process, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object> process, object subject, int step, Option options = Option.None)
		{
			EveryNodeIf(condition, process, subject, null, step, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object> process, object subject, object userData, int step, Option options = Option.None)
		{
			EveryNodeIf(condition, (current, context) => process(current), subject, userData, step, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object, PWalkContext> process, object subject, Option options = Option.None)
		{
			EveryNodeIf(condition, process, subject, null, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object, PWalkContext> process, object subject, object userData, Option options = Option.None)
		{
			EveryNodeIf(condition, process, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object, PWalkContext> process, object subject, int step, Option options = Option.None)
		{
			EveryNodeIf(condition, process, subject, null, step, options);
		}

		/// <summary>
		/// 該当ノード処理の実行
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 探索するノードが条件に該当する場合、処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object, PWalkContext> process, object subject, object userData, int step, Option options = Option.None)
		{
			if (condition == null)
			{
				throw new ArgumentNullException(nameof(condition));
			}

			if (process == null)
			{
				throw new ArgumentNullException(nameof(process));
			}

			if (subject == null)
			{
				// 入力データが null なので、探索の必要なし。
				return;
			}

			var context = new PWalkContext() { UserData = userData, };
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			WalkCore(condition, process, subject, context, step, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T>(Action<T> process, object subject, Option options = Option.None)
		{
			TargetType(process, subject, null, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T>(Action<T> process, object subject, object userData, Option options = Option.None)
		{
			TargetType(process, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T>(Action<T> process, object subject, int step, Option options = Option.None)
		{
			TargetType(process, subject, null, step, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T>(Action<T> process, object subject, object userData, int step, Option options = Option.None)
		{
			TargetType<T>((target, context) => process(target), subject, userData, step, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T>(Action<T, PWalkContext> process, object subject, Option options = Option.None)
		{
			TargetType(process, subject, null, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T>(Action<T, PWalkContext> process, object subject, object userData, Option options = Option.None)
		{
			TargetType(process, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T>(Action<T, PWalkContext> process, object subject, int step, Option options = Option.None)
		{
			TargetType(process, subject, null, step, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T>(Action<T, PWalkContext> process, object subject, object userData, int step, Option options = Option.None)
		{
			if (process == null)
			{
				throw new ArgumentNullException(nameof(process));
			}

			if (subject == null)
			{
				// 入力データが null なので、探索の必要なし。
				return;
			}

			var context = new PWalkContext() { UserData = userData, };
			context.Core.SystemData.TargetTypes.Add(typeof(T));
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			WalkCore((current, _) => current is T, (current, _) => process((T)current, _), subject, context, step, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2>(Action<T1> process1, Action<T2> process2, object subject, Option options = Option.None)
		{
			TargetType(process1, process2, subject, null, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2>(Action<T1> process1, Action<T2> process2, object subject, object userData, Option options = Option.None)
		{
			TargetType(process1, process2, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2>(Action<T1> process1, Action<T2> process2, object subject, int step, Option options = Option.None)
		{
			TargetType(process1, process2, subject, null, step, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2>(Action<T1> process1, Action<T2> process2, object subject, object userData, int step, Option options = Option.None)
		{
			TargetType<T1, T2>((target, context) => process1(target), (target, context) => process2(target), subject, userData, step, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, object subject, Option options = Option.None)
		{
			TargetType(process1, process2, subject, null, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, object subject, object userData, Option options = Option.None)
		{
			TargetType(process1, process2, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, object subject, int step, Option options = Option.None)
		{
			TargetType(process1, process2, subject, null, step, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, object subject, object userData, int step, Option options = Option.None)
		{
			if (process1 == null)
			{
				throw new ArgumentNullException(nameof(process1));
			}

			if (process2 == null)
			{
				throw new ArgumentNullException(nameof(process2));
			}

			if (subject == null)
			{
				// 入力データが null なので、探索の必要なし。
				return;
			}

			var context = new PWalkContext() { UserData = userData, };
			context.Core.SystemData.TargetTypes.Add(typeof(T1));
			context.Core.SystemData.TargetTypes.Add(typeof(T2));
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			Func<object, PWalkContext, bool>[] conditions =
			{
				(current, _) => current is T1,
				(current, _) => current is T2,
			};

			Action<object, PWalkContext>[] processes =
			{
				(current, _) => process1((T1)current, _),
				(current, _) => process2((T2)current, _),
			};

			WalkCore(conditions, processes, subject, context, step, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="process3">型 3 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2, T3>(Action<T1> process1, Action<T2> process2, Action<T3> process3, object subject, Option options = Option.None)
		{
			TargetType(process1, process2, process3, subject, null, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="process3">型 3 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2, T3>(Action<T1> process1, Action<T2> process2, Action<T3> process3, object subject, object userData, Option options = Option.None)
		{
			TargetType(process1, process2, process3, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="process3">型 3 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2, T3>(Action<T1> process1, Action<T2> process2, Action<T3> process3, object subject, int step, Option options = Option.None)
		{
			TargetType(process1, process2, process3, subject, null, step, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="process3">型 3 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2, T3>(Action<T1> process1, Action<T2> process2, Action<T3> process3, object subject, object userData, int step, Option options = Option.None)
		{
			TargetType<T1, T2, T3>((target, context) => process1(target), (target, context) => process2(target), (target, context) => process3(target), subject, userData, step, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="process3">型 3 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2, T3>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, object subject, Option options = Option.None)
		{
			TargetType(process1, process2, process3, subject, null, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="process3">型 3 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2, T3>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, object subject, object userData, Option options = Option.None)
		{
			TargetType(process1, process2, process3, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="process3">型 3 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2, T3>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, object subject, int step, Option options = Option.None)
		{
			TargetType(process1, process2, process3, subject, null, step, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="process3">型 3 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2, T3>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, object subject, object userData, int step, Option options = Option.None)
		{
			if (process1 == null)
			{
				throw new ArgumentNullException(nameof(process1));
			}

			if (process2 == null)
			{
				throw new ArgumentNullException(nameof(process2));
			}

			if (process3 == null)
			{
				throw new ArgumentNullException(nameof(process3));
			}

			if (subject == null)
			{
				// 入力データが null なので、探索の必要なし。
				return;
			}

			var context = new PWalkContext() { UserData = userData, };
			context.Core.SystemData.TargetTypes.Add(typeof(T1));
			context.Core.SystemData.TargetTypes.Add(typeof(T2));
			context.Core.SystemData.TargetTypes.Add(typeof(T3));
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			Func<object, PWalkContext, bool>[] conditions =
			{
				(current, _) => current is T1,
				(current, _) => current is T2,
				(current, _) => current is T3,
			};

			Action<object, PWalkContext>[] processes =
			{
				(current, _) => process1((T1)current, _),
				(current, _) => process2((T2)current, _),
				(current, _) => process3((T3)current, _),
			};

			WalkCore(conditions, processes, subject, context, step, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="process3">型 3 への処理</param>
		/// <param name="process4">型 4 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2, T3, T4>(Action<T1> process1, Action<T2> process2, Action<T3> process3, Action<T4> process4, object subject, Option options = Option.None)
		{
			TargetType(process1, process2, process3, process4, subject, null, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="process3">型 3 への処理</param>
		/// <param name="process4">型 4 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2, T3, T4>(Action<T1> process1, Action<T2> process2, Action<T3> process3, Action<T4> process4, object subject, object userData, Option options = Option.None)
		{
			TargetType(process1, process2, process3, process4, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="process3">型 3 への処理</param>
		/// <param name="process4">型 4 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2, T3, T4>(Action<T1> process1, Action<T2> process2, Action<T3> process3, Action<T4> process4, object subject, int step, Option options = Option.None)
		{
			TargetType(process1, process2, process3, process4, subject, null, step, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="process3">型 3 への処理</param>
		/// <param name="process4">型 4 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2, T3, T4>(Action<T1> process1, Action<T2> process2, Action<T3> process3, Action<T4> process4, object subject, object userData, int step, Option options = Option.None)
		{
			TargetType<T1, T2, T3, T4>((target, context) => process1(target), (target, context) => process2(target), (target, context) => process3(target), (target, context) => process4(target), subject, userData, step, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="process3">型 3 への処理</param>
		/// <param name="process4">型 4 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2, T3, T4>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, Action<T4, PWalkContext> process4, object subject, Option options = Option.None)
		{
			TargetType(process1, process2, process3, process4, subject, null, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="process3">型 3 への処理</param>
		/// <param name="process4">型 4 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2, T3, T4>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, Action<T4, PWalkContext> process4, object subject, object userData, Option options = Option.None)
		{
			TargetType(process1, process2, process3, process4, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="process3">型 3 への処理</param>
		/// <param name="process4">型 4 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2, T3, T4>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, Action<T4, PWalkContext> process4, object subject, int step, Option options = Option.None)
		{
			TargetType(process1, process2, process3, process4, subject, null, step, options);
		}

		/// <summary>
		/// 型対象処理の実行
		/// </summary>
		/// <param name="process1">型 1 への処理</param>
		/// <param name="process2">型 2 への処理</param>
		/// <param name="process3">型 3 への処理</param>
		/// <param name="process4">型 4 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された型が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力処理が null</exception>
		public static void TargetType<T1, T2, T3, T4>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, Action<T4, PWalkContext> process4, object subject, object userData, int step, Option options = Option.None)
		{
			if (process1 == null)
			{
				throw new ArgumentNullException(nameof(process1));
			}

			if (process2 == null)
			{
				throw new ArgumentNullException(nameof(process2));
			}

			if (process3 == null)
			{
				throw new ArgumentNullException(nameof(process3));
			}

			if (process4 == null)
			{
				throw new ArgumentNullException(nameof(process4));
			}

			if (subject == null)
			{
				// 入力データが null なので、探索の必要なし。
				return;
			}

			var context = new PWalkContext() { UserData = userData, };
			context.Core.SystemData.TargetTypes.Add(typeof(T1));
			context.Core.SystemData.TargetTypes.Add(typeof(T2));
			context.Core.SystemData.TargetTypes.Add(typeof(T3));
			context.Core.SystemData.TargetTypes.Add(typeof(T4));
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			Func<object, PWalkContext, bool>[] conditions =
			{
				(current, _) => current is T1,
				(current, _) => current is T2,
				(current, _) => current is T3,
				(current, _) => current is T4,
			};

			Action<object, PWalkContext>[] processes =
			{
				(current, _) => process1((T1)current, _),
				(current, _) => process2((T2)current, _),
				(current, _) => process3((T3)current, _),
				(current, _) => process4((T4)current, _),
			};

			WalkCore(conditions, processes, subject, context, step, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T>(Action<object> process, object subject, Option options = Option.None)
			where T : PWalkMarkAttribute
		{
			MarkedWith<T>(process, subject, null, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T>(Action<object> process, object subject, object userData, Option options = Option.None)
			where T : PWalkMarkAttribute
		{
			MarkedWith<T>(process, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T>(Action<object> process, object subject, int step, Option options = Option.None)
			where T : PWalkMarkAttribute
		{
			MarkedWith<T>(process, subject, null, step, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T>(Action<object> process, object subject, object userData, int step, Option options = Option.None)
			where T : PWalkMarkAttribute
		{
			MarkedWith<T>((current, _) => process(current), subject, userData, step, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T>(Action<object, PWalkContext> process, object subject, Option options = Option.None)
			where T : PWalkMarkAttribute
		{
			MarkedWith<T>(process, subject, null, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T>(Action<object, PWalkContext> process, object subject, object userData, Option options = Option.None)
			where T : PWalkMarkAttribute
		{
			MarkedWith<T>(process, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T>(Action<object, PWalkContext> process, object subject, int step, Option options = Option.None)
			where T : PWalkMarkAttribute
		{
			MarkedWith<T>(process, subject, null, step, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T>(Action<object, PWalkContext> process, object subject, object userData, int step, Option options = Option.None)
			where T : PWalkMarkAttribute
		{
			if (process == null)
			{
				throw new ArgumentNullException(nameof(process));
			}

			if (subject == null)
			{
				// 入力データが null なので、探索の必要なし。
				return;
			}

			var context = new PWalkContext() { UserData = userData, };
			context.Core.SystemData.MarkTypes.Add(typeof(T));
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			WalkCore((current, _) => false, process, subject, context, step, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2>(Action<object> process1, Action<object> process2, object subject, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2>(process1, process2, subject, null, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2>(Action<object> process1, Action<object> process2, object subject, object userData, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2>(process1, process2, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2>(Action<object> process1, Action<object> process2, object subject, int step, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2>(process1, process2, subject, null, step, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2>(Action<object> process1, Action<object> process2, object subject, object userData, int step, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2>((current, _) => process1(current), (current, _) => process2(current), subject, userData, step, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, object subject, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2>(process1, process2, subject, null, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, object subject, object userData, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2>(process1, process2, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, object subject, int step, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2>(process1, process2, subject, null, step, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, object subject, object userData, int step, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
		{
			if (subject == null)
			{
				// 入力データが null なので、探索の必要なし。
				return;
			}

			var context = new PWalkContext() { UserData = userData, };
			context.Core.SystemData.MarkTypes.Add(typeof(T1));
			context.Core.SystemData.MarkTypes.Add(typeof(T2));
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			Func<object, PWalkContext, bool>[] conditions =
			{
				(current, _) => false,
				(current, _) => false,
			};

			Action<object, PWalkContext>[] processes =
			{
				process1,
				process2,
			};

			WalkCore(conditions, processes, subject, context, step, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="process3">属性 3 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2, T3>(Action<object> process1, Action<object> process2, Action<object> process3, object subject, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2, T3>(process1, process2, process3, subject, null, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="process3">属性 3 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2, T3>(Action<object> process1, Action<object> process2, Action<object> process3, object subject, object userData, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2, T3>(process1, process2, process3, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="process3">属性 3 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2, T3>(Action<object> process1, Action<object> process2, Action<object> process3, object subject, int step, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2, T3>(process1, process2, process3, subject, null, step, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="process3">属性 3 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2, T3>(Action<object> process1, Action<object> process2, Action<object> process3, object subject, object userData, int step, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2, T3>((current, _) => process1(current), (current, _) => process2(current), (current, _) => process3(current), subject, userData, step, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="process3">属性 3 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2, T3>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, object subject, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2, T3>(process1, process2, process3, subject, null, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="process3">属性 3 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2, T3>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, object subject, object userData, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2, T3>(process1, process2, process3, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="process3">属性 3 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2, T3>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, object subject, int step, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2, T3>(process1, process2, process3, subject, null, step, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="process3">属性 3 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2, T3>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, object subject, object userData, int step, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
		{
			if (subject == null)
			{
				// 入力データが null なので、探索の必要なし。
				return;
			}

			var context = new PWalkContext() { UserData = userData, };
			context.Core.SystemData.MarkTypes.Add(typeof(T1));
			context.Core.SystemData.MarkTypes.Add(typeof(T2));
			context.Core.SystemData.MarkTypes.Add(typeof(T3));
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			Func<object, PWalkContext, bool>[] conditions =
			{
				(current, _) => false,
				(current, _) => false,
				(current, _) => false,
			};

			Action<object, PWalkContext>[] processes =
			{
				process1,
				process2,
				process3,
			};

			WalkCore((current, _) => false, null, subject, context, step, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="process3">属性 3 への処理</param>
		/// <param name="process4">属性 4 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2, T3, T4>(Action<object> process1, Action<object> process2, Action<object> process3, Action<object> process4, object subject, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2, T3, T4>(process1, process2, process3, process4, subject, null, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="process3">属性 3 への処理</param>
		/// <param name="process4">属性 4 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2, T3, T4>(Action<object> process1, Action<object> process2, Action<object> process3, Action<object> process4, object subject, object userData, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2, T3, T4>(process1, process2, process3, process4, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="process3">属性 3 への処理</param>
		/// <param name="process4">属性 4 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2, T3, T4>(Action<object> process1, Action<object> process2, Action<object> process3, Action<object> process4, object subject, int step, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2, T3, T4>(process1, process2, process3, process4, subject, null, step, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="process3">属性 3 への処理</param>
		/// <param name="process4">属性 4 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2, T3, T4>(Action<object> process1, Action<object> process2, Action<object> process3, Action<object> process4, object subject, object userData, int step, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2, T3, T4>((current, _) => process1(current), (current, _) => process3(current), (current, _) => process3(current), (current, _) => process4(current), subject, userData, step, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="process3">属性 3 への処理</param>
		/// <param name="process4">属性 4 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2, T3, T4>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, Action<object, PWalkContext> process4, object subject, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2, T3, T4>(process1, process2, process3, process4, subject, null, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="process3">属性 3 への処理</param>
		/// <param name="process4">属性 4 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2, T3, T4>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, Action<object, PWalkContext> process4, object subject, object userData, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2, T3, T4>(process1, process2, process3, process4, subject, userData, InfiniteStep, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="process3">属性 3 への処理</param>
		/// <param name="process4">属性 4 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2, T3, T4>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, Action<object, PWalkContext> process4, object subject, int step, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2, T3, T4>(process1, process2, process3, process4, subject, null, step, options);
		}

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process1">属性 1 への処理</param>
		/// <param name="process2">属性 2 への処理</param>
		/// <param name="process3">属性 3 への処理</param>
		/// <param name="process4">属性 4 への処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="userData">任意データ</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T1, T2, T3, T4>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, Action<object, PWalkContext> process4, object subject, object userData, int step, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute
		{
			if (subject == null)
			{
				// 入力データが null なので、探索の必要なし。
				return;
			}

			var context = new PWalkContext() { UserData = userData, };
			context.Core.SystemData.MarkTypes.Add(typeof(T1));
			context.Core.SystemData.MarkTypes.Add(typeof(T2));
			context.Core.SystemData.MarkTypes.Add(typeof(T3));
			context.Core.SystemData.MarkTypes.Add(typeof(T4));
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			Func<object, PWalkContext, bool>[] conditions =
			{
				(current, _) => false,
				(current, _) => false,
				(current, _) => false,
				(current, _) => false,
			};

			Action<object, PWalkContext>[] processes =
			{
				process1,
				process2,
				process3,
				process4,
			};

			WalkCore((current, _) => false, null, subject, context, step, options);
		}

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T>(object subject, Option options = Option.None)
			where T : PWalkMarkAttribute
		{
			InvokeMarkedWith<T>(subject, null, InfiniteStep, options);
		}

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="parameters">呼び出し引数</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T>(object subject, object[] parameters, Option options = Option.None)
			where T : PWalkMarkAttribute
		{
			InvokeMarkedWith<T>(subject, parameters, InfiniteStep, options);
		}

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T>(object subject, int step, Option options = Option.None)
			where T : PWalkMarkAttribute
		{
			InvokeMarkedWith<T>(subject, null, step, options);
		}

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="parameters">呼び出し引数</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T>(object subject, object[] parameters, int step, Option options = Option.None)
			where T : PWalkMarkAttribute
		{
			if (subject == null)
			{
				// 入力データが null なので、探索の必要なし。
				return;
			}

			var context = new PWalkContext();
			context.Core.SystemData.InvokeMarkTypes.Add(typeof(T));
			context.Core.SystemData.Parameters = parameters;
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			WalkCore((current, _) => false, null, subject, context, step, options);
		}

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T1, T2>(object subject, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
		{
			InvokeMarkedWith<T1, T2>(subject, null, InfiniteStep, options);
		}

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="parameters">呼び出し引数</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T1, T2>(object subject, object[] parameters, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
		{
			InvokeMarkedWith<T1, T2>(subject, parameters, InfiniteStep, options);
		}

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T1, T2>(object subject, int step, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
		{
			InvokeMarkedWith<T1, T2>(subject, null, step, options);
		}

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="parameters">呼び出し引数</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T1, T2>(object subject, object[] parameters, int step, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
		{
			if (subject == null)
			{
				// 入力データが null なので、探索の必要なし。
				return;
			}

			var context = new PWalkContext();
			context.Core.SystemData.InvokeMarkTypes.Add(typeof(T1));
			context.Core.SystemData.InvokeMarkTypes.Add(typeof(T2));
			context.Core.SystemData.Parameters = parameters;
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			WalkCore((current, _) => false, null, subject, context, step, options);
		}

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T1, T2, T3>(object subject, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
		{
			InvokeMarkedWith<T1, T2, T3>(subject, null, InfiniteStep, options);
		}

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="parameters">呼び出し引数</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T1, T2, T3>(object subject, object[] parameters, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
		{
			InvokeMarkedWith<T1, T2, T3>(subject, parameters, InfiniteStep, options);
		}

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T1, T2, T3>(object subject, int step, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
		{
			InvokeMarkedWith<T1, T2, T3>(subject, null, step, options);
		}

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="parameters">呼び出し引数</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T1, T2, T3>(object subject, object[] parameters, int step, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
		{
			if (subject == null)
			{
				// 入力データが null なので、探索の必要なし。
				return;
			}

			var context = new PWalkContext();
			context.Core.SystemData.InvokeMarkTypes.Add(typeof(T1));
			context.Core.SystemData.InvokeMarkTypes.Add(typeof(T2));
			context.Core.SystemData.InvokeMarkTypes.Add(typeof(T3));
			context.Core.SystemData.Parameters = parameters;
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			WalkCore((current, _) => false, null, subject, context, step, options);
		}

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T1, T2, T3, T4>(object subject, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute
		{
			InvokeMarkedWith<T1, T2, T3, T4>(subject, null, InfiniteStep, options);
		}

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="parameters">呼び出し引数</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T1, T2, T3, T4>(object subject, object[] parameters, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute
		{
			InvokeMarkedWith<T1, T2, T3, T4>(subject, parameters, InfiniteStep, options);
		}

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T1, T2, T3, T4>(object subject, int step, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute
		{
			InvokeMarkedWith<T1, T2, T3, T4>(subject, null, step, options);
		}

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="parameters">呼び出し引数</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T1, T2, T3, T4>(object subject, object[] parameters, int step, Option options = Option.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute
		{
			if (subject == null)
			{
				// 入力データが null なので、探索の必要なし。
				return;
			}

			var context = new PWalkContext();
			context.Core.SystemData.InvokeMarkTypes.Add(typeof(T1));
			context.Core.SystemData.InvokeMarkTypes.Add(typeof(T2));
			context.Core.SystemData.InvokeMarkTypes.Add(typeof(T3));
			context.Core.SystemData.InvokeMarkTypes.Add(typeof(T4));
			context.Core.SystemData.Parameters = parameters;
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			WalkCore((current, _) => false, null, subject, context, step, options);
		}

		/// <summary>
		/// パスによるノードの探索
		/// </summary>
		/// <param name="path">パス</param>
		/// <param name="subject">探索の根</param>
		/// <returns>探索の成功判定</returns>
		/// <remarks>
		/// パスで一意に確定するノードを探索し、コンテキストに探索結果を記録します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力パスが null</exception>
		public static FindResult FindNode(string path, object subject)
		{
			if (path == null)
			{
				throw new ArgumentNullException(nameof(path));
			}

			var context = new PWalkContext();
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			if (subject == null)
			{
				// 入力データが null なので、探索の必要なし。
				return new FindResult(false, context);
			}

			var result = FindCore(path.Split('/'), subject, context);
			if (!result)
			{
				context.Core.Nodes.Clear();
				context.Core.Links.Clear();
			}

			return new FindResult(result, context);
		}

		/// <summary>
		/// パスによるノードの探索再開
		/// </summary>
		/// <param name="path">パス</param>
		/// <param name="context">コンテキスト</param>
		/// <returns>探索の成功判定</returns>
		/// <remarks>
		/// パスで一意に確定するノードを探索し、コンテキストに探索結果を記録します。
		/// 書き込み済みのコンテキストを入力すると、その続きから探索を開始します。
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">入力パスが null</exception>
		public static FindResult FindNode(string path, PWalkContext context)
		{
			if (path == null)
			{
				throw new ArgumentNullException(nameof(path));
			}

			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			if (context.Nodes.Count == 0)
			{
				// 入力データが null なので、探索の必要なし。
				return new FindResult(false, context);
			}

			var clonedContext = new PWalkContext();
			clonedContext.Core.Nodes.AddRange(context.Nodes.Take(context.Nodes.Count - 1));
			clonedContext.Core.Links.AddRange(context.Links);
			clonedContext.Core.SystemData = context.Core.SystemData;

			var subject = context.CurrentNode.Value;
			var result = FindCore(path.Split('/'), subject, clonedContext);
			if (result)
			{
				context = clonedContext;
			}

			return new FindResult(result, context);
		}

		#region private members
		private static List<Type> keepOutTypes = new List<Type> { typeof(string), typeof(Guid), typeof(Type), };

		private static object[] SolveParameters(object[] parameters, object current, PWalkContext context, PWalkMarkAttribute mark)
		{
			object[] ret = null;
			if (parameters != null)
			{
				if (parameters.Any(_ => _ is ProxyParameter))
				{
					ret = new object[parameters.Length];
					for (var index = 0; index < parameters.Length; ++index)
					{
						var parameter = parameters[index];
						if (parameter is ProxyParameter proxy)
						{
							switch (proxy)
							{
								case ProxyParameter.Current:
									parameter = current;
									break;

								case ProxyParameter.Context:
									parameter = context;
									break;

								case ProxyParameter.Mark:
									parameter = mark;
									break;
							}
						}

						ret[index] = parameter;
					}
				}
				else
				{
					ret = parameters;
				}
			}

			return ret;
		}

		private static string SolveDictionaryItemNodeNameDefault(Type type, object value)
		{
			if (type == typeof(string))
			{
				return (string)value;
			}

			return null;
		}

		private static void WalkCore(Func<object, PWalkContext, bool> condition, Action<object, PWalkContext> process, object current, PWalkContext context, int step, Option options)
		{
			Func<object, PWalkContext, bool>[] conditions = { condition, };
			Action<object, PWalkContext>[] processes = { process, };
			WalkCore(conditions, processes, current, context, step, options);
		}

		private static void WalkCore(Func<object, PWalkContext, bool>[] conditions, Action<object, PWalkContext>[] processes, object current, PWalkContext context, int step, Option options)
		{
			var currentType = current?.GetType();
			var profile = current != null ? currentType.Profile() : null;

			var node = new Node(context.Core, context.Nodes.Count, current, profile);
			context.Core.Nodes.Add(node);
			context.Core.CurrentStep = step;
			context.NextStep = step > 0 ? step - 1 : step;

			foreach (var attribute in node.FetchAttributes())
			{
				attribute.Visit(current, context);
			}

			for (var index = 0; index < processes.Length; ++index)
			{
				var condition = conditions[index];
				var process = processes[index];
				if (condition(current, context))
				{
					process(current, context);
				}
			}

			if (context.Core.SystemData.MarkTypes.Count > 0)
			{
				foreach (var mark in node.FetchValueMarks())
				{
					for (var index = 0; index < context.Core.SystemData.MarkTypes.Count; ++index)
					{
						if (context.Core.SystemData.MarkTypes[index].IsAssignableFrom(mark.GetType()))
						{
							context.Core.CurrentMark = mark;
							processes[index](current, context);
							context.Core.CurrentMark = null;
						}
					}
				}
			}

			if (context.Core.SystemData.InvokeMarkTypes.Count > 0)
			{
				for (var index = 0; index < context.Core.SystemData.InvokeMarkTypes.Count; ++index)
				{
					var markType = context.Core.SystemData.InvokeMarkTypes[index];
					foreach (var function in node.FetchMarkedFunctions())
					{
						foreach (var mark in function.Marks.Where(_ => markType.IsAssignableFrom(_.GetType())))
						{
							context.Core.CurrentMark = mark;
							function.Invoke[current](SolveParameters(context.Core.SystemData.Parameters, current, context, mark));
							context.Core.CurrentMark = null;
						}
					}
				}
			}

			if (current != null &&
				!currentType.IsPrimitive &&
				!currentType.IsEnum &&
				!KeepOutTypes.Any(_ => _.IsAssignableFrom(currentType)))
			{
				var checkLoop = (options & Option.CheckLoop) == Option.CheckLoop;
				var checkVisit = (options & Option.CheckVisit) == Option.CheckVisit;

				if (step != 0 || context.NextStep != 0)
				{
					if (checkVisit)
					{
						context.Core.Visit.Add(current);
					}

					foreach (var member in profile.MemberValues.Values)
					{
						object child = null;
						try
						{
							child = member.Value[current];
						}
						catch (TargetInvocationException)
						{
							// 取得不能
							return;
						}

						if (child != null)
						{
							if (checkLoop)
							{
								if (context.Nodes.Any(_ => ReferenceEquals(_, current)))
								{
									// ループしているので、探索しない。
									return;
								}
							}

							if (checkVisit)
							{
								if (context.Core.Visit.Any(_ => ReferenceEquals(_, child)))
								{
									// 検証済みなので、探索しない。
									return;
								}
							}
						}

						var link = new Link(context.Core, context.Links.Count, member);
						context.Core.Links.Add(link);

						WalkCore(conditions, processes, child, context, context.NextStep, options);

						context.Core.Links.RemoveAt(context.Links.Count - 1);
					}
				}

				if (current is IDictionary dictionary)
				{
					var index = 0;
					var enumerator = dictionary.GetEnumerator();
					while (enumerator.MoveNext())
					{
						var child = enumerator.Value;
						if (child != null)
						{
							if (checkLoop)
							{
								if (context.Nodes.Any(_ => ReferenceEquals(_, current)))
								{
									// ループしているので、探索しない。
									continue;
								}
							}

							if (checkVisit)
							{
								if (context.Core.Visit.Any(_ => ReferenceEquals(_, child)))
								{
									// 検証済みなので、探索しない。
									continue;
								}
							}
						}

						var link = new Link(context.Core, context.Links.Count, node.IndexerMemberProfile) { Index = index++, KeyType = profile.DictionaryKeyType ?? typeof(object), Key = enumerator.Key, };
						context.Core.Links.Add(link);

						WalkCore(conditions, processes, child, context, step, options);

						context.Core.Links.RemoveAt(context.Links.Count - 1);
					}
				}
				else if (current is IEnumerable enumerable)
				{
					var index = 0;
					foreach (var child in enumerable)
					{
						if (child != null)
						{
							if (checkLoop)
							{
								if (context.Nodes.Any(_ => ReferenceEquals(_, current)))
								{
									// ループしているので、探索しない。
									continue;
								}
							}

							if (checkVisit)
							{
								if (context.Core.Visit.Any(_ => ReferenceEquals(_, child)))
								{
									// 検証済みなので、探索しない。
									continue;
								}
							}
						}

						var link = new Link(context.Core, context.Links.Count, node.IndexerMemberProfile) { Index = index++, };
						context.Core.Links.Add(link);

						WalkCore(conditions, processes, child, context, step, options);

						context.Core.Links.RemoveAt(context.Links.Count - 1);
					}
				}
			}

			if ((options & Option.CallProcessAlsoOnWayBack) == Option.CallProcessAlsoOnWayBack)
			{
				context.Core.States |= WalkState.WayBack;

				if (context.Core.SystemData.InvokeMarkTypes.Count > 0)
				{
					for (var index = context.Core.SystemData.InvokeMarkTypes.Count - 1; index >= 0; --index)
					{
						var markType = context.Core.SystemData.InvokeMarkTypes[index];
						foreach (var function in node.FetchMarkedFunctions().Reverse())
						{
							foreach (var mark in function.Marks.Where(_ => markType.IsAssignableFrom(_.GetType())).Reverse())
							{
								context.Core.CurrentMark = mark;
								function.Invoke[current](SolveParameters(context.Core.SystemData.Parameters, current, context, mark));
								context.Core.CurrentMark = null;
							}
						}
					}
				}

				if (context.Core.SystemData.MarkTypes.Count > 0)
				{
					foreach (var mark in node.FetchValueMarks().Reverse())
					{
						for (var index = context.Core.SystemData.MarkTypes.Count - 1; index >= 0; --index)
						{
							if (context.Core.SystemData.MarkTypes[index].IsAssignableFrom(mark.GetType()))
							{
								context.Core.CurrentMark = mark;
								processes[index](current, context);
								context.Core.CurrentMark = null;
							}
						}
					}
				}

				for (var index = processes.Length - 1; index >= 0; --index)
				{
					var condition = conditions[index];
					var process = processes[index];
					if (condition(current, context))
					{
						process(current, context);
					}
				}

				context.Core.States &= ~WalkState.WayBack;
			}

			context.Core.Nodes.RemoveAt(context.Nodes.Count - 1);
		}

		private static bool FindCore(string[] names, object current, PWalkContext context)
		{
			var currentType = current?.GetType();
			var profile = current != null ? currentType.Profile() : null;

			var node = new Node(context.Core, context.Nodes.Count, current, profile);
			context.Core.Nodes.Add(node);

			if (context.Nodes.Count == names.Length)
			{
				if (context.Nodes.Count == 1)
				{
					return string.IsNullOrEmpty(names[0]) || names[0] == node.Name;
				}

				return true;
			}

			var targetName = names[context.Nodes.Count];

			if (profile.MemberValues.TryGetValue(targetName, out var member))
			{
				switch (member.Archetype)
				{
					case Archetype.Default:
						object child = null;
						try
						{
							child = member.Value[current];
						}
						catch (TargetInvocationException)
						{
							// 取得不能
							return false;
						}

						var link = new Link(context.Core, context.Links.Count, member);
						context.Core.Links.Add(link);

						return FindCore(names, child, context);
				}
			}

			if (current is IDictionary dictionary)
			{
				var ret = false;
				var index = -1;
				object key = null;
				object child = null;

				try
				{
					var sampleIndex = 0;
					var enumerator = dictionary.GetEnumerator();
					while (enumerator.MoveNext())
					{
						var sampleName = context.Core.SystemData.SolveDictionaryItemNodeNameDelegate(profile.DictionaryKeyType, enumerator.Key);
						if (sampleName == targetName)
						{
							index = sampleIndex;
							key = enumerator.Key;
							child = enumerator.Value;
							ret = true;
							break;
						}

						++sampleIndex;
					}
				}
				catch (TargetInvocationException)
				{
					// 取得不能
					return false;
				}

				if (!ret)
				{
					if (int.TryParse(targetName, out index))
					{
						try
						{
							var sampleIndex = 0;
							var enumerator = dictionary.GetEnumerator();
							while (sampleIndex <= index)
							{
								enumerator.MoveNext();
								++sampleIndex;
							}

							key = enumerator.Key;
							child = enumerator.Value;
							ret = true;
						}
						catch (TargetInvocationException)
						{
							// 取得不能
							return false;
						}
					}
				}

				if (!ret)
				{
					return false;
				}

				var link = new Link(context.Core, context.Links.Count, member) { Index = index, KeyType = profile.DictionaryKeyType ?? typeof(object), Key = key, };
				context.Core.Links.Add(link);

				return FindCore(names, child, context);
			}
			else if (current is IEnumerable enumerable)
			{
				if (int.TryParse(targetName, out var index))
				{
					object child = null;
					try
					{
						var enumerator = enumerable.GetEnumerator();
						while (index-- >= 0)
						{
							enumerator.MoveNext();
						}

						child = enumerator.Current;
					}
					catch (TargetInvocationException)
					{
						// 取得不能
						return false;
					}

					var link = new Link(context.Core, context.Links.Count, member);
					context.Core.Links.Add(link);

					return FindCore(names, child, context);
				}
			}

			return false;
		}
		#endregion // private members
	}
}
