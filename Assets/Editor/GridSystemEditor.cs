using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(GridSystem))]
public class GridSystemEditor: Editor
{
    private GridSystem t;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if (GUILayout.Button("Apply Changes"))
        {
            CacheTarget();
            t.Recalculate();
        }
    }
    private void CacheTarget()
    {
        if (t != null)
            t = target as GridSystem;
    }
}
