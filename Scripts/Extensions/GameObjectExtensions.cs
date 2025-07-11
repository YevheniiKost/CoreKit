using UnityEngine;

namespace YeKostenko.CoreKit.Extensions
{
    public static class GameObjectExtensions
    {
        public static T GetMissingComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }

        public static void SetActive(this GameObject[] gameObjects, bool value)
        {
            foreach (var gameObject in gameObjects)
            {
                if (gameObject != null)
                {
                    gameObject.SetActive(value);
                }
            }
        }
        
        public static void SetEnabled(this Behaviour[] behaviours, bool value)
        {
            foreach (var behaviour in behaviours)
            {
                if (behaviour != null)
                {
                    behaviour.enabled = value;
                }
            }
        }
    }
}