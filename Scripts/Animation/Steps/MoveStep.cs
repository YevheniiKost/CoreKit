using System;
using UnityEngine;

namespace YevheniiKostenko.CoreKit.Animation
{
    [Serializable]
    public class MoveStep : AnimStep
    {
        [SerializeField]
        private Vector3 _to;

        [SerializeField]
        private bool _relative = false;

        private Vector3 _from;

        public Vector3 To => _to;
        public bool Relative => _relative;

        public override string DisplayName => "Move";

        public override void CaptureInitial(GameObject target)
        {
            _from = target.transform.localPosition;
        }

        public override void Apply(GameObject target, float eased01)
        {
            Vector3 end = _relative ? _from + _to : _to;
            target.transform.localPosition = Vector3.LerpUnclamped(_from, end, Mathf.Clamp01(eased01));
        }
        
        public override AnimStep CloneStep()
        {
            MoveStep clone = new MoveStep();
            clone._to = _to;
            clone._relative = _relative;
            CopyBaseTo(clone);
            return clone;
        }
    }
}