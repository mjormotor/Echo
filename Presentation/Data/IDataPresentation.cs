using System.Collections.Generic;

namespace Echo.Presentation
{
	public interface IDataPresentation
	{
		IEnumerable<IDataPropertyPresentation> Items { get; }
	}
}
