using UnityEngine;

using Logger = YeKostenko.CoreKit.Logging.Logger;

namespace YeKostenko.CoreKit.App
{
    public abstract class BaseApp : MonoBehaviour
    {
        private static BaseApp _instance;
        public static BaseApp Instance => _instance;

        private bool _isInitialized = false;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            InternalAppCreate();
        }

        private void InternalAppCreate()
        {
            Logger.Log("[App] OnAppCreate");

            OnAppCreate();
            _isInitialized = true;

            OnAppStart();
        }

        private void Update()
        {
            if (!_isInitialized) return;

            OnAppUpdate();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!_isInitialized) return;

            OnAppFocus(hasFocus);
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (!_isInitialized) return;

            OnAppPause(pauseStatus);
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                Logger.Log("[App] OnAppDestroy");
                OnAppDestroy();
            }
        }
        protected abstract void OnAppCreate();
        protected abstract void OnAppDestroy();

        protected virtual void OnAppStart() { }

        protected virtual void OnAppUpdate() { }

        protected virtual void OnAppFocus(bool isFocused) { }
        protected virtual void OnAppPause(bool pauseStatus) { }

        protected virtual void OnAppBackButton() { }
    }
}