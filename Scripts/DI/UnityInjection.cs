﻿using System;
using System.Linq;
using System.Reflection;

using UnityEngine;

using Object = UnityEngine.Object;

namespace YeKostenko.CoreKit.DI
{
    public static class UnityInjection
    {
        public static T InstantiateWithInjection<T>(this Container container, GameObject prefab,
            Transform parent = null) where T : MonoBehaviour
        {
            GameObject instance = Object.Instantiate(prefab, parent);
            T component = instance.GetComponent<T>();
            if (component == null)
                throw new InvalidOperationException($"Component of type {typeof(T)} not found on prefab {prefab.name}");

            container.InjectInto(component);
            return component;
        }

        public static void InjectInto(this Container container, MonoBehaviour mono)
        {
            MethodInfo injectMethod = mono.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(m =>
                    m.GetCustomAttribute<InjectAttribute>() != null &&
                    m.Name == "Construct");

            if (injectMethod == null) return;

            object[] parameters = injectMethod.GetParameters()
                .Select(p => container.Resolve(p.ParameterType))
                .ToArray();

            injectMethod.Invoke(mono, parameters);
        }

        public static void InjectIntoAllSceneMonos(this Container container)
        {
            foreach (MonoBehaviour mono in Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
            {
                container.InjectInto(mono);
            }
        }
    }
}