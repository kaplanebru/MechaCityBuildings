using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(BatimentRandomizer))]
public class BatimentRandomizerEditor : Editor
{
    private BatimentRandomizer t;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(8);
        t = (BatimentRandomizer)target;

        if (GUILayout.Button("Randomize"))
        {
            Undo.RecordObject(t, "Randomizer Apply");

            t.MixBatiments();

            EditorUtility.SetDirty(t);
            if (!Application.isPlaying)
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(t.gameObject.scene);
        }

        if (GUILayout.Button("Reset"))
        {
            t.ResetBatimentsToCurrentType();
        }
    }
}