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
    public Installer installer;
    public FloorResidentsDatabase floorResidentsDb;
    public FloorDatabase floorDb;
    public CityDrawer cityDrawer;
    public DispositionDb dispositionDb;



    private void OnEnable()
    {
        cityDrawer.OnCellsReady += RegisterMapOnFloorAndInstall;
        floorDb.OnFloorClearRequest += ClearStructuresOnFloor;
        floorDb.OnFloorCreated += AddFloorResidentsData;
        floorDb.OnLastFloorRemoved += RemoveLastFloorResidentsData;
    }
    private void OnDisable()
    {
        cityDrawer.OnCellsReady -= RegisterMapOnFloorAndInstall;
        floorDb.OnFloorClearRequest -= ClearStructuresOnFloor;
        floorDb.OnFloorCreated -= AddFloorResidentsData;
        floorDb.OnLastFloorRemoved -= RemoveLastFloorResidentsData;
    }
    
    private void RegisterMapOnFloorAndInstall(int floorIndex, HashSet<Vector2Int> cells)
    {
#if UNITY_EDITOR
        Undo.RecordObject(floorResidentsDb,"Map On Floor");
        Undo.RecordObject(installer, "Installment From Slot");

        floorResidentsDb.RegisterCells(floorIndex, cells.ToList());
        
        var slotDataSet = mapOrganizer.ToSlotData(cells);
        floorResidentsDb.RegisterSlots(floorIndex, slotDataSet.ToList());
        
        installer.InstallStructures(floorDb.GetFloorData(floorIndex), floorResidentsDb.GetFloor(floorIndex));
        
        EditorUtility.SetDirty(floorResidentsDb);
        EditorUtility.SetDirty(installer);
#endif
    }

    public void RandomizeAndInstallTotalZone()
    {
#if UNITY_EDITOR
        
        Undo.RecordObject(floorResidentsDb,"Installment From Cell");
        Undo.RecordObject(installer, "Installment From Cell");

        foreach (var floorData in floorDb.FloorDatas)
        {
            var floorIndex = floorData.Index;
            var floorResidents = floorResidentsDb.GetFloor(floorIndex);
            var cells = floorResidents.Cells.ToHashSet();
            
            //ClearStructuresOnFloor(floorIndex); 
            var structures = floorResidentsDb.ClearStructuresKeepCells(floorIndex);
            installer.ClearStructures(structures);
            
            
            floorResidentsDb.RegisterSlots(floorIndex, mapOrganizer.ToSlotData(cells).ToList());
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
    private void ClearStructuresOnFloor(int floorIndex)
    {
        var structures = floorResidentsDb.ClearResidents(floorIndex);
        installer.ClearStructures(structures);
    }
    
    private void RemoveLastFloorResidentsData()
    {
        floorResidentsDb.RemoveLastFloor();
    }
    
    public void RestoreMatrixIfNeeded()
    {
        int matrixSize = Mathf.RoundToInt(Mathf.Pow(mapOrganizer.GetSelectedStructureTypeAmount(), 2));
        if (mapOrganizer.adjacencyMatrixData == null || mapOrganizer.adjacencyMatrixData.Length != matrixSize)
        {
            mapOrganizer.adjacencyMatrixData = new bool[matrixSize];
        }
    }
    
    public DispositionData SaveCurrentDisposition(string dispositionName)
    {
        var slotDatasList = floorResidentsDb.floorResidents.Select(floorResidentData => floorResidentData.Slots).ToList();
        return new DispositionData(dispositionName, slotDatasList);
    }


    public void ResurrectDisposition(DispositionData dispositionData)
    {
        //TODO: DELETE ALL FLOORS
        for (var i = 0; i < floorResidentsDb.floorResidents.Count; i++)
        {
            floorResidentsDb.RegisterSlots(i, dispositionData.SlotsByFloor[i]);
            installer.InstallStructures(floorDb.GetFloorData(i), floorResidentsDb.GetFloor(i));
        }
        
        EditorUtility.SetDirty(floorResidentsDb);
        EditorUtility.SetDirty(installer);
    }

    
}