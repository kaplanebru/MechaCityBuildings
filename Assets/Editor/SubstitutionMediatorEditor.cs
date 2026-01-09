using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SubstitutionMediator))]
public class SubstitutionMediatorEditor : Editor
{
   
    private SubstitutionMediator t;

    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();

        EditorGUILayout.Space(8);

        if (GUILayout.Button("Apply"))
        {
            t = (SubstitutionMediator)target;
            Undo.RecordObject(t, "Substitution Apply");
            
            HardReset();
            t.Substitute();
            
            EditorUtility.SetDirty(t);
            if (!Application.isPlaying)
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(t.gameObject.scene);
        }

        if (GUILayout.Button("Reset"))
        {
            if(t == null)
                t = (SubstitutionMediator)target;
            
            HardReset();
        }
    }

    private void HardReset()
    {
        DeleteAllChildrenInEditor(t.transform);
        DeleteAllChildrenInEditor(t.pool.transform);
        
    }
    private void DeleteAllChildrenInEditor(Transform parent)
    {
        if (parent == null) return;
        if(parent.childCount == 0) return;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            var child = parent.GetChild(i).gameObject;
            Undo.DestroyObjectImmediate(child);
        }

        EditorUtility.SetDirty(parent);
    }
}