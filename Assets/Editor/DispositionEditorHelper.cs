using System;
using UnityEditor;
using UnityEngine;

public class DispositionEditorHelper
{
    private string _newDispositionName;
    private int _selectedDispositionIndex;

    public void AddDisposition(SavedDispositionDb savedDispositionDb)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            _newDispositionName = EditorGUILayout.TextField(
                new GUIContent("New Arrangement Name"),
                _newDispositionName
            );

            if (GUILayout.Button("Save", GUILayout.Width(80)))
            {
                savedDispositionDb.AddArrangement(_newDispositionName);
                EditorUtility.SetDirty(savedDispositionDb);
            }
        }
    }

    public string ShowArrangements(SavedDispositionDb savedDispositionDb)
    {
        if (savedDispositionDb.Names == null || savedDispositionDb.Names.Length == 0)
        {
            EditorGUILayout.HelpBox("No arrangements.", MessageType.Info);
            return null;
        }

        _selectedDispositionIndex =
            Mathf.Clamp(_selectedDispositionIndex, 0, savedDispositionDb.Names.Length - 1);

        _selectedDispositionIndex = EditorGUILayout.Popup(
            "Arrangement",
            _selectedDispositionIndex,
            savedDispositionDb.Names,
            GUILayout.MaxWidth(400)
        );

        return savedDispositionDb.Names[_selectedDispositionIndex];
    }

    public void ApplyOrRemoveArrangement(SavedDispositionDb savedDispositionDb)
    {
        string selectedName = ShowArrangements(savedDispositionDb);

        using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(selectedName)))
        {
            if (GUILayout.Button("Resurrect Arrangement"))
            {
                if (savedDispositionDb.TryGetDataByName(selectedName, out var arrangementData))
                {
                    //todo: ressurect city with given arrangementData

                    //if (!Application.isPlaying)
                    //UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }
            }

            if (GUILayout.Button("Remove Arrangement"))
            {
                if (savedDispositionDb.TryGetDataByName(selectedName, out var dispositionData))
                {
                    savedDispositionDb.RemoveArrangement(dispositionData);
                    EditorUtility.SetDirty(savedDispositionDb);
                }
            }
        }
    }
}