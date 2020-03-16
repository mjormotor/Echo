using System;
using Echo.Data;

namespace Echo.Presentation
{
	public class GeneralDataPropertyPresentation : Observable, IDataPropertyPresentation, IDisposable
	{
		public GeneralDataPropertyPresentation(IDataProperty core)
		{
			if (core == null)
			{
				throw new ArgumentNullException(nameof(core));
			}

			Core = core;

			Setup();
		}

		#region IDataPropertyPresentation interface support
		public object Value => Core.Value;

		public string Name => Core.Name;
		#endregion  // IDataPropertyPresentation interface support

		#region IDisposable interface support
		public void Dispose()
		{
			Teardown();
		}
		#endregion  // IDisposable interface support

		#region protected members
		#region protected virtual members
		protected virtual void OnSetup()
		{
			Core.DataPropertyChanging += OnCoreDataPropertyChanging;
			Core.DataPropertyChanged += OnCoreDataPropertyChanged;
		}

		protected virtual void OnTeardown()
		{
			Core.DataPropertyChanged -= OnCoreDataPropertyChanged;
			Core.DataPropertyChanging -= OnCoreDataPropertyChanging;
		}

		protected virtual void OnCoreDataPropertyChanging(DataPropertyChangingEventArgs e)
		{
		}

		protected virtual void OnCoreDataPropertyChanged(DataPropertyChangedEventArgs e)
		{
			RaisePropertyChanged(nameof(Value));
		}
		#endregion // protected virtual members

		protected IDataProperty Core { get; }
		#endregion // protected members

		#region private members
		private void Setup()
		{
			OnSetup();
		}

		private void Teardown()
		{
			OnTeardown();
		}

		private void OnCoreDataPropertyChanging(object sender, DataPropertyChangingEventArgs e)
		{
			OnCoreDataPropertyChanging(e);
		}

		private void OnCoreDataPropertyChanged(object sender, DataPropertyChangedEventArgs e)
		{
			OnCoreDataPropertyChanged(e);
		}
		#endregion // private members
	}
}
