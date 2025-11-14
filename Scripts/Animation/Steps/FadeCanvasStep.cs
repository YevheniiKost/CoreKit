using System;
using UnityEngine;

namespace YevheniiKostenko.CoreKit.Animation
{
    [Serializable]
    public class FadeCanvasStep : AnimStep
    {
        [SerializeField]
        [Range(0f, 1f)]
        private float _to = 1f;

        private float _from;
        private CanvasGroup _cg;

        public float To => _to;

        public override string DisplayName => "Fade (CanvasGroup)";

        public override void CaptureInitial(GameObject target)
        {
            _cg = target.GetComponent<CanvasGroup>();
            if (_cg == null)
            {
                _cg = target.AddComponent<CanvasGroup>();
            }
            _from = _cg.alpha;
        }

        public override void Apply(GameObject target, float eased01)
        {
            if (_cg == null)
            {
                return;
            }
            _cg.alpha = Mathf.LerpUnclamped(_from, _to, Mathf.Clamp01(eased01));
            _cg.blocksRaycasts = _cg.alpha > 0.99f;
            _cg.interactable = _cg.blocksRaycasts;
        }
        
        public override AnimStep CloneStep()
        {
            FadeCanvasStep clone = new FadeCanvasStep();
            clone._to = _to;
            CopyBaseTo(clone);
            return clone;
        }
    }
}