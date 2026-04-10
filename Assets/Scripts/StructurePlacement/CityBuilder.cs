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
        cityDrawer.OnSlotsReady += RegisterFloorResidentsAndInstall;
        floorDb.OnFloorClearRequest += ClearResidentsOnFloor;
        floorDb.OnFloorCreated += AddFloorResidentsData;
        floorDb.OnLastFloorRemoved += RemoveLastFloorResidentsData;
    }
    private void OnDisable()
    {
        cityDrawer.OnSlotsReady -= RegisterFloorResidentsAndInstall;
        floorDb.OnFloorClearRequest -= ClearResidentsOnFloor;
        floorDb.OnFloorCreated -= AddFloorResidentsData;
        floorDb.OnLastFloorRemoved -= RemoveLastFloorResidentsData;
    }

    private void RegisterFloorResidentsAndInstall(int floorIndex, HashSet<SlotData> cells)
    {
#if UNITY_EDITOR
        Undo.RecordObject(floorResidentsDb,"Installment From Cell");
        Undo.RecordObject(installer, "Installment From Cell");

        FloorResidentsData floorResidentsData = floorResidentsDb.GetFloor(floorIndex);
        
        floorResidentsDb.RegisterCells(floorIndex, cells.ToList());
        randomizer.OrderCellsOnFloor(cells, floorResidentsData);
        randomizer.MixAndApplyPlacements(floorResidentsData);
        installer.InstallStructures(floorDb.GetFloorData(floorIndex), floorResidentsData);

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
            randomizer.MixAndApplyPlacements(floorResidentsDb.GetFloor(floorData.Index));
            installer.InstallStructures(floorData, floorResidentsDb.GetFloor(floorData.Index));
        }
        
        EditorUtility.SetDirty(floorResidentsDb);
        EditorUtility.SetDirty(installer);
#endif
    }
    
    private void AddFloorResidentsData()
    {
        floorResidentsDb.AddFloorResidentsData();
    }
    private void ClearResidentsOnFloor(int floorIndex)
    {
        var structures = floorResidentsDb.ClearResidents(floorIndex);
        installer.ClearStructures(structures);
    }
    
    private void RemoveLastFloorResidentsData()
    {
        //ClearResidentsOnFloor(floorDb.GetFloorCount()-1); already cleared on delete call
        floorResidentsDb.RemoveLastFloor();
    }

   
    public void InstallGivenArrangement()
    {
        //placement data with placement floors
        //do we also need floor data (maybe later)
        
    }
    
}