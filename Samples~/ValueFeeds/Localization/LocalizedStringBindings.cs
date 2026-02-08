using System;
using BGCore.ValueFeeds;
using Core.ObjectsSystem;
using UnityEngine.Localization;

namespace BGCore.Samples.ValueFeeds
{
    public static class LocalizedStringBindings
    {
        public static IDisposable WatchText(this LocalizedString localized, Action<string> onText)
        {
            if (localized == null || onText == null)
                return ActionDisposable.Empty;

            void Handler(string value) => onText(value);
            localized.StringChanged += Handler;
            return new ActionDisposable(() => localized.StringChanged -= Handler);
        }

        public static ValueFeed<string> ToTextFeed(this LocalizedString localized)
        {
            return BuildFeed(localized);
        }

        public static ValueFeed<string> ToTextFeed(this LocalizedString localized, IDroppable lifetime)
        {
            var feed = BuildFeed(localized);
            if (lifetime != null)
                lifetime.Dropped += _ => feed.Dispose();
            return feed;
        }

        private static ValueFeed<string> BuildFeed(LocalizedString localized)
        {
            var relay = new ValueRelay<string>();

            if (localized != null && !localized.IsEmpty)
            {
                void Handler(string value) => relay.SetValue(value);
                localized.StringChanged += Handler;
                relay.RegisterDispose(() => localized.StringChanged -= Handler);
            }

            return relay.Feed;
        }
    }
}
