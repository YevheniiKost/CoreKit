using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace YeKostenko.CoreKit.Input
{
    public class SwipeMultiDirectionalComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private const float MinSwipeDistance = 50f;

        public event Action<float> OnSwipe; // Angle in degrees

        private Vector2 _startPos;
        private bool _active;

        public void OnPointerDown(PointerEventData eventData)
        {
            _startPos = eventData.position;
            _active = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_active) return;
            Vector2 delta = eventData.position - _startPos;
            if (delta.magnitude >= MinSwipeDistance)
            {
                float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
                OnSwipe?.Invoke(angle);
                _active = false;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _active = false;
        }
    }
}