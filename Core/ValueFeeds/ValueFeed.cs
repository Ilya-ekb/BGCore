using System;

namespace BGCore.Samples.ValueFeeds
{
    public sealed class ValueFeed<T> : IDisposable
    {
        private ValueRelay<T> source;

        public ValueFeed(ValueRelay<T> source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public T Value
        {
            get
            {
                if (source == null)
                    throw new ObjectDisposedException(GetType().Name);

                return source.Value;
            }
        }

        public IDisposable Subscribe(Action<T> handler, bool invokeNow = true)
        {
            return source.Subscribe(handler, invokeNow);
        }

        public void Unsubscribe(Action<T> handler)
        {
            source.Unsubscribe(handler);
        }

        public void Dispose()
        {
            if (source == null)
                return;

            source.Dispose();
            source = null;
        }
    }
}
