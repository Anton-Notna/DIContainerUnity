using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace DI.Editor
{
    [CustomEditor(typeof(BackedInjection))]
    public class BackedInjectionEditor : GraphicEditor
    {
        private List<Component> _currentComponents;

        public override void OnInspectorGUI()
        {
            BackedInjection backedInjection = (BackedInjection)target;
            List<(string, MessageType)> messages = new List<(string, MessageType)>();
            DrawInfo(backedInjection);

#pragma warning disable CS0618

            if (backedInjection.ForceSearch)
            {
                _currentComponents = backedInjection.Find();
            }
            else
            {
                if(GUILayout.Button("Search data"))
                    _currentComponents = backedInjection.Find();
            }

            bool validData = backedInjection.IsCurrentDataValid();
            if (validData == false)
                messages.Add(($"Data broken, need update", MessageType.Error));

            if (_currentComponents == null)
            {
                messages.Add(($"Cannot validate data, need searching", MessageType.Warning));
            }
            else
            {
                bool upToDateData = validData && backedInjection.IsActual(_currentComponents);
                if (upToDateData)
                {
                    messages.Add(($"Up-to-date data", MessageType.Info));
                }
                else
                {
                    if (backedInjection.ForceUpdate)
                    {
                        backedInjection.Set(_currentComponents);
                    }
                    else
                    {
                        messages.Add(($"Obsolete data, need update", MessageType.Warning));
                        if (GUILayout.Button("Update"))
                            backedInjection.Set(_currentComponents);
                    }
                }
            }

#pragma warning restore CS0618

            for (int i = 0; i < messages.Count; i++)
                EditorGUILayout.HelpBox(messages[i].Item1, messages[i].Item2);
        }

        private void DrawInfo(BackedInjection backedInjection)
        {
#pragma warning disable CS0618

            EditorGUILayout.LabelField($"Objects to inject: {backedInjection.ObjectsCount}");

            backedInjection.ForceSearch = EditorGUILayout.Toggle("Force search", backedInjection.ForceSearch);
            if (backedInjection.ForceSearch)
                backedInjection.ForceUpdate = EditorGUILayout.Toggle("Force update", backedInjection.ForceUpdate);

#pragma warning restore CS0618
        }
    }
}