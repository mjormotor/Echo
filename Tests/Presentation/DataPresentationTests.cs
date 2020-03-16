using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Echo.Data;

namespace Echo.Presentation.Tests
{
	[TestClass]
	public class DataPresentationTests
	{
		[TestMethod]
		public void DataPresentationTest()
		{
			var data = new Data();
			var presentation = DataPresentationService.Provide(data) as GeneralDataPresentation;
			Assert.IsNotNull(presentation);
			Assert.IsTrue(presentation.Items.Any());

			var item = presentation.Items.First() as INotifyPropertyChanged;
			Assert.IsNotNull(item);

			var invokeCount = 0;
			var oldValue = data.PropertyInt.Value;
			var value = oldValue + 421;

			PropertyChangedEventHandler handler = (object sender, PropertyChangedEventArgs e) =>
			{
				++invokeCount;
			};

			item.PropertyChanged += handler;

			data.PropertyInt.Value = value;

			item.PropertyChanged -= handler;

			Assert.IsTrue(invokeCount == 1);
		}

		#region private members
		#region class Data
		private class Data
		{
			public DataProperty<int> PropertyInt { get; } = new DataProperty<int>();
		}
		#endregion // class Data
		#endregion // private members
	}
}