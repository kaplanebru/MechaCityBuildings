using System;
using System.Linq;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(CityBuilder))]
public class CityBuilderEditor : Editor
{
    private CityBuilder t;
    private DispositionEditorHelper _dispositionHelper = new();
    private string _newDispositionName;
    
    public override void OnInspectorGUI()
    {
        DrawStructureDispositionMatrix();
        CacheTarget();
        CacheDispositionEditorHelper();

        EditorGUILayout.Space(8);

        if (GUILayout.Button("Randomize And Apply"))
        {
            t.RandomizeAndInstallTotalZone();
        }
        EditorGUILayout.Space(8);
        
        if (GUILayout.Button("Initiate Pools"))
        {
            CacheTarget();
            t.installer.InitiatePools();
            
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
            InstallerEditorHelper.RefreshPools(t.installer);
        }

        EditorGUILayout.Space(8);
        
        using (new EditorGUILayout.HorizontalScope())
        {
            _newDispositionName = EditorGUILayout.TextField(
                new GUIContent("New Disposition Name"),
                _newDispositionName
            );
            SaveDisposition();
        }

        EditorGUILayout.Space(8);
        
        ApplyOrRemoveDisposition();
        
        
        EditorGUILayout.Space(8);
        DrawDefaultInspector();
    }

    private void CacheTarget()
    {
        if (t == null)
            t = (CityBuilder)target; // Works for subclasses too
    }

    private void ResetEverything()
    {
        var count = t.floorDb.GetFloorCount();
        for (int i = 0; i < count; i++)
        {
            t.ClearFloorResidentsData(i);
        }
        t.floorDb.SetActiveFloor(0);
    }

    private void CacheDispositionEditorHelper()
    {
        if (_dispositionHelper == null)
            _dispositionHelper = new DispositionEditorHelper();
    }

    private void SaveDisposition()
    {
        if (GUILayout.Button("Save", GUILayout.Width(80)))
        {
            if (t.dispositionDb.IsNameTaken(_newDispositionName))
            {
                Debug.LogWarning($"Name {_newDispositionName} is already taken");
            }
            else
            {
                var disposition = t.SaveCurrentDisposition(_newDispositionName);
                t.dispositionDb.AddDisposition(disposition);
                EditorUtility.SetDirty(t.dispositionDb);
            }
        }
    }
    
    public void ApplyOrRemoveDisposition()
    {
        string selectedName = _dispositionHelper.ShowDispositions(t.dispositionDb);

        using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(selectedName)))
        {
            if (GUILayout.Button("Resurrect Disposition"))
            {
                if (t.dispositionDb.TryGetDataByName(selectedName, out var dispositionData))
                {
                    //todo: clear first
                    FloorManagement.DeleteEveryFloor(t.floorDb);
                    t.ResurrectDisposition(dispositionData);

                    //if (!Application.isPlaying)
                    //UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }
            }

            if (GUILayout.Button("Remove Disposition"))
            {
                if (t.dispositionDb.TryGetDataByName(selectedName, out var dispositionData))
                {
                    t.dispositionDb.RemoveDisposition(dispositionData);
                    EditorUtility.SetDirty(t.dispositionDb);
                }
            }
        }
    }

    private void DrawStructureDispositionMatrix()
    {
        CacheTarget();

        t.RestoreMatrixIfNeeded();
        MatrixMakerStructureTypeAdjacency.DisposeStructureTypes(t.mapOrganizer.cityData.GetSelectedStructureTypes(),
            t.mapOrganizer.adjacencyMatrixData);
    }
}