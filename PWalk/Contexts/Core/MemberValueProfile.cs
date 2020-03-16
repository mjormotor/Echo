using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

namespace Echo.PWalkService.Core
{
	[System.Diagnostics.DebuggerDisplay("{MemberInfo}")]
	internal class MemberValueProfile
	{
		public MemberValueProfile(FieldInfo member)
		{
			Core = new MemberFieldProfile(member);

			Setup();
		}

		public MemberValueProfile(PropertyInfo member)
		{
			Core = new MemberPropertyProfile(member);

			Setup();
		}

		public MemberInfo MemberInfo => Core.MemberInfo;

		public Type MemberType => Core.MemberType;

		public string Name => MemberInfo.Name;
		
		public Archetype Archetype => Core.Archetype;

		public IIndexer<object, object> Value => Core.Value;

		public IReadOnlyIndexer<object, IList> Collection => Core.Collection;

		public IReadOnlyIndexer<object, IDictionary> Dictionary => Core.Dictionary;

		public string NodeName
		{
			get
			{
				if (this.xmlElementAttribute != null)
				{
					return this.xmlElementAttribute.ElementName;
				}

				return Name;
			}
		}

		public string ArrayItemName
		{
			get
			{
				if (this.xmlArrayItemAttribute != null)
				{
					return this.xmlArrayItemAttribute.ElementName;
				}

				return Name;
			}
		}

		public IEnumerable<PWalkAttribute> Attributes => this.attributes;

		public IEnumerable<PWalkMarkAttribute> Marks => this.marks;

		public IEnumerable<PWalkOptionDataAttribute> OptionData => this.optionData;

		#region private members
		#region interface ICore
		private interface ICore
		{
			MemberInfo MemberInfo { get; }

			Type MemberType { get; }

			public Archetype Archetype { get; }

			IIndexer<object, object> Value { get; }

			IReadOnlyIndexer<object, IList> Collection { get; }

			IReadOnlyIndexer<object, IDictionary> Dictionary { get; }
		}
		#endregion // interface ICore

		#region class CollectionEmulator
		private abstract class CollectionEmulatorBase : IList
		{
			public CollectionEmulatorBase(IEnumerable core)
			{
				Core = core;
			}

			#region IList interface support
			#region ICollection interface support
			#region IEnumerable interface support
			public IEnumerator GetEnumerator() => Core.GetEnumerator();
			#endregion  // IEnumerable interface support

			int ICollection.Count => throw new NotSupportedException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_NOT_SUPPORTED_MUTATE_BY_INTERFACE_FORMAT, nameof(CollectionEmulatorBase), nameof(ICollection)));

			bool ICollection.IsSynchronized => false;

			object ICollection.SyncRoot => throw new NotSupportedException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_NOT_SUPPORTED_MUTATE_BY_INTERFACE_FORMAT, nameof(CollectionEmulatorBase), nameof(ICollection)));

			public void CopyTo(Array array, int arrayIndex)
			{
				if (array == null)
				{
					throw new ArgumentNullException(nameof(array));
				}

				if (array.Rank != 1)
				{
					throw new ArgumentException(Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_MULTI_DIMENSION_ARRAY);
				}

				if (array.GetLowerBound(0) != 0)
				{
					throw new ArgumentException(Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_NON_ZERO_LOWER_BOUND_OF_ARRAY);
				}

				if (arrayIndex < 0 || arrayIndex > array.Length)
				{
					throw new ArgumentOutOfRangeException(nameof(arrayIndex), Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_OUT_OF_RANGE_NEGATIVE_NUMBER);
				}

				var objects = array as object[];
				if (objects == null)
				{
					throw new ArgumentException(Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_INVALID_TYPED_ARRAY);
				}

				try
				{
					foreach (var item in this)
					{
						objects[arrayIndex++] = item;
					}
				}
				catch (ArrayTypeMismatchException)
				{
					throw new ArgumentException(Echo.Properties.Resources.MESSAGE_EXCEPTION_ARGUMENT_INVALID_TYPED_ARRAY);
				}
			}
			#endregion  // ICollection interface support

			public abstract object this[int index] { get; set; }

			bool IList.IsFixedSize => true;

			bool IList.IsReadOnly => true;

			int IList.Add(object value) => throw new NotSupportedException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_NOT_SUPPORTED_MUTATE_BY_INTERFACE_FORMAT, nameof(CollectionEmulatorBase), nameof(IList)));

			void IList.Clear() => throw new NotSupportedException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_NOT_SUPPORTED_MUTATE_BY_INTERFACE_FORMAT, nameof(CollectionEmulatorBase), nameof(IList)));

			public bool Contains(object value) => IndexOf(value) != -1;

			public int IndexOf(object value)
			{
				var index = 0;
				foreach (var item in Core)
				{
					if (Equals(item, value))
					{
						return index;
					}

					++index;
				}

				return -1;
			}

			void IList.Insert(int index, object value) => throw new NotSupportedException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_NOT_SUPPORTED_MUTATE_BY_INTERFACE_FORMAT, nameof(CollectionEmulatorBase), nameof(IList)));

			void IList.Remove(object value) => throw new NotSupportedException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_NOT_SUPPORTED_MUTATE_BY_INTERFACE_FORMAT, nameof(CollectionEmulatorBase), nameof(IList)));

			void IList.RemoveAt(int index) => throw new NotSupportedException(string.Format(Properties.Resources.MESSAGE_EXCEPTION_NOT_SUPPORTED_MUTATE_BY_INTERFACE_FORMAT, nameof(CollectionEmulatorBase), nameof(IList)));
			#endregion  // IList interface support

			#region private members
			private IEnumerable Core { get; }
			#endregion // private members
		}
		#endregion // class CollectionEmulator

		#region class MemberFieldProfile
		private class MemberFieldProfile : ICore
		{
			public MemberFieldProfile(FieldInfo fieldInfo)
			{
				MemberInfo = fieldInfo;
				Value = new ValueConnector(fieldInfo);
				Collection = new CollectionConnector(fieldInfo);
				Dictionary = new DictionaryConnector(fieldInfo);
			}

			public FieldInfo MemberInfo { get; }

			#region ICore interface support
			MemberInfo ICore.MemberInfo => MemberInfo;

			public Type MemberType => MemberInfo.FieldType;

			public Archetype Archetype => Archetype.Default;

			public IIndexer<object, object> Value { get; }

			public IReadOnlyIndexer<object, IList> Collection { get; }

			public IReadOnlyIndexer<object, IDictionary> Dictionary { get; }
			#endregion  // ICore interface support

			#region private members
			#region class ValueConnector
			private class ValueConnector : IIndexer<object, object>
			{
				public ValueConnector(FieldInfo core)
				{
					Core = core;
				}

				#region IReadOnlyIndexer<object, object> interface support
				public object this[object target]
				{
					get { return Core.GetValue(target); }
					set { Core.SetValue(target, value); }
				}
				#endregion  // IReadOnlyIndexer<object, object> interface support

				#region private members
				private FieldInfo Core { get; }
				#endregion // private members
			}
			#endregion // class ValueConnector

			#region class CollectionConnector
			private class CollectionConnector : IReadOnlyIndexer<object, IList>
			{
				public CollectionConnector(FieldInfo core)
				{
					Core = core;
				}

				#region IReadOnlyIndexer<object, IList> interface support
				public IList this[object target]
				{
					get
					{
						var value = Core.GetValue(target);
						if (value is IList list)
						{
							return list;
						}
						else if (value is IEnumerable enumerable)
						{
							return new Emulator(enumerable, Core, target);
						}

						return null;
					}
				}
				#endregion  // IReadOnlyIndexer<object, IList> interface support

				#region private members
				#region class Emulator
				private class Emulator : CollectionEmulatorBase
				{
					public Emulator(IEnumerable core, FieldInfo member, object target)
						: base(core)
					{
						Member = member;
						Target = target;
					}

					#region CollectionEmulatorBase implement
					public override object this[int index]
					{
						get { return Target.GetType().InvokeMember(Member.Name, BindingFlags.GetField, null, Target, new object[] { index, }); }
						set { Target.GetType().InvokeMember(Member.Name, BindingFlags.SetField, null, Target, new object[] { index, value, }); }
					}
					#endregion  // CollectionEmulatorBase implement

					#region private members
					private FieldInfo Member { get; }

					private object Target { get; }
					#endregion // private members
				}
				#endregion // class Emulator

				private FieldInfo Core { get; }
				#endregion // private members
			}
			#endregion // class CollectionConnector

			#region class DictionaryConnector
			private class DictionaryConnector : IReadOnlyIndexer<object, IDictionary>
			{
				public DictionaryConnector(FieldInfo core)
				{
					Core = core;
				}

				#region IReadOnlyIndexer<object, IDictionary> interface support
				public IDictionary this[object target]
				{
					get
					{
						return Core.GetValue(target) as IDictionary;
					}
				}
				#endregion  // IReadOnlyIndexer<object, IDictionary> interface support

				#region private members
				private FieldInfo Core { get; }
				#endregion // private members
			}
			#endregion // class DictionaryConnector
			#endregion // private members
		}
		#endregion // class MemberFieldProfile

		#region class MemberPropertyProfile
		private class MemberPropertyProfile : ICore
		{
			public MemberPropertyProfile(PropertyInfo propertyInfo)
			{
				MemberInfo = propertyInfo;
				Value = new ValueConnector(propertyInfo);
				Collection = new CollectionConnector(propertyInfo);
				Dictionary = new DictionaryConnector(propertyInfo);

				Setup();
			}

			public PropertyInfo MemberInfo { get; }

			#region ICore interface support
			MemberInfo ICore.MemberInfo => MemberInfo;

			public Type MemberType => MemberInfo.PropertyType;

			public Archetype Archetype => this.archetype;

			public IIndexer<object, object> Value { get; }

			public IReadOnlyIndexer<object, IList> Collection { get; }

			public IReadOnlyIndexer<object, IDictionary> Dictionary { get; }
			#endregion  // ICore interface support

			#region private members
			#region class ValueConnector
			private class ValueConnector : IIndexer<object, object>
			{
				public ValueConnector(PropertyInfo core)
				{
					Core = core;
				}

				#region IReadOnlyIndexer<object, object> interface support
				public object this[object target]
				{
					get { return Core.GetValue(target); }
					set { Core.SetValue(target, value); }
				}
				#endregion  // IReadOnlyIndexer<object, object> interface support

				#region private members
				private PropertyInfo Core { get; }
				#endregion // private members
			}
			#endregion // class ValueConnector

			#region class CollectionConnector
			private class CollectionConnector : IReadOnlyIndexer<object, IList>
			{
				public CollectionConnector(PropertyInfo core)
				{
					Core = core;
				}

				#region IReadOnlyIndexer<object, IList> interface support
				public IList this[object target]
				{
					get
					{
						var value = Core.GetValue(target);
						if (value is IList list)
						{
							return list;
						}
						else if (value is IEnumerable enumerable)
						{
							return new Emulator(enumerable, Core, target);
						}

						return null;
					}
				}
				#endregion  // IReadOnlyIndexer<object, IList> interface support

				#region private members
				#region class Emulator
				private class Emulator : CollectionEmulatorBase
				{
					public Emulator(IEnumerable core, PropertyInfo member, object target)
						: base(core)
					{
						Member = member;
						Target = target;
					}

					#region CollectionEmulatorBase implement
					public override object this[int index]
					{
						get { return Target.GetType().InvokeMember(Member.Name, BindingFlags.GetProperty, null, Target, new object[] { index, }); }
						set { Target.GetType().InvokeMember(Member.Name, BindingFlags.SetProperty, null, Target, new object[] { index, value, }); }
					}
					#endregion  // CollectionEmulatorBase implement

					#region private members
					private PropertyInfo Member { get; }

					private object Target { get; }
					#endregion // private members
				}
				#endregion // class Provider

				private PropertyInfo Core { get; }
				#endregion // private members
			}
			#endregion // class CollectionConnector

			#region class DictionaryConnector
			private class DictionaryConnector : IReadOnlyIndexer<object, IDictionary>
			{
				public DictionaryConnector(PropertyInfo core)
				{
					Core = core;
				}

				#region IReadOnlyIndexer<object, IDictionary> interface support
				public IDictionary this[object target]
				{
					get
					{
						return Core.GetValue(target) as IDictionary;
					}
				}
				#endregion  // IReadOnlyIndexer<object, IDictionary> interface support

				#region private members
				private PropertyInfo Core { get; }
				#endregion // private members
			}
			#endregion // class DictionaryConnector

			private Archetype archetype;

			private void Setup()
			{
				var parameters = MemberInfo.GetIndexParameters();
				if (parameters.Length == 1)
				{
					if (typeof(IDictionary).IsAssignableFrom(MemberInfo.DeclaringType))
					{
						this.archetype = Archetype.Dictionary;
					}
					else
					{
						this.archetype = Archetype.Collection;
					}
				}
			}
			#endregion // private members
		}
		#endregion // class MemberPropertyProfile

		private List<PWalkAttribute> attributes = new List<PWalkAttribute>();
		private List<PWalkMarkAttribute> marks = new List<PWalkMarkAttribute>();
		private List<PWalkOptionDataAttribute> optionData = new List<PWalkOptionDataAttribute>();
		private XmlElementAttribute xmlElementAttribute;
		private XmlArrayItemAttribute xmlArrayItemAttribute;

		private ICore Core { get; }

		private void Setup()
		{
			foreach (var attribute in Attribute.GetCustomAttributes(MemberInfo))
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

				if (attribute is XmlElementAttribute xmlElementAttribute)
				{
					this.xmlElementAttribute = xmlElementAttribute;
				}

				if (attribute is XmlArrayItemAttribute xmlArrayItemAttribute)
				{
					this.xmlArrayItemAttribute = xmlArrayItemAttribute;
				}
			}
		}
		#endregion // private members
	}
}
