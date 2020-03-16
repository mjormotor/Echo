using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Echo.PWalkService
{
	using Core;

    /// <summary>
    /// プロパティ構造
    /// </summary>
    public enum Archetype
	{
		/// <summary>
		/// 通常
		/// </summary>
		Default,

		/// <summary>
		/// 配列
		/// </summary>
		Collection,

		/// <summary>
		/// 辞書
		/// </summary>
		Dictionary,
	}

	/// <summary>
	/// プロパティ探索リンク
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("{profile}")]
	public class Link
	{
		/// <summary>
		/// 名前
		/// </summary>
		public string Name => this.profile.Name;

		/// <summary>
		/// 型
		/// </summary>
		public Type Type => this.profile.MemberType;

		/// <summary>
		/// 親ノード
		/// </summary>
		public Node ParentNode
		{
			get
			{
				var context = Owner;
				return context.Nodes[this.level];
			}
		}

		/// <summary>
		/// 子ノード
		/// </summary>
		public Node ChildNode
		{
			get
			{
				Node ret = null;

				var context = Owner;
				if (context.Nodes.Count > this.level + 1)
				{
					ret = context.Nodes[this.level + 1];
				}

				return ret;
			}
		}

		/// <summary>
		/// 構造
		/// </summary>
		public Archetype Archetype => this.profile.Archetype;

		/// <summary>
		/// 番号
		/// </summary>
		public int Index { get; internal set; } = -1;

		/// <summary>
		/// キー型
		/// </summary>
		public Type KeyType { get; internal set; }

		/// <summary>
		/// キー
		/// </summary>
		public object Key { get; internal set; }

		#region internal members
		internal Link(ContextCore owner, int level, MemberValueProfile profile)
		{
			this.owner = new WeakReference(owner);
			this.profile = profile;
			this.level = level;
		}

		internal string NodeName
		{
			get
			{
				switch (Archetype)
				{
					case Archetype.Collection:
						return Index.ToString();

					case Archetype.Dictionary:
						var owner = Owner;
						var name = owner.SystemData.SolveDictionaryItemNodeNameDelegate(KeyType, Key);
						if (!string.IsNullOrEmpty(name))
						{
							return name;
						}

						return Index.ToString();
				}

				return this.profile.NodeName;
			}
		}

		internal MemberInfo MemberInfo => this.profile.MemberInfo;

		internal IIndexer<object, object> Value => this.profile.Value;

		internal IReadOnlyIndexer<object, IList> Collection => this.profile.Collection;

		internal IReadOnlyIndexer<object, IDictionary> Dictionary => this.profile.Dictionary;

		internal IEnumerable<PWalkAttribute> Attributes => this.profile.Attributes;

		internal IEnumerable<PWalkMarkAttribute> Marks => this.profile.Marks;

		internal IEnumerable<T> FetchOptionData<T>()
			where T : PWalkOptionDataAttribute
		{
			foreach (var optionData in this.profile.OptionData.OfType<T>())
			{
				yield return optionData;
			}

			var parentNode = ParentNode;
			foreach (var optionData in parentNode.FetchOptionData<T>())
			{
				yield return optionData;
			}
		}
		#endregion // internal members

		#region private members
		private WeakReference owner;
		private MemberValueProfile profile;
		private int level;

		private ContextCore Owner
		{
			get
			{
				return (ContextCore)this.owner.Target;
			}
		}
		#endregion // internal members
	}
}
