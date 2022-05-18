using System;
using System.Collections.Generic;
using UnityEngine;

namespace DI
{
    public class BackedInjection : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private List<Component> _components = new List<Component>();
        [SerializeField, HideInInspector]
        private bool _forceUpdate = true;
        [SerializeField, HideInInspector]
        private bool _forceSearch = true;

        public IReadOnlyList<Component> Components => _components;

#if UNITY_EDITOR

        [Obsolete("Only for Editor")]
        public int ObjectsCount => _components == null ? 0 : _components.Count;

        [Obsolete("Only for Editor")]
        public bool ForceUpdate
        {
            get => _forceUpdate;
            set => _forceUpdate = value;
        }

        [Obsolete("Only for Editor")]
        public bool ForceSearch
        {
            get => _forceSearch;
            set 
            {
                if (_forceSearch == true && value == false)
                    _forceUpdate = false;

                _forceSearch = value;
            } 
        }

        [Obsolete("Only for Editor")]
        public List<Component> Find()
        {
            List<Component> result = new List<Component>();
            Component[] components = GetComponentsInChildren<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].GetType().IsDefined(typeof(DITargetAttribute), true))
                    result.Add(components[i]);
            }

            return result;
        }

        [Obsolete("Only for Editor")]
        public bool IsCurrentDataValid()
        {
            if (_components == null)
                return false;

            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] == null)
                    return false;
            }

            return true;
        }

        [Obsolete("Only for Editor")]
        public bool IsActual(List<Component> components)
        {
            if (_components == null || components == null)
                return false;

            if (_components.Count != components.Count)
                return false;

            for (int i = 0; i < _components.Count; i++)
                if (_components[i] != components[i])
                    return false;
            
            return true;
        }

        [Obsolete("Only for Editor")]
        public void Set(List<Component> components) => 
            _components = new List<Component>(components);

        [ContextMenu(nameof(Bake))]
        private void Bake()
        {
            _components.Clear();
            Component[] components = GetComponentsInChildren<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].GetType().IsDefined(typeof(DITargetAttribute), true))
                    _components.Add(components[i]);
            }
        }

#endif

    }
}