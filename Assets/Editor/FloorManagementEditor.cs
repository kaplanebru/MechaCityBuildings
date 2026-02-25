using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FloorManagement))]
public class FloorManagementEditor : Editor
{
    protected FloorManagement floorManagement;
    private int floorIndex;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space(8);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Increase Floor"))
            {
                CacheTarget();
                floorManagement.IncreaseFloor();
            }
            
            if (GUILayout.Button("Delete Last Floor"))
            {
                CacheTarget();
                floorManagement.DeleteLastFloor();
            }
        }
        
        EditorGUILayout.Space(2);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Switch Active Floor To"))
            {
                CacheTarget();
                floorManagement.SwitchActiveFloor(floorIndex);
            }
            
            floorIndex = EditorGUILayout.IntField(floorIndex); //todo enum field
        }
        EditorGUILayout.Space(8);

        DrawDefaultInspector();
    }

    private void CacheTarget()
    {
        if (floorManagement != null)
            floorManagement = target as FloorManagement;
    }
}