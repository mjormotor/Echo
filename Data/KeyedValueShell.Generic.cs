using System;
using System.Collections.Generic;

namespace Echo.Data
{
	/// <summary>
	/// キー付き値格納
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("{KeyValuePair}")]
	public class KeyedValueShell<TKey, TValue> : IKeyedValueShell<TKey, TValue>
	{
		public KeyedValueShell(TKey key, TValue value)
		{
			KeyValuePair = new KeyValuePair<TKey, TValue>(key, value);
		}

		public KeyedValueShell(KeyValuePair<TKey, TValue> keyValuePair)
		{
			KeyValuePair = keyValuePair;
		}

		public KeyValuePair<TKey, TValue> KeyValuePair { get; }

		#region IKeyedValueShell<TKey, TValue> interface support
		public TKey Key => KeyValuePair.Key;

		public TValue Core => KeyValuePair.Value;
		#endregion  // IKeyedValueShell<TKey, TValue> interface support

		#region ToString method override
		public override string ToString() => KeyValuePair.ToString();
		#endregion  // ToString method override
	}
}
