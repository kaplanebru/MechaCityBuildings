using System;
using System.Linq;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(CityBuilder))]
public class CityBuilderEditor : Editor
{
    private CityBuilder t;
    private DispositionEditorHelper _dispositionHelper = new();
    public SavedDispositionDb savedDispositionDb;


    public override void OnInspectorGUI()
    {
        DrawStructureDispositionMatrix();
        
        EditorGUILayout.Space(8);
        
        if (GUILayout.Button("Randomize And Apply"))
        {
            CacheTarget();
            t.RandomizeAndInstallTotalZone();
        }

        EditorGUILayout.Space(8);
        if (GUILayout.Button("Add Arrangement"))
        {
            CacheArrangementHelper();
            _dispositionHelper.AddDisposition(savedDispositionDb);
        }

        CacheArrangementHelper();

        EditorGUILayout.Space(8);
        if (GUILayout.Button("Load or Remove Arrangement"))
        {
            CacheArrangementHelper();
            _dispositionHelper.ApplyOrRemoveArrangement(savedDispositionDb);
        }
        
        EditorGUILayout.Space(8);
        DrawDefaultInspector();
    }

    private void CacheTarget()
    {
        if (t == null)
            t = (CityBuilder)target; // Works for subclasses too
    }

    private void CacheArrangementHelper()
    {
        if (_dispositionHelper == null)
            _dispositionHelper = new DispositionEditorHelper();
    }
    
    private void DrawStructureDispositionMatrix()
    {
        CacheTarget();
        
        t.RestoreMatrixIfNeeded();
        MatrixMakerStructureTypeAdjacency.DisposeStructureTypes(t.mapOrganizer.cityData.GetSelectedStructureTypes(), t.mapOrganizer.adjacencyMatrixData);
    }

   
}