using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class CityBuilder : MonoBehaviour
{
    public ReferenceHolder units;
    public CityData cityData;
   
    
    private void OnEnable()
    {
        units.CityDrawer.OnCellsReady += RegisterMapOnFloorAndInstall;
        units.FloorDb.OnFloorClearRequest += ClearStructuresOnFloor;
        units.FloorDb.OnFloorCreated += AddFloorResidentsData;
        units.FloorDb.OnLastFloorRemoved += RemoveLastFloorResidentsData;
    }
    private void OnDisable()
    {
        units.CityDrawer.OnCellsReady -= RegisterMapOnFloorAndInstall;
        units.FloorDb.OnFloorClearRequest -= ClearStructuresOnFloor;
        units.FloorDb.OnFloorCreated -= AddFloorResidentsData;
        units.FloorDb.OnLastFloorRemoved -= RemoveLastFloorResidentsData;
    }
    
    private void RegisterMapOnFloorAndInstall(int floorIndex, HashSet<Vector2Int> cells)
    {
#if UNITY_EDITOR
        Undo.RecordObject(units.FloorResidentsDb,"Map On Floor");
        Undo.RecordObject(units.Installer, "Installment From Slot");

        units.FloorResidentsDb.RegisterCellsForFloor(floorIndex, cells.ToList());
        
        var slotDataSet = units.MapOrganizer.ToSlotData(cells, cityData);
        var floorResidents = units.FloorResidentsDb.GetFloor(floorIndex);
        
        PreventDuplicates(floorResidents, slotDataSet);
        units.FloorResidentsDb.RegisterSlotsForFloor(floorIndex, slotDataSet.ToList());
        
        units.Installer.InstallStructures(units.FloorDb.GetFloorData(floorIndex), floorResidents, slotDataSet.ToList());
        
        EditorUtility.SetDirty(units.FloorResidentsDb);
        EditorUtility.SetDirty(units.Installer);
#endif
    }

    private void PreventDuplicates(FloorResidentsData floorResidents, HashSet<SlotData> slotDataSet)
    {
        foreach (var slot in floorResidents.Slots)
        {
            if (slotDataSet.Contains(slot))
                slotDataSet.Remove(slot);
        }
    }

    public void RandomizeAndInstallTotalZone()
    {
#if UNITY_EDITOR
        
        Undo.RecordObject(units.FloorResidentsDb,"Installment From Cell");
        Undo.RecordObject(units.Installer, "Installment From Cell");

        foreach (var floorData in units.FloorDb.FloorDatas)
        {
            var floorIndex = floorData.Index;
            var floorResidents = units.FloorResidentsDb.GetFloor(floorIndex);
            var cells = floorResidents.Cells.ToHashSet();
            
            var structures = units.FloorResidentsDb.ClearStructuresKeepCells(floorIndex);
            units.Installer.ClearStructures(structures);
            
            units.FloorResidentsDb.RegisterSlotsForFloor(floorIndex, units.MapOrganizer.ToSlotData(cells, cityData).ToList());
            units.Installer.InstallStructures(floorData, units.FloorResidentsDb.GetFloor(floorData.Index));
        }
        
        EditorUtility.SetDirty(units.FloorResidentsDb);
        EditorUtility.SetDirty(units.Installer);
#endif
    }
    
    private void AddFloorResidentsData()
    {
        units.FloorResidentsDb.AddFloorResidentsData();
    }
    public void ClearStructuresOnFloor(int floorIndex)
    {
        var structures = units.FloorResidentsDb.ClearResidents(floorIndex);
        units.Installer.ClearStructures(structures);
    }

    public void ClearFloorResidentsData(int floorIndex)
    {
        ClearStructuresOnFloor(floorIndex);
        units.FloorResidentsDb.ClearResidents(floorIndex);
        if (floorIndex > 0)
        {
            units.FloorDb.OnFloorClearRequest?.Invoke(floorIndex);
        }
    }
    private void RemoveLastFloorResidentsData()
    {
        units.FloorResidentsDb.RemoveLastFloor();
    }
    
    public void RestoreMatrixIfNeeded()
    {
        int matrixSize = Mathf.RoundToInt(Mathf.Pow(cityData.RandomizerDataSet.Length, 2));

        if (cityData.matrix == null || cityData.matrix.Length != matrixSize)
        {
            cityData.matrix = new bool[matrixSize];
        }
    }
}