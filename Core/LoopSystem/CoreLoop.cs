using System;
using System.Collections.Generic;
using Core.Entities.Loopables;

namespace Core.LoopSystem
{
    public class CoreLoop
    {
        private class InnerComparer : IComparer<Loopable>
        {
            private readonly int loopType;

            public InnerComparer(int loopType)
            {
                this.loopType = loopType;
            }

            public int Compare(Loopable x, Loopable y)
            {
                if (x is { } && y is { } && x.GetOrder(loopType) > y.GetOrder(loopType))
                    return 1;

                if (x is { } && y is { } && x.GetOrder(loopType) < y.GetOrder(loopType))
                    return -1;

                return 0;
            }
        }

        private readonly int loopType;
        private readonly InnerComparer comparer;

        private uint behaviourOrder;

        private readonly LoopSession session;
        private long Now => DateTime.Now.Ticks;
        private float DeltaTime => (float)(deltaTicks.TotalSeconds - deltaTicks.Seconds);
        
        private TimeSpan deltaTicks => TimeSpan.FromTicks(Now - lastTick);
        private long lastTick;
        public CoreLoop(int type)
        {
            comparer = new InnerComparer(type); 
            loopType = type;
            session = new LoopSession();
            lastTick = Now;
        }

        public void ExecuteAllEvents()
        {
            ProcessSync();
            CallAllLoopables();
        }

        public void Add(Loopable loopable)
        {
            session.ForAdd.Add(loopable);
        }

        public void Remove(Loopable loopable)
        {
            InnerRemove(loopable);
        }

        public void SyncAction(Action method)
        {
            lock (session.SyncRoot)
            {
                session.Actions.Add(method);
                session.ActionsCount++;
            }
        }

        private void CallAllLoopables()
        {
            Modify();
            InnerCall(session.Process, session);

            if (session.Destroyed)
            {
                return;
            }

            for (;;)
            {
                var newLoops = Modify();
                if (newLoops != null)
                {
                    InnerCall(newLoops, session);
                    if (session.Destroyed)
                        return;
                }
                else break;
            }
        }

        private void InnerCall(List<Loopable> loopables, LoopSession session)
        {
            for (var i = 0; i < loopables.Count; i++)
            {
                if (session.Destroyed)
                    return;

                var current = loopables[i];
                if (current.CallActions)
                    current.GetAction(loopType)?.Invoke(DeltaTime);
            }
            lastTick = Now;
        }

        private void InnerRemove(Loopable behaviour)
        {
            var idx = session.ForAdd.IndexOf(behaviour);

            if (idx != -1)
                session.ForAdd.RemoveAt(idx);
            else
                session.ForRemove.Add(behaviour);
        }

        private void ProcessSync()
        {
            if (session.ActionsCount is 0)
                return;

            List<Action> actions;
            lock (session.SyncRoot)
            {
                if (session.ActionsCount is 0)
                    return;

                actions = new List<Action>(session.Actions);
                session.ActionsCount = 0;
                session.Actions.Clear();
            }

            foreach (var action in actions)
            {
                if (session.Destroyed)
                    return;
                action();
            }
        }


        private List<Loopable> Modify()
        {
            if (session.ForRemove.Count > 0)
            {
                foreach (var loopable in session.ForRemove)
                {
                    var index = GetIndex(loopable);
                    if (index >= 0)
                        session.Process.RemoveAt(index);
                }

                session.ForRemove.Clear();
            }

            if (session.ForAdd.Count > 0)
            {
                var newLoops = new List<Loopable>();

                foreach (var loopable in session.ForAdd)
                {
                    session.Process.Add(loopable);

                    if (loopable.CallActions && loopable.CallWhenAdded)
                        newLoops.Add(loopable);
                    loopable.SetOrder(loopType, behaviourOrder++);
                }

                session.ForAdd.Clear();
                return newLoops;
            }

            return null;
        }

        private int GetIndex(Loopable loopable) => session.Process.BinarySearch(loopable, comparer);
    }
}