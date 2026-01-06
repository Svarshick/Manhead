using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor.EntityEditor.Generic
{
    public class TypeSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        //it can't be generic because unity initialization
        private Type _baseType;
        private string _message;
        private Action<Type> _onSelectEntry;

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent(_message))
            };

            // Find all non-abstract types inheriting from T
            var types = TypeCache.GetTypesDerivedFrom(_baseType)
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .OrderBy(t => t.Name);

            foreach (var type in types)
                tree.Add(new SearchTreeEntry(new GUIContent(type.Name))
                {
                    level = 1,
                    userData = type
                });

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            if (SearchTreeEntry.userData is Type type)
            {
                _onSelectEntry?.Invoke(type);
                return true;
            }

            return false;
        }

        public void Init(Type baseType, Action<Type> onSelectEntry, string message = "Types")
        {
            _baseType = baseType;
            _onSelectEntry = onSelectEntry;
            _message = message;
        }
    }
}