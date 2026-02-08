using System;
using System.Collections.Concurrent;
using Core.Entities.Loopables;
using Core.LoopSystem;

namespace Game.Networks
{
    public class ThreadDispatcher : ControlLoopable
    {
        private readonly ConcurrentQueue<Action> events;
        
        public ThreadDispatcher()
        {
            events = new ConcurrentQueue<Action>();
        }

        public void AddEvent(Action action)
        {
            PlayIfNeed();
            events.Enqueue(action);  
        } 
        
        protected override void OnAlive()
        {
            base.OnAlive();
            LoopOn(Loops.Update, OnUpdate);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayIfNeed();
        }

        private void PlayIfNeed()
        {
            if (IsActive && !CallActions)
                Play();
        }

        private void OnUpdate(float deltaTime)
        {
            while (events.TryDequeue(out var action))
                action?.Invoke();
        }
    }
}