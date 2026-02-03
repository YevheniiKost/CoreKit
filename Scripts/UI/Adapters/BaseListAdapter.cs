using System.Collections.Generic;
using UnityEngine;

namespace YeKostenko.CoreKit.UI.Adapters
{
      public class BaseListAdapter<T, K> where T : MonoBehaviour
    {
        private readonly T _viewRef;
        private readonly List<T> _viewInstances;

        private List<K> _data;
        private readonly Transform _content;

        public BaseListAdapter(T viewRef, Transform content, bool preloaded)
        {
            _viewRef = viewRef;
            _content = content;

            _viewInstances = new List<T>();
            _data = new List<K>();

            if (preloaded)
            {
                for (int i = 0; i < content.childCount; i += 1)
                {
                    T viewInstance = content.GetChild(i).GetComponent<T>();
                    if (viewInstance != null)
                    {
                        _viewInstances.Add(viewInstance);
                    }
                }
            }
        }

        public BaseListAdapter(T viewRef, Transform content) : this(viewRef, content, false)
        {
        }

        public void SetData(List<K> newData)
        {
            _data = newData;
            Refresh();
        }

        public virtual void ClearData()
        {
            _data.Clear();
            Refresh();
        }

        public void NotifyDataSetChanged()
        {
            Refresh();
        }

        protected List<T> GetViews()
        {
            return _viewInstances;
        }

        protected int GetDataCount()
        {
            return _data.Count;
        }

        protected virtual void BindView(int position, T view, K data)
        {
        }

        private T GetViewForItem(int index)
        {
            if (index < _viewInstances.Count)
            {
                return _viewInstances[index];
            }

            T view = Object.Instantiate(_viewRef.gameObject, _content, false).GetComponent<T>();
            _viewInstances.Add(view);

            return view;
        }

        private void Refresh()
        {
            for (int i = 0; i < _data.Count; i++)
            {
                T view = GetViewForItem(i);
                BindView(i, view, _data[i]);

                view.gameObject.SetActive(true);
            }

            for (int i = _data.Count; i < _viewInstances.Count; i += 1)
            {
                T view = _viewInstances[i];
                view.gameObject.SetActive(false);
            }
        }
    }
}