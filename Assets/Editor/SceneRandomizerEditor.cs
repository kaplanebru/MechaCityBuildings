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

        if (GUILayout.Button("Randomize"))
        {
           Randomize();
        }
        
        EditorGUILayout.Space(8);
        
        if (GUILayout.Button("Randomize And Apply To AutoReplacers: New Pool"))
        {
            Randomize();
            Eventbus.OnRandomizerApplyButtonClickedForNewPool?.Invoke();
        }
        if (GUILayout.Button("Randomize And Apply To AutoReplacers: Same Pool"))
        {
            Randomize();
            Eventbus.OnRandomizerApplyButtonClickedForSamePool?.Invoke();
        }
        
        EditorGUILayout.Space(8);

        using (new EditorGUILayout.HorizontalScope())
        {
            _resetType = (ReplacementType)EditorGUILayout.EnumPopup(_resetType, GUILayout.MaxWidth(160));

            if (GUILayout.Button("Reset"))
            {
               Reset();
            }
        }
    }

    private void Randomize()
    {
        t = (SceneRandomizer)target;
        Undo.RecordObject(t, "Randomizer Apply");

        t.MixAndApply();

        EditorUtility.SetDirty(t);
        if (!Application.isPlaying)
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(t.gameObject.scene);
    }

    private void Reset()
    {
        if(t == null) t = (SceneRandomizer)target;
                
        Undo.RecordObject(t, "Reset");
        t.ResetAllToGivenType(_resetType); 
        EditorUtility.SetDirty(t);
    }
}

  
      
