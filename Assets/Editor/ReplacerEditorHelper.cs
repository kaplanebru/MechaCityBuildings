#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ReplacerEditorHelper
{
    public static void Replace(ReplacerBase t)
    {
        Undo.RecordObject(t, "ReplaceGiven All");
        t.ExecuteReplacements();
        SetSceneDirty(t);
    }

    public static void ReplaceSavedData(ReplacerBase t, PlaceholderData[] placeholderDataSet)
    {
        Undo.RecordObject(t, "ReplaceGiven All Saved Data");
        t.ReplaceGiven(placeholderDataSet);
        SetSceneDirty(t);
    }

    public static void DeleteAllChildrenInEditor(Transform parent)
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

    public static void RefreshPool(ReplacerBase t)
    {
        DeleteAllChildrenInEditor(t.parent);
        DeleteAllChildrenInEditor(t.pool.transform);
        t.pool.ClearPool();
    }

    public static void ReleaseItemsToPool(ReplacerBase t)
    {
        if (t.pool.pool.Count == 0)
            return;
        t.ReleaseItemsToPool();
    }

    public static void SetSceneDirty(ReplacerBase t)
    {
        EditorUtility.SetDirty(t);
        if (!Application.isPlaying)
            EditorSceneManager.MarkSceneDirty(t.gameObject.scene);
    }
}
#endif