using System;
using System.Linq;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(CityBuilder))]
public class CityBuilderEditor : Editor
{
    private CityBuilder t;
    private string _newDispositionName;
    private int floorIndex;

    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DrawStructureDispositionMatrix();
        CacheTarget();

        EditorGUILayout.Space(8);

        GUI.backgroundColor = Color.cyan;

        if (GUILayout.Button("RANDOMIZE ALL Floors Together", GUILayout.Height(25)))
        {
            t.RandomizeAndInstallTotalZone();
        }
        EditorGUILayout.Space(4);
        
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("RANDOMIZE Selected Floor", GUILayout.Height(25)))
            {
                t.RandomizeSelectedFloor(floorIndex);
            }
            
            floorIndex = EditorGUILayout.IntField(floorIndex); //todo enum field
        }
        EditorGUILayout.Space(8);
        
        GUI.backgroundColor = Color.white;

        GUILayout.Label("Pool Settings", EditorStyles.boldLabel);
        

        if (GUILayout.Button("Reset Everything"))
        {
            CacheTarget();
            ResetEverything();
        }

        if (GUILayout.Button("Reset Everything and POOLS"))
        {
            ResetEverything();
            //RefreshPools(t.units.Installer);
        }

        EditorGUILayout.Space(8);
    }

    private void CacheTarget()
    {
        if (t == null)
            t = (CityBuilder)target; // Works for subclasses too
    }

    private void ResetEverything()
    {
        var count = t.units.FloorDb.GetFloorCount();
        for (int i = 0; i < count; i++)
        {
            t.ClearFloorResidentsData(i);
        }
        t.units.FloorDb.SetActiveFloor(0);
    }
    

    private void DrawStructureDispositionMatrix()
    {
        CacheTarget();

        t.RestoreMatrixSizeIfNeeded();
        MatrixMakerStructureTypeAdjacency.DisposeStructureTypes(t.cityData.GetStructureTypes().ToArray(),t.cityData.matrix);
        var structureTypesFromPools = t.units.Installer.GetStructureTypesFromPools().ToHashSet();
        //MatrixMakerStructureTypeAdjacency.DisposeStructureTypes(structureTypesFromPools.ToArray(), t.cityData.matrix);
    }
}