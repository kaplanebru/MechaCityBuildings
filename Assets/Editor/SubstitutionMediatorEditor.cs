using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SubstitutionMediator))]
public class SubstitutionMediatorEditor : Editor
{
    private SubstitutionMediator t;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

       // EditorGUILayout.Space(8);

        if (GUILayout.Button("Replace Selected: Same Pool"))
        {
            if (t == null)
                t = (SubstitutionMediator)target;
            Undo.RecordObject(t, "Replace Selected: Same Pool");

            ReleaseItemsToPool();
            t.ReplaceSelected();

            SetSceneDirty();
        }
        
        if (GUILayout.Button("Replace Selected: Reset Pool"))
        {
            if (t == null)
                t = (SubstitutionMediator)target;
            Undo.RecordObject(t, "Replace Selected: Reset Pool");

            HardReset();
            t.ReplaceSelected();

            SetSceneDirty();
        }

        EditorGUILayout.Space(8);
        
        if (GUILayout.Button("Replace All: Reset Pool"))
        {
            if (t == null)
                t = (SubstitutionMediator)target;
            Undo.RecordObject(t, "Replace All: Reset Pool");

            HardReset();
            t.ReplaceAllFromScene();
            
            SetSceneDirty();
        }

        if (GUILayout.Button("Replace All: Same Pool"))
        {
            if (t == null)
                t = (SubstitutionMediator)target;
            Undo.RecordObject(t, "Replace All: Same Pool");

            ReleaseItemsToPool();
            t.ReplaceAllFromScene();

            SetSceneDirty();
        }
        
        EditorGUILayout.Space(8);

        if (GUILayout.Button("Reset To Pool"))
        {
            if (t == null)
                t = (SubstitutionMediator)target;

            ReleaseItemsToPool();
        }

        if (GUILayout.Button("Hard Reset"))
        {
            if (t == null)
                t = (SubstitutionMediator)target;

            HardReset();
        }
    }

    private void ReleaseItemsToPool()
    {
        t.pool.ReleaseItemsToPool(t.Replacements);
    }

    private void HardReset()
    {
        DeleteAllChildrenInEditor(t.parent);
        DeleteAllChildrenInEditor(t.pool.transform);
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

    private void SetSceneDirty()
    {
        EditorUtility.SetDirty(t);
        if (!Application.isPlaying)
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(t.gameObject.scene);
    }
}