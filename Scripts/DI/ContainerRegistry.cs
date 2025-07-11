using System.Collections.Generic;

namespace YeKostenko.CoreKit.DI
{
    public static class ContainerRegistry
    {
        private static readonly List<Container> s_containers = new();

        public static IReadOnlyList<Container> All => s_containers;

        public static void Register(Container container)
        {
            if (!s_containers.Contains(container))
            {
                s_containers.Add(container);
            }
        }

        public static void Unregister(Container container)
        {
            s_containers.Remove(container);
        }

        public static void Clear()
        {
            s_containers.Clear();
        }
    }
}