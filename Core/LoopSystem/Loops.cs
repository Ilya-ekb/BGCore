namespace Core.LoopSystem
{
    public static class Loops
    {
        public static int Timer { get; private set; }
        public static int FixedUpdate { get; private set; }
        public static int Update { get; private set; }
        public static int LateUpdate { get; private set; }
        public static int Gizmos { get; private set; }
        public static int GizmosSelected { get; private set; }

        public static void Initiate()
        {
            Timer = CoreLoopService.AddNewLoop();
            FixedUpdate = CoreLoopService.AddNewLoop();
            Update = CoreLoopService.AddNewLoop();
            LateUpdate = CoreLoopService.AddNewLoop();
            Gizmos = CoreLoopService.AddNewLoop();
            GizmosSelected = CoreLoopService.AddNewLoop();
        }

        public static void Dispose()
        {
            CoreLoopService.ClearLoops();
            GEvent.ClearActions();
        }
    }
}