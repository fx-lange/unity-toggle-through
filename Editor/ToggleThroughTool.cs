using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace FxLange.ToggleThrough
{
    [EditorTool("Toggle Through")]
    internal class ToggleThroughTool : EditorTool
    {
        private int _activeIndex;

        public override void OnActivated()
        {
            Selection.selectionChanged += OnSelectionChanged;

            Init();
        }

        public override void OnWillBeDeactivated()
        {
            Selection.selectionChanged -= OnSelectionChanged;

            Clear();
        }

        private void OnSelectionChanged()
        {
            Clear();
            Init();
        }

        private void Clear()
        {
        }

        private void Init()
        {
            if (Selection.gameObjects is {Length: >= 2})
            {
                _activeIndex = Selection.gameObjects[0].activeSelf ? 0 : -1;
            }
            else
            {
                _activeIndex = -1;
            }
        }

        private void Toggle()
        {
            var selection = Selection.gameObjects;

            if (selection.Length == 1)
            {
                var singleSelection = selection[0];
                singleSelection.SetActive(!singleSelection.activeSelf);
                return;
            }

            _activeIndex++;
            _activeIndex %= selection.Length;

            for (var index = 0; index < selection.Length; index++)
            {
                var gameObject = selection[index];
                gameObject.SetActive(index == _activeIndex);
            }
        }

        public override void OnToolGUI(EditorWindow window)
        {
            if (window is not SceneView)
            {
                return;
            }

            EditorGUI.BeginChangeCheck();
            
            Handles.BeginGUI();
            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    if (GUILayout.Button("Toggle Through"))
                    {
                        Toggle();
                    }
                }

                GUILayout.FlexibleSpace();
            }
            Handles.EndGUI();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(Selection.objects, "Toggle GameObjects");
            }
        }
    }
}