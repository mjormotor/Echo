using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Echo.PWalkService.Tests
{
	[TestClass]
	public class PWalkTests
	{
		[TestMethod]
		public void TargetTypeTest()
		{
			var root = new Root();

			PWalk.TargetType<Target1>(_ => _.Invoke(), root, 1);

			Assert.IsTrue(root.WalkPathClass.Target.InvokedCount == 1);
			Assert.IsTrue(root.WalkPathProperty.Target.InvokedCount == 1);
			Assert.IsTrue(root.WalkSkipProperty.Target.InvokedCount == 0);
		}

		[TestMethod]
		public void MarkedWithTest()
		{
			var root = new Root();
			
			Action<object, PWalkContext> action = (_, context) =>
			{
				var type = context.CurrentNode.AssignedType;
				if (type != null)
				{
					if (type == typeof(string))
					{
						var stringMark = (StringMarkAttribute)context.CurrentMark;
						context.CurrentNode.Value = stringMark.Text;
					}
				}
			};

			PWalk.MarkedWith<StringMarkAttribute>(action, root);

			Assert.IsTrue(root.StringHolder.Text == Target2.VisitedText);
		}

		[TestMethod]
		public void InvokeMarkedFunctionTest()
		{
			var root = new Root();

			PWalk.InvokeMarkedWith<FuncMarkAttribute>(root, new object[] { PWalkProxyParameter.Mark, });

			Assert.IsTrue(root.FuncHolder.Text == Target3.VisitedText);
		}

		[TestMethod]
		public void InvokeMarkedEventTest()
		{
			var root = new Root();

			const string VisitedText = "visited";
			var invokeCount = 0;
			EventHandler<ItemEventArgs<string>> handler = (sender, e) =>
			{
				++invokeCount;
				Assert.IsTrue(e.Item == VisitedText);
			};

			root.FuncHolder.Visited += handler;
			PWalk.InvokeMarkedWith<EventMarkAttribute>(root, new object[] { PWalkProxyParameter.Current, new ItemEventArgs<string>(VisitedText) });
			root.FuncHolder.Visited -= handler;

			Assert.IsTrue(invokeCount == 1);

			var builder = new StringBuilder();
		}

		#region private members
		#region class Root
		private class Root
		{
			public Target1Shell WalkPathClass { get; } = new Target1Shell();

			[PWalkPath(typeof(Target1))]
			public Target1Holder WalkPathProperty { get; } = new Target1Holder();

			public Target1Holder WalkSkipProperty { get; } = new Target1Holder();

			public Target2 StringHolder { get; } = new Target2();

			public Target3 FuncHolder { get; } = new Target3();
		}
		#endregion // class Root

		#region class Target1
		private class Target1
		{
			public int InvokedCount => this.invokedCount;

			public void Invoke()
			{
				++this.invokedCount;
			}

			#region private members
			private int invokedCount;
			#endregion // private members
		}
		#endregion // class Target1

		#region class Target2
		private class Target2
		{
			public const string VisitedText = "visited";

			[StringMark(VisitedText)]
			public string Text { get; set; }
		}
		#endregion // class Target2

		#region class Target3
		private class Target3
		{
			public const string VisitedText = "visited";

			[EventMark]
			public event EventHandler<ItemEventArgs<string>> Visited;

			public string Text => this.text;

			[FuncMark(VisitedText)]
			public void Func(FuncMarkAttribute mark)
			{
				this.text = mark.Text;
			}

			public void RaiseVisited(string text)
			{
				Visited?.Invoke(this, new ItemEventArgs<string>(text));
			}

			#region private members
			private string text;
			#endregion // private members
		}
		#endregion // class Target3

		#region class Target1Shell
		[PWalkPath(typeof(Target1))]
		private class Target1Shell
		{
			public Target1 Target { get; } = new Target1();
		}
		#endregion // class Target1Shell

		#region class Target1Holder
		private class Target1Holder
		{
			public Target1 Target { get; } = new Target1();
		}
		#endregion // class Target1Holder

		#region class StringMarkAttribute
		private class StringMarkAttribute : PWalkMarkAttribute
		{
			public StringMarkAttribute(string text)
			{
				Text = text;
			}

			public string Text { get; }
		}
		#endregion // class StringMarkAttribute

		#region class FuncMarkAttribute
		private class FuncMarkAttribute : PWalkMarkAttribute
		{
			public FuncMarkAttribute(string text)
			{
				Text = text;
			}

			public string Text { get; }
		}
		#endregion // class FuncMarkAttribute

		#region class EventMarkAttribute
		private class EventMarkAttribute : PWalkMarkAttribute
		{
		}
		#endregion // class EventMarkAttribute
		#endregion // private members
	}
}