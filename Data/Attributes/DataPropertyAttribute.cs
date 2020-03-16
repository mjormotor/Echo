using Echo.PWalkService;

namespace Echo.Data
{
	/// <summary>
	/// データプロパティ探索路属性
	/// </summary>
	public class DataPropertyAttribute : PWalkPathAttribute
	{
		public DataPropertyAttribute()
			: base (typeof(IDataProperty))
		{
		}
	}
}
