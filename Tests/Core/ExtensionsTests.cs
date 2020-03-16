using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Echo.Tests
{
	[TestClass]
	public class ExtensionsTests
	{
		[TestMethod]
		public void EvaluateInheritanceDistanceTest()
		{
			var objectType = typeof(object);
			var baseType = typeof(Base);
			var interfaceType = typeof(Interface);
			var mediumType = typeof(Medium);
			var derivedType = typeof(Derived);

			Assert.IsTrue(baseType.EvaluateInheritanceDistance(baseType) == 0);
			Assert.IsTrue(interfaceType.EvaluateInheritanceDistance(baseType) == -1);
			Assert.IsTrue(mediumType.EvaluateInheritanceDistance(baseType) == 1);
			Assert.IsTrue(mediumType.EvaluateInheritanceDistance(interfaceType) == 1);
			Assert.IsTrue(baseType.EvaluateInheritanceDistance(mediumType) == -1);
			Assert.IsTrue(derivedType.EvaluateInheritanceDistance(baseType) == 2);
			Assert.IsTrue(derivedType.EvaluateInheritanceDistance(mediumType) == 1);
			Assert.IsTrue(derivedType.EvaluateInheritanceDistance(interfaceType) == 2);
			Assert.IsTrue(derivedType.EvaluateInheritanceDistance(objectType) == 3);
		}

		#region private members
		#region class Base
		private class Base
		{
		}
		#endregion // class Base

		#region interface Interface
		private interface Interface
		{
		}
		#endregion // interface Interface

		#region class Medium
		private class Medium : Base, Interface
		{
		}
		#endregion // class Medium

		#region class Derived
		private class Derived : Medium
		{
		}
		#endregion // class Derived
		#endregion // private members
	}
}