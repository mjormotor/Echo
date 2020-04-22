using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Echo.PWalkService
{
	using Core;

	/// <summary>
	/// プロパティ探索
	/// </summary>
	/// <remarks>
	/// 入力されたオブジェクトのプロパティを探索し、それに対する処理を実行します。
	/// </remarks>
	public static class PWalk
	{
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
		public static IList<Type> KeepOutTypes => Instance.KeepOutTypes;

		/// <summary>
		/// 辞書の要素ノード名の解決デリゲート
		/// </summary>
		/// <remarks>
		/// ここに追加されている型の持つプロパティは探索しません。
		/// 
		/// 他の型がここに追加されている型のプロパティを持っている場合、そのプロパティ自体は探索されますが、
		/// そのプロパティ値からさらに中のプロパティは探索されません。
		/// </remarks>
		public static Func<Type, object, string> SolveDictionaryItemNodeNameDelegate
		{
			get { return Instance.SolveDictionaryItemNodeNameDelegate; }
			set { Instance.SolveDictionaryItemNodeNameDelegate = value; }
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
		public static void EveryNode(Action<object> process, object subject, PWalkOption options = PWalkOption.None) => Instance.EveryNode(process, subject, options);

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
		public static void EveryNode(Action<object> process, object subject, object userData, PWalkOption options = PWalkOption.None) => Instance.EveryNode(process, subject, userData, options);

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
		public static void EveryNode(Action<object> process, object subject, int step, PWalkOption options = PWalkOption.None) => Instance.EveryNode(process, subject, step, options);

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
		public static void EveryNode(Action<object> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None) => Instance.EveryNode(process, subject, userData, step, options);

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
		public static void EveryNode(Action<object, PWalkContext> process, object subject, PWalkOption options = PWalkOption.None) => Instance.EveryNode(process, subject, options);

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
		public static void EveryNode(Action<object, PWalkContext> process, object subject, object userData, PWalkOption options = PWalkOption.None) => Instance.EveryNode(process, subject, userData, options);

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
		public static void EveryNode(Action<object, PWalkContext> process, object subject, int step, PWalkOption options = PWalkOption.None) => Instance.EveryNode(process, subject, step, options);

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
		public static void EveryNode(Action<object, PWalkContext> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None) => Instance.EveryNode(process, subject, userData, step, options);

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
		public static void EveryNodeIf(Func<bool> condition, Action<object> process, object subject, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, options);

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
		public static void EveryNodeIf(Func<bool> condition, Action<object> process, object subject, object userData, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, userData, options);

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
		public static void EveryNodeIf(Func<bool> condition, Action<object> process, object subject, int step, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, step, options);

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
		public static void EveryNodeIf(Func<bool> condition, Action<object> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, userData, step, options);

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
		public static void EveryNodeIf(Func<bool> condition, Action<object, PWalkContext> process, object subject, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, options);

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
		public static void EveryNodeIf(Func<bool> condition, Action<object, PWalkContext> process, object subject, object userData, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, userData, options);

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
		public static void EveryNodeIf(Func<bool> condition, Action<object, PWalkContext> process, object subject, int step, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, step, options);

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
		public static void EveryNodeIf(Func<bool> condition, Action<object, PWalkContext> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, userData, step, options);

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
		public static void EveryNodeIf(Func<object, bool> condition, Action<object> process, object subject, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, options);

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
		public static void EveryNodeIf(Func<object, bool> condition, Action<object> process, object subject, object userData, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, userData, options);

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
		public static void EveryNodeIf(Func<object, bool> condition, Action<object> process, object subject, int step, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, step, options);

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
		public static void EveryNodeIf(Func<object, bool> condition, Action<object> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, userData, step, options);

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
		public static void EveryNodeIf(Func<object, bool> condition, Action<object, PWalkContext> process, object subject, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, options);

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
		public static void EveryNodeIf(Func<object, bool> condition, Action<object, PWalkContext> process, object subject, object userData, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, userData, options);

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
		public static void EveryNodeIf(Func<object, bool> condition, Action<object, PWalkContext> process, object subject, int step, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, step, options);

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
		public static void EveryNodeIf(Func<object, bool> condition, Action<object, PWalkContext> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, userData, step, options);

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
		public static void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object> process, object subject, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, options);

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
		public static void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object> process, object subject, object userData, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, userData, options);

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
		public static void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object> process, object subject, int step, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, step, options);

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
		public static void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, userData, step, options);

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
		public static void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object, PWalkContext> process, object subject, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, options);

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
		public static void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object, PWalkContext> process, object subject, object userData, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, userData, options);

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
		public static void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object, PWalkContext> process, object subject, int step, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, step, options);

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
		public static void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object, PWalkContext> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None) => Instance.EveryNodeIf(condition, process, subject, userData, step, options);

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
		public static void TargetType<T>(Action<T> process, object subject, PWalkOption options = PWalkOption.None) => Instance.TargetType(process, subject, options);

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
		public static void TargetType<T>(Action<T> process, object subject, object userData, PWalkOption options = PWalkOption.None) => Instance.TargetType(process, subject, userData, options);

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
		public static void TargetType<T>(Action<T> process, object subject, int step, PWalkOption options = PWalkOption.None) => Instance.TargetType(process, subject, step, options);

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
		public static void TargetType<T>(Action<T> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None) => Instance.TargetType(process, subject, userData, step, options);

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
		public static void TargetType<T>(Action<T, PWalkContext> process, object subject, PWalkOption options = PWalkOption.None) => Instance.TargetType(process, subject, options);

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
		public static void TargetType<T>(Action<T, PWalkContext> process, object subject, object userData, PWalkOption options = PWalkOption.None) => Instance.TargetType(process, subject, userData, options);

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
		public static void TargetType<T>(Action<T, PWalkContext> process, object subject, int step, PWalkOption options = PWalkOption.None) => Instance.TargetType(process, subject, step, options);

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
		public static void TargetType<T>(Action<T, PWalkContext> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None) => Instance.TargetType(process, subject, userData, step, options);

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
		public static void TargetType<T1, T2>(Action<T1> process1, Action<T2> process2, object subject, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, subject, options);

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
		public static void TargetType<T1, T2>(Action<T1> process1, Action<T2> process2, object subject, object userData, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, subject, userData, options);

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
		public static void TargetType<T1, T2>(Action<T1> process1, Action<T2> process2, object subject, int step, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, subject, step, options);

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
		public static void TargetType<T1, T2>(Action<T1> process1, Action<T2> process2, object subject, object userData, int step, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, subject, userData, step, options);

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
		public static void TargetType<T1, T2>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, object subject, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, subject, options);

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
		public static void TargetType<T1, T2>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, object subject, object userData, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, subject, userData, options);

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
		public static void TargetType<T1, T2>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, object subject, int step, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, subject, step, options);

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
		public static void TargetType<T1, T2>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, object subject, object userData, int step, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, subject, userData, step, options);

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
		public static void TargetType<T1, T2, T3>(Action<T1> process1, Action<T2> process2, Action<T3> process3, object subject, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, process3, subject, options);

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
		public static void TargetType<T1, T2, T3>(Action<T1> process1, Action<T2> process2, Action<T3> process3, object subject, object userData, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, process3, subject, userData, options);

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
		public static void TargetType<T1, T2, T3>(Action<T1> process1, Action<T2> process2, Action<T3> process3, object subject, int step, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, process3, subject, step, options);

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
		public static void TargetType<T1, T2, T3>(Action<T1> process1, Action<T2> process2, Action<T3> process3, object subject, object userData, int step, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, process3, subject, userData, step, options);

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
		public static void TargetType<T1, T2, T3>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, object subject, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, process3, subject, options);

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
		public static void TargetType<T1, T2, T3>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, object subject, object userData, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, process3, subject, userData, options);

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
		public static void TargetType<T1, T2, T3>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, object subject, int step, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, process3, subject, step, options);

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
		public static void TargetType<T1, T2, T3>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, object subject, object userData, int step, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, process3, subject, userData, step, options);

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
		public static void TargetType<T1, T2, T3, T4>(Action<T1> process1, Action<T2> process2, Action<T3> process3, Action<T4> process4, object subject, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, process3, process4, subject, options);

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
		public static void TargetType<T1, T2, T3, T4>(Action<T1> process1, Action<T2> process2, Action<T3> process3, Action<T4> process4, object subject, object userData, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, process3, process4, subject, userData, options);

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
		public static void TargetType<T1, T2, T3, T4>(Action<T1> process1, Action<T2> process2, Action<T3> process3, Action<T4> process4, object subject, int step, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, process3, process4, subject, step, options);

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
		public static void TargetType<T1, T2, T3, T4>(Action<T1> process1, Action<T2> process2, Action<T3> process3, Action<T4> process4, object subject, object userData, int step, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, process3, process4, subject, userData, step, options);

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
		public static void TargetType<T1, T2, T3, T4>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, Action<T4, PWalkContext> process4, object subject, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, process3, process4, subject, options);

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
		public static void TargetType<T1, T2, T3, T4>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, Action<T4, PWalkContext> process4, object subject, object userData, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, process3, process4, subject, userData, options);

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
		public static void TargetType<T1, T2, T3, T4>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, Action<T4, PWalkContext> process4, object subject, int step, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, process3, process4, subject, step, options);

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
		public static void TargetType<T1, T2, T3, T4>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, Action<T4, PWalkContext> process4, object subject, object userData, int step, PWalkOption options = PWalkOption.None) => Instance.TargetType(process1, process2, process3, process4, subject, userData, step, options);

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T>(Action<object> process, object subject, PWalkOption options = PWalkOption.None)
			where T : PWalkMarkAttribute => Instance.MarkedWith<T>(process, subject, options);

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
		public static void MarkedWith<T>(Action<object> process, object subject, object userData, PWalkOption options = PWalkOption.None)
			where T : PWalkMarkAttribute => Instance.MarkedWith<T>(process, subject, userData, options);

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
		public static void MarkedWith<T>(Action<object> process, object subject, int step, PWalkOption options = PWalkOption.None)
			where T : PWalkMarkAttribute => Instance.MarkedWith<T>(process, subject, step, options);

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
		public static void MarkedWith<T>(Action<object> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
			where T : PWalkMarkAttribute => Instance.MarkedWith<T>(process, subject, userData, step, options);

		/// <summary>
		/// 標識対象処理の実行
		/// </summary>
		/// <param name="process">処理</param>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された標識属性が見つかった場合、それに対する処理を実行します。
		/// </remarks>
		public static void MarkedWith<T>(Action<object, PWalkContext> process, object subject, PWalkOption options = PWalkOption.None)
			where T : PWalkMarkAttribute => Instance.MarkedWith<T>(process, subject, options);

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
		public static void MarkedWith<T>(Action<object, PWalkContext> process, object subject, object userData, PWalkOption options = PWalkOption.None)
			where T : PWalkMarkAttribute => Instance.MarkedWith<T>(process, subject, userData, options);

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
		public static void MarkedWith<T>(Action<object, PWalkContext> process, object subject, int step, PWalkOption options = PWalkOption.None)
			where T : PWalkMarkAttribute => Instance.MarkedWith<T>(process, subject, step, options);

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
		public static void MarkedWith<T>(Action<object, PWalkContext> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
			where T : PWalkMarkAttribute => Instance.MarkedWith<T>(process, subject, userData, step, options);

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
		public static void MarkedWith<T1, T2>(Action<object> process1, Action<object> process2, object subject, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2>(process1, process2, subject, options);

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
		public static void MarkedWith<T1, T2>(Action<object> process1, Action<object> process2, object subject, object userData, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2>(process1, process2, subject, userData, options);

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
		public static void MarkedWith<T1, T2>(Action<object> process1, Action<object> process2, object subject, int step, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2>(process1, process2, subject, step, options);

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
		public static void MarkedWith<T1, T2>(Action<object> process1, Action<object> process2, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2>(process1, process2, subject, userData, step, options);

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
		public static void MarkedWith<T1, T2>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, object subject, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2>(process1, process2, subject, options);

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
		public static void MarkedWith<T1, T2>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, object subject, object userData, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2>(process1, process2, subject, userData, options);

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
		public static void MarkedWith<T1, T2>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, object subject, int step, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2>(process1, process2, subject, step, options);

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
		public static void MarkedWith<T1, T2>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2>(process1, process2, subject, userData, step, options);

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
		public static void MarkedWith<T1, T2, T3>(Action<object> process1, Action<object> process2, Action<object> process3, object subject, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2, T3>(process1, process2, process3, subject, options);

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
		public static void MarkedWith<T1, T2, T3>(Action<object> process1, Action<object> process2, Action<object> process3, object subject, object userData, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2, T3>(process1, process2, process3, subject, userData, options);

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
		public static void MarkedWith<T1, T2, T3>(Action<object> process1, Action<object> process2, Action<object> process3, object subject, int step, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2, T3>(process1, process2, process3, subject, step, options);

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
		public static void MarkedWith<T1, T2, T3>(Action<object> process1, Action<object> process2, Action<object> process3, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2, T3>(process1, process2, process3, subject, userData, step, options);

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
		public static void MarkedWith<T1, T2, T3>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, object subject, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2, T3>(process1, process2, process3, subject, options);

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
		public static void MarkedWith<T1, T2, T3>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, object subject, object userData, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2, T3>(process1, process2, process3, subject, userData, options);

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
		public static void MarkedWith<T1, T2, T3>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, object subject, int step, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2, T3>(process1, process2, process3, subject, step, options);

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
		public static void MarkedWith<T1, T2, T3>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2, T3>(process1, process2, process3, subject, userData, step, options);

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
		public static void MarkedWith<T1, T2, T3, T4>(Action<object> process1, Action<object> process2, Action<object> process3, Action<object> process4, object subject, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2, T3, T4>(process1, process2, process3, process4, subject, options);

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
		public static void MarkedWith<T1, T2, T3, T4>(Action<object> process1, Action<object> process2, Action<object> process3, Action<object> process4, object subject, object userData, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2, T3, T4>(process1, process2, process3, process4, subject, userData, options);

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
		public static void MarkedWith<T1, T2, T3, T4>(Action<object> process1, Action<object> process2, Action<object> process3, Action<object> process4, object subject, int step, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2, T3, T4>(process1, process2, process3, process4, subject, step, options);

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
		public static void MarkedWith<T1, T2, T3, T4>(Action<object> process1, Action<object> process2, Action<object> process3, Action<object> process4, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2, T3, T4>(process1, process2, process3, process4, subject, userData, step, options);

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
		public static void MarkedWith<T1, T2, T3, T4>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, Action<object, PWalkContext> process4, object subject, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2, T3, T4>(process1, process2, process3, process4, subject, options);

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
		public static void MarkedWith<T1, T2, T3, T4>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, Action<object, PWalkContext> process4, object subject, object userData, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2, T3, T4>(process1, process2, process3, process4, subject, userData, options);

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
		public static void MarkedWith<T1, T2, T3, T4>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, Action<object, PWalkContext> process4, object subject, int step, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2, T3, T4>(process1, process2, process3, process4, subject, step, options);

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
		public static void MarkedWith<T1, T2, T3, T4>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, Action<object, PWalkContext> process4, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute => Instance.MarkedWith<T1, T2, T3, T4>(process1, process2, process3, process4, subject, userData, step, options);

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T>(object subject, PWalkOption options = PWalkOption.None)
			where T : PWalkMarkAttribute => Instance.InvokeMarkedWith<T>(subject, options);

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="parameters">呼び出し引数</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T>(object subject, object[] parameters, PWalkOption options = PWalkOption.None)
			where T : PWalkMarkAttribute => Instance.InvokeMarkedWith<T>(subject, parameters, options);

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T>(object subject, int step, PWalkOption options = PWalkOption.None)
			where T : PWalkMarkAttribute => Instance.InvokeMarkedWith<T>(subject, step, options);

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
		public static void InvokeMarkedWith<T>(object subject, object[] parameters, int step, PWalkOption options = PWalkOption.None)
			where T : PWalkMarkAttribute => Instance.InvokeMarkedWith<T>(subject, parameters, step, options);

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T1, T2>(object subject, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute => Instance.InvokeMarkedWith<T1, T2>(subject, options);

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="parameters">呼び出し引数</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T1, T2>(object subject, object[] parameters, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute => Instance.InvokeMarkedWith<T1, T2>(subject, parameters, options);

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T1, T2>(object subject, int step, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute => Instance.InvokeMarkedWith<T1, T2>(subject, step, options);

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
		public static void InvokeMarkedWith<T1, T2>(object subject, object[] parameters, int step, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute => Instance.InvokeMarkedWith<T1, T2>(subject, parameters, step, options);

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T1, T2, T3>(object subject, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute => Instance.InvokeMarkedWith<T1, T2, T3>(subject, options);

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="parameters">呼び出し引数</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T1, T2, T3>(object subject, object[] parameters, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute => Instance.InvokeMarkedWith<T1, T2, T3>(subject, parameters, options);

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T1, T2, T3>(object subject, int step, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute => Instance.InvokeMarkedWith<T1, T2, T3>(subject, step, options);

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
		public static void InvokeMarkedWith<T1, T2, T3>(object subject, object[] parameters, int step, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute => Instance.InvokeMarkedWith<T1, T2, T3>(subject, parameters, step, options);

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T1, T2, T3, T4>(object subject, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute => Instance.InvokeMarkedWith<T1, T2, T3, T4>(subject, options);

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="parameters">呼び出し引数</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T1, T2, T3, T4>(object subject, object[] parameters, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute => Instance.InvokeMarkedWith<T1, T2, T3, T4>(subject, parameters, options);

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="step">探索距離</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public static void InvokeMarkedWith<T1, T2, T3, T4>(object subject, int step, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute => Instance.InvokeMarkedWith<T1, T2, T3, T4>(subject, step, options);

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
		public static void InvokeMarkedWith<T1, T2, T3, T4>(object subject, object[] parameters, int step, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute => Instance.InvokeMarkedWith<T1, T2, T3, T4>(subject, parameters, step, options);

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
		public static FindResult FindNode(string path, object subject) => Instance.FindNode(path, subject);

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
		public static FindResult FindNode(string path, PWalkContext context) => Instance.FindNode(path, context);

		#region internal members
		internal static IEnumerable<Type> GenerateKeepOutTypesDefault()
		{
			yield return typeof(string);
			yield return typeof(Guid);
			yield return typeof(Type);
		}

		internal static string SolveDictionaryItemNodeNameDefault(Type type, object value)
		{
			if (type == typeof(string))
			{
				return (string)value;
			}

			return null;
		}

		internal static void WalkCore(Func<object, PWalkContext, bool> condition, Action<object, PWalkContext> process, object current, PWalkContext context, int step, PWalkOption options)
		{
			Func<object, PWalkContext, bool>[] conditions = { condition, };
			Action<object, PWalkContext>[] processes = { process, };
			WalkCore(conditions, processes, current, context, step, options);
		}

		internal static void WalkCore(Func<object, PWalkContext, bool>[] conditions, Action<object, PWalkContext>[] processes, object current, PWalkContext context, int step, PWalkOption options)
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
				!context.Core.SystemData.KeepOutTypes.Any(_ => _.IsAssignableFrom(currentType)))
			{
				var checkLoop = (options & PWalkOption.CheckLoop) == PWalkOption.CheckLoop;
				var checkVisit = (options & PWalkOption.CheckVisit) == PWalkOption.CheckVisit;

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

			if ((options & PWalkOption.CallProcessAlsoOnWayBack) == PWalkOption.CallProcessAlsoOnWayBack)
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

		internal static bool FindCore(string[] names, object current, PWalkContext context)
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
						var sampleName = context.Core.SystemData.SolveDictionaryItemNodeNameDelegate(profile.DictionaryKeyType ?? typeof(object), enumerator.Key);
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
		#endregion // internal members

		#region private members
		private static readonly PWalkManager Instance = new PWalkManager(GenerateKeepOutTypesDefault());

		private static object[] SolveParameters(object[] parameters, object current, PWalkContext context, PWalkMarkAttribute mark)
		{
			object[] ret = null;
			if (parameters != null)
			{
				if (parameters.Any(_ => _ is PWalkProxyParameter))
				{
					ret = new object[parameters.Length];
					for (var index = 0; index < parameters.Length; ++index)
					{
						var parameter = parameters[index];
						if (parameter is PWalkProxyParameter proxy)
						{
							switch (proxy)
							{
								case PWalkProxyParameter.Current:
									parameter = current;
									break;

								case PWalkProxyParameter.Context:
									parameter = context;
									break;

								case PWalkProxyParameter.Mark:
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
		#endregion // private members
	}
}
