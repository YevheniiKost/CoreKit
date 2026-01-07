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
        private Vector3 _fromEuler;

        public Vector3 ToEuler => _toEuler;
        public bool Relative => _relative;

        public override string DisplayName => "Rotate";

        public override void CaptureInitial(GameObject target)
        {
            _from = target.transform.localRotation;
            _fromEuler = _from.eulerAngles;
        }

        public override void Apply(GameObject target, float eased01)
        {
            float t = Mathf.Clamp01(eased01);

            Vector3 targetEuler = _relative ? _fromEuler + _toEuler : _toEuler;

            // Use LerpAngle per axis so interpolation wraps correctly (e.g. 350 -> 10 degrees)
            float x = Mathf.Lerp(_fromEuler.x, targetEuler.x, t);
            float y = Mathf.Lerp(_fromEuler.y, targetEuler.y, t);
            float z = Mathf.Lerp(_fromEuler.z, targetEuler.z, t);

            target.transform.localRotation = Quaternion.Euler(new Vector3(x, y, z));
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