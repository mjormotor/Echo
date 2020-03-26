namespace Echo.Data
{
	/// <summary>
	/// データプロパティ辞書値コンテキスト
	/// </summary>
	public class DataPropertyDictionaryValueContext<TKey, TValue> : IReadOnlyDataPropertyDictionaryValueContext<TKey, TValue>
	{
		/// <summary>
		/// 変更後のキー
		/// </summary>
		public TKey NewKey { get; set; }

		/// <summary>
		/// 入力時のキー
		/// </summary>
		public TKey InputKey { get; }

		/// <summary>
		/// 変更前のキー
		/// </summary>
		public TKey OldKey { get; }

		/// <summary>
		/// 変更後の値
		/// </summary>
		public TValue NewValue { get; set; }

		/// <summary>
		/// 入力時の値
		/// </summary>
		public TValue InputValue { get; }

		/// <summary>
		/// 変更前の値
		/// </summary>
		public TValue OldValue { get; }

		#region internal members
		internal DataPropertyDictionaryValueContext(TKey key, DataProperty<TValue> property)
		{
			NewKey = key;
			InputKey = key;
			OldKey = default;
			NewValue = property.Value;
			InputValue = property.Value;
			OldValue = default;
			Property = property;
		}

		internal DataPropertyDictionaryValueContext(TKey oldKey, TValue oldValue, TKey inputKey, TValue inputValue, DataProperty<TValue> property)
		{
			NewKey = inputKey;
			InputKey = inputKey;
			OldKey = oldKey;
			NewValue = inputValue;
			InputValue = inputValue;
			OldValue = oldValue;
			Property = property;
		}

		internal DataProperty<TValue> Property { get; }
		#endregion // internal members
	}
}
