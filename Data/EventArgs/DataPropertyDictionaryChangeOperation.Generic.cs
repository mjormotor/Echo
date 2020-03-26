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
		/// 設定値
		/// </summary>
		public DataPropertyDictionaryValueContext<TKey, TValue> SettingValue { get; }

		#region IReadOnlyDataPropertyDictionaryChangeOperation<TKey, TValue> interface support
		IReadOnlyDataPropertyDictionaryValueContext<TKey, TValue> IReadOnlyDataPropertyDictionaryChangeOperation<TKey, TValue>.SettingValue => SettingValue;
		#endregion  // IReadOnlyDataPropertyDictionaryChangeOperation<TKey, TValue> interface support

		#region internal members
		internal static DataPropertyDictionaryChangeOperation<TKey, TValue> Insert(int index, TKey key, DataProperty<TValue> property)
		{
			var settingValue = new DataPropertyDictionaryValueContext<TKey, TValue>(key, property);
			return new DataPropertyDictionaryChangeOperation<TKey, TValue>(DataPropertyCollectionChangeAction.Insert, index, settingValue);
		}

		internal static DataPropertyDictionaryChangeOperation<TKey, TValue> Remove(int index)
		{
			return new DataPropertyDictionaryChangeOperation<TKey, TValue>(DataPropertyCollectionChangeAction.Remove, index);
		}

		internal static DataPropertyDictionaryChangeOperation<TKey, TValue> Move(int index, int movingIndex)
		{
			return new DataPropertyDictionaryChangeOperation<TKey, TValue>(DataPropertyCollectionChangeAction.Move, index, movingIndex);
		}

		internal static DataPropertyDictionaryChangeOperation<TKey, TValue> Set(int index, TKey oldKey, TValue oldValue, TKey inputKey, TValue inputValue, DataProperty<TValue> property)
		{
			var settingValue = new DataPropertyDictionaryValueContext<TKey, TValue>(oldKey, oldValue, inputKey, inputValue, property);
			return new DataPropertyDictionaryChangeOperation<TKey, TValue>(DataPropertyCollectionChangeAction.Set, index, settingValue);
		}

		internal static DataPropertyDictionaryChangeOperation<TKey, TValue> Swap(int index, int movingIndex)
		{
			return new DataPropertyDictionaryChangeOperation<TKey, TValue>(DataPropertyCollectionChangeAction.Swap, index, movingIndex);
		}
		#endregion // internal members

		#region private members
		private int index;
		private int movingIndex;

		private DataPropertyDictionaryChangeOperation(DataPropertyCollectionChangeAction action, int index, DataPropertyDictionaryValueContext<TKey, TValue> settingValue = null)
			: this(action, index, -1, settingValue)
		{
			SettingValue = settingValue;
		}

		private DataPropertyDictionaryChangeOperation(DataPropertyCollectionChangeAction action, int index, int movingIndex, DataPropertyDictionaryValueContext<TKey, TValue> settingValue = null)
		{
			this.index = index;
			this.movingIndex = movingIndex;

			Action = action;
			SettingValue = settingValue;
		}
		#endregion // private members
	}
}
