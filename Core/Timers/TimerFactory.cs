using System;
using System.Collections.Generic;
using Core.ObjectsSystem;

namespace Core.Timers
{
    public static class TimerFactory
    {
        private static readonly List<ITimer> allTimers = new List<ITimer>();
        public static ITimer CreateTimer(int updateType, float period, Action<ITimer> onReachedPeriodAction,
            bool playOnAwake = true, bool invokeOnce = false)
        {
            var timer = new Timer(updateType, period, onReachedPeriodAction, playOnAwake, invokeOnce);

            timer.Dropped += OnTimerDropped;

            allTimers.Add(timer);

            return timer;
        }

        public static void StopAll()
        {
            for (var i = 0; i < allTimers.Count; i++)
                allTimers[i].Stop();
            allTimers.Clear();
        }

        public static void PlayALl()
        {
            for (var i = 0; i < allTimers.Count; i++)
                allTimers[i].Play();
        }

        public static void PauseAll()
        {
            for (var i = 0; i < allTimers.Count; i++)
                allTimers[i].Pause();
        }

        private static void OnTimerDropped(IDroppable droppable)
        {
            if (droppable is ITimer timer)
                allTimers.Remove(timer);
        }
    }
}
