using System;
using System.Linq;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(CityBuilder))]
public class CityBuilderEditor : Editor
{
    private CityBuilder t;
    private string _newDispositionName;
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DrawStructureDispositionMatrix();
        CacheTarget();

        EditorGUILayout.Space(8);

        if (GUILayout.Button("RANDOMIZE And APPLY", GUILayout.Height(20)))
        {
            t.RandomizeAndInstallTotalZone();
        }
        EditorGUILayout.Space(8);
        
        GUILayout.Label("Pool Settings", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Initiate Pools"))
        {
            CacheTarget();
            t.units.Installer.InitiatePools();
            
            EditorUtility.SetDirty(t);
        }

        if (GUILayout.Button("Reset Everything"))
        {
            CacheTarget();
            ResetEverything();
        }

        if (GUILayout.Button("Reset Everything and POOLS"))
        {
            ResetEverything();
            InstallerEditorHelper.RefreshPools(t.units.Installer);
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

        t.RestoreMatrixIfNeeded();
        MatrixMakerStructureTypeAdjacency.DisposeStructureTypes(t.cityData.GetSelectedStructureTypes(),
            t.cityData.matrix);
    }
}