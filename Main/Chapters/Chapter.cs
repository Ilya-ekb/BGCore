using System;
using Core.Main.Locations;
using UnityEngine;

namespace Core.Main.Chapters
{
    [Serializable]
    public class Chapter
    {
        public LocationSetting StaticLocationSetting => statLocationSetting;
        public LocationSetting DynamicLocationSetting => dynamicLocationSetting;

        [SerializeField, HideInInspector] private string chapterName;
        [SerializeField] private LocationSetting statLocationSetting;
        [SerializeField] private LocationSetting dynamicLocationSetting;

        public void OnValidate()
        {
            var none = "NONE";
            chapterName = $"{(statLocationSetting ? statLocationSetting.SceneName : none)}~" +
                          $"{(dynamicLocationSetting ? dynamicLocationSetting.SceneName : none)}";
        }
    }
}