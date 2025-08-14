namespace Core.Timers
{
    public struct TimerArgs
    {
        public ITimer Timer { get; }
        public float Period { get; }
        public float Value { get; }
        public float DeltaTime { get; }

        public TimerArgs(ITimer timer, float value, float period, float deltaTime)
        {
            Timer = timer;
            Period = period;
            Value = value;
            DeltaTime = deltaTime;
        }

        public override string ToString()
        {
            return $"Value: {Value}, Period: {Period}, DeltaTime: {DeltaTime}";
        }
    }
}