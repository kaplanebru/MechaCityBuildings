using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ReplacerController))]
public class ReplacerControllerEditor: Editor
{
    private ReplacerController t;

    private void OnEnable()
    {
        Eventbus.OnRandomizingExecuted += ReplaceAll;
        Eventbus.OnReplacementWithSavedRequest += ReplaceSaved;
    }
    private void OnDisable()
    {
        Eventbus.OnRandomizingExecuted -= ReplaceAll;
        Eventbus.OnReplacementWithSavedRequest -= ReplaceSaved;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space(8);
        
        EditorGUILayout.Space(8);
        if (GUILayout.Button("Refresh Pool With New Object"))
            RefreshPool();
        
        EditorGUILayout.Space(8);
        if (GUILayout.Button("Release All"))
            ReleaseAll();
        
    }

    private void ReplaceAll()
    {
        CacheTarget();
        t.autoReplacers.ForEach(ReplacerEditorHelper.Replace);
        Debug.Log("Replace all");

    }

    private void ReleaseAll()
    {
        CacheTarget();
        t.autoReplacers.ForEach(ReplacerEditorHelper.ReleaseItemsToPool);
    }

    private void RefreshPool()
    {
        CacheTarget();
        t.autoReplacers.ForEach(ReplacerEditorHelper.RefreshPool);
        ReplaceAll();
    }
    
    private void ReplaceSaved(Dictionary<ReplacementType, List<PlaceholderData>> categorizedBuildings)
    {
        ReleaseAll();
        foreach (var categorizedBuilding in categorizedBuildings)
        {
            if (t.TryGet(categorizedBuilding.Key, out ReplacerBase replacer))
            {
                ReplacerEditorHelper.ReplaceSavedData(replacer, 
                    categorizedBuilding.Value.ToArray());
            }
            else
            {
                Debug.LogWarning("Replacer not found: " + categorizedBuilding.Key);
            }
        }
        Debug.Log("Replace saved attempt");
    }
    private void CacheTarget()
    {
        if (t == null)
            t = (ReplacerController)target;
    }
}
