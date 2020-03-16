using System.Collections.Generic;
using System.Xml;

namespace Echo.Data.Core
{
	/// <summary>
	/// デシリアライズコンテキスト
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("{TargetElement}")]
	internal class DataPropertySerializeContext
	{
		public DataPropertySerializeContext()
			: this(new Dictionary<object, string>())
		{
		}

		public DataPropertySerializeContext(IDictionary<object, string> referencedPaths)
		{
			ReferencedPaths = referencedPaths;
		}

		/// <summary>
		/// 対象要素
		/// </summary>
		public XmlElement TargetElement { get; set; }

		/// <summary>
		/// ルートパス
		/// </summary>
		public string RootPath { get; set; } = string.Empty;

		/// <summary>
		/// 被参照項目
		/// </summary>
		public IDictionary<object, string> ReferencedPaths { get; }
	}
}
