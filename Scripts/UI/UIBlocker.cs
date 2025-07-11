using UnityEngine;

namespace YeKostenko.CoreKit.UI
{
    public static class UIBlocker
    {
        private static GameObject _prefab;
        private static GameObject _instance;

        public static void ShowAbove(Transform parent)
        {
            if (_prefab == null)
                _prefab = Resources.Load<GameObject>("UI/Blocker");
            _instance = Object.Instantiate(_prefab, parent.parent);
            _instance.transform.SetSiblingIndex(parent.GetSiblingIndex());
        }

        public static void Hide()
        {
            if (_instance != null)
                Object.Destroy(_instance);
        }
    }
}