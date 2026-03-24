using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class CityBuilder : MonoBehaviour
{
    public Randomizer randomizer;
    public Installer installer;
    public FloorResidentsDatabase floorResidentsDb;
    public FloorDatabase floorDb;
    [SerializeField] private CityDrawer cityDrawer;
    
    private void OnEnable()
    {
        cityDrawer.OnCellsReady += SetFloorResidentsAndInstall;
        floorDb.OnFloorClearRequest += ClearResidentsOnFloor;
        floorDb.OnFloorCreated += AddFloorResidentsData;
        floorDb.OnLastFloorRemoved += RemoveLastFloorResidentsData;
    }

   

    private void OnDisable()
    {
        cityDrawer.OnCellsReady -= SetFloorResidentsAndInstall;
        floorDb.OnFloorClearRequest -= ClearResidentsOnFloor;
        floorDb.OnFloorCreated -= AddFloorResidentsData;
        floorDb.OnLastFloorRemoved -= RemoveLastFloorResidentsData;
    }

    private void SetFloorResidentsAndInstall(int floorIndex, HashSet<Vector2Int> cells, HashSet<CellWorldData> worldCells)
    {
#if UNITY_EDITOR
        Undo.RecordObject(floorResidentsDb,"Installment From Cell");
        Undo.RecordObject(installer, "Installment From Cell");
        
        floorResidentsDb.RegisterCells(floorIndex, cells.ToList());
        randomizer.SetPlacementsOnFloor(worldCells, floorIndex);
        randomizer.MixAndApplyPlacements(floorIndex);

        var floorData = floorDb.GetFloorData(floorIndex);
        installer.InstallStructures(floorData);

        EditorUtility.SetDirty(floorResidentsDb);
        EditorUtility.SetDirty(installer);

        //if (!Application.isPlaying)
            //UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
    }

    public void RandomizeAndInstallTotalZone()
    {
#if UNITY_EDITOR
        
        Undo.RecordObject(floorResidentsDb,"Installment From Cell");
        Undo.RecordObject(installer, "Installment From Cell");

        foreach (var floorData in floorDb.FloorDatas)
        {
            randomizer.MixAndApplyPlacements(floorData.Index);
            installer.InstallStructures(floorData);
        }
        
        EditorUtility.SetDirty(floorResidentsDb);
        EditorUtility.SetDirty(installer);
#endif
    }
    
    private void AddFloorResidentsData(FloorData floorData)
    {
        floorResidentsDb.AddFloorResidentsData(floorData);
    }
    private void ClearResidentsOnFloor(int floorIndex)
    {
        var structures = floorResidentsDb.ClearResidents(floorIndex);
        installer.ClearStructures(structures);
    }
    
    private void RemoveLastFloorResidentsData()
    {
        ClearResidentsOnFloor(floorDb.GetFloorCount()-1);
        floorResidentsDb.RemoveLastFloor();
    }

   
    public void InstallGivenArrangement()
    {
        //placement data with placement floors
        //do we also need floor data (maybe later)
        
    }
    
}