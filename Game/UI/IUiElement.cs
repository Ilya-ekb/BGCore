﻿using Core.ObjectsSystem;
using UnityEngine;

namespace Game.UI
{
    public interface IUiElement : IDroppable
    {
        Component RootComponent { get; }
        Transform  ContentHolder { get; }
        bool IsShown { get; }
        void Show();
        void Hide();
        void Update<TUiAgs>(object sender, TUiAgs ags);
    }
}