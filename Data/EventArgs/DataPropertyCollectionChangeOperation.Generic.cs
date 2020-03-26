namespace Echo.Data
{
	/// <summary>
	/// データプロパティ列変更操作
	/// </summary>
	public class DataPropertyCollectionChangeOperation<T> : IReadOnlyDataPropertyCollectionChangeOperation<T>
	{
		/// <summary>
		/// 操作の種別
		/// </summary>
		public DataPropertyCollectionChangeAction Action { get; }

		/// <summary>
		/// 位置
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// 移動先の位置
		/// </summary>
		public int MovingIndex { get; set; }

		/// <summary>
		/// 設定値
		/// </summary>
		public DataPropertyCollectionValueContext<T> SettingValue { get; }

		#region IReadOnlyDataPropertyCollectionChangeOperation<T> interface support
		IReadOnlyDataPropertyCollectionValueContext<T> IReadOnlyDataPropertyCollectionChangeOperation<T>.SettingValue => SettingValue;
		#endregion  // IReadOnlyDataPropertyCollectionChangeOperation<T> interface support

		#region internal members
		internal static DataPropertyCollectionChangeOperation<T> Insert(int index, DataProperty<T> property)
		{
			var settingValue = new DataPropertyCollectionValueContext<T>(property);
			return new DataPropertyCollectionChangeOperation<T>(DataPropertyCollectionChangeAction.Insert, index, settingValue);
		}

		internal static DataPropertyCollectionChangeOperation<T> Remove(int index)
		{
			return new DataPropertyCollectionChangeOperation<T>(DataPropertyCollectionChangeAction.Remove, index);
		}

		internal static DataPropertyCollectionChangeOperation<T> Move(int index, int movingIndex)
		{
			return new DataPropertyCollectionChangeOperation<T>(DataPropertyCollectionChangeAction.Move, index, movingIndex);
		}

		internal static DataPropertyCollectionChangeOperation<T> Set(int index, T oldValue, T inputValue, DataProperty<T> property)
		{
			var settingValue = new DataPropertyCollectionValueContext<T>(oldValue, inputValue, property);
			return new DataPropertyCollectionChangeOperation<T>(DataPropertyCollectionChangeAction.Set, index, settingValue);
		}

		internal static DataPropertyCollectionChangeOperation<T> Swap(int index, int movingIndex)
		{
			return new DataPropertyCollectionChangeOperation<T>(DataPropertyCollectionChangeAction.Swap, index, movingIndex);
		}
		#endregion // internal members

		#region private members
		private DataPropertyCollectionChangeOperation(DataPropertyCollectionChangeAction action, int index, DataPropertyCollectionValueContext<T> settingValue = null)
			: this(action, index, -1, settingValue)
		{
			SettingValue = settingValue;
		}

		private DataPropertyCollectionChangeOperation(DataPropertyCollectionChangeAction action, int index, int movingIndex, DataPropertyCollectionValueContext<T> settingValue = null)
		{
			Action = action;
			Index = index;
			MovingIndex = movingIndex;
			SettingValue = settingValue;
		}
		#endregion // private members
	}
}
