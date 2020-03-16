using System;
using System.Runtime.CompilerServices;

namespace Echo.Data
{
	/// <summary>
	/// データプロパティ
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("{Value}")]
	[System.Diagnostics.DebuggerTypeProxy(typeof(DataProperty<>.DebugView))]
	public class DataProperty<T> : IDataProperty, IDataPropertyShell
	{
		public DataProperty([CallerMemberName] string name = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name));
			}

			NameCore = name;
		}

		public DataProperty(T value, [CallerMemberName] string name = null)
			: this(name)
		{
			DefaultValue = value;
			ValueCore = value;
		}

		/// <summary>
		/// プロパティ変更前
		/// </summary>
		public event EventHandler<DataPropertyChangingEventArgs<T>> DataPropertyChanging;

		/// <summary>
		/// プロパティ変更後
		/// </summary>
		public event EventHandler<DataPropertyChangedEventArgs<T>> DataPropertyChanged;

		/// <summary>
		/// 値
		/// </summary>
		[DataProperty]
		public T Value
		{
			get { return ValueCore; }
			set { SetValue(value); }
		}

		/// <summary>
		/// 初期値
		/// </summary>
		public T DefaultValue { get; }

		#region IDataProperty interface support
		#region IDataPropertyCore interface support
		public string Name => NameCore;

		public Type PropertyType => typeof(T);
		#endregion  // IDataPropertyCore interface support

		event EventHandler<DataPropertyChangingEventArgs> IDataProperty.DataPropertyChanging
		{
			add { _DataPropertyChanging += value; }
			remove { _DataPropertyChanging -= value; }
		}

		event EventHandler<DataPropertyChangedEventArgs> IDataProperty.DataPropertyChanged
		{
			add { _DataPropertyChanged += value; }
			remove { _DataPropertyChanged -= value; }
		}

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		string IDataProperty.Name
		{
			get { return NameCore; }
			set { NameCore = value; }
		}

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		object IDataProperty.Value
		{
			get { return Value; }
			set { Value = (T)value; }
		}
		#endregion  // IDataProperty interface support

		#region IDataPropertyShell interface support
		IDataProperty IDataPropertyShell.Core => this;
		#endregion  // IDataPropertyShell interface support

		#region protected members
		#region protected virtual members
		/// <summary>
		/// 設定の操作
		/// </summary>
		protected virtual void SetValue(T value)
		{
			var oldValue = ValueCore;
			if (!Equals(value, oldValue))
			{
				CheckReentrancy();

				var e = new DataPropertyChangingEventArgs<T>(this, value, oldValue);
				OnDataPropertyChanging(e);
				if (!e.Cancel)
				{
					var newValue = e.NewValue;
					if (!Equals(newValue, oldValue))
					{
						ValueCore = newValue;
						OnDataPropertyChanged(new DataPropertyChangedEventArgs<T>(this, newValue, value, oldValue));
					}
				}
			}
		}

		/// <summary>
		/// プロパティ変更前の処理
		/// </summary>
		protected virtual void OnDataPropertyChanging(DataPropertyChangingEventArgs<T> e)
		{
			if (DataPropertyChanging != null || _DataPropertyChanging != null)
			{
				using (BlockReentrancy())
				{
					DataPropertyChanging?.Invoke(this, e);
					_DataPropertyChanging?.Invoke(this, e);
				}
			}
		}

		/// <summary>
		/// プロパティ変更後の処理
		/// </summary>
		protected virtual void OnDataPropertyChanged(DataPropertyChangedEventArgs<T> e)
		{
			_DataPropertyChanged?.Invoke(this, e);
			DataPropertyChanged?.Invoke(this, e);
		}
		#endregion // protected virtual members

		/// <summary>
		/// 名前
		/// </summary>
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		protected internal string NameCore { get; set; }

		/// <summary>
		/// 値
		/// </summary>
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		protected T ValueCore { get; set; }

		/// <summary>
		/// 再操作の防止
		/// </summary>
		protected IDisposable BlockReentrancy()
		{
			return this.watcher.Scope();
		}

		/// <summary>
		/// 再操作の確認
		/// </summary>
		protected void CheckReentrancy()
		{
			if (this.watcher.IsInScope)
			{
				var invocationCount = 0;
				if (DataPropertyChanging != null)
				{
					invocationCount += DataPropertyChanging.GetInvocationList().Length;
				}

				if (_DataPropertyChanging != null)
				{
					invocationCount += _DataPropertyChanging.GetInvocationList().Length;
				}

				if (invocationCount > 1)
				{
					throw new InvalidOperationException(string.Format(Echo.Properties.Resources.MESSAGE_EXCEPTION_INVALID_OPERATION_REENTRANCY_FORMAT, nameof(DataProperty<T>), nameof(DataPropertyChanging)));
				}
			}
		}
		#endregion // protected members

		#region private members
		#region class DebugView
		private class DebugView
		{
			public DebugView(DataProperty<T> target)
			{
				this.target = target;
			}

			public T Value => this.target.Value;

			public T DefaultValue => this.target.DefaultValue;

			public string Name => this.target.Name;

			public Type PropertyType => this.target.PropertyType;

			#region private members
			private DataProperty<T> target;
			#endregion // private members
		}
		#endregion // class DebugView

		private ScopeWatcher watcher = new ScopeWatcher();
		private EventHandler<DataPropertyChangingEventArgs> _DataPropertyChanging;
		private EventHandler<DataPropertyChangedEventArgs> _DataPropertyChanged;
		#endregion // private members
	}
}
