using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SubstitutionMediator))]
public class SubstitutionMediatorEditor : Editor
{
   
    private SubstitutionMediator t;
    public override void OnInspectorGUI()
    {
        
        t = target as SubstitutionMediator;

        GUI.changed = false;

        if (GUILayout.Button("Apply"))
        {
            t.Substitute();
            EditorUtility.SetDirty(t);
        }
    }
}