using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static readonly double TickToSeconds = 1.0 / Stopwatch.Frequency;
        private long lastTimestamp;

        private readonly List<Action> actionsBuffer = new List<Action>(8);
        private readonly List<Loopable> newLoopsBuffer = new List<Loopable>(8);
        public CoreLoop(int type)
        {
            comparer = new InnerComparer(type); 
            loopType = type;
            session = new LoopSession();
            lastTimestamp = Stopwatch.GetTimestamp();
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
            var now = Stopwatch.GetTimestamp();
            var deltaTime = (float)((now - lastTimestamp) * TickToSeconds);
            for (var i = 0; i < loopables.Count; i++)
            {
                if (session.Destroyed)
                    return;

                var current = loopables[i];
                if (current.CallActions)
                    current.GetAction(loopType)?.Invoke(deltaTime);
            }
            lastTimestamp = now;
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

            lock (session.SyncRoot)
            {
                if (session.ActionsCount is 0)
                    return;

                actionsBuffer.Clear();
                actionsBuffer.AddRange(session.Actions);
                session.ActionsCount = 0;
                session.Actions.Clear();
            }

            for (var i = 0; i < actionsBuffer.Count; i++)
            {
                if (session.Destroyed)
                    return;
                actionsBuffer[i]?.Invoke();
            }

            actionsBuffer.Clear();
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
                newLoopsBuffer.Clear();

                foreach (var loopable in session.ForAdd)
                {
                    session.Process.Add(loopable);

                    if (loopable.CallActions && loopable.CallWhenAdded)
                        newLoopsBuffer.Add(loopable);
                    loopable.SetOrder(loopType, behaviourOrder++);
                }

                session.ForAdd.Clear();
                return newLoopsBuffer;
            }

            return null;
        }

        private int GetIndex(Loopable loopable) => session.Process.BinarySearch(loopable, comparer);
    }
}