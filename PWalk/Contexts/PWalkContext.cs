using System.Collections.Generic;
using System.Linq;

namespace Echo.PWalkService
{
	using Core;

	/// <summary>
	/// プロパティ探索コンテキスト
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("{Nodes}")]
	public class PWalkContext
	{
		/// <summary>
		/// 任意データ
		/// </summary>
		public object UserData { get; set; }

		/// <summary>
		/// 残り探索距離
		/// </summary>
		public int CurrentStep => Core.CurrentStep;

		/// <summary>
		/// 次の探索距離
		/// </summary>
		public int NextStep { get; set; }

		/// <summary>
		/// 現在のノード
		/// </summary>
		public Node CurrentNode => Nodes.Last();

		/// <summary>
		/// 探索ノード
		/// </summary>
		public IReadOnlyList<Node> Nodes => Core.Nodes;

		/// <summary>
		/// 探索リンク
		/// </summary>
		public IReadOnlyList<Link> Links => Core.Links;

		/// <summary>
		/// 現在の探索標識
		/// </summary>
		public PWalkMarkAttribute CurrentMark => Core.CurrentMark;

		/// <summary>
		/// 往路判定
		/// </summary>
		public bool IsOnWayForward => !IsOnWayBack;

		/// <summary>
		/// 復路判定
		/// </summary>
		public bool IsOnWayBack => (Core.States & WalkState.WayBack) == WalkState.WayBack;

		#region internal members
		internal PWalkContext()
		{
			Core = new ContextCore();
		}

		internal ContextCore Core { get; }
		#endregion // internal members
	}
}
