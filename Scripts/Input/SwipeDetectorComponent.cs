using System;

using UnityEngine;
using UnityEngine.EventSystems;

namespace YeKostenko.CoreKit.Input
{
    public class SwipeDetectorComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private const float MinSwipeDistance = 50f;
        private bool _active;

        private Vector2 _startPos;

        public event Action<SwipeDirection> OnSwipe;

        public void OnDrag(PointerEventData eventData)
        {
            if (!_active)
            {
                return;
            }

            Vector2 delta = eventData.position - _startPos;
            if (delta.magnitude >= MinSwipeDistance)
            {
                SwipeDirection dir = DetermineDirection(delta);
                OnSwipe?.Invoke(dir);
                _active = false;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _startPos = eventData.position;
            _active = true;
        }

        public void OnPointerUp(PointerEventData eventData) => _active = false;

        private SwipeDirection DetermineDirection(Vector2 delta)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                return delta.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
            }

            return delta.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
        }
    }
}