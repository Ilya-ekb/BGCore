using System;
using Core.ObjectsSystem;

namespace Core.Timers
{
    public interface ITimer : IDroppable
    {
        event Action<TimerArgs> OnTimerTick;
        float Period { get; }
        bool IsPlaying { get; }
        void Play();
        void Pause();
        void Stop(bool withTickReached = false);

        void AddHandler(Action<ITimer> onTickAction);
        void RemoveHandler(Action<ITimer> onTickAction);
        void SetHandler(Action<ITimer> onTickAction);
        void SetPeriod(float period);
        void AddPeriod(float step);
    }
}