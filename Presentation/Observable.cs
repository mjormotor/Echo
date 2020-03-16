using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Echo.Presentation
{
    public class Observable : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged interface support
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion  // INotifyPropertyChanged interface support

		#region protected members
		protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			var ret = false;
			if (!Equals(value, field))
			{
				field = value;
				RaisePropertyChanged(propertyName);
				ret = true;
			}

			return ret;
		}

		protected void RaisePropertyChanged([CallerMemberName] string propertyName = null) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

		protected void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
		#endregion // protected members
	}
}
