using System;

namespace Echo.PWalkService
{
	/// <summary>
	/// プロパティ探索のオプション動作
	/// </summary>
	[Flags]
	public enum PWalkOption
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
}
