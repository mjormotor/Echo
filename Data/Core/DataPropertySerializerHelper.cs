using System;
using System.Xml;

namespace Echo.Data.Core
{
	internal static class DataPropertySerializerHelper
	{
		public const string QNameAttributeName = "qname";

		public static Type DeserializeType(this XmlElement self)
		{
			Type ret = null;
			var typeName = self.GetAttribute(QNameAttributeName);
			if (!string.IsNullOrEmpty(typeName))
			{
				ret = Type.GetType(typeName);
			}

			return ret;
		}

		public static void SerializeType(this XmlElement self, Type type)
		{
			self.SetAttribute(QNameAttributeName, type.AssemblyQualifiedName);
		}
	}
}
