using System;

namespace Core.ObjectsSystem
{
    public interface IDroppable
    {
        bool IsAlive { get; }
        event Action<IDroppable> Alived; 
        event Action<IDroppable> Dropped;
        void SetAlive();
        void Drop();
    }
}
