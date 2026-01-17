using System;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(ReplacementMediator), true)]
[CanEditMultipleObjects]
public class ReplacementMediatorEditorBase : Editor
{
    protected ReplacementMediator t;

    private void SubscribeToEvents()
    {
        CacheTarget();
        if (!t.randomizable) return;
        UnsubscribeFromEvents();
        Eventbus.OnRandomizerApplyButtonClickedForNewPool += ReplaceResetPool;
        Eventbus.OnRandomizerApplyButtonClickedForSamePool += ReplaceResetPool;
        t.Subscribe();
        Debug.Log("enabled");
        
    }

    private void UnsubscribeFromEvents()
    {
        CacheTarget();
        if (!t.randomizable) return;
        Eventbus.OnRandomizerApplyButtonClickedForNewPool -= ReplaceResetPool;
        Eventbus.OnRandomizerApplyButtonClickedForSamePool -= ReplaceResetPool;
        t.Unsubscribe();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space(8);
        
        if (GUILayout.Button("Suscribe To Events"))
            SubscribeToEvents();
        if (GUILayout.Button("Unsubscribe From Events"))
            UnsubscribeFromEvents();

        EditorGUILayout.Space(8);

        if (GUILayout.Button("Replace : Same Pool"))
            ReplaceSamePool();

        if (GUILayout.Button("Replace : Reset Pool"))
            ReplaceResetPool();

        EditorGUILayout.Space(8);

        if (GUILayout.Button("Reset To Pool"))
        {
            CacheTarget();
            //ReleaseItemsToPool();
        }

        if (GUILayout.Button("Hard Reset"))
        {
            CacheTarget();
            HardReset();
        }
    }

    protected void CacheTarget()
    {
        if (t == null)
            t = (ReplacementMediator)target; // Works for subclasses too
    }

    /*protected void ReleaseItemsToPool()
    {
        if (t.pool.pool.Count == 0)
            return;
        t.ReleaseItemsToPool();
    }*/

    protected void HardReset()
    {
        DeleteAllChildrenInEditor(t.parent);
        DeleteAllChildrenInEditor(t.pool.transform);
        t.pool.ClearPool();
    }

    private void DeleteAllChildrenInEditor(Transform parent)
    {
        if (parent == null) return;
        if (parent.childCount == 0) return;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            var child = parent.GetChild(i).gameObject;
            Undo.DestroyObjectImmediate(child);
        }

        EditorUtility.SetDirty(parent);
    }

    protected void SetSceneDirty()
    {
        EditorUtility.SetDirty(t);
        if (!Application.isPlaying)
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(t.gameObject.scene);
    }

    protected void ReplaceSamePool()
    {
        CacheTarget();
        Undo.RecordObject(t, "Replace All: Same Pool");

        //ReleaseItemsToPool();
        t.ExecuteReplacements();

        SetSceneDirty();
    }

    protected void ReplaceResetPool()
    {
        CacheTarget();
        Undo.RecordObject(t, "Replace All: Reset Pool");

        HardReset();
        t.ExecuteReplacements();

        SetSceneDirty();
    }
}