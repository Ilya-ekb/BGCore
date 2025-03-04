using System;

namespace Core.ObjectsSystem
{
    public abstract class BaseDroppable : IDroppable
    {
        public string Name { get; }
        public bool IsAlive { get; protected set; }
        public event Action<IDroppable> Alived;
        public event Action<IDroppable> Dropped;

        protected readonly IDroppable parent;

        protected BaseDroppable(IDroppable parent)
        {
            this.parent = parent;
            if (parent != null)
                this.parent.Dropped += _ => Drop();
            Name = GetType().Name;
        }

        public virtual TDroppable GetObject<TDroppable>()
        {
            return this is TDroppable result ? result : default;
        }

        public void SetAlive()
        {
            if (IsAlive)
                return;
            IsAlive = true;
            OnAlive();
        }

        public void Drop()
        {
            if (!IsAlive)
                return;
            IsAlive = false;
            OnDrop();
        }

        protected virtual void OnAlive()
        {
            IsAlive = true;
            Alived?.Invoke(this);
        }

        protected virtual void OnDrop()
        {
            IsAlive = false;
            Dropped?.Invoke(this);
            Alived = null;
            Dropped = null;
        }
    }
}