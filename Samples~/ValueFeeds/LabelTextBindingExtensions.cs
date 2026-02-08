using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace BGCore.Samples.ValueFeeds
{
    public static class LabelTextBindingExtensions
    {
        private static readonly List<(WeakReference<Label> labelRef, IDisposable unsubscriber)> LabelSubscriptions = new();

        public static IDisposable SetTextSource(this Label label, ValueFeed<string> source)
        {
            if (label == null)
                return ActionDisposable.Empty;

            UnsubscribeLabel(label);

            if (source == null)
            {
                label.text = null;
                return ActionDisposable.Empty;
            }

            void Handler(string value)
            {
                if (label != null)
                {
                    label.text = value;
                    return;
                }

                UnsubscribeLabel(label);
            }

            source.Subscribe(Handler);

            var unsubscriber = new ActionDisposable(() => source.Unsubscribe(Handler));
            SubscribeLabel(label, unsubscriber);
            return unsubscriber;
        }

        private static void SubscribeLabel(Label label, IDisposable unsubscriber)
        {
            LabelSubscriptions.Add((new WeakReference<Label>(label), unsubscriber));
        }

        private static void UnsubscribeLabel(Label label)
        {
            for (var i = 0; i < LabelSubscriptions.Count; ++i)
            {
                var subscription = LabelSubscriptions[i];
                if (subscription.labelRef.TryGetTarget(out var target))
                {
                    if (target == label)
                    {
                        RemoveLabelAt(i);
                        return;
                    }
                }
                else
                {
                    RemoveLabelAt(i);
                    --i;
                }
            }
        }

        private static void RemoveLabelAt(int index)
        {
            LabelSubscriptions[index].unsubscriber.Dispose();
            if (index + 1 < LabelSubscriptions.Count)
                LabelSubscriptions[index] = LabelSubscriptions[LabelSubscriptions.Count - 1];
            LabelSubscriptions.RemoveAt(LabelSubscriptions.Count - 1);
        }
    }
}
