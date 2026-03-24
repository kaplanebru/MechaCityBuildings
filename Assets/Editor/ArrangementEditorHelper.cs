using System;
using UnityEditor;
using UnityEngine;

public class ArrangementEditorHelper
{
    private string _newArrangementName;
    private int _selectedArrangementIndex;

    public void AddArrangement(SavedArrangements savedArrangementDatabase)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            _newArrangementName = EditorGUILayout.TextField(
                new GUIContent("New Arrangement Name"),
                _newArrangementName
            );

            if (GUILayout.Button("Save", GUILayout.Width(80)))
            {
                savedArrangementDatabase.AddArrangement(_newArrangementName);
                EditorUtility.SetDirty(savedArrangementDatabase);
            }
        }
    }

    public string ShowArrangements(SavedArrangements savedArrangementDatabase)
    {
        if (savedArrangementDatabase.Names == null || savedArrangementDatabase.Names.Length == 0)
        {
            EditorGUILayout.HelpBox("No arrangements.", MessageType.Info);
            return null;
        }

        _selectedArrangementIndex =
            Mathf.Clamp(_selectedArrangementIndex, 0, savedArrangementDatabase.Names.Length - 1);

        _selectedArrangementIndex = EditorGUILayout.Popup(
            "Arrangement",
            _selectedArrangementIndex,
            savedArrangementDatabase.Names,
            GUILayout.MaxWidth(400)
        );

        return savedArrangementDatabase.Names[_selectedArrangementIndex];
    }

    public void ApplyOrRemoveArrangement(SavedArrangements savedArrangementDatabase)
    {
        string selectedName = ShowArrangements(savedArrangementDatabase);

        using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(selectedName)))
        {
            if (GUILayout.Button("Resurrect Arrangement"))
            {
                if (savedArrangementDatabase.TryGetDataByName(selectedName, out var arrangementData))
                {
                    //todo: ressurect city with given arrangementData

                    //if (!Application.isPlaying)
                    //UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }
            }

            if (GUILayout.Button("Remove Arrangement"))
            {
                if (savedArrangementDatabase.TryGetDataByName(selectedName, out var arrangementData))
                {
                    savedArrangementDatabase.RemoveArrangement(arrangementData);
                    EditorUtility.SetDirty(savedArrangementDatabase);
                }
            }
        }
    }
}