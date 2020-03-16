using System;

namespace Echo
{
	public class ScopeWatcher
	{
		public int ScopeCount => this.scopeCount;

		public bool IsInScope => ScopeCount > 0;

		public IDisposable Scope()
		{
			var ret = new ScopeMonitor(this);
			return ret;
		}

		#region private members
		#region class ScopeMonitor
		private class ScopeMonitor : IDisposable
		{
			public ScopeMonitor(ScopeWatcher watcher)
			{
				++watcher.scopeCount;
				this.dispose = () =>
				{
					--watcher.scopeCount;
					this.dispose = null;
				};
			}

			#region IDisposable interface support
			public void Dispose()
			{
				this.dispose?.Invoke();
			}
			#endregion  // IDisposable interface support

			#region private members
			private Action dispose;
			#endregion // private members
		}
		#endregion // class ScopeMonitor

		private volatile int scopeCount;
		#endregion // private members
	}
}
