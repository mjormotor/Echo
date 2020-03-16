namespace Echo.Data
{
	/// <summary>
	/// データプロパティ列変更操作
	/// </summary>
	public class DataPropertyDictionaryChangeOperation<TKey, TValue> : IReadOnlyDataPropertyDictionaryChangeOperation<TKey, TValue>
	{
		/// <summary>
		/// 操作の種別
		/// </summary>
		public DataPropertyCollectionChangeAction Action { get; }

		/// <summary>
		/// 位置
		/// </summary>
		public int Index
		{
			get { return this.index; }
			set { this.index = value; }
		}

		/// <summary>
		/// 移動先の位置
		/// </summary>
		public int MovingIndex
		{
			get { return this.movingIndex; }
			set { this.movingIndex = value; }
		}

		/// <summary>
		/// キー
		/// </summary>
		public TKey Key { get; set; }

		/// <summary>
		/// 移動先のキー
		/// </summary>
		public TKey MovingKey { get; set; }

		/// <summary>
		/// 設定値
		/// </summary>
		public DataPropertyValueContext<TValue> SettingValue { get; }

		#region IReadOnlyDataPropertyDictionaryChangeOperation<TKey, TValue> interface support
		#region IReadOnlyDataPropertyCollectionChangeOperation<TValue> interface support
		IReadOnlyDataPropertyValueContext<TValue> IReadOnlyDataPropertyCollectionChangeOperation<TValue>.SettingValue => SettingValue;
		#endregion  // IReadOnlyDataPropertyCollectionChangeOperation<TValue> interface support
		#endregion  // IReadOnlyDataPropertyDictionaryChangeOperation<TKey, TValue> interface support

		#region internal members
		internal static DataPropertyDictionaryChangeOperation<TKey, TValue> Insert(int index, TKey key, DataProperty<TValue> property)
		{
			var settingValue = new DataPropertyValueContext<TValue>(property);
			return new DataPropertyDictionaryChangeOperation<TKey, TValue>(DataPropertyCollectionChangeAction.Insert, index, key, settingValue);
		}

		internal static DataPropertyDictionaryChangeOperation<TKey, TValue> Remove(int index, TKey key)
		{
			return new DataPropertyDictionaryChangeOperation<TKey, TValue>(DataPropertyCollectionChangeAction.Remove, index, key);
		}

		internal static DataPropertyDictionaryChangeOperation<TKey, TValue> Move(int index, TKey key, int movingIndex, TKey movingKey)
		{
			return new DataPropertyDictionaryChangeOperation<TKey, TValue>(DataPropertyCollectionChangeAction.Move, index, key, movingIndex, movingKey);
		}

		internal static DataPropertyDictionaryChangeOperation<TKey, TValue> Swap(int index, TKey key, int movingIndex, TKey movingKey)
		{
			return new DataPropertyDictionaryChangeOperation<TKey, TValue>(DataPropertyCollectionChangeAction.Swap, index, key, movingIndex, movingKey);
		}
		#endregion // internal members

		#region private members
		private int index;
		private int movingIndex;

		private DataPropertyDictionaryChangeOperation(DataPropertyCollectionChangeAction action, int index, TKey key, DataPropertyValueContext<TValue> settingValue = null)
			: this(action, index, key, -1, default(TKey), settingValue)
		{
			SettingValue = settingValue;
		}

		private DataPropertyDictionaryChangeOperation(DataPropertyCollectionChangeAction action, int index, TKey key, int movingIndex, TKey movingKey, DataPropertyValueContext<TValue> settingValue = null)
		{
			this.index = index;
			this.movingIndex = movingIndex;

			Action = action;
			Key = key;
			MovingKey = movingKey;
			SettingValue = settingValue;
		}
		#endregion // private members
	}
}
