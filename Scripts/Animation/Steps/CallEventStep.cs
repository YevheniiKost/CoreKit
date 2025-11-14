using System;
using UnityEngine;
using UnityEngine.Events;

namespace YevheniiKostenko.CoreKit.Animation
{
    [Serializable]
    public class CallEventStep : AnimStep
    {
        [SerializeField]
        private UnityEvent _onStep;
        
        public override string DisplayName => "Call Event";

        public override void CaptureInitial(GameObject target)
        {
        }

        public override void Apply(GameObject target, float t01)
        {
            if (t01 >= 1f) _onStep?.Invoke();
        }
        
        public override AnimStep CloneStep()
        {
            CallEventStep clone = new CallEventStep();
            clone._onStep = _onStep;
            CopyBaseTo(clone);
            return clone;
        }
    }
}