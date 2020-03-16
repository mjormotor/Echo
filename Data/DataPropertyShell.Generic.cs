using System.Collections.Generic;

namespace Echo.Data
{
	/// <summary>
	/// キー付きデータプロパティ
	/// </summary>
	[System.Diagnostics.DebuggerTypeProxy(typeof(DataPropertyShell<,>.DebugView))]
	public class DataPropertyShell<TKey, TValue> : KeyedValueShell<TKey, DataProperty<TValue>>, IDataPropertyShell
	{
		public DataPropertyShell(TKey key, DataProperty<TValue> value)
			: base(key, value)
		{
		}

		public DataPropertyShell(KeyValuePair<TKey, DataProperty<TValue>> keyValuePair)
			: base(keyValuePair)
		{
		}

		public DataPropertyShell(KeyedValueShell<TKey, DataProperty<TValue>> keyValueShell)
			: base(keyValueShell.KeyValuePair)
		{
		}

		#region IDataPropertyShell interface support
		IDataProperty IDataPropertyShell.Core => Core;
		#endregion  // IDataPropertyShell interface 


		#region private members
		#region class DebugView
		private class DebugView
		{
			public DebugView(DataPropertyShell<TKey, TValue> target)
			{
				this.target = target;
			}

			public TKey Key => this.target.Key;

			public DataProperty<TValue> Core => this.target.Core;

			#region private members
			private DataPropertyShell<TKey, TValue> target;
			#endregion // private members
		}
		#endregion // class DebugView
		#endregion // private members
	}
}
