using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Echo.PWalkService;

namespace Echo.Data
{
	using Core;

	/// <summary>
	/// データプロパティ永続化
	/// </summary>
	/// <remarks>
	/// データプロパティを持つオブジェクトを XML 形式で永続化します。
	/// </remarks>
	public class DataPropertySerializer
	{
		/// <summary>
		/// 既定の XML の書き出し設定
		/// </summary>
		public XmlWriterSettings DefaultXmlWriterSettings { get; set; } = new XmlWriterSettings()
		{
			NewLineChars = "\r\n",
			IndentChars = "\t",
			Indent = true,
			Encoding = Encoding.UTF8,
			CheckCharacters = false,
		};

		/// <summary>
		/// 永続化
		/// </summary>
		public void Serialize(Stream stream, object data)
		{
			var document = new XmlDocument() { PreserveWhitespace = true, };
			Serialize(document, data);
			document.Save(stream);
		}

		/// <summary>
		/// 永続化
		/// </summary>
		public void Serialize(XmlDocument document, object data)
		{
			Serialize(document, data, null);
		}

		/// <summary>
		/// 永続化
		/// </summary>
		public void Serialize(XmlDocument document, object data, string rootNodeName)
		{
			Serialize(document, data, null, DefaultXmlWriterSettings);
		}

		/// <summary>
		/// 永続化
		/// </summary>
		public void Serialize(XmlDocument document, object data, string rootNodeName, XmlWriterSettings writerSettings)
		{
			document.RemoveAll();

			if (data == null)
			{
				return;
			}

			if (string.IsNullOrEmpty(rootNodeName))
			{
				rootNodeName = data.GetType().Name;
			}

			var declaration = document.CreateXmlDeclaration("1.0", null, null);
			document.AppendChild(declaration);

			var element = document.CreateElement(rootNodeName);
			document.AppendChild(element);

			document.DocumentElement.SetAttribute("xmlns:xsi", XmlSchema.InstanceNamespace);
			document.DocumentElement.SetAttribute("xmlns:xsd", XmlSchema.Namespace);

			var context = new DataPropertySerializeContext() { TargetElement = element, };
			PWalk.TargetType<IDataPropertyCore>(SerializeWalk, data, context, 1, PWalkOption.CallProcessAlsoOnWayBack | PWalkOption.CheckLoop | PWalkOption.CheckVisit);

			if (document.PreserveWhitespace && writerSettings != null)
			{
				using var stream = new MemoryStream();
				using var writer = XmlWriter.Create(stream, writerSettings);
				document.Save(writer);
				stream.Position = 0;
				document.Load(stream);
			}
		}

		/// <summary>
		/// 逆永続化
		/// </summary>
		public void Deserialize(Stream stream, object data)
		{
			var document = new XmlDocument() { PreserveWhitespace = true, };
			document.Load(stream);
			Deserialize(document, data);
		}

		/// <summary>
		/// 逆永続化
		/// </summary>
		public void Deserialize(XmlDocument document, object data)
		{
			if (data == null)
			{
				return;
			}

			DeserializeCore(document.DocumentElement, data, string.Empty);
		}

		#region private members
		private enum Archetype
		{
			Default,
			KeyValuePair,
			KeyValueShell,
		}

		private const string ItemNodeName = "Item";
		private const string KeyNodeName = "Key";
		private const string ValueNodeName = "Value";
		private const string CodeAttributeName = "code";
		private const string ReferenceAttributeName = "reference";
		private const string ArchetypeAttributeName = "archetype";

		private static readonly PWalkManager PWalk = new PWalkManager() { SolveDictionaryItemNodeNameDelegate = SolveDictionaryItemNodeName, };

		private static string SolveDictionaryItemNodeName(Type type, object value)
		{
			if (type == typeof(string))
			{
				return (string)value;
			}
			else if (type == typeof(bool))
			{
				return XmlConvert.ToString((bool)value);
			}
			else if (type == typeof(char))
			{
				return XmlConvert.ToString((char)value);
			}
			else if (type == typeof(sbyte))
			{
				return XmlConvert.ToString((sbyte)value);
			}
			else if (type == typeof(byte))
			{
				var byteValue = (byte)value;
				return byteValue.ToString("X02");
			}
			else if (type == typeof(short))
			{
				return XmlConvert.ToString((short)value);
			}
			else if (type == typeof(ushort))
			{
				return XmlConvert.ToString((ushort)value);
			}
			else if (type == typeof(int))
			{
				return XmlConvert.ToString((int)value);
			}
			else if (type == typeof(uint))
			{
				return XmlConvert.ToString((uint)value);
			}
			else if (type == typeof(long))
			{
				return XmlConvert.ToString((long)value);
			}
			else if (type == typeof(ulong))
			{
				return XmlConvert.ToString((ulong)value);
			}
			else if (type == typeof(float))
			{
				return XmlConvert.ToString((float)value);
			}
			else if (type == typeof(double))
			{
				return XmlConvert.ToString((double)value);
			}
			else if (type == typeof(decimal))
			{
				return XmlConvert.ToString((decimal)value);
			}
			else if (type == typeof(DateTime))
			{
				return XmlConvert.ToString((DateTime)value, XmlDateTimeSerializationMode.Local);
			}
			else if (type == typeof(Guid))
			{
				return XmlConvert.ToString((byte)value);
			}
			else if (type == typeof(TimeSpan))
			{
				return XmlConvert.ToString((Guid)value);
			}
			else if (type == typeof(DateTimeOffset))
			{
				return XmlConvert.ToString((DateTimeOffset)value);
			}
			else if (type.IsEnum)
			{
				var isFlag = type.IsDefined(typeof(FlagsAttribute), false);
				if (!isFlag)
				{
					return EnumHelper.ToPrimalName(value);
				}
				else
				{
					var flags = new List<object>(EnumHelper.ToFlags(value));
					return string.Join('|', flags.Select(_ => EnumHelper.ToPrimalName(_)));
				}
			}

			return null;
		}

		private static void SerializeWalk(IDataPropertyCore current, PWalkContext pwalk)
		{
			var context = (DataPropertySerializeContext)pwalk.UserData;
			if (context.TargetElement != null)
			{
				var document = context.TargetElement.OwnerDocument;
				if (pwalk.IsOnWayForward)
				{
					var link = pwalk.CurrentNode.ParentLink;
					if (link != null && link.Archetype == PWalkService.Archetype.Dictionary)
					{
						var parent = pwalk.CurrentNode.Parent;
						if (parent.Value is IDataPropertyDictionary property)
						{
							var child = document.CreateElement(current.Name);
							child.SetAttribute(ArchetypeAttributeName, nameof(Archetype.KeyValuePair));
							context.TargetElement.AppendChild(child);
							context.TargetElement = child;

							var key = document.CreateElement(KeyNodeName);
							SerializeKeyCore(key, property.KeyType, link.Key, pwalk);
							child.AppendChild(key);

							var value = document.CreateElement(ValueNodeName);
							SerializePropertyCore(value, (IDataProperty)current, pwalk);
							child.AppendChild(value);
						}
					}
					else
					{
						var child = document.CreateElement(current.Name);
						if (current is IDataProperty property)
						{
							SerializePropertyCore(child, property, pwalk);
						}

						context.TargetElement.AppendChild(child);
						context.TargetElement = child;
					}
				}
				else
				{
					var element = pwalk.CurrentNode.UserData as XmlElement;
					if (element == null)
					{
						element = context.TargetElement;
					}

					var parent = element.ParentNode as XmlElement;
					context.TargetElement = parent;
				}
			}
		}

		private static void SerializeValueCore(XmlElement element, object data, PWalkContext pwalk, bool isKey = false)
		{
			element.RemoveAll();

			if (data == null)
			{
				return;
			}

			var type = data.GetType();
			var nodeName = type.Name;

			var document = element.OwnerDocument;
			var node = document.CreateElement(nodeName);
			node.SerializeType(type);
			element.AppendChild(node);

			var context = (DataPropertySerializeContext)pwalk.UserData;
			if (!type.IsValueType)
			{
				if (context.ReferencedPaths.TryGetValue(data, out var path))
				{
					node.SetAttribute(ReferenceAttributeName, path);
					return;
				}

				path = context.RootPath + pwalk.CurrentNode.Path;
				if (isKey)
				{
					path = $"{path}.{KeyNodeName}";
				}

				context.ReferencedPaths.Add(data, path);
			}

			var childRootPath = $"{context.RootPath}{pwalk.CurrentNode.Path}/Value";
			var childContext = new DataPropertySerializeContext(context.ReferencedPaths) { TargetElement = node, RootPath = childRootPath, };
			PWalk.TargetType<IDataPropertyCore>(SerializeWalk, data, childContext, 1, PWalkOption.CallProcessAlsoOnWayBack | PWalkOption.CheckLoop | PWalkOption.CheckVisit);
		}

		private static void SerializePropertyCore(XmlElement element, IDataProperty property, PWalkContext pwalk)
		{
			var type = property.PropertyType;
			var value = property.Value;

			if (type == typeof(object))
			{
				if (value != null)
				{
					type = value.GetType();
					if (SerializeGeneralValueCore(element, type, value, pwalk))
					{
						element.SerializeType(type);
					}
					else
					{
						SerializeValueCore(element, value, pwalk);
					}
				}
			}
			else
			{
				if (!SerializeGeneralValueCore(element, type, value, pwalk))
				{
					SerializeValueCore(element, value, pwalk);
				}
			}
		}

		private static void SerializeKeyCore(XmlElement element, Type type, object key, PWalkContext pwalk)
		{
			if (type == typeof(object))
			{
				type = key.GetType();
				if (SerializeGeneralValueCore(element, type, key, pwalk))
				{
					element.SerializeType(type);
				}
				else
				{
					SerializeValueCore(element, key, pwalk, true);
				}
			}
			else
			{
				if (!SerializeGeneralValueCore(element, type, key, pwalk))
				{
					SerializeValueCore(element, key, pwalk, true);
				}
			}
		}

		private static bool SerializeGeneralValueCore(XmlElement element, Type type, object value, PWalkContext pwalk)
		{
			var ret = false;
			var normalizedType = type;
			if (normalizedType.IsNullable())
			{
				normalizedType = normalizedType.GetGenericArguments()[0];
			}

			if (normalizedType == typeof(bool))
			{
				ret = SerializeBooleanValueCore(element, value);
			}
			else if (normalizedType == typeof(char))
			{
				ret = SerializeCharValueCore(element, value);
			}
			else if (normalizedType == typeof(sbyte))
			{
				ret = SerializeSByteValueCore(element, value);
			}
			else if (normalizedType == typeof(byte))
			{
				ret = SerializeByteValueCore(element, value);
			}
			else if (normalizedType == typeof(short))
			{
				ret = SerializeInt16ValueCore(element, value);
			}
			else if (normalizedType == typeof(ushort))
			{
				ret = SerializeUInt16ValueCore(element, value);
			}
			else if (normalizedType == typeof(int))
			{
				ret = SerializeInt32ValueCore(element, value);
			}
			else if (normalizedType == typeof(uint))
			{
				ret = SerializeUInt32ValueCore(element, value);
			}
			else if (normalizedType == typeof(long))
			{
				ret = SerializeInt64ValueCore(element, value);
			}
			else if (normalizedType == typeof(ulong))
			{
				ret = SerializeUInt64ValueCore(element, value);
			}
			else if (normalizedType == typeof(float))
			{
				ret = SerializeSingleValueCore(element, value);
			}
			else if (normalizedType == typeof(double))
			{
				ret = SerializeDoubleValueCore(element, value);
			}
			else if (normalizedType == typeof(decimal))
			{
				ret = SerializeDecimalValueCore(element, value);
			}
			else if (normalizedType == typeof(DateTime))
			{
				ret = SerializeDateTimeValueCore(element, value);
			}
			else if (normalizedType == typeof(string))
			{
				ret = SerializeStringValueCore(element, value);
			}
			else if (normalizedType == typeof(Guid))
			{
				ret = SerializeGuidValueCore(element, value);
			}
			else if (normalizedType == typeof(TimeSpan))
			{
				ret = SerializeTimeSpanValueCore(element, value);
			}
			else if (normalizedType == typeof(DateTimeOffset))
			{
				ret = SerializeDateTimeOffsetValueCore(element, value);
			}
			else if (normalizedType.IsEnum)
			{
				ret = SerializeEnumValueCore(normalizedType, element, value);
			}

			return ret;
		}

		private static bool SerializeBooleanValueCore(XmlElement element, object value)
		{
			var ret = false;
			if (value is bool boolValue)
			{
				element.InnerText = boolValue ? "1" : "0";
				ret = true;
			}

			return ret;
		}

		private static bool SerializeCharValueCore(XmlElement element, object value)
		{
			var ret = false;
			if (value is char charValue)
			{
				element.InnerText = XmlConvert.ToString(charValue);
				ret = true;
			}

			return ret;
		}

		private static bool SerializeSByteValueCore(XmlElement element, object value)
		{
			var ret = false;
			if (value is sbyte sbyteValue)
			{
				element.InnerText = XmlConvert.ToString(sbyteValue);
				ret = true;
			}

			return ret;
		}

		private static bool SerializeByteValueCore(XmlElement element, object value)
		{
			var ret = false;
			if (value is byte byteValue)
			{
				element.InnerText = byteValue.ToString("X02");
				ret = true;
			}

			return ret;
		}

		private static bool SerializeInt16ValueCore(XmlElement element, object value)
		{
			var ret = false;
			if (value is short shortValue)
			{
				element.InnerText = XmlConvert.ToString(shortValue);
				ret = true;
			}

			return ret;
		}

		private static bool SerializeUInt16ValueCore(XmlElement element, object value)
		{
			var ret = false;
			if (value is ushort ushortValue)
			{
				element.InnerText = XmlConvert.ToString(ushortValue);
				ret = true;
			}

			return ret;
		}

		private static bool SerializeInt32ValueCore(XmlElement element, object value)
		{
			var ret = false;
			if (value is int intValue)
			{
				element.InnerText = XmlConvert.ToString(intValue);
				ret = true;
			}

			return ret;
		}

		private static bool SerializeUInt32ValueCore(XmlElement element, object value)
		{
			var ret = false;
			if (value is uint uintValue)
			{
				element.InnerText = XmlConvert.ToString(uintValue);
				ret = true;
			}

			return ret;
		}

		private static bool SerializeInt64ValueCore(XmlElement element, object value)
		{
			var ret = false;
			if (value is long longValue)
			{
				element.InnerText = XmlConvert.ToString(longValue);
				ret = true;
			}

			return ret;
		}

		private static bool SerializeUInt64ValueCore(XmlElement element, object value)
		{
			var ret = false;
			if (value is ulong ulongValue)
			{
				element.InnerText = XmlConvert.ToString(ulongValue);
				ret = true;
			}

			return ret;
		}

		private static bool SerializeSingleValueCore(XmlElement element, object value)
		{
			var ret = false;
			if (value is float singleValue)
			{
				element.InnerText = XmlConvert.ToString(singleValue);
				ret = true;
			}

			return ret;
		}

		private static bool SerializeDoubleValueCore(XmlElement element, object value)
		{
			var ret = false;
			if (value is double doubleValue)
			{
				element.InnerText = XmlConvert.ToString(doubleValue);
				ret = true;
			}

			return ret;
		}

		private static bool SerializeDecimalValueCore(XmlElement element, object value)
		{
			var ret = false;
			if (value is decimal decimalValue)
			{
				element.InnerText = XmlConvert.ToString(decimalValue);
				ret = true;
			}

			return ret;
		}

		private static bool SerializeDateTimeValueCore(XmlElement element, object value)
		{
			var ret = false;
			if (value is DateTime dateTimeValue)
			{
				element.InnerText = XmlConvert.ToString(dateTimeValue, XmlDateTimeSerializationMode.Local);
				ret = true;
			}

			return ret;
		}

		private static bool SerializeStringValueCore(XmlElement element, object value)
		{
			var ret = false;
			if (value is string stringValue)
			{
				element.InnerText = stringValue;
				ret = true;
			}

			return ret;
		}

		private static bool SerializeGuidValueCore(XmlElement element, object value)
		{
			var ret = false;
			if (value is Guid guidValue)
			{
				element.InnerText = XmlConvert.ToString(guidValue);
				ret = true;
			}

			return ret;
		}

		private static bool SerializeTimeSpanValueCore(XmlElement element, object value)
		{
			var ret = false;
			if (value is TimeSpan timeSpanValue)
			{
				element.InnerText = XmlConvert.ToString(timeSpanValue);
				ret = true;
			}

			return ret;
		}

		private static bool SerializeDateTimeOffsetValueCore(XmlElement element, object value)
		{
			var ret = false;
			if (value is DateTimeOffset dateTimeOffsetValue)
			{
				element.InnerText = XmlConvert.ToString(dateTimeOffsetValue);
				ret = true;
			}

			return ret;
		}

		private static bool SerializeEnumValueCore(Type type, XmlElement element, object value)
		{
			var ret = false;
			if (value != null)
			{
				var isFlag = type.IsDefined(typeof(FlagsAttribute), false);
				if (!isFlag)
				{
					element.InnerText = EnumHelper.ToPrimalName(value);
					element.SetAttribute(CodeAttributeName, value.GetHashCode().ToString());
					ret = true;
				}
				else
				{
					var flags = new List<object>(EnumHelper.ToFlags(value));
					if (flags.Count == 1)
					{
						element.InnerText = EnumHelper.ToPrimalName(flags[0]);
						element.SetAttribute(CodeAttributeName, value.GetHashCode().ToString("X08"));
						ret = true;
					}
					else
					{
						var document = element.OwnerDocument;
						foreach (var flag in flags)
						{
							var item = document.CreateElement(ItemNodeName);
							item.InnerText = EnumHelper.ToPrimalName(flag);
							item.SetAttribute(CodeAttributeName, flag.GetHashCode().ToString("X08"));
							ret = true;
							element.AppendChild(item);
						}
					}
				}
			}

			return ret;
		}

		private static void DeserializeCore(XmlElement element, object data, string currentPath)
		{
			var itemCount = 0;
			var result = PWalk.FindNode(currentPath, data);
			if (result)
			{
				var pwalk = result.Context;
				var parent = pwalk.CurrentNode.Value;
				var parentCollection = parent as IDataPropertyCollection;
				var parentDictionary = parent as IDataPropertyDictionary;
				foreach (var node in element.ChildNodes)
				{
					if (node is XmlElement child)
					{
						var path = $"{currentPath}/{child.Name}";
						result = PWalk.FindNode(path, pwalk);
						if (result)
						{
							var target = result.Context.CurrentNode.Value;
							if (target is IDataProperty property)
							{
								DeserializePropertyCore(child, property, data, out var subject);
								if (property.Value != null && subject != null)
								{
									DeserializeCore(subject, data, $"{currentPath}/{subject.ParentNode.Name}/Value");
								}
							}
							else
							{
								DeserializeCore(child, data, path);
							}
						}
						else if (parentCollection != null)
						{
							if (parentCollection.Count <= itemCount)
							{
								parentCollection.Extend();
							}

							var itemPath = $"{currentPath}/{itemCount++}";
							result = PWalk.FindNode(itemPath, pwalk);
							if (result)
							{
								var target = result.Context.CurrentNode.Value;
								if (target is IDataProperty property)
								{
									DeserializePropertyCore(child, property, data, out var subject);
									if (property.Value != null && subject != null)
									{
										DeserializeCore(subject, data, $"{itemPath}/Value");
									}
								}
							}
						}
						else if (parentDictionary != null)
						{
							if (child.HasAttribute(ArchetypeAttributeName))
							{
								var archetype = (Archetype)Enum.Parse(typeof(Archetype), child.GetAttribute(ArchetypeAttributeName));
								switch (archetype)
								{
									case Archetype.KeyValuePair:
										var keyNode = child[KeyNodeName];
										var valueNode = child[ValueNodeName];
										if (keyNode != null && valueNode != null)
										{
											var key = DeserializeKeyCore(keyNode, parentDictionary.KeyType, data, out var subject);
											if (key != null)
											{
												if (subject != null)
												{
													DeserializeCore(subject, key, string.Empty);
												}

												DeserializeDictionaryItemCore(parentDictionary, key, valueNode, data, $"{currentPath}/{itemCount++}", pwalk);
											}
										}

										break;
								}
							}
						}
					}
				}

				if (parentCollection != null)
				{
					if (parentCollection.Count > itemCount)
					{
						parentCollection.Reduce(parentCollection.Count - itemCount);
					}
				}
			}
		}

		private static void DeserializeDictionaryItemCore(IDataPropertyDictionary dataPropertyDictionary, object key, XmlElement element, object data, string path, PWalkContext pwalk)
		{
			if (!dataPropertyDictionary.Contains(key))
			{
				dataPropertyDictionary.Extend(key);
			}

			var result = PWalk.FindNode(path, pwalk);
			if (result)
			{
				var target = result.Context.CurrentNode.Value;
				if (target is IDataProperty property)
				{
					DeserializePropertyCore(element, property, data, out var subject);
					if (property.Value != null && subject != null)
					{
						DeserializeCore(subject, data, $"{path}/Value");
					}
				}
			}
		}

		private static object DeserializeValueCore(XmlElement element, Type type, object value, object data, out XmlElement subject)
		{
			var ret = value;
			subject = null;
			foreach (var node in element.ChildNodes)
			{
				var child = node as XmlElement;
				if (child != null)
				{
					if (ret == null)
					{
						var path = child.GetAttribute(ReferenceAttributeName);
						if (!string.IsNullOrEmpty(path))
						{
							var result = PWalk.FindNode(path, data);
							if (result)
							{
								if (result.Context.CurrentNode.Value is IDataProperty referenced)
								{
									ret = referenced.Value;
								}
							}
							else
							{
								if (path.EndsWith($".{KeyNodeName}"))
								{
									result = PWalk.FindNode(path.Substring(0, path.Length - (KeyNodeName.Length + 1)), data);
									if (result.Context.CurrentNode.ParentLink != null)
									{
										ret = result.Context.CurrentNode.ParentLink.Key;
									}
								}
							}
						}
						else
						{
							ret = Activator.CreateInstance(child.DeserializeType() ?? type);
						}
					}

					if (ret != null)
					{
						subject = child;
						break;
					}
				}
			}

			return ret;
		}

		private static void DeserializePropertyCore(XmlElement element, IDataProperty property, object data, out XmlElement subject)
		{
			subject = null;
			var type = property.PropertyType;
			if (type == typeof(object))
			{
				type = element.DeserializeType();
				if (type != null)
				{
					if (DeserializeGeneralValueCore(element, type, out var value))
					{
						property.Value = value;
					}
					else
					{
						property.Value = DeserializeValueCore(element, type, property.Value, data, out subject);
					}
				}
			}
			else
			{
				if (DeserializeGeneralValueCore(element, type, out var value))
				{
					property.Value = value;
				}
				else
				{
					property.Value = DeserializeValueCore(element, type, property.Value, data, out subject);
				}
			}
		}

		private static object DeserializeKeyCore(XmlElement element, Type type, object data, out XmlElement subject)
		{
			object ret = null;
			subject = null;
			if (type == typeof(object))
			{
				type = element.DeserializeType();
				if (type != null)
				{
					if (!DeserializeGeneralValueCore(element, type, out ret))
					{
						ret = DeserializeValueCore(element, type, ret, data, out subject);
					}
				}
			}
			else
			{
				if (!DeserializeGeneralValueCore(element, type, out ret))
				{
					ret = DeserializeValueCore(element, type, ret, data, out subject);
				}
			}

			return ret;
		}

		private static bool DeserializeGeneralValueCore(XmlElement element, Type type, out object value)
		{
			var ret = false;
			var normalizedType = type;
			var isNullable = false;
			if (normalizedType.IsNullable())
			{
				normalizedType = normalizedType.GetGenericArguments()[0];
				isNullable = true;
			}

			if (normalizedType == typeof(bool))
			{
				ret = DeserializeBooleanValueCore(element, out value);
				if (ret && value == null && !isNullable)
				{
					value = default(bool);
				}
			}
			else if (normalizedType == typeof(char))
			{
				ret = DeserializeCharValueCore(element, out value);
				if (ret && value == null && !isNullable)
				{
					value = default(char);
				}
			}
			else if (normalizedType == typeof(sbyte))
			{
				ret = DeserializeSByteValueCore(element, out value);
				if (ret && value == null && !isNullable)
				{
					value = default(sbyte);
				}
			}
			else if (normalizedType == typeof(byte))
			{
				ret = DeserializeByteValueCore(element, out value);
				if (ret && value == null && !isNullable)
				{
					value = default(byte);
				}
			}
			else if (normalizedType == typeof(short))
			{
				ret = DeserializeInt16ValueCore(element, out value);
				if (ret && value == null && !isNullable)
				{
					value = default(short);
				}
			}
			else if (normalizedType == typeof(ushort))
			{
				ret = DeserializeUInt16ValueCore(element, out value);
				if (ret && value == null && !isNullable)
				{
					value = default(ushort);
				}
			}
			else if (normalizedType == typeof(int))
			{
				ret = DeserializeInt32ValueCore(element, out value);
				if (ret && value == null && !isNullable)
				{
					value = default(int);
				}
			}
			else if (normalizedType == typeof(uint))
			{
				ret = DeserializeUInt32ValueCore(element, out value);
				if (ret && value == null && !isNullable)
				{
					value = default(uint);
				}
			}
			else if (normalizedType == typeof(long))
			{
				ret = DeserializeInt64ValueCore(element, out value);
				if (ret && value == null && !isNullable)
				{
					value = default(long);
				}
			}
			else if (normalizedType == typeof(ulong))
			{
				ret = DeserializeUInt64ValueCore(element, out value);
				if (ret && value == null && !isNullable)
				{
					value = default(ulong);
				}
			}
			else if (normalizedType == typeof(float))
			{
				ret = DeserializeSingleValueCore(element, out value);
				if (ret && value == null && !isNullable)
				{
					value = default(float);
				}
			}
			else if (normalizedType == typeof(double))
			{
				ret = DeserializeDoubleValueCore(element, out value);
				if (ret && value == null && !isNullable)
				{
					value = default(double);
				}
			}
			else if (normalizedType == typeof(decimal))
			{
				ret = DeserializeDecimalValueCore(element, out value);
				if (ret && value == null && !isNullable)
				{
					value = default(decimal);
				}
			}
			else if (normalizedType == typeof(DateTime))
			{
				ret = DeserializeDateTimeValueCore(element, out value);
				if (ret && value == null && !isNullable)
				{
					value = default(DateTime);
				}
			}
			else if (normalizedType == typeof(string))
			{
				ret = DeserializeStringValueCore(element, out value);
			}
			else if (normalizedType == typeof(Guid))
			{
				ret = DeserializeGuidValueCore(element, out value);
				if (ret && value == null && !isNullable)
				{
					value = default(Guid);
				}
			}
			else if (normalizedType == typeof(TimeSpan))
			{
				ret = DeserializeTimeSpanValueCore(element, out value);
				if (ret && value == null && !isNullable)
				{
					value = default(TimeSpan);
				}
			}
			else if (normalizedType == typeof(DateTimeOffset))
			{
				ret = DeserializeDateTimeOffsetValueCore(element, out value);
				if (ret && value == null && !isNullable)
				{
					value = default(DateTimeOffset);
				}
			}
			else if (normalizedType.IsEnum)
			{
				ret = DeserializeEnumValueCore(normalizedType, element, out value);
				if (ret && value == null && !isNullable)
				{
					value = Enum.ToObject(type, 0);
				}
			}
			else
			{
				value = null;
			}

			return ret;
		}

		private static bool DeserializeBooleanValueCore(XmlElement element, out object value)
		{
			var ret = false;
			value = null;
			if (element.IsEmpty || string.IsNullOrEmpty(element.InnerText))
			{
				return true;
			}

			try
			{
				var numericValue = XmlConvert.ToDouble(element.InnerText);
				value = numericValue != 0.0;
				ret = true;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			return ret;
		}

		private static bool DeserializeCharValueCore(XmlElement element, out object value)
		{
			var ret = false;
			value = null;
			if (element.IsEmpty || string.IsNullOrEmpty(element.InnerText))
			{
				return true;
			}

			try
			{
				value = XmlConvert.ToChar(element.InnerText);
				ret = true;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			return ret;
		}

		private static bool DeserializeSByteValueCore(XmlElement element, out object value)
		{
			var ret = false;
			value = null;
			if (element.IsEmpty || string.IsNullOrEmpty(element.InnerText))
			{
				return true;
			}

			try
			{
				value = XmlConvert.ToSByte(element.InnerText);
				ret = true;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			return ret;
		}

		private static bool DeserializeByteValueCore(XmlElement element, out object value)
		{
			var ret = false;
			value = null;
			if (element.IsEmpty || string.IsNullOrEmpty(element.InnerText))
			{
				return true;
			}

			try
			{
				value = Convert.ToByte(element.InnerText, 16);
				ret = true;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			return ret;
		}

		private static bool DeserializeInt16ValueCore(XmlElement element, out object value)
		{
			var ret = false;
			value = null;
			if (element.IsEmpty || string.IsNullOrEmpty(element.InnerText))
			{
				return true;
			}

			try
			{
				value = XmlConvert.ToInt16(element.InnerText);
				ret = true;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			return ret;
		}

		private static bool DeserializeUInt16ValueCore(XmlElement element, out object value)
		{
			var ret = false;
			value = null;
			if (element.IsEmpty || string.IsNullOrEmpty(element.InnerText))
			{
				return true;
			}

			try
			{
				value = XmlConvert.ToUInt16(element.InnerText);
				ret = true;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			return ret;
		}

		private static bool DeserializeInt32ValueCore(XmlElement element, out object value)
		{
			var ret = false;
			value = null;
			if (element.IsEmpty || string.IsNullOrEmpty(element.InnerText))
			{
				return true;
			}

			try
			{
				value = XmlConvert.ToInt32(element.InnerText);
				ret = true;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			return ret;
		}

		private static bool DeserializeUInt32ValueCore(XmlElement element, out object value)
		{
			var ret = false;
			value = null;
			if (element.IsEmpty || string.IsNullOrEmpty(element.InnerText))
			{
				return true;
			}

			try
			{
				value = XmlConvert.ToUInt32(element.InnerText);
				ret = true;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			return ret;
		}

		private static bool DeserializeInt64ValueCore(XmlElement element, out object value)
		{
			var ret = false;
			value = null;
			if (element.IsEmpty || string.IsNullOrEmpty(element.InnerText))
			{
				return true;
			}

			try
			{
				value = XmlConvert.ToInt64(element.InnerText);
				ret = true;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			return ret;
		}

		private static bool DeserializeUInt64ValueCore(XmlElement element, out object value)
		{
			var ret = false;
			value = null;
			if (element.IsEmpty || string.IsNullOrEmpty(element.InnerText))
			{
				return true;
			}

			try
			{
				value = XmlConvert.ToUInt64(element.InnerText);
				ret = true;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			return ret;
		}

		private static bool DeserializeSingleValueCore(XmlElement element, out object value)
		{
			var ret = false;
			value = null;
			if (element.IsEmpty || string.IsNullOrEmpty(element.InnerText))
			{
				return true;
			}

			try
			{
				value = XmlConvert.ToSingle(element.InnerText);
				ret = true;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			return ret;
		}

		private static bool DeserializeDoubleValueCore(XmlElement element, out object value)
		{
			var ret = false;
			value = null;
			if (element.IsEmpty || string.IsNullOrEmpty(element.InnerText))
			{
				return true;
			}

			try
			{
				value = XmlConvert.ToDouble(element.InnerText);
				ret = true;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			return ret;
		}

		private static bool DeserializeDecimalValueCore(XmlElement element, out object value)
		{
			var ret = false;
			value = null;
			if (element.IsEmpty || string.IsNullOrEmpty(element.InnerText))
			{
				return true;
			}

			try
			{
				value = XmlConvert.ToDecimal(element.InnerText);
				ret = true;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			return ret;
		}

		private static bool DeserializeDateTimeValueCore(XmlElement element, out object value)
		{
			var ret = false;
			value = null;
			if (element.IsEmpty || string.IsNullOrEmpty(element.InnerText))
			{
				return true;
			}

			try
			{
				value = XmlConvert.ToDateTime(element.InnerText, XmlDateTimeSerializationMode.Local);
				ret = true;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			return ret;
		}

		private static bool DeserializeStringValueCore(XmlElement element, out object value)
		{
			value = !element.IsEmpty ? element.InnerText : value = null;

			return true;
		}

		private static bool DeserializeGuidValueCore(XmlElement element, out object value)
		{
			var ret = false;
			value = null;
			if (element.IsEmpty || string.IsNullOrEmpty(element.InnerText))
			{
				return true;
			}

			try
			{
				value = XmlConvert.ToGuid(element.InnerText);
				ret = true;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			return ret;
		}

		private static bool DeserializeTimeSpanValueCore(XmlElement element, out object value)
		{
			var ret = false;
			value = null;
			if (element.IsEmpty || string.IsNullOrEmpty(element.InnerText))
			{
				return true;
			}

			try
			{
				value = XmlConvert.ToTimeSpan(element.InnerText);
				ret = true;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			return ret;
		}

		private static bool DeserializeDateTimeOffsetValueCore(XmlElement element, out object value)
		{
			var ret = false;
			value = null;
			if (element.IsEmpty || string.IsNullOrEmpty(element.InnerText))
			{
				return true;
			}

			try
			{
				value = XmlConvert.ToDateTimeOffset(element.InnerText);
				ret = true;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			return ret;
		}

		private static bool DeserializeEnumValueCore(Type type, XmlElement element, out object value)
		{
			var ret = false;
			value = null;
			if (element.IsEmpty || string.IsNullOrEmpty(element.InnerText))
			{
				return true;
			}

			if (Enum.TryParse(type, element.InnerText, true, out value))
			{
				ret = true;
			}
			else if (element.HasAttribute(CodeAttributeName))
			{
				value = Enum.ToObject(type, Convert.ToInt32(element.GetAttribute(CodeAttributeName), 16));
				ret = true;
			}
			else
			{
				var isFlag = type.IsDefined(typeof(FlagsAttribute), false);
				if (isFlag)
				{
					var code = 0x00000000;
					foreach (var child in element.ChildNodes)
					{
						if (child is XmlElement flagElement)
						{
							if (flagElement.Name == ItemNodeName)
							{
								if (Enum.TryParse(type, flagElement.InnerText, true, out var flag))
								{
									code |= flag.GetHashCode();
								}
								else if (flagElement.HasAttribute(CodeAttributeName))
								{
									code |= Convert.ToInt32(flagElement.GetAttribute(CodeAttributeName), 16);
								}
							}
						}
					}

					value = Enum.ToObject(type, code);
					ret = true;
				}
			}

			return ret;
		}
		#endregion // private members
	}
}
