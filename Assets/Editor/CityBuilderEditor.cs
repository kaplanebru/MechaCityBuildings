using System;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(CityBuilder))]
public class CityBuilderEditor : Editor
{
    private CityBuilder t;
    private ArrangementEditorHelper arrangementHelper = new();
    public SavedArrangements savedArrangements;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(8);
        if (GUILayout.Button("Randomize And Apply"))
        {
            CacheTarget();
            t.RandomizeAndInstallTotalZone();
        }

        EditorGUILayout.Space(8);
        if (GUILayout.Button("Add Arrangement"))
        {
            CacheArrangementHelper();
            arrangementHelper.AddArrangement(savedArrangements);
        }

        CacheArrangementHelper();

        EditorGUILayout.Space(8);
        if (GUILayout.Button("Load or Remove Arrangement"))
        {
            CacheArrangementHelper();
            arrangementHelper.ApplyOrRemoveArrangement(savedArrangements);
        }
    }

    private void CacheTarget()
    {
        if (t == null)
            t = (CityBuilder)target; // Works for subclasses too
    }

    private void CacheArrangementHelper()
    {
        if (arrangementHelper == null)
            arrangementHelper = new ArrangementEditorHelper();
    }
}