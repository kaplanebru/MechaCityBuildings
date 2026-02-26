using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CityBuilder))]
public class CityBuilderEditor : Editor
{
    //User pref buraya eklenebilir
    protected CityBuilder t;
    private int floorIndex;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space(8);

        if (GUILayout.Button("Recalculate Grid (If Needed)"))
        {
            CacheTarget();
            t.units.gridSystem.Recalculate();

        }

        EditorGUILayout.Space(8);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Start Painting On Floor"))
            {
                CacheTarget();
                t.units.gridSystem.Recalculate(); //TODO: if needed
                t.units.painter.StartPainting();
            }

            //TODO: add null check: if no painting - return
            if (GUILayout.Button("Construct Buildings On Paint"))
            {
                CacheTarget();
                //TODO: stop painting
                t.units.builder.ConstructBuildingsOnCells();
            }
        }


        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Increase Floor"))
            {
                CacheTarget();
                t.units.floorManagement.IncreaseFloor();
            }

            if (GUILayout.Button("Delete Last Floor"))
            {
                CacheTarget();
                t.units.floorManagement.DeleteLastFloor();
            }
        }
        
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Switch Active Floor To"))
            {
                CacheTarget();
                t.units.floorManagement.SwitchActiveFloor(floorIndex);
            }

            floorIndex = EditorGUILayout.IntField(floorIndex); //todo enum field
        }

        EditorGUILayout.Space(8);


        EditorGUILayout.Space(8);
        DrawDefaultInspector();
    }

    private void CacheTarget()
    {
        if (t != null)
            t = target as CityBuilder;
    }
}