using System;
using Core.Entities.Loopables;
using UnityEngine;

namespace Core.Timers
{
    public class Timer : Loopable, ITimer
    {
        public event Action<TimerArgs> OnTimerTick;
        public float Period { get; private set; }

        public bool IsPlaying { get; private set; }

        private readonly int loop;
        private readonly bool invokeOnce;
        private readonly bool playOnAwake;
        
        private float value;

        private Action<ITimer> onReachedPeriod;

        public Timer(int loopType, float period, Action<ITimer> onReachedPeriodAction, bool playOnAwake, bool once = false) 
        {
            loop = loopType;
            Period = period;
            onReachedPeriod = onReachedPeriodAction;
            invokeOnce = once;

            if (!playOnAwake) return;
            SetAlive();
            Play();
        }

        public void Play()
        {
            if(!IsAlive) SetAlive();
            IsPlaying = true;
        }

        public void Pause()
        {
            IsPlaying = false;
        }

        public void Stop(bool withTickReached = false)
        {
            Pause();
            
            if (withTickReached)
            {
                onReachedPeriod?.Invoke(this);
            }
            
            value = 0;
        }

        public void AddHandler(Action<ITimer> onReachedPeriodAction)
        {
            onReachedPeriod += onReachedPeriodAction;
        }

        public void RemoveHandler(Action<ITimer> onReachedPeriodAction)
        {
            onReachedPeriod -= onReachedPeriodAction;
        }

        public void SetHandler(Action<ITimer> onReachedPeriodAction)
        {
            onReachedPeriod = onReachedPeriodAction;
        }
        
        public void SetPeriod(float period)
        {
            Period = period;
        }

        public void AddPeriod(float step)
        {
            Period += step;
        }

        protected override void OnAlive()
        {
            base.OnAlive();
            LoopOn(loop, Execute, playOnAwake);
        }

        private void Execute(float deltaTime)
        {
            if (!IsPlaying)
            {
                return;
            }
            
            if (value > Period)
            {
                value = 0;
                onReachedPeriod?.Invoke(this);
                if (invokeOnce)
                {
                    Drop();
                }
                return;
            }

            value += deltaTime;
            OnTimerTick?.Invoke(new TimerArgs(this, value, Period, deltaTime));
        }

        protected override void OnDrop()
        {
            OnTimerTick = null;
            onReachedPeriod = null;
            
            base.OnDrop();
        }
    }

}