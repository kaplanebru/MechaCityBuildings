using System;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(ReplacerBase), true)]
[CanEditMultipleObjects]
public class ReplacerBaseEditor : Editor
{
    protected ReplacerBase t;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space(8);

        if (GUILayout.Button("Replace"))
        {
            CacheTarget();
            ReplacerEditorHelper.Replace(t);
        }
        
        EditorGUILayout.Space(8);

        if (GUILayout.Button("Release To Pool")) //TODO
        {
            CacheTarget();
            ReplacerEditorHelper.ReleaseItemsToPool(t);
        }

        if (GUILayout.Button("Refresh Pool With New Object"))
        {
            CacheTarget();
            ReplacerEditorHelper.RefreshPool(t);
        }
    }

    private void CacheTarget()
    {
        if (t != null)
            t = target as ReplacerBase;
    }


}