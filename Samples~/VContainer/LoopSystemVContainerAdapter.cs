using System;
using Core.LoopSystem;
using VContainer.Unity;

namespace BGCore.Samples.VContainer
{
    public sealed class LoopSystemVContainerAdapter : IStartable, ITickable, IFixedTickable, ILateTickable, IDisposable
    {
        public void Start()
        {
            Loops.Initiate();
        }

        public void Tick()
        {
            CoreLoopService.Execute(Loops.Timer);
            CoreLoopService.Execute(Loops.Update);
        }

        public void FixedTick()
        {
            CoreLoopService.Execute(Loops.FixedUpdate);
        }

        public void LateTick()
        {
            CoreLoopService.Execute(Loops.LateUpdate);
        }

        public void Dispose()
        {
            Loops.Dispose();
        }
    }
}
