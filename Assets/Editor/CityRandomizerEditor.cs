using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(CityRandomizer))]
public class CityRandomizerEditor : Editor
{
    private CityRandomizer t;

    public string[] arrangementKeys;
    private int _selectedArrangementIndex;


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(8);

        if (GUILayout.Button("Randomize"))
            Randomize();


        EditorGUILayout.Space(8);
        if (GUILayout.Button("Randomize And Apply"))
        {
            Randomize();
            PublishExecutionCompletedEvent();
        }

        EditorGUILayout.Space(16);
        SaveArrangement();

        EditorGUILayout.Space(8);
        ApplyOrRemoveArrangement();
        EditorGUILayout.Space(8);
    }

    private void PublishExecutionCompletedEvent()
    {
        Eventbus.OnRandomizingExecuted?.Invoke();
    }

    private void Randomize()
    {
        CacheTarget();
        Undo.RecordObject(t, "Randomizer Apply");

        t.MixAndApply();

        EditorUtility.SetDirty(t);
        if (!Application.isPlaying)
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(t.gameObject.scene);
    }

    protected void CacheTarget()
    {
        if (t == null)
            t = (CityRandomizer)target; // Works for subclasses too
    }

    private string _newArrangementName;

    private void SaveArrangement()
    {
        CacheTarget();
        using (new EditorGUILayout.HorizontalScope())
        {
            _newArrangementName = EditorGUILayout.TextField(
                new GUIContent("New Arrangement Name"),
                _newArrangementName
            );

            if (GUILayout.Button("Save", GUILayout.Width(80)))
            {
                t.SaveCurrentArrangement(_newArrangementName);
                arrangementKeys = t.arrangementCache.RefreshNames();
            }
        }
    }

    private string ShowArrangements()
    {
        if (arrangementKeys == null || arrangementKeys.Length == 0)
        {
            EditorGUILayout.HelpBox("No arrangements.", MessageType.Info);
            return null;
        }

        _selectedArrangementIndex = Mathf.Clamp(_selectedArrangementIndex, 0, arrangementKeys.Length - 1);

        _selectedArrangementIndex = EditorGUILayout.Popup(
            "Arrangement",
            _selectedArrangementIndex,
            arrangementKeys,
            GUILayout.MaxWidth(400)
        );

        return arrangementKeys[_selectedArrangementIndex];
    }

    private void ApplyOrRemoveArrangement()
    {
        string selectedName = ShowArrangements();

        using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(selectedName)))
        {
            if (GUILayout.Button("Resurrect Arrangement"))
            {
                t.ResurrectArrangement(selectedName);
                Undo.RecordObject(t, "Resurrect Arrangement");

                EditorUtility.SetDirty(t);
                if (!Application.isPlaying)
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(t.gameObject.scene);
            }

            if (GUILayout.Button("Remove Arrangement"))
            {
                t.arrangementCache.Remove(selectedName);
                arrangementKeys = t.arrangementCache.RefreshNames();
                Undo.RecordObject(t, "Remove Arrangement");
            }
        }
    }
}