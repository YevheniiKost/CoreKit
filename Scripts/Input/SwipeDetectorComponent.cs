using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace YeKostenko.CoreKit.Input
{
    public class SwipeDetectorComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private const float MinSwipeDistance = 50f; 

        public event Action<SwipeDirection> OnSwipe;

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
                SwipeDirection dir = DetermineDirection(delta);
                OnSwipe?.Invoke(dir);
                _active = false; 
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _active = false;
        }

        SwipeDirection DetermineDirection(Vector2 delta)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                return delta.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
            }
            else
            {
                return delta.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
            }
        }
    }
}