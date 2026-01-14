using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SceneRandomizer))]
public class SceneRandomizerEditor : Editor
{
    private SceneRandomizer t;
    private ReplacementType _resetType = ReplacementType.RightBatiment;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(8);
        t = (SceneRandomizer)target;

        if (GUILayout.Button("Randomize"))
        {
            Undo.RecordObject(t, "Randomizer Apply");

            t.MixAndApply();

            EditorUtility.SetDirty(t);
            if (!Application.isPlaying)
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(t.gameObject.scene);
        }
        
        EditorGUILayout.Space(8);

        using (new EditorGUILayout.HorizontalScope())
        {
            _resetType = (ReplacementType)EditorGUILayout.EnumPopup(_resetType, GUILayout.MaxWidth(160));

            if (GUILayout.Button("Reset"))
            {
                Undo.RecordObject(t, "Reset");
                t.ResetAllToGivenType(_resetType); 
                EditorUtility.SetDirty(t);
            }
        }
    }
}

  
      
