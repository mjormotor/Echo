using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace Echo.PWalkService.Core
{
	[System.Diagnostics.DebuggerDisplay("{Type}")]
	internal class TypeProfile
	{
		public TypeProfile(Type type)
		{
			Type = type;
			DictionaryKeyType = EvaluateDictionaryKeyType();

			Setup();
		}

		public Type Type { get; }

		public string RootNodeName
		{
			get
			{
				var ret = Type.Name;
				if (this.xmlRootAttribute != null)
				{
					ret = this.xmlRootAttribute.ElementName;
				}

				return ret;
			}
		}

		public Type DictionaryKeyType { get; }

		public IReadOnlyDictionary<string, MemberValueProfile> MemberValues => this.memberValues;

		public IReadOnlyList<MemberFunctionProfile> MemberFunctions => this.memberFunctions;

		public IEnumerable<PWalkAttribute> Attributes => this.attributes;

		public IEnumerable<PWalkMarkAttribute> Marks => this.marks;

		public IEnumerable<PWalkOptionDataAttribute> OptionData => this.optionData;

		#region private members
		private static readonly PropertyInfo ListIndexer = PrepareListIndexer();
		private static readonly PropertyInfo DictionaryIndexer = PrepareDictionaryIndexer();

		private static PropertyInfo PrepareListIndexer()
		{
			PropertyInfo ret = null;
			foreach (var property in typeof(IList).GetProperties())
			{
				var parameters = property.GetIndexParameters();
				if (parameters.Length == 1)
				{
					ret = property;
				}
			}

			return ret;
		}

		private static PropertyInfo PrepareDictionaryIndexer()
		{
			PropertyInfo ret = null;
			foreach (var property in typeof(IDictionary).GetProperties())
			{
				var parameters = property.GetIndexParameters();
				if (parameters.Length == 1)
				{
					ret = property;
					break;
				}
			}

			return ret;
		}

		private Dictionary<string, MemberValueProfile> memberValues = new Dictionary<string, MemberValueProfile>();
		private List<MemberFunctionProfile> memberFunctions = new List<MemberFunctionProfile>();
		private List<PWalkAttribute> attributes = new List<PWalkAttribute>();
		private List<PWalkMarkAttribute> marks = new List<PWalkMarkAttribute>();
		private List<PWalkOptionDataAttribute> optionData = new List<PWalkOptionDataAttribute>();
		private XmlRootAttribute xmlRootAttribute;

		private void Setup()
		{
			foreach (var attribute in Attribute.GetCustomAttributes(Type))
			{
				if (attribute is PWalkAttribute businessAttribute)
				{
					this.attributes.Add(businessAttribute);
				}

				if (attribute is PWalkMarkAttribute markAttribute)
				{
					this.marks.Add(markAttribute);
				}

				if (attribute is PWalkOptionDataAttribute optionDataAttribute)
				{
					this.optionData.Add(optionDataAttribute);
				}

				if (attribute is XmlRootAttribute xmlRootAttribute)
				{
					this.xmlRootAttribute = xmlRootAttribute;
				}
			}

			foreach (var member in Type.GetMembers())
			{
				switch (member.MemberType)
				{
					case MemberTypes.Event:
						var eventInfo = (EventInfo)member;
						this.memberFunctions.Add(new MemberFunctionProfile(eventInfo));
						break;

					case MemberTypes.Field:
						var fieldInfo = (FieldInfo)member;
						this.memberValues.Add(member.Name, new MemberValueProfile(fieldInfo));
						break;

					case MemberTypes.Method:
						var methodInfo = (MethodInfo)member;
						this.memberFunctions.Add(new MemberFunctionProfile(methodInfo));
						break;

					case MemberTypes.Property:
						var property = (PropertyInfo)member;
						this.memberValues.Add(member.Name, new MemberValueProfile(property));
						break;
				}
			}

			if (typeof(IDictionary).IsAssignableFrom(Type))
			{
				if (!this.memberValues.Values.Any(_ => _.Archetype == Archetype.Dictionary))
				{
					if (!this.memberValues.ContainsKey(DictionaryIndexer.Name))
					{
						this.memberValues.Add(DictionaryIndexer.Name, new MemberValueProfile(DictionaryIndexer));
					}
				}
			}
			else if (typeof(IList).IsAssignableFrom(Type))
			{
				if (!this.memberValues.Values.Any(_ => _.Archetype == Archetype.Collection))
				{
					if (!this.memberValues.ContainsKey(ListIndexer.Name))
					{
						this.memberValues.Add(ListIndexer.Name, new MemberValueProfile(ListIndexer));
					}
				}
			}
		}

		private Type EvaluateDictionaryKeyType()
		{
			foreach (var sample in Type.GetInterfaces().Where(_ => _.IsGenericType))
			{
				if (sample.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>))
				{
					return sample.GetGenericArguments()[0];
				}
				else if (sample.GetGenericTypeDefinition() == typeof(IDictionary<,>))
				{
					return sample.GetGenericArguments()[0];
				}
			}

			return null;
		}
		#endregion // private members
	}
}
