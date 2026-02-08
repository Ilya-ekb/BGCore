using System;

namespace BGCore.Samples.ValueFeeds
{
    public sealed class ActionDisposable : IDisposable
    {
        public static readonly ActionDisposable Empty = new ActionDisposable(null);

        private Action onDispose;

        public ActionDisposable(Action onDispose)
        {
            this.onDispose = onDispose;
        }

        public void Dispose()
        {
            if (onDispose == null)
                return;

            var action = onDispose;
            onDispose = null;
            action.Invoke();
        }
    }
}
