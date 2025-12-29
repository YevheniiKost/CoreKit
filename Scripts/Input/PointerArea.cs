using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace YeKostenko.CoreKit.Input
{
    public class PointerArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private UnityEvent _onPointerDownEvent;
        [SerializeField]
        private UnityEvent _onPointerUpEvent;
        
        public bool IsPointerDown { get; private set; }
        
        public event Action OnPointerDownEvent;
        public event Action OnPointerUpEvent;

        public void OnPointerDown(PointerEventData eventData)
        {
            IsPointerDown = true;
            OnPointerDownEvent?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsPointerDown = false;
            OnPointerUpEvent?.Invoke();
        }
    }
}