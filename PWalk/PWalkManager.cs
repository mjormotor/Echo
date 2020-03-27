using System;
using System.Collections.Generic;
using System.Linq;

namespace Echo.PWalkService
{
	/// <summary>
	/// プロパティ探索
	/// </summary>
	/// <remarks>
	/// 入力されたオブジェクトのプロパティを探索し、それに対する処理を実行します。
	/// </remarks>
	public class PWalkManager
	{
		public PWalkManager()
		{
			KeepOutTypes = new List<Type>(PWalk.KeepOutTypes);
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
		public IList<Type> KeepOutTypes { get; }

		/// <summary>
		/// 辞書の要素ノード名の解決デリゲート
		/// </summary>
		/// <remarks>
		/// ここに追加されている型の持つプロパティは探索しません。
		/// 
		/// 他の型がここに追加されている型のプロパティを持っている場合、そのプロパティ自体は探索されますが、
		/// そのプロパティ値からさらに中のプロパティは探索されません。
		/// </remarks>
		public Func<Type, object, string> SolveDictionaryItemNodeNameDelegate { get; set; } = PWalk.SolveDictionaryItemNodeNameDefault;

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
		public void EveryNode(Action<object> process, object subject, PWalkOption options = PWalkOption.None)
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
		public void EveryNode(Action<object> process, object subject, object userData, PWalkOption options = PWalkOption.None)
		{
			EveryNode(process, subject, userData, PWalk.InfiniteStep, options);
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
		public void EveryNode(Action<object> process, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void EveryNode(Action<object> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
		public void EveryNode(Action<object, PWalkContext> process, object subject, PWalkOption options = PWalkOption.None)
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
		public void EveryNode(Action<object, PWalkContext> process, object subject, object userData, PWalkOption options = PWalkOption.None)
		{
			EveryNode(process, subject, userData, PWalk.InfiniteStep, options);
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
		public void EveryNode(Action<object, PWalkContext> process, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void EveryNode(Action<object, PWalkContext> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
		public void EveryNodeIf(Func<bool> condition, Action<object> process, object subject, PWalkOption options = PWalkOption.None)
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
		public void EveryNodeIf(Func<bool> condition, Action<object> process, object subject, object userData, PWalkOption options = PWalkOption.None)
		{
			EveryNodeIf(condition, process, subject, userData, PWalk.InfiniteStep, options);
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
		public void EveryNodeIf(Func<bool> condition, Action<object> process, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void EveryNodeIf(Func<bool> condition, Action<object> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
		public void EveryNodeIf(Func<bool> condition, Action<object, PWalkContext> process, object subject, PWalkOption options = PWalkOption.None)
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
		public void EveryNodeIf(Func<bool> condition, Action<object, PWalkContext> process, object subject, object userData, PWalkOption options = PWalkOption.None)
		{
			EveryNodeIf(condition, process, subject, userData, PWalk.InfiniteStep, options);
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
		public void EveryNodeIf(Func<bool> condition, Action<object, PWalkContext> process, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void EveryNodeIf(Func<bool> condition, Action<object, PWalkContext> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
		public void EveryNodeIf(Func<object, bool> condition, Action<object> process, object subject, PWalkOption options = PWalkOption.None)
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
		public void EveryNodeIf(Func<object, bool> condition, Action<object> process, object subject, object userData, PWalkOption options = PWalkOption.None)
		{
			EveryNodeIf(condition, process, subject, userData, PWalk.InfiniteStep, options);
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
		public void EveryNodeIf(Func<object, bool> condition, Action<object> process, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void EveryNodeIf(Func<object, bool> condition, Action<object> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
		{
			EveryNodeIf(condition, (current, context) => process(current), subject, userData, PWalk.InfiniteStep, options);
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
		public void EveryNodeIf(Func<object, bool> condition, Action<object, PWalkContext> process, object subject, PWalkOption options = PWalkOption.None)
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
		public void EveryNodeIf(Func<object, bool> condition, Action<object, PWalkContext> process, object subject, object userData, PWalkOption options = PWalkOption.None)
		{
			EveryNodeIf(condition, process, subject, userData, PWalk.InfiniteStep, options);
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
		public void EveryNodeIf(Func<object, bool> condition, Action<object, PWalkContext> process, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void EveryNodeIf(Func<object, bool> condition, Action<object, PWalkContext> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
		{
			EveryNodeIf((current, context) => condition(current), process, subject, userData, PWalk.InfiniteStep, options);
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
		public void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object> process, object subject, PWalkOption options = PWalkOption.None)
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
		public void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object> process, object subject, object userData, PWalkOption options = PWalkOption.None)
		{
			EveryNodeIf(condition, process, subject, userData, PWalk.InfiniteStep, options);
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
		public void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object> process, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
		public void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object, PWalkContext> process, object subject, PWalkOption options = PWalkOption.None)
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
		public void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object, PWalkContext> process, object subject, object userData, PWalkOption options = PWalkOption.None)
		{
			EveryNodeIf(condition, process, subject, userData, PWalk.InfiniteStep, options);
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
		public void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object, PWalkContext> process, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void EveryNodeIf(Func<object, PWalkContext, bool> condition, Action<object, PWalkContext> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
			context.Core.SystemData.KeepOutTypes = KeepOutTypes;
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			PWalk.WalkCore(condition, process, subject, context, step, options);
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
		public void TargetType<T>(Action<T> process, object subject, PWalkOption options = PWalkOption.None)
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
		public void TargetType<T>(Action<T> process, object subject, object userData, PWalkOption options = PWalkOption.None)
		{
			TargetType(process, subject, userData, PWalk.InfiniteStep, options);
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
		public void TargetType<T>(Action<T> process, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void TargetType<T>(Action<T> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
		public void TargetType<T>(Action<T, PWalkContext> process, object subject, PWalkOption options = PWalkOption.None)
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
		public void TargetType<T>(Action<T, PWalkContext> process, object subject, object userData, PWalkOption options = PWalkOption.None)
		{
			TargetType(process, subject, userData, PWalk.InfiniteStep, options);
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
		public void TargetType<T>(Action<T, PWalkContext> process, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void TargetType<T>(Action<T, PWalkContext> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
			context.Core.SystemData.KeepOutTypes = KeepOutTypes;
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			PWalk.WalkCore((current, _) => current is T, (current, _) => process((T)current, _), subject, context, step, options);
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
		public void TargetType<T1, T2>(Action<T1> process1, Action<T2> process2, object subject, PWalkOption options = PWalkOption.None)
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
		public void TargetType<T1, T2>(Action<T1> process1, Action<T2> process2, object subject, object userData, PWalkOption options = PWalkOption.None)
		{
			TargetType(process1, process2, subject, userData, PWalk.InfiniteStep, options);
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
		public void TargetType<T1, T2>(Action<T1> process1, Action<T2> process2, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void TargetType<T1, T2>(Action<T1> process1, Action<T2> process2, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
		public void TargetType<T1, T2>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, object subject, PWalkOption options = PWalkOption.None)
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
		public void TargetType<T1, T2>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, object subject, object userData, PWalkOption options = PWalkOption.None)
		{
			TargetType(process1, process2, subject, userData, PWalk.InfiniteStep, options);
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
		public void TargetType<T1, T2>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void TargetType<T1, T2>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
			context.Core.SystemData.KeepOutTypes = KeepOutTypes;
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

			PWalk.WalkCore(conditions, processes, subject, context, step, options);
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
		public void TargetType<T1, T2, T3>(Action<T1> process1, Action<T2> process2, Action<T3> process3, object subject, PWalkOption options = PWalkOption.None)
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
		public void TargetType<T1, T2, T3>(Action<T1> process1, Action<T2> process2, Action<T3> process3, object subject, object userData, PWalkOption options = PWalkOption.None)
		{
			TargetType(process1, process2, process3, subject, userData, PWalk.InfiniteStep, options);
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
		public void TargetType<T1, T2, T3>(Action<T1> process1, Action<T2> process2, Action<T3> process3, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void TargetType<T1, T2, T3>(Action<T1> process1, Action<T2> process2, Action<T3> process3, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
		public void TargetType<T1, T2, T3>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, object subject, PWalkOption options = PWalkOption.None)
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
		public void TargetType<T1, T2, T3>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, object subject, object userData, PWalkOption options = PWalkOption.None)
		{
			TargetType(process1, process2, process3, subject, userData, PWalk.InfiniteStep, options);
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
		public void TargetType<T1, T2, T3>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void TargetType<T1, T2, T3>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
			context.Core.SystemData.KeepOutTypes = KeepOutTypes;
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

			PWalk.WalkCore(conditions, processes, subject, context, step, options);
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
		public void TargetType<T1, T2, T3, T4>(Action<T1> process1, Action<T2> process2, Action<T3> process3, Action<T4> process4, object subject, PWalkOption options = PWalkOption.None)
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
		public void TargetType<T1, T2, T3, T4>(Action<T1> process1, Action<T2> process2, Action<T3> process3, Action<T4> process4, object subject, object userData, PWalkOption options = PWalkOption.None)
		{
			TargetType(process1, process2, process3, process4, subject, userData, PWalk.InfiniteStep, options);
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
		public void TargetType<T1, T2, T3, T4>(Action<T1> process1, Action<T2> process2, Action<T3> process3, Action<T4> process4, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void TargetType<T1, T2, T3, T4>(Action<T1> process1, Action<T2> process2, Action<T3> process3, Action<T4> process4, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
		public void TargetType<T1, T2, T3, T4>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, Action<T4, PWalkContext> process4, object subject, PWalkOption options = PWalkOption.None)
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
		public void TargetType<T1, T2, T3, T4>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, Action<T4, PWalkContext> process4, object subject, object userData, PWalkOption options = PWalkOption.None)
		{
			TargetType(process1, process2, process3, process4, subject, userData, PWalk.InfiniteStep, options);
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
		public void TargetType<T1, T2, T3, T4>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, Action<T4, PWalkContext> process4, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void TargetType<T1, T2, T3, T4>(Action<T1, PWalkContext> process1, Action<T2, PWalkContext> process2, Action<T3, PWalkContext> process3, Action<T4, PWalkContext> process4, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
			context.Core.SystemData.KeepOutTypes = KeepOutTypes;
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

			PWalk.WalkCore(conditions, processes, subject, context, step, options);
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
		public void MarkedWith<T>(Action<object> process, object subject, PWalkOption options = PWalkOption.None)
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
		public void MarkedWith<T>(Action<object> process, object subject, object userData, PWalkOption options = PWalkOption.None)
			where T : PWalkMarkAttribute
		{
			MarkedWith<T>(process, subject, userData, PWalk.InfiniteStep, options);
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
		public void MarkedWith<T>(Action<object> process, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void MarkedWith<T>(Action<object> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
		public void MarkedWith<T>(Action<object, PWalkContext> process, object subject, PWalkOption options = PWalkOption.None)
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
		public void MarkedWith<T>(Action<object, PWalkContext> process, object subject, object userData, PWalkOption options = PWalkOption.None)
			where T : PWalkMarkAttribute
		{
			MarkedWith<T>(process, subject, userData, PWalk.InfiniteStep, options);
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
		public void MarkedWith<T>(Action<object, PWalkContext> process, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void MarkedWith<T>(Action<object, PWalkContext> process, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
			context.Core.SystemData.KeepOutTypes = KeepOutTypes;
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			PWalk.WalkCore((current, _) => false, process, subject, context, step, options);
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
		public void MarkedWith<T1, T2>(Action<object> process1, Action<object> process2, object subject, PWalkOption options = PWalkOption.None)
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
		public void MarkedWith<T1, T2>(Action<object> process1, Action<object> process2, object subject, object userData, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2>(process1, process2, subject, userData, PWalk.InfiniteStep, options);
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
		public void MarkedWith<T1, T2>(Action<object> process1, Action<object> process2, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void MarkedWith<T1, T2>(Action<object> process1, Action<object> process2, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
		public void MarkedWith<T1, T2>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, object subject, PWalkOption options = PWalkOption.None)
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
		public void MarkedWith<T1, T2>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, object subject, object userData, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2>(process1, process2, subject, userData, PWalk.InfiniteStep, options);
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
		public void MarkedWith<T1, T2>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void MarkedWith<T1, T2>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
			context.Core.SystemData.KeepOutTypes = KeepOutTypes;
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

			PWalk.WalkCore(conditions, processes, subject, context, step, options);
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
		public void MarkedWith<T1, T2, T3>(Action<object> process1, Action<object> process2, Action<object> process3, object subject, PWalkOption options = PWalkOption.None)
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
		public void MarkedWith<T1, T2, T3>(Action<object> process1, Action<object> process2, Action<object> process3, object subject, object userData, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2, T3>(process1, process2, process3, subject, userData, PWalk.InfiniteStep, options);
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
		public void MarkedWith<T1, T2, T3>(Action<object> process1, Action<object> process2, Action<object> process3, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void MarkedWith<T1, T2, T3>(Action<object> process1, Action<object> process2, Action<object> process3, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
		public void MarkedWith<T1, T2, T3>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, object subject, PWalkOption options = PWalkOption.None)
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
		public void MarkedWith<T1, T2, T3>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, object subject, object userData, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2, T3>(process1, process2, process3, subject, userData, PWalk.InfiniteStep, options);
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
		public void MarkedWith<T1, T2, T3>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void MarkedWith<T1, T2, T3>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
			context.Core.SystemData.KeepOutTypes = KeepOutTypes;
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

			PWalk.WalkCore((current, _) => false, null, subject, context, step, options);
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
		public void MarkedWith<T1, T2, T3, T4>(Action<object> process1, Action<object> process2, Action<object> process3, Action<object> process4, object subject, PWalkOption options = PWalkOption.None)
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
		public void MarkedWith<T1, T2, T3, T4>(Action<object> process1, Action<object> process2, Action<object> process3, Action<object> process4, object subject, object userData, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2, T3, T4>(process1, process2, process3, process4, subject, userData, PWalk.InfiniteStep, options);
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
		public void MarkedWith<T1, T2, T3, T4>(Action<object> process1, Action<object> process2, Action<object> process3, Action<object> process4, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void MarkedWith<T1, T2, T3, T4>(Action<object> process1, Action<object> process2, Action<object> process3, Action<object> process4, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
		public void MarkedWith<T1, T2, T3, T4>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, Action<object, PWalkContext> process4, object subject, PWalkOption options = PWalkOption.None)
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
		public void MarkedWith<T1, T2, T3, T4>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, Action<object, PWalkContext> process4, object subject, object userData, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute
		{
			MarkedWith<T1, T2, T3, T4>(process1, process2, process3, process4, subject, userData, PWalk.InfiniteStep, options);
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
		public void MarkedWith<T1, T2, T3, T4>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, Action<object, PWalkContext> process4, object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void MarkedWith<T1, T2, T3, T4>(Action<object, PWalkContext> process1, Action<object, PWalkContext> process2, Action<object, PWalkContext> process3, Action<object, PWalkContext> process4, object subject, object userData, int step, PWalkOption options = PWalkOption.None)
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
			context.Core.SystemData.KeepOutTypes = KeepOutTypes;
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

			PWalk.WalkCore((current, _) => false, null, subject, context, step, options);
		}

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public void InvokeMarkedWith<T>(object subject, PWalkOption options = PWalkOption.None)
			where T : PWalkMarkAttribute
		{
			InvokeMarkedWith<T>(subject, null, PWalk.InfiniteStep, options);
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
		public void InvokeMarkedWith<T>(object subject, object[] parameters, PWalkOption options = PWalkOption.None)
			where T : PWalkMarkAttribute
		{
			InvokeMarkedWith<T>(subject, parameters, PWalk.InfiniteStep, options);
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
		public void InvokeMarkedWith<T>(object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void InvokeMarkedWith<T>(object subject, object[] parameters, int step, PWalkOption options = PWalkOption.None)
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
			context.Core.SystemData.KeepOutTypes = KeepOutTypes;
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			PWalk.WalkCore((current, _) => false, null, subject, context, step, options);
		}

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public void InvokeMarkedWith<T1, T2>(object subject, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
		{
			InvokeMarkedWith<T1, T2>(subject, null, PWalk.InfiniteStep, options);
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
		public void InvokeMarkedWith<T1, T2>(object subject, object[] parameters, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
		{
			InvokeMarkedWith<T1, T2>(subject, parameters, PWalk.InfiniteStep, options);
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
		public void InvokeMarkedWith<T1, T2>(object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void InvokeMarkedWith<T1, T2>(object subject, object[] parameters, int step, PWalkOption options = PWalkOption.None)
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
			context.Core.SystemData.KeepOutTypes = KeepOutTypes;
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			PWalk.WalkCore((current, _) => false, null, subject, context, step, options);
		}

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public void InvokeMarkedWith<T1, T2, T3>(object subject, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
		{
			InvokeMarkedWith<T1, T2, T3>(subject, null, PWalk.InfiniteStep, options);
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
		public void InvokeMarkedWith<T1, T2, T3>(object subject, object[] parameters, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
		{
			InvokeMarkedWith<T1, T2, T3>(subject, parameters, PWalk.InfiniteStep, options);
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
		public void InvokeMarkedWith<T1, T2, T3>(object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void InvokeMarkedWith<T1, T2, T3>(object subject, object[] parameters, int step, PWalkOption options = PWalkOption.None)
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
			context.Core.SystemData.KeepOutTypes = KeepOutTypes;
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			PWalk.WalkCore((current, _) => false, null, subject, context, step, options);
		}

		/// <summary>
		/// 標識対象関数の呼び出し
		/// </summary>
		/// <param name="subject">探索の根</param>
		/// <param name="options">オプション動作</param>
		/// <remarks>
		/// 入力された属性がついた関数かイベントを呼び出します。
		/// </remarks>
		public void InvokeMarkedWith<T1, T2, T3, T4>(object subject, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute
		{
			InvokeMarkedWith<T1, T2, T3, T4>(subject, null, PWalk.InfiniteStep, options);
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
		public void InvokeMarkedWith<T1, T2, T3, T4>(object subject, object[] parameters, PWalkOption options = PWalkOption.None)
			where T1 : PWalkMarkAttribute
			where T2 : PWalkMarkAttribute
			where T3 : PWalkMarkAttribute
			where T4 : PWalkMarkAttribute
		{
			InvokeMarkedWith<T1, T2, T3, T4>(subject, parameters, PWalk.InfiniteStep, options);
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
		public void InvokeMarkedWith<T1, T2, T3, T4>(object subject, int step, PWalkOption options = PWalkOption.None)
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
		public void InvokeMarkedWith<T1, T2, T3, T4>(object subject, object[] parameters, int step, PWalkOption options = PWalkOption.None)
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
			context.Core.SystemData.KeepOutTypes = KeepOutTypes;
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			PWalk.WalkCore((current, _) => false, null, subject, context, step, options);
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
		public FindResult FindNode(string path, object subject)
		{
			if (path == null)
			{
				throw new ArgumentNullException(nameof(path));
			}

			var context = new PWalkContext();
			context.Core.SystemData.KeepOutTypes = KeepOutTypes;
			context.Core.SystemData.SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeNameDelegate;

			if (subject == null)
			{
				// 入力データが null なので、探索の必要なし。
				return new FindResult(false, context);
			}

			var result = PWalk.FindCore(path.Split('/'), subject, context);
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
		public FindResult FindNode(string path, PWalkContext context)
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
			var result = PWalk.FindCore(path.Split('/'), subject, clonedContext);
			if (result)
			{
				context = clonedContext;
			}

			return new FindResult(result, context);
		}

		#region internal members
		internal PWalkManager(IEnumerable<Type> keepOutTypes)
		{
			KeepOutTypes = new List<Type>(keepOutTypes);
		}
		#endregion // internal members
	}
}
