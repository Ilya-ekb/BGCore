using System;
using Core;

namespace BGCore.ValueFeeds
{
    public sealed class ValueRelay<T> : IDisposable
    {
        private readonly ROProperty<T> property;
        private bool isDisposed;
        private Action onDispose;

        public ValueRelay()
        {
            property = new ROProperty<T>();
            Feed = new ValueFeed<T>(this);
        }

        public ValueRelay(T value) : this()
        {
            property.SetValue(value);
        }

        public T Value
        {
            get => property.RValue;
            set
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().Name);

                property.SetValue(value);
            }
        }

        public ValueFeed<T> Feed { get; }

        public void SetValue(T value)
        {
            Value = value;
        }

        public IDisposable Subscribe(Action<T> handler, bool invokeNow = true)
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().Name);

            if (handler == null)
                return ActionDisposable.Empty;

            if (invokeNow)
                handler(property.RValue);

            property.Subscribe(handler);
            return new ActionDisposable(() => property.Unsubscribe(handler));
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            isDisposed = true;
            property.UnsubscribeAll();
            onDispose?.Invoke();
            onDispose = null;
        }

        public void Unsubscribe(Action<T> handler)
        {
            if (handler == null)
                return;

            property.Unsubscribe(handler);
        }

        public void RegisterDispose(Action action)
        {
            if (action == null)
                return;

            if (isDisposed)
            {
                action.Invoke();
                return;
            }

            onDispose += action;
        }
    }
}
