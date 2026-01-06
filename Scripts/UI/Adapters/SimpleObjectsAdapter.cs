using System.Collections.Generic;
using UnityEngine;

namespace YeKostenko.CoreKit.UI.Adapters
{
    public class SimpleObjectsAdapter
    {
        private readonly Transform _parent;
        private readonly GameObject _itemPrefab;
        
        private Queue<GameObject> _availableItems = new Queue<GameObject>();
        private List<GameObject> _activeItems = new List<GameObject>();
        
        public SimpleObjectsAdapter(Transform parent, GameObject itemPrefab)
        {
            _parent = parent;
            _itemPrefab = itemPrefab;
        }

        public void SetItemCount(int count)
        {
            while (_activeItems.Count < count)
            {
                GameObject item;
                
                if (_availableItems.Count > 0)
                {
                    item = _availableItems.Dequeue();
                    item.SetActive(true);
                }
                else
                {
                    item = Object.Instantiate(_itemPrefab, _parent);
                }
                
                _activeItems.Add(item);
            }

            while (_activeItems.Count > count)
            {
                var lastIndex = _activeItems.Count - 1;
                var item = _activeItems[lastIndex];
                _activeItems.RemoveAt(lastIndex);
                
                item.SetActive(false);
                _availableItems.Enqueue(item);
            }
        }
    }
}