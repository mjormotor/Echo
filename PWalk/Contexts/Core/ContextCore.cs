using System.Collections.Generic;

namespace Echo.PWalkService.Core
{
	/// <summary>
	/// 探索状態
	/// </summary>
	internal enum WalkState
	{
		/// <summary>
		/// 復路
		/// </summary>
		WayBack = 0x00000001 << 0,
	}

	[System.Diagnostics.DebuggerDisplay("{Nodes}")]
	internal class ContextCore
	{
		public SystemData SystemData { get; set; } = new SystemData();

		public int CurrentStep { get; set; }

		public List<Node> Nodes { get; } = new List<Node>();

		public List<Link> Links { get; } = new List<Link>();

		public PWalkMarkAttribute CurrentMark { get; set; }

		public WalkState States { get; set; }

		public IList<object> Visit { get; } = new List<object>();
	}
}
