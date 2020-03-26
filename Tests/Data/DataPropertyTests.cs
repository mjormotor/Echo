using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Echo.Data.Tests
{
	[TestClass]
	public class DataPropertyTests
	{
		[TestMethod]
		public void DataPropertyTest()
		{
			var data = new Data();
			Assert.IsTrue(data.PropertyInt32.Name == nameof(data.PropertyInt32));
		}

		[TestMethod]
		public void DataPropertyEventTest()
		{
			var data = new Data();

			var changingInvokeCount = 0;
			var changedInvokeCount = 0;
			var oldValue = data.PropertyInt32.Value;
			var value = oldValue + 421;
			EventHandler<DataPropertyChangingEventArgs<int>> changingHandler = (object sender, DataPropertyChangingEventArgs<int> e) =>
			{
				++changingInvokeCount;
				Assert.IsTrue(e.OldValue == oldValue);
				Assert.IsTrue(e.NewValue == value);
				Assert.IsTrue(e.InputValue == value);
			};

			EventHandler<DataPropertyChangedEventArgs<int>> changedHandler = (object sender, DataPropertyChangedEventArgs<int> e) =>
			{
				++changedInvokeCount;
				Assert.IsTrue(e.OldValue == oldValue);
				Assert.IsTrue(e.NewValue == value);
				Assert.IsTrue(e.InputValue == value);
			};

			data.PropertyInt32.DataPropertyChanging += changingHandler;
			data.PropertyInt32.DataPropertyChanged += changedHandler;

			data.PropertyInt32.Value = value;

			data.PropertyInt32.DataPropertyChanged -= changedHandler;
			data.PropertyInt32.DataPropertyChanging -= changingHandler;

			Assert.IsTrue(changingInvokeCount == 1);
			Assert.IsTrue(changedInvokeCount == 1);
			Assert.IsTrue(data.PropertyInt32.Value == value);
		}

		[TestMethod]
		public void DataPropertyCancelTest()
		{
			var data = new Data();

			var changingInvokeCount = 0;
			var changedInvokeCount = 0;
			var oldValue = data.PropertyInt32.Value;
			var value = oldValue + 421;
			EventHandler<DataPropertyChangingEventArgs<int>> changingHandler = (object sender, DataPropertyChangingEventArgs<int> e) =>
			{
				++changingInvokeCount;
				e.Cancel = true;
			};

			EventHandler<DataPropertyChangedEventArgs<int>> changedHandler = (object sender, DataPropertyChangedEventArgs<int> e) =>
			{
				++changedInvokeCount;
			};

			data.PropertyInt32.DataPropertyChanging += changingHandler;
			data.PropertyInt32.DataPropertyChanged += changedHandler;

			data.PropertyInt32.Value = value;

			data.PropertyInt32.DataPropertyChanged -= changedHandler;
			data.PropertyInt32.DataPropertyChanging -= changingHandler;

			Assert.IsTrue(changingInvokeCount == 1);
			Assert.IsTrue(changedInvokeCount == 0);
			Assert.IsTrue(data.PropertyInt32.Value == oldValue);
		}

		[TestMethod]
		public void DataPropertyArrangeTest()
		{
			var data = new Data();

			var changingInvokeCount = 0;
			var changedInvokeCount = 0;
			var oldValue = data.PropertyInt32.Value;
			var value = oldValue + 421;
			var arrangedValue = value + 421;
			EventHandler<DataPropertyChangingEventArgs<int>> changingHandler = (object sender, DataPropertyChangingEventArgs<int> e) =>
			{
				++changingInvokeCount;
				e.NewValue = arrangedValue;
			};

			EventHandler<DataPropertyChangedEventArgs<int>> changedHandler = (object sender, DataPropertyChangedEventArgs<int> e) =>
			{
				++changedInvokeCount;
				Assert.IsTrue(e.OldValue == oldValue);
				Assert.IsTrue(e.NewValue == arrangedValue);
				Assert.IsTrue(e.InputValue == value);
			};

			data.PropertyInt32.DataPropertyChanging += changingHandler;
			data.PropertyInt32.DataPropertyChanged += changedHandler;

			data.PropertyInt32.Value = value;

			data.PropertyInt32.DataPropertyChanged -= changedHandler;
			data.PropertyInt32.DataPropertyChanging -= changingHandler;

			Assert.IsTrue(changingInvokeCount == 1);
			Assert.IsTrue(changedInvokeCount == 1);
			Assert.IsTrue(data.PropertyInt32.Value == arrangedValue);
		}

		[TestMethod]
		public void DataPropertyPassTest()
		{
			var data = new Data();

			var changingInvokeCount = 0;
			var changedInvokeCount = 0;
			var oldValue = data.PropertyInt32.Value;
			var value = oldValue;
			EventHandler<DataPropertyChangingEventArgs<int>> changingHandler = (object sender, DataPropertyChangingEventArgs<int> e) =>
			{
				++changingInvokeCount;
			};

			EventHandler<DataPropertyChangedEventArgs<int>> changedHandler = (object sender, DataPropertyChangedEventArgs<int> e) =>
			{
				++changedInvokeCount;
			};

			data.PropertyInt32.DataPropertyChanging += changingHandler;
			data.PropertyInt32.DataPropertyChanged += changedHandler;

			data.PropertyInt32.Value = value;

			data.PropertyInt32.DataPropertyChanged -= changedHandler;
			data.PropertyInt32.DataPropertyChanging -= changingHandler;

			Assert.IsTrue(changingInvokeCount == 0);
			Assert.IsTrue(changedInvokeCount == 0);
		}

		[TestMethod]
		public void DataPropertySerializeTest()
		{
			var data = new Data();

			var today = DateTime.Today;
			var text = "<test>";
			var guid = Guid.NewGuid();
			var span = new TimeSpan(4, 2, 1);
			var offset = DateTimeOffset.Now;
			data.PropertyBoolean.Value = true;
			data.PropertyChar.Value = (char)TypeCode.Char;
			data.PropertySByte.Value = (sbyte)TypeCode.SByte;
			data.PropertyByte.Value = (byte)TypeCode.Byte;
			data.PropertyInt16.Value = (short)TypeCode.Int16;
			data.PropertyUInt16.Value = (ushort)TypeCode.UInt16;
			data.PropertyInt32.Value = (int)TypeCode.Int32;
			data.PropertyUInt32.Value = (uint)TypeCode.UInt32;
			data.PropertyInt64.Value = (long)TypeCode.Int64;
			data.PropertyUInt64.Value = (ulong)TypeCode.UInt64;
			data.PropertySingle.Value = ((float)TypeCode.Single) + 0.01f;
			data.PropertyDouble.Value = ((double)TypeCode.Double) + 0.01;
			data.PropertyDecimal.Value = ((decimal)TypeCode.Decimal) + 0.01m;
			data.PropertyDateTime.Value = today;
			data.PropertyString.Value = text;
			data.PropertyGuid.Value = guid;
			data.PropertyTimeSpan.Value = span;
			data.PropertyDateTimeOffset.Value = offset;
			data.PropertyEnum.Value = Data.Enum.Two;
			data.PropertyFlag.Value = Data.Flag.FlagOne | Data.Flag.FlagTwo;

			var serializer = new DataPropertySerializer();
			using var stream = new MemoryStream();

			serializer.Serialize(stream, data);
			var xml = Encoding.UTF8.GetString(stream.GetBuffer());
			stream.Position = 0;

			var original = data;
			data = new Data();
			serializer.Deserialize(stream, data);

			Assert.IsTrue(data.PropertyBoolean.Value == true);
			Assert.IsTrue(data.PropertyChar.Value == (char)TypeCode.Char);
			Assert.IsTrue(data.PropertySByte.Value == (sbyte)TypeCode.SByte);
			Assert.IsTrue(data.PropertyByte.Value == (byte)TypeCode.Byte);
			Assert.IsTrue(data.PropertyInt16.Value == (short)TypeCode.Int16);
			Assert.IsTrue(data.PropertyUInt16.Value == (ushort)TypeCode.UInt16);
			Assert.IsTrue(data.PropertyInt32.Value == (int)TypeCode.Int32);
			Assert.IsTrue(data.PropertyUInt32.Value == (uint)TypeCode.UInt32);
			Assert.IsTrue(data.PropertyInt64.Value == (long)TypeCode.Int64);
			Assert.IsTrue(data.PropertyUInt64.Value == (ulong)TypeCode.UInt64);
			Assert.IsTrue(data.PropertySingle.Value == ((float)TypeCode.Single) + 0.01f);
			Assert.IsTrue(data.PropertyDouble.Value == ((double)TypeCode.Double) + 0.01);
			Assert.IsTrue(data.PropertyDecimal.Value == ((decimal)TypeCode.Decimal) + 0.01m);
			Assert.IsTrue(data.PropertyDateTime.Value == today);
			Assert.IsTrue(data.PropertyString.Value == text);
			Assert.IsTrue(data.PropertyGuid.Value == guid);
			Assert.IsTrue(data.PropertyTimeSpan.Value == span);
			Assert.IsTrue(data.PropertyDateTimeOffset.Value == offset);
			Assert.IsTrue(data.PropertyEnum.Value == Data.Enum.Two);
			Assert.IsTrue(data.PropertyFlag.Value == (Data.Flag.FlagOne | Data.Flag.FlagTwo));
		}

		[TestMethod]
		public void DataNullablePropertySerializeTest()
		{
			var data = new DataWithNullable();

			var today = DateTime.Today;
			var text = "<test>";
			var guid = Guid.NewGuid();
			var span = new TimeSpan(4, 2, 1);
			var offset = DateTimeOffset.Now;
			data.PropertyBoolean.Value = true;
			data.PropertyChar.Value = (char)TypeCode.Char;
			data.PropertySByte.Value = (sbyte)TypeCode.SByte;
			data.PropertyByte.Value = (byte)TypeCode.Byte;
			data.PropertyInt16.Value = (short)TypeCode.Int16;
			data.PropertyUInt16.Value = (ushort)TypeCode.UInt16;
			data.PropertyInt32.Value = (int)TypeCode.Int32;
			data.PropertyUInt32.Value = (uint)TypeCode.UInt32;
			data.PropertyInt64.Value = (long)TypeCode.Int64;
			data.PropertyUInt64.Value = (ulong)TypeCode.UInt64;
			data.PropertySingle.Value = ((float)TypeCode.Single) + 0.01f;
			data.PropertyDouble.Value = ((double)TypeCode.Double) + 0.01;
			data.PropertyDecimal.Value = ((decimal)TypeCode.Decimal) + 0.01m;
			data.PropertyDateTime.Value = today;
			data.PropertyString.Value = text;
			data.PropertyGuid.Value = guid;
			data.PropertyTimeSpan.Value = span;
			data.PropertyDateTimeOffset.Value = offset;
			data.PropertyEnum.Value = DataWithNullable.Enum.Two;
			data.PropertyFlag.Value = DataWithNullable.Flag.FlagOne | DataWithNullable.Flag.FlagTwo;

			var serializer = new DataPropertySerializer();
			using var stream = new MemoryStream();

			serializer.Serialize(stream, data);
			var xml = Encoding.UTF8.GetString(stream.GetBuffer());
			stream.Position = 0;

			var original = data;
			data = new DataWithNullable();
			serializer.Deserialize(stream, data);

			Assert.IsTrue(data.PropertyBoolean.Value == true);
			Assert.IsTrue(data.PropertyChar.Value == (char)TypeCode.Char);
			Assert.IsTrue(data.PropertySByte.Value == (sbyte)TypeCode.SByte);
			Assert.IsTrue(data.PropertyByte.Value == (byte)TypeCode.Byte);
			Assert.IsTrue(data.PropertyInt16.Value == (short)TypeCode.Int16);
			Assert.IsTrue(data.PropertyUInt16.Value == (ushort)TypeCode.UInt16);
			Assert.IsTrue(data.PropertyInt32.Value == (int)TypeCode.Int32);
			Assert.IsTrue(data.PropertyUInt32.Value == (uint)TypeCode.UInt32);
			Assert.IsTrue(data.PropertyInt64.Value == (long)TypeCode.Int64);
			Assert.IsTrue(data.PropertyUInt64.Value == (ulong)TypeCode.UInt64);
			Assert.IsTrue(data.PropertySingle.Value == ((float)TypeCode.Single) + 0.01f);
			Assert.IsTrue(data.PropertyDouble.Value == ((double)TypeCode.Double) + 0.01);
			Assert.IsTrue(data.PropertyDecimal.Value == ((decimal)TypeCode.Decimal) + 0.01m);
			Assert.IsTrue(data.PropertyDateTime.Value == today);
			Assert.IsTrue(data.PropertyString.Value == text);
			Assert.IsTrue(data.PropertyGuid.Value == guid);
			Assert.IsTrue(data.PropertyTimeSpan.Value == span);
			Assert.IsTrue(data.PropertyDateTimeOffset.Value == offset);
			Assert.IsTrue(data.PropertyEnum.Value == DataWithNullable.Enum.Two);
			Assert.IsTrue(data.PropertyFlag.Value == (DataWithNullable.Flag.FlagOne | DataWithNullable.Flag.FlagTwo));
		}

		[TestMethod]
		public void DataPropertyCompatibleSerializeTest()
		{
			var data = new Data();

			var today = DateTime.Today;
			var text = "<test>";
			var guid = Guid.NewGuid();
			var span = new TimeSpan(4, 2, 1);
			var offset = DateTimeOffset.Now;
			data.PropertyBoolean.Value = true;
			data.PropertyChar.Value = (char)TypeCode.Char;
			data.PropertySByte.Value = (sbyte)TypeCode.SByte;
			data.PropertyByte.Value = (byte)TypeCode.Byte;
			data.PropertyInt16.Value = (short)TypeCode.Int16;
			data.PropertyUInt16.Value = (ushort)TypeCode.UInt16;
			data.PropertyInt32.Value = (int)TypeCode.Int32;
			data.PropertyUInt32.Value = (uint)TypeCode.UInt32;
			data.PropertyInt64.Value = (long)TypeCode.Int64;
			data.PropertyUInt64.Value = (ulong)TypeCode.UInt64;
			data.PropertySingle.Value = ((float)TypeCode.Single) + 0.01f;
			data.PropertyDouble.Value = ((double)TypeCode.Double) + 0.01;
			data.PropertyDecimal.Value = ((decimal)TypeCode.Decimal) + 0.01m;
			data.PropertyDateTime.Value = today;
			data.PropertyString.Value = text;
			data.PropertyGuid.Value = guid;
			data.PropertyTimeSpan.Value = span;
			data.PropertyDateTimeOffset.Value = offset;
			data.PropertyEnum.Value = Data.Enum.Two;
			data.PropertyFlag.Value = Data.Flag.FlagOne | Data.Flag.FlagTwo;

			var serializer = new DataPropertySerializer();
			using var stream = new MemoryStream();

			serializer.Serialize(stream, data);
			var xml = Encoding.UTF8.GetString(stream.GetBuffer());
			stream.Position = 0;

			var nullable = new DataWithNullable();
			serializer.Deserialize(stream, nullable);

			Assert.IsTrue(nullable.PropertyBoolean.Value == true);
			Assert.IsTrue(nullable.PropertyChar.Value == (char)TypeCode.Char);
			Assert.IsTrue(nullable.PropertySByte.Value == (sbyte)TypeCode.SByte);
			Assert.IsTrue(nullable.PropertyByte.Value == (byte)TypeCode.Byte);
			Assert.IsTrue(nullable.PropertyInt16.Value == (short)TypeCode.Int16);
			Assert.IsTrue(nullable.PropertyUInt16.Value == (ushort)TypeCode.UInt16);
			Assert.IsTrue(nullable.PropertyInt32.Value == (int)TypeCode.Int32);
			Assert.IsTrue(nullable.PropertyUInt32.Value == (uint)TypeCode.UInt32);
			Assert.IsTrue(nullable.PropertyInt64.Value == (long)TypeCode.Int64);
			Assert.IsTrue(nullable.PropertyUInt64.Value == (ulong)TypeCode.UInt64);
			Assert.IsTrue(nullable.PropertySingle.Value == ((float)TypeCode.Single) + 0.01f);
			Assert.IsTrue(nullable.PropertyDouble.Value == ((double)TypeCode.Double) + 0.01);
			Assert.IsTrue(nullable.PropertyDecimal.Value == ((decimal)TypeCode.Decimal) + 0.01m);
			Assert.IsTrue(nullable.PropertyDateTime.Value == today);
			Assert.IsTrue(nullable.PropertyString.Value == text);
			Assert.IsTrue(nullable.PropertyGuid.Value == guid);
			Assert.IsTrue(nullable.PropertyTimeSpan.Value == span);
			Assert.IsTrue(nullable.PropertyDateTimeOffset.Value == offset);
			Assert.IsTrue(nullable.PropertyEnum.Value == DataWithNullable.Enum.Two);
			Assert.IsTrue(nullable.PropertyFlag.Value == (DataWithNullable.Flag.FlagOne | DataWithNullable.Flag.FlagTwo));
		}

		[TestMethod]
		public void DataPropertyNullValueSerializeTest()
		{
			var nullable = new DataWithNullable();

			var serializer = new DataPropertySerializer();
			using var stream = new MemoryStream();

			serializer.Serialize(stream, nullable);
			var xml = Encoding.UTF8.GetString(stream.GetBuffer());
			stream.Position = 0;

			var data = new Data();

			var today = DateTime.Today;
			var text = "<test>";
			var guid = Guid.NewGuid();
			var span = new TimeSpan(4, 2, 1);
			var offset = DateTimeOffset.Now;
			data.PropertyBoolean.Value = true;
			data.PropertyChar.Value = (char)TypeCode.Char;
			data.PropertySByte.Value = (sbyte)TypeCode.SByte;
			data.PropertyByte.Value = (byte)TypeCode.Byte;
			data.PropertyInt16.Value = (short)TypeCode.Int16;
			data.PropertyUInt16.Value = (ushort)TypeCode.UInt16;
			data.PropertyInt32.Value = (int)TypeCode.Int32;
			data.PropertyUInt32.Value = (uint)TypeCode.UInt32;
			data.PropertyInt64.Value = (long)TypeCode.Int64;
			data.PropertyUInt64.Value = (ulong)TypeCode.UInt64;
			data.PropertySingle.Value = ((float)TypeCode.Single) + 0.01f;
			data.PropertyDouble.Value = ((double)TypeCode.Double) + 0.01;
			data.PropertyDecimal.Value = ((decimal)TypeCode.Decimal) + 0.01m;
			data.PropertyDateTime.Value = today;
			data.PropertyString.Value = text;
			data.PropertyGuid.Value = guid;
			data.PropertyTimeSpan.Value = span;
			data.PropertyDateTimeOffset.Value = offset;
			data.PropertyEnum.Value = Data.Enum.Two;
			data.PropertyFlag.Value = Data.Flag.FlagOne | Data.Flag.FlagTwo;

			serializer.Deserialize(stream, data);

			Assert.IsTrue(data.PropertyBoolean.Value == default(bool));
			Assert.IsTrue(data.PropertyChar.Value == default(char));
			Assert.IsTrue(data.PropertySByte.Value == default(sbyte));
			Assert.IsTrue(data.PropertyByte.Value == default(byte));
			Assert.IsTrue(data.PropertyInt16.Value == default(short));
			Assert.IsTrue(data.PropertyUInt16.Value == default(ushort));
			Assert.IsTrue(data.PropertyInt32.Value == default(int));
			Assert.IsTrue(data.PropertyUInt32.Value == default(uint));
			Assert.IsTrue(data.PropertyInt64.Value == default(long));
			Assert.IsTrue(data.PropertyUInt64.Value == default(ulong));
			Assert.IsTrue(data.PropertySingle.Value == default(float));
			Assert.IsTrue(data.PropertyDouble.Value == default(double));
			Assert.IsTrue(data.PropertyDecimal.Value == default(decimal));
			Assert.IsTrue(data.PropertyDateTime.Value == default(DateTime));
			Assert.IsTrue(data.PropertyString.Value == null);
			Assert.IsTrue(data.PropertyGuid.Value == default(Guid));
			Assert.IsTrue(data.PropertyTimeSpan.Value == default(TimeSpan));
			Assert.IsTrue(data.PropertyDateTimeOffset.Value == default(DateTimeOffset));
			Assert.IsTrue(data.PropertyEnum.Value == default(Data.Enum));
			Assert.IsTrue(data.PropertyFlag.Value == default(Data.Flag));
		}

		[TestMethod]
		public void DataPropertyTreeSerializeTest()
		{
			var root = new BinaryTreeNode();
			root.Text.Value = "root";
			root.Left.Value = new BinaryTreeNode();
			root.Left.Value.Text.Value = "left";
			root.Right.Value = new BinaryTreeNode();
			root.Right.Value.Text.Value = "right";

			var serializer = new DataPropertySerializer();
			using var stream = new MemoryStream();

			serializer.Serialize(stream, root);
			var xml = Encoding.UTF8.GetString(stream.GetBuffer());
			stream.Position = 0;

			var original = root;
			root = new BinaryTreeNode();
			serializer.Deserialize(stream, root);

			Assert.IsTrue(root.Text.Value == "root");
			Assert.IsNotNull(root.Left.Value);
			Assert.IsTrue(root.Left.Value.Text.Value == "left");
			Assert.IsNotNull(root.Right.Value);
			Assert.IsTrue(root.Right.Value.Text.Value == "right");
		}

		[TestMethod]
		public void DataPropertyTreeSharedNodeSerializeTest()
		{
			var root = new BinaryTreeNode();
			root.Text.Value = "root";
			root.Left.Value = new BinaryTreeNode();
			root.Left.Value.Text.Value = "left";
			root.Right.Value = root.Left.Value;

			var serializer = new DataPropertySerializer();
			using var stream = new MemoryStream();

			serializer.Serialize(stream, root);
			var xml = Encoding.UTF8.GetString(stream.GetBuffer());
			stream.Position = 0;

			var original = root;
			root = new BinaryTreeNode();
			serializer.Deserialize(stream, root);

			Assert.IsTrue(root.Text.Value == "root");
			Assert.IsNotNull(root.Left.Value);
			Assert.IsTrue(root.Left.Value.Text.Value == "left");
			Assert.IsNotNull(root.Right.Value);
			Assert.IsTrue(ReferenceEquals(root.Right.Value, root.Left.Value));
		}

		[TestMethod]
		public void DataPropertyGraphSerializeTest()
		{
			var graph = new Graph();
			graph.Text.Value = "graph";
			var node0 = graph.AddNode("node0");
			var node1 = graph.AddNode("node1");
			var node2 = graph.AddNode("node2");
			graph.Link(node0, node1, "edge01");
			graph.Link(node1, node2, "edge12");
			graph.Link(node2, node0, "edge20");

			var serializer = new DataPropertySerializer();
			using var stream = new MemoryStream();

			serializer.Serialize(stream, graph);
			var xml = Encoding.UTF8.GetString(stream.GetBuffer());
			stream.Position = 0;

			var original = graph;
			graph = new Graph();
			serializer.Deserialize(stream, graph);

			Assert.IsTrue(graph.Text.Value == "graph");
			Assert.IsTrue(graph.Nodes.Count == 3);
			Assert.IsTrue(graph.Nodes[0].Value.Text.Value == "node0");
			Assert.IsTrue(graph.Nodes[1].Value.Text.Value == "node1");
			Assert.IsTrue(graph.Nodes[2].Value.Text.Value == "node2");
			Assert.IsTrue(graph.Nodes[0].Value.LinkOut.Count == 1);
			Assert.IsTrue(graph.Nodes[0].Value.LinkIn.Count == 1);
			Assert.IsTrue(graph.Nodes[1].Value.LinkOut.Count == 1);
			Assert.IsTrue(graph.Nodes[1].Value.LinkIn.Count == 1);
			Assert.IsTrue(graph.Nodes[2].Value.LinkOut.Count == 1);
			Assert.IsTrue(graph.Nodes[2].Value.LinkIn.Count == 1);
			Assert.IsTrue(graph.Nodes[0].Value.LinkOut[0].Value.Text.Value == "edge01");
			Assert.IsTrue(graph.Nodes[0].Value.LinkIn[0].Value.Text.Value == "edge20");
			Assert.IsTrue(graph.Nodes[1].Value.LinkOut[0].Value.Text.Value == "edge12");
			Assert.IsTrue(graph.Nodes[1].Value.LinkIn[0].Value.Text.Value == "edge01");
			Assert.IsTrue(graph.Nodes[2].Value.LinkOut[0].Value.Text.Value == "edge20");
			Assert.IsTrue(graph.Nodes[2].Value.LinkIn[0].Value.Text.Value == "edge12");
			Assert.IsTrue(ReferenceEquals(graph.Nodes[0].Value.LinkOut[0].Value, graph.Nodes[1].Value.LinkIn[0].Value));
			Assert.IsTrue(ReferenceEquals(graph.Nodes[1].Value.LinkOut[0].Value, graph.Nodes[2].Value.LinkIn[0].Value));
			Assert.IsTrue(ReferenceEquals(graph.Nodes[2].Value.LinkOut[0].Value, graph.Nodes[0].Value.LinkIn[0].Value));
			var edge01 = graph.Nodes[0].Value.LinkOut[0].Value;
			var edge12 = graph.Nodes[1].Value.LinkOut[0].Value;
			var edge20 = graph.Nodes[2].Value.LinkOut[0].Value;
			Assert.IsTrue(ReferenceEquals(edge01.LinkFrom.Value, graph.Nodes[0].Value));
			Assert.IsTrue(ReferenceEquals(edge01.LinkTo.Value, graph.Nodes[1].Value));
			Assert.IsTrue(ReferenceEquals(edge12.LinkFrom.Value, graph.Nodes[1].Value));
			Assert.IsTrue(ReferenceEquals(edge12.LinkTo.Value, graph.Nodes[2].Value));
			Assert.IsTrue(ReferenceEquals(edge20.LinkFrom.Value, graph.Nodes[2].Value));
			Assert.IsTrue(ReferenceEquals(edge20.LinkTo.Value, graph.Nodes[0].Value));
		}

		[TestMethod]
		public void DataPropertyCollectionSerializeTest()
		{
			var data = new DataCollector();
			data.PropertyIntCollection.Add(4);
			data.PropertyIntCollection.Add(2);
			data.PropertyIntCollection.Add(1);
			data.PropertyDataCollection.Add(new Data());
			data.PropertyDataCollection.Add(new Data());
			data.PropertyDataCollection[0].Value.PropertyInt32.Value = 5;
			data.PropertyDataCollection[1].Value.PropertyInt32.Value = 3;

			var serializer = new DataPropertySerializer();
			using var stream = new MemoryStream();

			serializer.Serialize(stream, data);
			var xml = Encoding.UTF8.GetString(stream.GetBuffer());
			stream.Position = 0;

			var original = data;
			data = new DataCollector();
			serializer.Deserialize(stream, data);

			Assert.IsTrue(data.PropertyIntCollection.Count == 3);
			Assert.IsTrue(data.PropertyIntCollection[0].Value == 4);
			Assert.IsTrue(data.PropertyIntCollection[1].Value == 2);
			Assert.IsTrue(data.PropertyIntCollection[2].Value == 1);
			Assert.IsTrue(data.PropertyDataCollection.Count == 2);
			Assert.IsTrue(data.PropertyDataCollection[0].Value.PropertyInt32.Value == 5);
			Assert.IsTrue(data.PropertyDataCollection[1].Value.PropertyInt32.Value == 3);
		}

		[TestMethod]
		public void DataPropertyCollectionOperatorTest()
		{
			var data = new DataCollector();

			using (var collectionOperator = data.PropertyIntCollection.GetCollectionOperator())
			{
				collectionOperator.Add(4);
				collectionOperator.Add(2);
				collectionOperator.Add(1);
				Assert.IsTrue(collectionOperator.Count == 3);
				Assert.IsTrue(collectionOperator[0] == 4);
				Assert.IsTrue(collectionOperator[1] == 2);
				Assert.IsTrue(collectionOperator[2] == 1);
				Assert.IsTrue(data.PropertyIntCollection.Count == 0);
			}

			Assert.IsTrue(data.PropertyIntCollection.Count == 3);
			Assert.IsTrue(data.PropertyIntCollection[0].Value == 4);
			Assert.IsTrue(data.PropertyIntCollection[1].Value == 2);
			Assert.IsTrue(data.PropertyIntCollection[2].Value == 1);
		}

		[TestMethod]
		public void DataPropertyDictionarySerializeTest()
		{
			var data = new DataMapper();
			data.PropertyStringIntMap.Add("y", 0);
			data.PropertyStringIntMap.Add("u", 4);
			data.PropertyStringIntMap.Add("k", 2);
			data.PropertyStringIntMap.Add("i", 1);
			var root = new Data();
			root.PropertyString.Value = "root";
			var body = new Data();
			body.PropertyString.Value = "body";
			var arml = new Data();
			arml.PropertyString.Value = "arml";
			var armr = new Data();
			armr.PropertyString.Value = "armr";
			data.PropertyPedigree.Add(root, null);
			data.PropertyPedigree.Add(body, root);
			data.PropertyPedigree.Add(arml, body);
			data.PropertyPedigree.Add(armr, body);

			var serializer = new DataPropertySerializer();
			using var stream = new MemoryStream();

			serializer.Serialize(stream, data);
			var xml = Encoding.UTF8.GetString(stream.GetBuffer());
			stream.Position = 0;

			var original = data;
			data = new DataMapper();
			serializer.Deserialize(stream, data);

			Assert.IsTrue(data.PropertyStringIntMap.Count == 4);
			Assert.IsTrue(data.PropertyStringIntMap["y"].Value == 0);
			Assert.IsTrue(data.PropertyStringIntMap["u"].Value == 4);
			Assert.IsTrue(data.PropertyStringIntMap["k"].Value == 2);
			Assert.IsTrue(data.PropertyStringIntMap["i"].Value == 1);
			Assert.IsTrue(data.PropertyPedigree.Count == 4);
			root = data.PropertyPedigree.Keys.FirstOrDefault(_ => _.PropertyString.Value == "root");
			body = data.PropertyPedigree.Keys.FirstOrDefault(_ => _.PropertyString.Value == "body");
			arml = data.PropertyPedigree.Keys.FirstOrDefault(_ => _.PropertyString.Value == "arml");
			armr = data.PropertyPedigree.Keys.FirstOrDefault(_ => _.PropertyString.Value == "armr");
			Assert.IsTrue(data.PropertyPedigree[root].Value == null);
			Assert.IsTrue(data.PropertyPedigree[body].Value == root);
			Assert.IsTrue(data.PropertyPedigree[arml].Value == body);
			Assert.IsTrue(data.PropertyPedigree[armr].Value == body);
		}

		[TestMethod]
		public void DataPropertyPedigreeTest()
		{
			var data = new DataPedigree();
			var root = new SimpleData();
			root.PropertyString.Value = "root";
			var body = new SimpleData();
			body.PropertyString.Value = "body";
			var arml = new SimpleData();
			arml.PropertyString.Value = "arml";
			var armr = new SimpleData();
			armr.PropertyString.Value = "armr";
			data.PropertyPedigree.Add(arml);
			data.PropertyPedigree.Add(armr);
			data.PropertyPedigree.Add(body);
			data.PropertyPedigree.Add(root);

			Assert.IsTrue(data.PropertyPedigree.Count == 4);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children.Count == 4);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children[0].Core.PropertyString.Value == "arml");
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children[1].Core.PropertyString.Value == "armr");
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children[2].Core.PropertyString.Value == "body");
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children[3].Core.PropertyString.Value == "root");

			data.PropertyPedigree.Parent(arml, body);
			data.PropertyPedigree.Parent(armr, body);

			Assert.IsTrue(data.PropertyPedigree.Count == 4);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children.Count == 2);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children[0].Core.PropertyString.Value == "body");
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children[1].Core.PropertyString.Value == "root");
			var bodyNode = data.PropertyPedigree.Tree.Root.Children[0];
			Assert.IsTrue(bodyNode.Children.Count == 2);
			Assert.IsTrue(bodyNode.Children[0].Core.PropertyString.Value == "arml");
			Assert.IsTrue(bodyNode.Children[1].Core.PropertyString.Value == "armr");

			data.PropertyPedigree.Parent(body, root);

			Assert.IsTrue(data.PropertyPedigree.Count == 4);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children.Count == 1);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children[0].Core.PropertyString.Value == "root");
			var rootNode = data.PropertyPedigree.Tree.Root.Children[0];
			Assert.IsTrue(rootNode.Children.Count == 1);
			Assert.IsTrue(rootNode.Children[0].Core.PropertyString.Value == "body");
			bodyNode = rootNode.Children[0];
			Assert.IsTrue(bodyNode.Children.Count == 2);
			Assert.IsTrue(bodyNode.Children[0].Core.PropertyString.Value == "arml");
			Assert.IsTrue(bodyNode.Children[1].Core.PropertyString.Value == "armr");

			data.PropertyPedigree.Unparent(arml);
			data.PropertyPedigree.Unparent(armr);

			Assert.IsTrue(data.PropertyPedigree.Count == 4);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children.Count == 1);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children[0].Core.PropertyString.Value == "root");
			rootNode = data.PropertyPedigree.Tree.Root.Children[0];
			Assert.IsTrue(rootNode.Children.Count == 3);
			Assert.IsTrue(rootNode.Children[0].Core.PropertyString.Value == "body");
			Assert.IsTrue(rootNode.Children[1].Core.PropertyString.Value == "armr");
			Assert.IsTrue(rootNode.Children[2].Core.PropertyString.Value == "arml");

			data.PropertyPedigree.Unparent(body);
			data.PropertyPedigree.Unparent(arml);
			data.PropertyPedigree.Unparent(armr);

			Assert.IsTrue(data.PropertyPedigree.Count == 4);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children.Count == 4);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children[0].Core.PropertyString.Value == "root");
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children[1].Core.PropertyString.Value == "armr");
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children[2].Core.PropertyString.Value == "arml");
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children[3].Core.PropertyString.Value == "body");

			data.PropertyPedigree.Parent(body, root);

			Assert.IsTrue(data.PropertyPedigree.Count == 4);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children.Count == 3);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children[0].Core.PropertyString.Value == "root");
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children[1].Core.PropertyString.Value == "armr");
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children[2].Core.PropertyString.Value == "arml");
			rootNode = data.PropertyPedigree.Tree.Root.Children[0];
			Assert.IsTrue(rootNode.Children.Count == 1);
			Assert.IsTrue(rootNode.Children[0].Core.PropertyString.Value == "body");

			data.PropertyPedigree.Parent(arml, body);
			data.PropertyPedigree.Parent(armr, body);

			Assert.IsTrue(data.PropertyPedigree.Count == 4);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children.Count == 1);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children[0].Core.PropertyString.Value == "root");
			rootNode = data.PropertyPedigree.Tree.Root.Children[0];
			Assert.IsTrue(rootNode.Children.Count == 1);
			Assert.IsTrue(rootNode.Children[0].Core.PropertyString.Value == "body");
			bodyNode = rootNode.Children[0];
			Assert.IsTrue(bodyNode.Children.Count == 2);
			Assert.IsTrue(bodyNode.Children[0].Core.PropertyString.Value == "arml");
			Assert.IsTrue(bodyNode.Children[1].Core.PropertyString.Value == "armr");

			data.PropertyPedigree.Remove(body);

			Assert.IsTrue(data.PropertyPedigree.Count == 3);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children.Count == 1);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children[0].Core.PropertyString.Value == "root");
			rootNode = data.PropertyPedigree.Tree.Root.Children[0];
			Assert.IsTrue(rootNode.Children.Count == 2);
			Assert.IsTrue(rootNode.Children[0].Core.PropertyString.Value == "arml");
			Assert.IsTrue(rootNode.Children[1].Core.PropertyString.Value == "armr");

			data.PropertyPedigree.Remove(arml);

			Assert.IsTrue(data.PropertyPedigree.Count == 2);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children.Count == 1);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children[0].Core.PropertyString.Value == "root");
			rootNode = data.PropertyPedigree.Tree.Root.Children[0];
			Assert.IsTrue(rootNode.Children.Count == 1);
			Assert.IsTrue(rootNode.Children[0].Core.PropertyString.Value == "armr");

			data.PropertyPedigree.Remove(root);

			Assert.IsTrue(data.PropertyPedigree.Count == 1);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children.Count == 1);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children[0].Core.PropertyString.Value == "armr");

			data.PropertyPedigree.Add(root);
			data.PropertyPedigree.Add(body, root);
			data.PropertyPedigree.Add(arml, body);
			data.PropertyPedigree.Parent(armr, body);

			Assert.IsTrue(data.PropertyPedigree.Count == 4);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children.Count == 1);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children[0].Core.PropertyString.Value == "root");
			rootNode = data.PropertyPedigree.Tree.Root.Children[0];
			Assert.IsTrue(rootNode.Children.Count == 1);
			Assert.IsTrue(rootNode.Children[0].Core.PropertyString.Value == "body");
			bodyNode = rootNode.Children[0];
			Assert.IsTrue(bodyNode.Children.Count == 2);
			Assert.IsTrue(bodyNode.Children[0].Core.PropertyString.Value == "arml");
			Assert.IsTrue(bodyNode.Children[1].Core.PropertyString.Value == "armr");

			data.PropertyPedigree.Remove(root);

			Assert.IsTrue(data.PropertyPedigree.Count == 3);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children.Count == 1);
			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children[0].Core.PropertyString.Value == "body");
			bodyNode = data.PropertyPedigree.Tree.Root.Children[0];
			Assert.IsTrue(bodyNode.Children.Count == 2);
			Assert.IsTrue(bodyNode.Children[0].Core.PropertyString.Value == "arml");
			Assert.IsTrue(bodyNode.Children[1].Core.PropertyString.Value == "armr");
		}

		[TestMethod]
		public void DataPropertyPedigreeSerializeTest()
		{
			var data = new DataPedigree();
			var rootOriginal = new SimpleData();
			rootOriginal.PropertyString.Value = "root";
			var bodyOriginal = new SimpleData();
			bodyOriginal.PropertyString.Value = "body";
			var armlOriginal = new SimpleData();
			armlOriginal.PropertyString.Value = "arml";
			var armrOriginal = new SimpleData();
			armrOriginal.PropertyString.Value = "armr";
			data.PropertyPedigree.Add(rootOriginal, null);
			data.PropertyPedigree.Add(bodyOriginal, rootOriginal);
			data.PropertyPedigree.Add(armlOriginal, bodyOriginal);
			data.PropertyPedigree.Add(armrOriginal, bodyOriginal);

			var serializer = new DataPropertySerializer();
			using var stream = new MemoryStream();

			serializer.Serialize(stream, data);
			var xml = Encoding.UTF8.GetString(stream.GetBuffer());
			stream.Position = 0;

			var original = data;
			data = new DataPedigree();
			serializer.Deserialize(stream, data);

			Assert.IsTrue(data.PropertyPedigree.Count == 4);

			Assert.IsTrue(data.PropertyPedigree.Tree.Root.Children.Count == 1);
			var root = data.PropertyPedigree.Tree.Root.Children[0];
			Assert.IsTrue(root.Core.PropertyString.Value == "root");

			Assert.IsTrue(root.Children.Count == 1);
			var body = root.Children[0];
			Assert.IsTrue(body.Core.PropertyString.Value == "body");

			Assert.IsTrue(body.Children.Count == 2);
			var arml = body.Children[0];
			var armr = body.Children[1];
			Assert.IsTrue(arml.Core.PropertyString.Value == "arml");
			Assert.IsTrue(armr.Core.PropertyString.Value == "armr");
		}

		#region private members
		#region class SimpleData
		[System.Diagnostics.DebuggerDisplay("{PropertyString.Value,nq}")]
		public class SimpleData
		{
			public DataProperty<string> PropertyString { get; } = new DataProperty<string>();
		}
		#endregion // class SimpleData

		#region class Data
		public class Data
		{
			#region enum Enum
			public enum Enum
			{
				None,
				One,
				Two,
				Count,
			}
			#endregion // enum Enum

			#region enum Flag
			[Flags]
			public enum Flag
			{
				None = 0x00000000,
				FlagOne = 0x00000001 << 0,
				FlagTwo = 0x00000001 << 1,
				Alias = FlagOne,
			}
			#endregion // enum Flag

			public DataProperty<bool> PropertyBoolean { get; } = new DataProperty<bool>();

			public DataProperty<char> PropertyChar { get; } = new DataProperty<char>();

			public DataProperty<sbyte> PropertySByte { get; } = new DataProperty<sbyte>();

			public DataProperty<byte> PropertyByte { get; } = new DataProperty<byte>();

			public DataProperty<short> PropertyInt16 { get; } = new DataProperty<short>();

			public DataProperty<ushort> PropertyUInt16 { get; } = new DataProperty<ushort>();

			public DataProperty<int> PropertyInt32 { get; } = new DataProperty<int>();

			public DataProperty<uint> PropertyUInt32 { get; } = new DataProperty<uint>();

			public DataProperty<long> PropertyInt64 { get; } = new DataProperty<long>();

			public DataProperty<ulong> PropertyUInt64 { get; } = new DataProperty<ulong>();

			public DataProperty<float> PropertySingle { get; } = new DataProperty<float>();

			public DataProperty<double> PropertyDouble { get; } = new DataProperty<double>();

			public DataProperty<decimal> PropertyDecimal { get; } = new DataProperty<decimal>();

			public DataProperty<DateTime> PropertyDateTime { get; } = new DataProperty<DateTime>();

			public DataProperty<string> PropertyString { get; } = new DataProperty<string>();

			public DataProperty<Guid> PropertyGuid { get; } = new DataProperty<Guid>();

			public DataProperty<TimeSpan> PropertyTimeSpan { get; } = new DataProperty<TimeSpan>();

			public DataProperty<DateTimeOffset> PropertyDateTimeOffset { get; } = new DataProperty<DateTimeOffset>();

			public DataProperty<Enum> PropertyEnum { get; } = new DataProperty<Enum>();

			public DataProperty<Flag> PropertyFlag { get; } = new DataProperty<Flag>();

		}
		#endregion // class Data

		#region class DataWithNullable
		public class DataWithNullable
		{
			#region enum Enum
			public enum Enum
			{
				None,
				One,
				Two,
				Count,
			}
			#endregion // enum Enum

			#region enum Flag
			[Flags]
			public enum Flag
			{
				None = 0x00000000,
				FlagOne = 0x00000001 << 0,
				FlagTwo = 0x00000001 << 1,
				Alias = FlagOne,
			}
			#endregion // enum Flag

			public DataProperty<bool?> PropertyBoolean { get; } = new DataProperty<bool?>();

			public DataProperty<char?> PropertyChar { get; } = new DataProperty<char?>();

			public DataProperty<sbyte?> PropertySByte { get; } = new DataProperty<sbyte?>();

			public DataProperty<byte?> PropertyByte { get; } = new DataProperty<byte?>();

			public DataProperty<short?> PropertyInt16 { get; } = new DataProperty<short?>();

			public DataProperty<ushort?> PropertyUInt16 { get; } = new DataProperty<ushort?>();

			public DataProperty<int?> PropertyInt32 { get; } = new DataProperty<int?>();

			public DataProperty<uint?> PropertyUInt32 { get; } = new DataProperty<uint?>();

			public DataProperty<long?> PropertyInt64 { get; } = new DataProperty<long?>();

			public DataProperty<ulong?> PropertyUInt64 { get; } = new DataProperty<ulong?>();

			public DataProperty<float?> PropertySingle { get; } = new DataProperty<float?>();

			public DataProperty<double?> PropertyDouble { get; } = new DataProperty<double?>();

			public DataProperty<decimal?> PropertyDecimal { get; } = new DataProperty<decimal?>();

			public DataProperty<DateTime?> PropertyDateTime { get; } = new DataProperty<DateTime?>();

			public DataProperty<string> PropertyString { get; } = new DataProperty<string>();

			public DataProperty<Guid?> PropertyGuid { get; } = new DataProperty<Guid?>();

			public DataProperty<TimeSpan?> PropertyTimeSpan { get; } = new DataProperty<TimeSpan?>();

			public DataProperty<DateTimeOffset?> PropertyDateTimeOffset { get; } = new DataProperty<DateTimeOffset?>();

			public DataProperty<Enum?> PropertyEnum { get; } = new DataProperty<Enum?>();

			public DataProperty<Flag?> PropertyFlag { get; } = new DataProperty<Flag?>();

		}
		#endregion // class DataWithNullable

		#region class BinaryTreeNode
		public class BinaryTreeNode
		{
			public DataProperty<BinaryTreeNode> Left { get; } = new DataProperty<BinaryTreeNode>();

			public DataProperty<BinaryTreeNode> Right { get; } = new DataProperty<BinaryTreeNode>();

			public DataProperty<string> Text { get; } = new DataProperty<string>();
		}
		#endregion // class BinaryTreeNode

		#region class DataCollector
		public class DataCollector
		{
			public DataPropertyCollection<int> PropertyIntCollection { get; } = new DataPropertyCollection<int>();

			public DataPropertyCollection<Data> PropertyDataCollection { get; } = new DataPropertyCollection<Data>();
		}
		#endregion // class DataCollector

		#region class DataMapper
		public class DataMapper
		{
			public DataPropertyDictionary<string, int> PropertyStringIntMap { get; } = new DataPropertyDictionary<string, int>();

			public DataPropertyDictionary<Data, Data> PropertyPedigree { get; } = new DataPropertyDictionary<Data, Data>();
		}
		#endregion // class DataMapper

		#region class DataPedigree
		public class DataPedigree
		{
			public DataPropertyPedigree<SimpleData> PropertyPedigree { get; } = new DataPropertyPedigree<SimpleData>();
		}
		#endregion // class DataPedigree

		#region class Graph
		public class Graph
		{
			public DataPropertyCollection<GraphNode> Nodes { get; } = new DataPropertyCollection<GraphNode>();

			public GraphNode AddNode(string name)
			{
				var ret = new GraphNode();
				ret.Text.Value = name;
				Nodes.Add(ret);

				return ret;
			}

			public GraphEdge Link(GraphNode source, GraphNode destination, string name)
			{
				var ret = new GraphEdge();
				ret.Text.Value = name;
				ret.LinkFrom.Value = source;
				ret.LinkTo.Value = destination;

				source.LinkOut.Add(ret);
				destination.LinkIn.Add(ret);

				return ret;
			}

			public DataProperty<string> Text { get; } = new DataProperty<string>();
		}
		#endregion // class Graph

		#region class GraphNode
		public class GraphNode
		{
			public DataPropertyCollection<GraphEdge> LinkIn { get; } = new DataPropertyCollection<GraphEdge>();

			public DataPropertyCollection<GraphEdge> LinkOut { get; } = new DataPropertyCollection<GraphEdge>();

			public DataProperty<string> Text { get; } = new DataProperty<string>();
		}
		#endregion // class GraphNode

		#region class GraphEdge
		public class GraphEdge
		{
			public DataProperty<GraphNode> LinkFrom { get; } = new DataProperty<GraphNode>();

			public DataProperty<GraphNode> LinkTo { get; } = new DataProperty<GraphNode>();

			public DataProperty<string> Text { get; } = new DataProperty<string>();
		}
		#endregion // class GraphEdge
		#endregion // private members
	}
}