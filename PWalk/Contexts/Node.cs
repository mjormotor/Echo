using System;
using System.Collections.Generic;
using System.Linq;

namespace Echo.PWalkService
{
	using Core;

    /// <summary>
    /// プロパティ探索ノード
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Node({Value})")]
	public class Node
	{
		/// <summary>
		/// 任意データ
		/// </summary>
		public object UserData { get; set; }

		/// <summary>
		/// 値
		/// </summary>
		public object Value
		{
			get
			{
				return this.value;
			}

			set
			{
				var parent = Parent;
				if (parent == null)
				{
					throw new InvalidOperationException("OverwritingRootValueNotAllowed");
				}

				if (value != this.value)
				{
					var link = ParentLink;
					switch (link.Archetype)
					{
						case Archetype.Default:
							link.Value[parent.Value] = value;
							break;

						case Archetype.Collection:
							link.Collection[parent.Value][link.Index] = value;
							break;

						case Archetype.Dictionary:
							link.Dictionary[parent.Value][link.Key] = value;
							break;
					}

					this.value = value;
					this.profile = value != null ? value.GetType().Profile() : null;
				}
			}
		}

		/// <summary>
		/// 親
		/// </summary>
		public Node Parent
		{
			get
			{
				Node ret = null;
				if (this.level > 0)
				{
					var context = Owner;
					ret = context.Nodes[this.level - 1];
				}

				return ret;
			}
		}

		/// <summary>
		/// 子
		/// </summary>
		public Node Child
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
		/// 親リンク
		/// </summary>
		public Link ParentLink
		{
			get
			{
				Link ret = null;
				if (this.level > 0)
				{
					var context = Owner;
					ret = context.Links[this.level - 1];
				}

				return ret;
			}
		}

		/// <summary>
		/// 子リンク
		/// </summary>
		public Link ChildLink
		{
			get
			{
				Link ret = null;

				var context = Owner;
				if (context.Links.Count > this.level)
				{
					ret = context.Links[this.level];
				}

				return ret;
			}
		}

		/// <summary>
		/// ノード名
		/// </summary>
		public string Name
		{
			get
			{
				string ret = null;

				var link = ParentLink;
				if (link != null)
				{
					ret = link.NodeName;
				}
				else
				{
					ret = this.profile.RootNodeName;
				}

				return ret;
			}
		}

		/// <summary>
		/// 格納型
		/// </summary>
		public Type AssignedType
		{
			get
			{
				Type ret = null;

				var link = ParentLink;
				if (link != null)
				{
					ret = link.Type;
				}

				return ret;
			}
		}

		/// <summary>
		/// パス
		/// </summary>
		public string Path
		{
			get
			{
				var ret = string.Empty;
				var parent = Parent;
				if (parent != null)
				{
					ret = $"{parent.Path}/{Name}";
				}

				return ret;
			}
		}

		/// <summary>
		/// メンバ値
		/// </summary>
		public IIndexer<string, object> MemberValue => MemberValueCore;

		/// <summary>
		/// 属性の収集
		/// </summary>
		internal IEnumerable<Attribute> FetchAttributes(Type type)
		{
			if (this.profile != null)
			{
				foreach (var attribute in Attribute.GetCustomAttributes(this.profile.Type).Where(_ => type.IsAssignableFrom(_.GetType())))
				{
					yield return attribute;
				}
			}

			var parentLink = ParentLink;
			if (parentLink != null)
			{
				foreach (var attribute in Attribute.GetCustomAttributes(parentLink.MemberInfo).Where(_ => type.IsAssignableFrom(_.GetType())))
				{
					yield return attribute;
				}
			}
		}

		/// <summary>
		/// 添付データの収集
		/// </summary>
		public IEnumerable<T> FetchOptionData<T>()
			where T : PWalkOptionDataAttribute
		{
			if (this.profile != null)
			{
				foreach (var optionData in this.profile.OptionData.OfType<T>())
				{
					yield return optionData;
				}
			}

			var parentLink = ParentLink;
			if (parentLink != null)
			{
				foreach (var optionData in parentLink.FetchOptionData<T>())
				{
					yield return optionData;
				}
			}
		}

		#region internal members
		internal Node(ContextCore owner, int level, object value, TypeProfile profile)
		{
			this.value = value;
			this.owner = new WeakReference(owner);
			this.profile = profile;
			this.level = level;

			MemberValueCore = new MemberValueConnector(this);
		}

		internal MemberValueProfile IndexerMemberProfile
		{
			get
			{
				return this.profile?.MemberValues.Values.FirstOrDefault(_ => _.Archetype == Archetype.Collection || _.Archetype == Archetype.Dictionary);
			}
		}

		internal IEnumerable<PWalkAttribute> FetchAttributes()
		{
			if (this.profile != null)
			{
				foreach (var attribute in this.profile.Attributes)
				{
					yield return attribute;
				}
			}

			var parentLink = ParentLink;
			if (parentLink != null)
			{
				foreach (var attribute in parentLink.Attributes)
				{
					yield return attribute;
				}
			}
		}

		internal IEnumerable<PWalkMarkAttribute> FetchValueMarks()
		{
			if (this.profile != null)
			{
				foreach (var mark in this.profile.Marks)
				{
					yield return mark;
				}
			}

			var parentLink = ParentLink;
			if (parentLink != null)
			{
				foreach (var mark in parentLink.Marks)
				{
					yield return mark;
				}
			}
		}

		internal IEnumerable<MemberFunctionProfile> FetchMarkedFunctions()
		{
			if (this.profile != null)
			{
				foreach (var function in this.profile.MemberFunctions)
				{
					yield return function;
				}
			}
		}
		#endregion // internal members

		#region private members
		#region class MemberValueConnector
		private class MemberValueConnector : IIndexer<string, object>
		{
			public MemberValueConnector(Node core)
			{
				Core = core;
			}

			#region IIndexer<string, object> interface support
			public object this[string name]
			{
				get { return Core.profile.MemberValues[name].Value[Core.Value]; }
				set { Core.profile.MemberValues[name].Value[Core.Value] = value; }
			}
			#endregion  // IIndexer<string, object> interface support

			#region private members
			private Node Core { get; }
			#endregion // private members
		}
		#endregion // class MemberValueConnector

		private object value;
		private WeakReference owner;
		private TypeProfile profile;
		private int level;

		private ContextCore Owner
		{
			get
			{
				return (ContextCore)this.owner.Target;
			}
		}

		private MemberValueConnector MemberValueCore { get; }
		#endregion // private members
	}
}
