using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Echo.Presentation.Tests
{
	[TestClass]
	public class ResettableObservableCollectionTests
	{
		[TestMethod]
		public void AddRangeTest()
		{
			var collection = new ResettableObservableCollection<Item>();

			var invokeCount = 0;
			Item[] items = { new Item(), new Item(), };
			NotifyCollectionChangedEventHandler handler = (object sender, NotifyCollectionChangedEventArgs e) =>
			{
				++invokeCount;
				Assert.IsTrue(e.Action == NotifyCollectionChangedAction.Add);
				Assert.IsTrue(e.OldItems == null);
				Assert.IsTrue(e.NewItems != null);
				Assert.IsTrue(e.OldStartingIndex == -1);
				Assert.IsTrue(e.NewStartingIndex == 0);
				Assert.IsTrue(e.NewItems.Count == items.Length);
				Assert.IsTrue(e.NewItems.Cast<Item>().SequenceEqual(items));
			};

			collection.CollectionChanged += handler;

			collection.AddRange(items);

			collection.CollectionChanged -= handler;

			Assert.IsTrue(invokeCount == 1);
		}

		[TestMethod]
		public void ClearTest()
		{
			var collection = new ResettableObservableCollection<Item>();

			var invokeCount = 0;
			Item[] items = { new Item(), new Item(), };
			collection.AddRange(items);
			NotifyCollectionChangedEventHandler handler = (object sender, NotifyCollectionChangedEventArgs e) =>
			{
				++invokeCount;
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Remove:
						Assert.IsTrue(invokeCount == 1);
						Assert.IsTrue(e.OldItems != null);
						Assert.IsTrue(e.NewItems == null);
						Assert.IsTrue(e.OldStartingIndex == 0);
						Assert.IsTrue(e.NewStartingIndex == -1);
						Assert.IsTrue(e.OldItems.Count == items.Length);
						Assert.IsTrue(e.OldItems.Cast<Item>().SequenceEqual(items));
						break;

					case NotifyCollectionChangedAction.Reset:
						Assert.IsTrue(invokeCount == 2);
						Assert.IsTrue(e.OldItems == null);
						Assert.IsTrue(e.NewItems == null);
						Assert.IsTrue(e.OldStartingIndex == -1);
						Assert.IsTrue(e.NewStartingIndex == -1);
						break;

					default:
						Assert.Fail();
						break;
				}
			};

			collection.CollectionChanged += handler;

			collection.Clear();

			collection.CollectionChanged -= handler;

			Assert.IsTrue(invokeCount == 2);
		}

		#region private members
		#region class Item
		private class Item
		{
		}
		#endregion // class Item
		#endregion // private members
	}
}