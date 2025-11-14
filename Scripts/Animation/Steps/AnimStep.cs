using System;
using UnityEngine;

namespace YevheniiKostenko.CoreKit.Animation
{
    [Serializable]
    public abstract class AnimStep
    {
        [SerializeField]
        [Min(0f)]
        private float _duration = 0.3f;

        [SerializeField]
        [Min(0f)]
        private float _delay = 0f;

        [SerializeField]
        private AnimEase _ease = AnimEase.Linear;

        [SerializeField]
        private bool _useCurve = false;

        [SerializeField]
        private AnimationCurve _easeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public float Duration => _duration;
        public float Delay => _delay;
        public AnimEase Ease => _ease;
        public bool UseCurve => _useCurve;
        public AnimationCurve EaseCurve => _easeCurve;

        public abstract string DisplayName { get; }

        public abstract void CaptureInitial(GameObject target);

        public abstract void Apply(GameObject target, float eased01);

        public virtual void Complete(GameObject target)
        {
            Apply(target, 1f);
        }

        public abstract AnimStep CloneStep();

        protected void CopyBaseTo(AnimStep dst)
        {
            if (dst == null)
            {
                return;
            }
            dst._duration = _duration;
            dst._delay = _delay;
            dst._ease = _ease;
            dst._useCurve = _useCurve;
            if (_easeCurve != null)
            {
                dst._easeCurve = new AnimationCurve(_easeCurve.keys);
            }
            else
            {
                dst._easeCurve = null;
            }
        }
    }
}