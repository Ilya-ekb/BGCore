using Core.ObjectsSystem;
using Game.Characters.View;
using Game.Settings;
using GameData;
using UnityEngine;

namespace GameLogic.Views
{
    public abstract class BaseLocationObjectView<TSetting, TObject> : BaseDroppable
        where TSetting : ViewSetting
        where TObject : Component
    {
        protected virtual Vector3 Position
        {
            get => Root.transform.position;
            set => Root.transform.position = value;
        }

        protected virtual Quaternion Rotation
        {
            get => Root.transform.rotation;
            set => Root.transform.rotation = value;
        }
        
        public TObject Root { get; set; }
        
        protected readonly TSetting setting;
        protected readonly TObject resource;
        protected readonly IContext context;
        protected BaseLocationObjectView(TSetting setting, IContext context)
        {
            this.context = context;
            resource = Resources.Load<TObject>(setting.rootObjectPath);
            if(!resource)
                Debug.LogError($"<COLOR=YELLOW>{typeof(TObject).Name}</COLOR> is not loaded from {setting.rootObjectPath}");
            this.setting = setting;
        }

        protected override void OnAlive()
        {
            base.OnAlive();
            CreateView(location?.Root.transform);
        }
        
        protected override void OnDrop()
        {
            base.OnDrop();
            if(Root)
                Object.DestroyImmediate(Root.gameObject);
            Root = null;
        }
        
        protected void CreateView(Transform parent)
        {
            Root = Object.Instantiate(resource, parent);
            Root.name = $"[{GetType().Name}] {resource.name}";
        }
    }
}