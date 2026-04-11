using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class CityBuilder : MonoBehaviour
{
    public MapOrganizer mapOrganizer;
    public Randomizer randomizer;
    public Installer installer;
    public FloorResidentsDatabase floorResidentsDb;
    public FloorDatabase floorDb;
    public CityDrawer cityDrawer;

    private void OnEnable()
    {
        cityDrawer.OnCellsReady += RegisterMapOnFloorAndInstall;
        floorDb.OnFloorClearRequest += ClearResidentsOnFloor;
        floorDb.OnFloorCreated += AddFloorResidentsData;
        floorDb.OnLastFloorRemoved += RemoveLastFloorResidentsData;
    }
    private void OnDisable()
    {
        cityDrawer.OnCellsReady -= RegisterMapOnFloorAndInstall;
        floorDb.OnFloorClearRequest -= ClearResidentsOnFloor;
        floorDb.OnFloorCreated -= AddFloorResidentsData;
        floorDb.OnLastFloorRemoved -= RemoveLastFloorResidentsData;
    }
    
    private void RegisterMapOnFloorAndInstall(int floorIndex, HashSet<Vector2Int> cells)
    {
#if UNITY_EDITOR
        Undo.RecordObject(floorResidentsDb,"Map On Floor");
        Undo.RecordObject(installer, "Installment From Slot");

        floorResidentsDb.RegisterCells(floorIndex, cells.ToList());
        
        //TODO: frequency thing here
        var slotDataSet = mapOrganizer.ToSlotData(cells);//todo can register slotdata but who cares, maybe for optimization?
        floorResidentsDb.RegisterSlots(floorIndex, slotDataSet.ToList());
        
        installer.InstallStructures(floorDb.GetFloorData(floorIndex), floorResidentsDb.GetFloor(floorIndex));
        
        EditorUtility.SetDirty(floorResidentsDb);
        EditorUtility.SetDirty(installer);
#endif
    }

    private void RegisterFloorResidentsAndInstall(int floorIndex, HashSet<SlotData> slots)
    {
#if UNITY_EDITOR
        Undo.RecordObject(floorResidentsDb,"Installment From Cell");
        Undo.RecordObject(installer, "Installment From Cell");

        FloorResidentsData floorResidentsData = floorResidentsDb.GetFloor(floorIndex);
        
        floorResidentsDb.RegisterSlots(floorIndex, slots.ToList());
        randomizer.OrderCellsOnFloor(slots, floorResidentsData);
        //randomizer.MixAndApplyPlacements(floorResidentsData);
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
    
    public void InitiateMatrixIfNeeded()
    {
        int matrixSize = Mathf.RoundToInt(Mathf.Pow(mapOrganizer.structureTypeDatas.Length, 2));
        if (mapOrganizer.adjacency == null || mapOrganizer.adjacency.Length != matrixSize)
        {
            mapOrganizer.adjacency = new bool[matrixSize];
        }
    }
    public void InstallGivenArrangement()
    {
        //placement data with placement floors
        //do we also need floor data (maybe later)
    }
    
}