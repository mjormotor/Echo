using System;
using System.Collections.Generic;
using Echo.Data;
using Echo.PWalkService;

namespace Echo.Presentation
{
	public class GeneralDataPresentation : Observable, IDataPresentation, IDisposable
	{
		public GeneralDataPresentation(object core)
		{
			if (core == null)
			{
				throw new ArgumentNullException(nameof(core));
			}

			Core = core;

			Setup();
		}

		#region IDataPresentation interface support
		public IEnumerable<IDataPropertyPresentation> Items => ItemsCore;
		#endregion  // IDataPresentation interface support

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
			PWalk.TargetType<IDataProperty>(ProvidePropertyPresentation, Core);
		}

		protected virtual void OnTeardown()
		{
			foreach (var item in Items)
			{
				var disposable = item as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}

			ItemsCore.Clear();
		}
		#endregion // protected virtual members

		protected object Core { get; }

		protected IList<IDataPropertyPresentation> ItemsCore { get; } = new List<IDataPropertyPresentation>();
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

		private void ProvidePropertyPresentation(IDataProperty target, PWalkContext pwalk)
		{
			ItemsCore.Add(new GeneralDataPropertyPresentation(target));
		}
		#endregion // private members
	}
}
