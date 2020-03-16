using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Echo.Data.Tests
{
	using Dictionary = Dictionary<object, object>;
	using DataPropertyDictionary = DataPropertyDictionary<object, object>;

	[TestClass]
	public class ComparativeTests
	{
		[TestMethod]
		public void XmlSerializerCollectionSerializeTest()
		{
			var data = new DataWithCollection();
			data.Items.Add(new Item() { Text = "Item0", });
			data.Items.Add(new Item() { Text = "Item1", });
			data.Items.Add(null);
			data.Items.Add(new Item() { Text = "Item2", });

			var serializer = new XmlSerializer(typeof(DataWithCollection));
			using var stream = new MemoryStream();

			serializer.Serialize(stream, data);
			var xml = Encoding.UTF8.GetString(stream.GetBuffer());
		}

		[TestMethod]
		public void XmlSerializerCollectionsSerializeTest()
		{
			var data = new DataWithCollectionsCollection();
			var items0 = new Collection<Item> { new Item() { Text = "Item00", }, new Item() { Text = "Item01", }, new Item() { Text = "Item02", }, };
			var items1 = new Collection<Item> { new Item() { Text = "Item10", }, new Item() { Text = "Item11", }, new Item() { Text = "Item12", }, };
			var items2 = new Collection<Item> { new Item() { Text = "Item20", }, new Item() { Text = "Item21", }, new Item() { Text = "Item22", }, };

			var serializer = new XmlSerializer(typeof(DataWithCollectionsCollection));
			using var stream = new MemoryStream();

			serializer.Serialize(stream, data);
			var xml = Encoding.UTF8.GetString(stream.GetBuffer());
		}

		//[TestMethod]
		public void XmlSerializerDictionarySerializeTest()
		{
			var data = new DataWithDictionary();
			data.Items.Add("item0", new Item() { Text = "Item0", });
			data.Items.Add("item1", new Item() { Text = "Item1", });
			data.Items.Add("null", null);
			data.Items.Add("item2", new Item() { Text = "Item2", });

			var serializer = new XmlSerializer(typeof(DataWithDictionary));
			using var stream = new MemoryStream();

			serializer.Serialize(stream, data);
			var xml = Encoding.UTF8.GetString(stream.GetBuffer());
		}

		[TestMethod]
		public void DataContractSerializerCollectionSerializeTest()
		{
			var data = new DataWithCollection();
			data.Items.Add(new Item() { Text = "Item0", });
			data.Items.Add(new Item() { Text = "Item1", });
			data.Items.Add(null);
			data.Items.Add(new Derived() { Text = "Item2", DerivedText = "Derived" });
			data.Items.Add(data.Items[0]);

			var serializer = new DataContractSerializer(typeof(DataWithCollection));
			using var stream = new MemoryStream();
			using var writer = XmlWriter.Create(stream, DefaultXmlWriterSettings);

			serializer.WriteObject(writer, data);
			writer.Flush();
			var xml = Encoding.UTF8.GetString(stream.GetBuffer());
			stream.Position = 0;

			data = (DataWithCollection)serializer.ReadObject(stream);
			Assert.IsFalse(ReferenceEquals(data.Items[4], data.Items[0]));
		}

		[TestMethod]
		public void DataContractSerializerCollectionsSerializeTest()
		{
			var data = new DataWithCollectionsCollection();
			var items0 = new Collection<Item> { new Item() { Text = "Item00", }, new Item() { Text = "Item01", }, new Item() { Text = "Item02", }, };
			var items1 = new Collection<Item> { new Item() { Text = "Item10", }, new Item() { Text = "Item11", }, new Item() { Text = "Item12", }, };
			var items2 = new Collection<Item> { new Item() { Text = "Item20", }, new Item() { Text = "Item21", }, new Item() { Text = "Item22", }, };

			var serializer = new DataContractSerializer(typeof(DataWithCollectionsCollection));
			using var stream = new MemoryStream();
			using var writer = XmlWriter.Create(stream, DefaultXmlWriterSettings);

			serializer.WriteObject(writer, data);
			writer.Flush();
			var xml = Encoding.UTF8.GetString(stream.GetBuffer());
			stream.Position = 0;

			data = (DataWithCollectionsCollection)serializer.ReadObject(stream);
		}

		[TestMethod]
		public void DataContractSerializerDictionarySerializeTest()
		{
			var data = new DataWithDictionary();
			data.Items.Add("item0", new Item() { Text = "Item0", });
			data.Items.Add("item1", new Item() { Text = "Item1", });
			data.Items.Add("null", null);
			data.Items.Add("item2", new Derived() { Text = "Item2", DerivedText = "Derived" });
			data.Items.Add("item00", data.Items["item0"]);

			var serializer = new DataContractSerializer(typeof(DataWithDictionary));
			using var stream = new MemoryStream();
			using var writer = XmlWriter.Create(stream, DefaultXmlWriterSettings);

			serializer.WriteObject(writer, data);
			writer.Flush();
			var xml = Encoding.UTF8.GetString(stream.GetBuffer());
			stream.Position = 0;

			data = (DataWithDictionary)serializer.ReadObject(stream);
			Assert.IsFalse(ReferenceEquals(data.Items["item00"], data.Items["item0"]));
		}

		[TestMethod]
		public void DataContractSerializerReverseDictionarySerializeTest()
		{
			var data = new DataWithReverseDictionary();
			data.Items.Add(new Item() { Text = "Item0", }, "item0");
			data.Items.Add(new Item() { Text = "Item1", }, "item1");
			data.Items.Add(new Item(), null);
			data.Items.Add(new Derived() { Text = "Item2", DerivedText = "Derived" }, "item2");

			var serializer = new DataContractSerializer(typeof(DataWithReverseDictionary));
			using var stream = new MemoryStream();
			using var writer = XmlWriter.Create(stream, DefaultXmlWriterSettings);

			serializer.WriteObject(writer, data);
			writer.Flush();
			var xml = Encoding.UTF8.GetString(stream.GetBuffer());
			stream.Position = 0;

			data = (DataWithReverseDictionary)serializer.ReadObject(stream);
		}

		[TestMethod]
		public void ExceptionMessageTest()
		{
			System.Diagnostics.Debug.WriteLine($"> {nameof(Dictionary)}.{nameof(Dictionary.KeyCollection)}.{nameof(Dictionary.KeyCollection)}(null)");
			try
			{
				var keyCollection = new Dictionary.KeyCollection(null);
			}
			catch (ArgumentNullException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			System.Diagnostics.Debug.WriteLine(string.Empty);

			System.Diagnostics.Debug.WriteLine($"> {nameof(DataPropertyDictionary)}.{nameof(DataPropertyDictionary.KeyCollection)}.{nameof(DataPropertyDictionary.KeyCollection)}(null)");
			try
			{
				var keyCollection = new DataPropertyDictionary.KeyCollection(null);
			}
			catch (ArgumentNullException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			System.Diagnostics.Debug.WriteLine(string.Empty);
			System.Diagnostics.Debug.WriteLine(string.Empty);

			System.Diagnostics.Debug.WriteLine($"> {nameof(Dictionary)}.{nameof(Dictionary.KeyCollection)}.{nameof(ICollection<object>.Add)}(null)");
			try
			{
				var dictionary = new Dictionary();
				((ICollection<object>)dictionary.Keys).Add(null);
			}
			catch (NotSupportedException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			System.Diagnostics.Debug.WriteLine(string.Empty);

			System.Diagnostics.Debug.WriteLine($"> {nameof(DataPropertyDictionary)}.{nameof(DataPropertyDictionary.KeyCollection)}.{nameof(ICollection<object>.Add)}(null)");
			try
			{
				var dictionary = new DataPropertyDictionary();
				((ICollection<object>)dictionary.Keys).Add(null);
			}
			catch (NotSupportedException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			System.Diagnostics.Debug.WriteLine(string.Empty);
			System.Diagnostics.Debug.WriteLine(string.Empty);

			System.Diagnostics.Debug.WriteLine($"> {nameof(Dictionary)}.{nameof(Dictionary.KeyCollection)}.{nameof(Dictionary.KeyCollection.CopyTo)}(array, -1)");
			try
			{
				var dictionary = new Dictionary();
				dictionary.Keys.CopyTo(new object[] { }, -1);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			System.Diagnostics.Debug.WriteLine(string.Empty);

			System.Diagnostics.Debug.WriteLine($"> {nameof(DataPropertyDictionary)}.{nameof(DataPropertyDictionary.KeyCollection)}.{nameof(DataPropertyDictionary.KeyCollection.CopyTo)}(array, -1)");
			try
			{
				var dictionary = new DataPropertyDictionary();
				dictionary.Keys.CopyTo(new object[] { }, -1);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			System.Diagnostics.Debug.WriteLine(string.Empty);
			System.Diagnostics.Debug.WriteLine(string.Empty);

			System.Diagnostics.Debug.WriteLine($"> {nameof(Dictionary)}.{nameof(Dictionary.KeyCollection)}.{nameof(Dictionary.KeyCollection.CopyTo)}(shortArray, 0)");
			try
			{
				var dictionary = new Dictionary();
				dictionary.Add(1, 1);
				dictionary.Keys.CopyTo(new object[] { }, 0);
			}
			catch (ArgumentException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			System.Diagnostics.Debug.WriteLine(string.Empty);

			System.Diagnostics.Debug.WriteLine($"> {nameof(DataPropertyDictionary)}.{nameof(DataPropertyDictionary.KeyCollection)}.{nameof(DataPropertyDictionary.KeyCollection.CopyTo)}(shortArray, 0)");
			try
			{
				var dictionary = new DataPropertyDictionary();
				dictionary.Add(1, 1);
				dictionary.Keys.CopyTo(new object[] { }, 0);
			}
			catch (ArgumentException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			System.Diagnostics.Debug.WriteLine(string.Empty);
			System.Diagnostics.Debug.WriteLine(string.Empty);

			System.Diagnostics.Debug.WriteLine($"> {nameof(Dictionary)}.{nameof(Dictionary.KeyCollection)}.{nameof(Dictionary.KeyCollection.CopyTo)}(bottomedUpArray, 0)");
			try
			{
				var dictionary = new Dictionary();
				((ICollection)dictionary.Keys).CopyTo(Array.CreateInstance(typeof(object), new int[] { 0, }, new int [] { 1, }), 0);
			}
			catch (ArgumentException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			System.Diagnostics.Debug.WriteLine(string.Empty);

			System.Diagnostics.Debug.WriteLine($"> {nameof(DataPropertyDictionary)}.{nameof(DataPropertyDictionary.KeyCollection)}.{nameof(DataPropertyDictionary.KeyCollection.CopyTo)}(bottomedUpArray, 0)");
			try
			{
				var dictionary = new DataPropertyDictionary();
				((ICollection)dictionary.Keys).CopyTo(Array.CreateInstance(typeof(object), new int[] { 0, }, new int [] { 1, }), 0);
			}
			catch (ArgumentException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			System.Diagnostics.Debug.WriteLine(string.Empty);
			System.Diagnostics.Debug.WriteLine(string.Empty);

			System.Diagnostics.Debug.WriteLine($"> {nameof(Dictionary)}.{nameof(Dictionary.KeyCollection)}.{nameof(Dictionary.KeyCollection.CopyTo)}(rank2Array, 0)");
			try
			{
				var dictionary = new Dictionary();
				((ICollection)dictionary.Keys).CopyTo(new object[,] { }, 0);
			}
			catch (ArgumentException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			System.Diagnostics.Debug.WriteLine(string.Empty);

			System.Diagnostics.Debug.WriteLine($"> {nameof(DataPropertyDictionary)}.{nameof(DataPropertyDictionary.KeyCollection)}.{nameof(DataPropertyDictionary.KeyCollection.CopyTo)}(rank2Array, 0)");
			try
			{
				var dictionary = new DataPropertyDictionary();
				((ICollection)dictionary.Keys).CopyTo(new object[,] { }, 0);
			}
			catch (ArgumentException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			System.Diagnostics.Debug.WriteLine(string.Empty);
			System.Diagnostics.Debug.WriteLine(string.Empty);

			System.Diagnostics.Debug.WriteLine($"> {nameof(Dictionary)}.{nameof(Dictionary.KeyCollection)}.{nameof(Dictionary.KeyCollection.CopyTo)}(mismatchTypedArray, 0)");
			try
			{
				var dictionary = new Dictionary();
				((ICollection)dictionary.Keys).CopyTo(new int[] { }, 0);
			}
			catch (ArgumentException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			System.Diagnostics.Debug.WriteLine(string.Empty);

			System.Diagnostics.Debug.WriteLine($"> {nameof(DataPropertyDictionary)}.{nameof(DataPropertyDictionary.KeyCollection)}.{nameof(DataPropertyDictionary.KeyCollection.CopyTo)}(mismatchTypedArray, 0)");
			try
			{
				var dictionary = new DataPropertyDictionary();
				((ICollection)dictionary.Keys).CopyTo(new int[] { }, 0);
			}
			catch (ArgumentException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			System.Diagnostics.Debug.WriteLine(string.Empty);
			System.Diagnostics.Debug.WriteLine(string.Empty);

			System.Diagnostics.Debug.WriteLine($"> {nameof(Dictionary)}.{nameof(Dictionary.ValueCollection)}.{nameof(ICollection<object>.Add)}(null)");
			try
			{
				var dictionary = new Dictionary();
				((ICollection<object>)dictionary.Values).Add(null);
			}
			catch (NotSupportedException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			System.Diagnostics.Debug.WriteLine(string.Empty);

			System.Diagnostics.Debug.WriteLine($"> {nameof(DataPropertyDictionary)}.{nameof(DataPropertyDictionary.ItemCollection)}.{nameof(ICollection<object>.Add)}(null)");
			try
			{
				var dictionary = new DataPropertyDictionary();
				((ICollection<DataProperty<object>>)dictionary.Items).Add(null);
			}
			catch (NotSupportedException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			System.Diagnostics.Debug.WriteLine(string.Empty);
			System.Diagnostics.Debug.WriteLine(string.Empty);
		}

		#region private members
		private static readonly XmlWriterSettings DefaultXmlWriterSettings = new XmlWriterSettings()
		{
			NewLineChars = "\r\n",
			IndentChars = "\t",
			Indent = true,
			Encoding = Encoding.UTF8,
			CheckCharacters = false,
		};

		#region class Item
		[DataContract]
		public class Item
		{
			[DataMember]
			public string Text { get; set; }
		}
		#endregion // class Item

		#region class Item
		[DataContract]
		public class Item<T>

		{
			[DataMember]
			public string Text { get; set; }
		}
		#endregion // class Item

		#region class Derived
		[DataContract]
		private class Derived : Item
		{
			[DataMember]
			public string DerivedText { get; set; }
		}
		#endregion // class Derived

		#region class DataWithCollection
		[KnownType(typeof(Derived))]
		public class DataWithCollection
		{
			[DataMember]
			public Collection<Item> Items { get; } = new Collection<Item>();
		}
		#endregion // class DataWithCollection

		#region class DataWithCollectionsCollection
		[KnownType(typeof(Derived))]
		public class DataWithCollectionsCollection
		{
			[DataMember]
			public Collection<Collection<Item>> Items { get; } = new Collection<Collection<Item>>();
		}
		#endregion // class DataWithCollectionsCollection

		#region class DataWithDictionary
		[KnownType(typeof(Derived))]
		public class DataWithDictionary
		{
			[DataMember]
			public Dictionary<string, Item> Items { get; } = new Dictionary<string, Item>();
		}
		#endregion // class DataWithDictionary

		#region class DataWithDictionary
		[KnownType(typeof(Derived))]
		public class DataWithReverseDictionary
		{
			[DataMember]
			public Dictionary<Item, string> Items { get; } = new Dictionary<Item, string>();
		}
		#endregion // class DataWithDictionary
		#endregion // private members
	}
}