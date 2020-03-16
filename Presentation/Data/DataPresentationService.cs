using System;
using System.Collections.Generic;

namespace Echo.Presentation
{
	/// <summary>
	/// 表示方法
	/// </summary>
	public delegate object Presenter(object target);

	/// <summary>
	/// 表示方法の用意
	/// </summary>
	public static class DataPresentationService
	{
		/// <summary>
		/// 表示方法の登録
		/// </summary>
		public static void RegisterPresenter(Type type, Presenter presenter)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (presenter == null)
			{
				throw new ArgumentNullException(nameof(presenter));
			}

			provider.RegisterPresenter(type, presenter);
		}

		/// <summary>
		/// 表示方法の抹消
		/// </summary>
		public static void UnregisterPresenter(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			provider.UnregisterPresenter(type);
		}

		/// <summary>
		/// 表示方法の抹消
		/// </summary>
		public static void UnregisterPresenter(Presenter presenter)
		{
			if (presenter == null)
			{
				throw new ArgumentNullException(nameof(presenter));
			}

			provider.UnregisterPresenter(presenter);
		}

		/// <summary>
		/// 表示方法の登録
		/// </summary>
		public static object Provide(object target)
		{
			if (target == null)
			{
				throw new ArgumentNullException(nameof(target));
			}

			return provider.Provide(target);
		}

		#region private members
		#region class Provider
		private class Provider
		{
			public void RegisterPresenter(Type type, Presenter presenter)
			{
				this.presenters.Add(type, presenter);
			}

			public void UnregisterPresenter(Type type)
			{
				this.presenters.Remove(type);
			}

			public void UnregisterPresenter(Presenter presenter)
			{
				var types = new List<Type>(this.presenters.FetchKeys(presenter));
				foreach (var type in types)
				{
					UnregisterPresenter(type);
				}
			}

			public object Provide(object target)
			{
				object ret = null;
				if (target != null)
				{
					var type = target.GetType();
					if (this.presenters.TryGetValue(type, out var presenter))
					{
						ret = presenter(target);
					}
					else
					{
						ret = DefaultPresenter(target);
					}
				}

				return ret;
			}

			#region private members
			private Dictionary<Type, Presenter> presenters = new Dictionary<Type, Presenter>();

			private object DefaultPresenter(object target)
			{
				var ret = new GeneralDataPresentation(target);
				return ret;
			}
			#endregion // private members
		}
		#endregion // class Provider

		private static Provider provider = new Provider();
		#endregion // private members
	}
}
