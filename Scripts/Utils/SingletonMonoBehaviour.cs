using UnityEngine;

namespace YevheniiKostenko.CoreKit.Utils
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private static bool _applicationIsQuitting;

        /// <summary>
        /// Gets the singleton instance. If none exists in the scene, a new GameObject with the component is created.
        /// Returns null if the application is quitting.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting) return null;

                if (_instance != null) return _instance;

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>();
                        if (_instance == null)
                        {
                            var go = new GameObject(typeof(T).Name);
                            _instance = go.AddComponent<T>();
                        }
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Override to make the singleton persist between scene loads.
        /// </summary>
        protected virtual bool PersistBetweenScenes => false;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = (T)this;
                if (PersistBetweenScenes)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }

            _applicationIsQuitting = false;
        }

        protected virtual void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}