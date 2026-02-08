using System;
using System.Collections.Generic;
using TMPro;
using BGCore.ValueFeeds;

namespace BGCore.Samples.ValueFeeds
{
    public static class TMPTextBindingExtensions
    {
        private static readonly List<(WeakReference<TMP_Text> textRef, IDisposable unsubscriber)> TextSubscriptions = new();

        public static IDisposable SetTextSource(this TMP_Text text, ValueFeed<string> source)
        {
            if (text == null)
                return ActionDisposable.Empty;

            UnsubscribeText(text);

            if (source == null)
            {
                text.text = null;
                return ActionDisposable.Empty;
            }

            void Handler(string value)
            {
                if (text != null)
                {
                    text.text = value;
                    return;
                }

                UnsubscribeText(text);
            }

            source.Subscribe(Handler);

            var unsubscriber = new ActionDisposable(() => source.Unsubscribe(Handler));
            SubscribeText(text, unsubscriber);
            return unsubscriber;
        }

        private static void SubscribeText(TMP_Text text, IDisposable unsubscriber)
        {
            TextSubscriptions.Add((new WeakReference<TMP_Text>(text), unsubscriber));
        }

        private static void UnsubscribeText(TMP_Text text)
        {
            for (var i = 0; i < TextSubscriptions.Count; ++i)
            {
                var subscription = TextSubscriptions[i];
                if (subscription.textRef.TryGetTarget(out var target))
                {
                    if (target == text)
                    {
                        RemoveTextAt(i);
                        return;
                    }
                }
                else
                {
                    RemoveTextAt(i);
                    --i;
                }
            }
        }

        private static void RemoveTextAt(int index)
        {
            TextSubscriptions[index].unsubscriber.Dispose();
            if (index + 1 < TextSubscriptions.Count)
                TextSubscriptions[index] = TextSubscriptions[TextSubscriptions.Count - 1];
            TextSubscriptions.RemoveAt(TextSubscriptions.Count - 1);
        }

    }
}
