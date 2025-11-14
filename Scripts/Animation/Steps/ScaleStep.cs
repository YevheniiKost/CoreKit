using System;
using UnityEngine;

namespace YevheniiKostenko.CoreKit.Animation
{
    [Serializable]
    public class ScaleStep : AnimStep
    {
        [SerializeField]
        private Vector3 _to = Vector3.one;
        
        [SerializeField]
        private bool _overrideStartScale = false;
        [SerializeField]
        private Vector3 _startScale = Vector3.one;

        [SerializeField]
        private bool _relative = false;

        private Vector3 _from;

        public Vector3 To => _to;
        public bool Relative => _relative;

        public override string DisplayName => "Scale";

        public override void CaptureInitial(GameObject target)
        {
            _from = _overrideStartScale ? _startScale : target.transform.localScale;
        }

        public override void Apply(GameObject target, float eased01)
        {
            Vector3 end = _relative ? _from + _to : _to;
            target.transform.localScale = Vector3.LerpUnclamped(_from, end, Mathf.Clamp01(eased01));
        }
        
        public override AnimStep CloneStep()
        {
            ScaleStep clone = new ScaleStep();
            clone._to = _to;
            clone._relative = _relative;
            CopyBaseTo(clone);
            return clone;
        }
    }
}