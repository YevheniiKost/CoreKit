using System;
using UnityEngine;

namespace YevheniiKostenko.CoreKit.Animation
{
    [CreateAssetMenu(menuName = "Tools/CoreKit/Animation/Set")]
    public class AnimSet : ScriptableObject
    {
        [SerializeField]
        private AnimSequence _show;

        [SerializeField]
        private AnimSequence _hide;

        [SerializeField]
        private AnimSequence _idle;

        [Serializable]
        public class Named
        {
            public string Key;
            public AnimSequence Sequence;
        }

        [SerializeField]
        private Named[] _extra;

        public AnimSequence Show => _show;
        public AnimSequence Hide => _hide;
        public AnimSequence Idle => _idle;

        public AnimSequence Get(string key)
        {
            if (string.Equals(key, "Show", StringComparison.OrdinalIgnoreCase))
            {
                return _show;
            }
            if (string.Equals(key, "Hide", StringComparison.OrdinalIgnoreCase))
            {
                return _hide;
            }
            if (_extra != null)
            {
                foreach (Named n in _extra)
                {
                    if (n.Key == key)
                    {
                        return n.Sequence;
                    }
                }
            }
            return null;
        }
    }
}