using System;
using UnityEngine;

namespace YevheniiKostenko.CoreKit.Animation
{
    [Serializable]
    public class RotateStep : AnimStep
    {
        [SerializeField]
        private Vector3 _toEuler;

        [SerializeField]
        private bool _relative = false;

        private Quaternion _from;

        public Vector3 ToEuler => _toEuler;
        public bool Relative => _relative;

        public override string DisplayName => "Rotate";

        public override void CaptureInitial(GameObject target)
        {
            _from = target.transform.localRotation;
        }

        public override void Apply(GameObject target, float eased01)
        {
            Quaternion end = _relative ? _from * Quaternion.Euler(_toEuler) : Quaternion.Euler(_toEuler);
            target.transform.localRotation = Quaternion.SlerpUnclamped(_from, end, Mathf.Clamp01(eased01));
        }
        
        public override AnimStep CloneStep()
        {
            RotateStep clone = new RotateStep();
            clone._toEuler = _toEuler;
            clone._relative = _relative;
            CopyBaseTo(clone);
            return clone;
        }
    }
}