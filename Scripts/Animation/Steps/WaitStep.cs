using System;
using UnityEngine;

namespace YevheniiKostenko.CoreKit.Animation
{
    [Serializable]
    public class WaitStep : AnimStep
    {
        public override string DisplayName => "Wait";

        public override void CaptureInitial(GameObject target)
        {
        }

        public override void Apply(GameObject target, float t01)
        {
            /* no-op */
        }
        
        public override AnimStep CloneStep()
        {
            WaitStep clone = new WaitStep();
            CopyBaseTo(clone);
            return clone;
        }
    }
}