using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuadSample))]
public class QuadSampleEditor: Editor
{
    private QuadSample t;
    public override void OnInspectorGUI()
    {
        //using (new EditorGUILayout.HorizontalScope())
        //{
            RestoreCache();
            
            t.data.WidthHeight = EditorGUILayout.Vector2IntField("Width Height", t.data.WidthHeight);
            if (GUILayout.Button("Apply"))
            {
                t.Generate();
            }
        //}
        
        EditorGUILayout.Space(8);
        
        DrawDefaultInspector();
    }

    private void RestoreCache()
    {
        if(t == null)
            t = target as QuadSample;
    }
}
