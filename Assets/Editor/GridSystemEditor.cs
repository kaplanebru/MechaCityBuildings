using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridSystem))]
public class GridSystemEditor: Editor
{
    protected GridSystem gridSystem;
    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space(8);

        if (GUILayout.Button("Initialize Drawing Grid"))
        {
            CacheTarget();
            gridSystem.Initialize();
        }
        
        EditorGUILayout.Space(8);
        DrawDefaultInspector();
        EditorGUILayout.Space(8);
        
        if (GUILayout.Button("Apply Changes"))
        {
            CacheTarget();
            gridSystem.Initialize();
        }

        if (GUILayout.Button("Construct Buildings On Cells"))
        {
            CacheTarget();
            gridSystem.ConstructBuildingsOnCells();
        }
    }
    
    private void CacheTarget()
    {
        if (gridSystem != null)
            gridSystem = target as GridSystem;
    }
}
