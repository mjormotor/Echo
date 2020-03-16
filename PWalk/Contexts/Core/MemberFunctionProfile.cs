using System;
using System.Collections.Generic;
using System.Reflection;

namespace Echo.PWalkService.Core
{
	[System.Diagnostics.DebuggerDisplay("{MemberInfo}")]
	internal class MemberFunctionProfile
	{
		public MemberFunctionProfile(MethodInfo member)
		{
			Core = new MemberMethodProfile(member);

			Setup();
		}

		public MemberFunctionProfile(EventInfo member)
		{
			Core = new MemberEventProfile(member);

			Setup();
		}

		public MemberInfo MemberInfo => Core.MemberInfo;

		public string Name => MemberInfo.Name;

		public IReadOnlyIndexer<object, Action<object[]>> Invoke => Core.Invoke;

		public IEnumerable<PWalkMarkAttribute> Marks => this.marks;

		public IEnumerable<PWalkOptionDataAttribute> OptionData => this.optionData;

		#region private members
		#region interface ICore
		private interface ICore
		{
			MemberInfo MemberInfo { get; }

			IReadOnlyIndexer<object, Action<object[]>> Invoke { get; }
		}
		#endregion // interface ICore

		#region class MemberMethodProfile
		private class MemberMethodProfile : ICore
		{
			public MemberMethodProfile(MethodInfo methodInfo)
			{
				MemberInfo = methodInfo;
				Invoke = new InvokeConnector(methodInfo);
			}

			public MethodInfo MemberInfo { get; }

			#region ICore interface support
			MemberInfo ICore.MemberInfo => MemberInfo;

			public IReadOnlyIndexer<object, Action<object[]>> Invoke { get; }
			#endregion  // ICore interface support

			#region private members
			#region class InvokeConnector
			private class InvokeConnector : IReadOnlyIndexer<object, Action<object[]>>
			{
				public InvokeConnector(MethodInfo core)
				{
					Core = core;

					Setup();
				}

				#region IReadOnlyIndexer<object, Action<object[]>> interface support
				public Action<object[]> this[object target]
				{
					get
					{
						return _ => Core.Invoke(target, _);
					}
				}
				#endregion  // IReadOnlyIndexer<object, Action<object[]>> interface support

				#region private members
				private FieldInfo fieldInfo;

				private MethodInfo Core { get; }

				private void Setup()
				{
					var type = Core.DeclaringType;
					var name = Core.Name;
					this.fieldInfo = type.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				#endregion // private members
			}
			#endregion // class InvokeConnector
			#endregion // private members
		}
		#endregion // class MemberMethodProfile

		#region class MemberEventProfile
		private class MemberEventProfile : ICore
		{
			public MemberEventProfile(EventInfo eventInfo)
			{
				MemberInfo = eventInfo;
				Invoke = new InvokeConnector(eventInfo);
			}

			public EventInfo MemberInfo { get; }

			#region ICore interface support
			MemberInfo ICore.MemberInfo => MemberInfo;

			public IReadOnlyIndexer<object, Action<object[]>> Invoke { get; }
			#endregion  // ICore interface support

			#region private members
			#region class InvokeConnector
			private class InvokeConnector : IReadOnlyIndexer<object, Action<object[]>>
			{
				public InvokeConnector(EventInfo core)
				{
					Core = core;

					Setup();
				}

				#region IReadOnlyIndexer<object, Action<object[]>> interface support
				public Action<object[]> this[object target]
				{
					get
					{
						return _ =>
						{
							var eventDelegates = this.fieldInfo.GetValue(target) as MulticastDelegate;
							if (eventDelegates != null)
							{
								foreach (var eventDelegate in eventDelegates.GetInvocationList())
								{
									eventDelegate.DynamicInvoke(_);
								}
							}
						};
					}
				}
				#endregion  // IReadOnlyIndexer<object, Action<object[]>> interface support

				#region private members
				private FieldInfo fieldInfo;

				private EventInfo Core { get; }

				private void Setup()
				{
					var type = Core.DeclaringType;
					var name = Core.Name;
					this.fieldInfo = type.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				#endregion // private members
			}
			#endregion // class InvokeConnector
			#endregion // private members
		}
		#endregion // class MemberFieldProfile

		private List<PWalkMarkAttribute> marks = new List<PWalkMarkAttribute>();
		private List<PWalkOptionDataAttribute> optionData = new List<PWalkOptionDataAttribute>();

		private ICore Core { get; }

		private void Setup()
		{
			foreach (var attribute in Attribute.GetCustomAttributes(MemberInfo))
			{
				if (attribute is PWalkMarkAttribute markAttribute)
				{
					this.marks.Add(markAttribute);
				}

				if (attribute is PWalkOptionDataAttribute optionDataAttribute)
				{
					this.optionData.Add(optionDataAttribute);
				}
			}
		}
		#endregion // private members
	}
}
