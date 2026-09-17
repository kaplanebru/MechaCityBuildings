using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

//TODO: bug fix EVEN AFTER DELETING POOL AND STRUCTURE 3 POOL, WHEN CELANING ALL AND MAKİNG A NEW CONSTRUCTION SYSTEM ASSUMES SLOT 3 EVENT THERES NO POOL 3
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
        units.Installer.OnStructureRemovalRequest += RemoveStructureDataFromAllFloors;
    }

    private void OnDisable()
    {
        units.CityDrawer.OnCellsReady -= RegisterMapOnFloorAndInstall;
        units.FloorDb.OnFloorClearRequest -= ClearStructuresOnFloor;
        units.FloorDb.OnFloorCreated -= AddFloorResidentsData;
        units.FloorDb.OnLastFloorRemoved -= RemoveLastFloorResidentsData;
        units.Installer.OnStructureRemovalRequest -= RemoveStructureDataFromAllFloors;
    }

    private void RegisterMapOnFloorAndInstall(int floorIndex, HashSet<Vector2Int> cells)
    {
#if UNITY_EDITOR
        Undo.RecordObject(units.FloorResidentsDb, "Map On Floor");
        Undo.RecordObject(units.Installer, "Installment From Slot");

        if (!TryCellToSlotData(cells,
                out var slotDataSet)) //var slotDataSet = units.MapOrganizer.ToSlotData(cells, cityData);
            return;
        
        units.FloorResidentsDb.RegisterCellsForFloor(floorIndex, cells.ToList());
        var floorResidents = units.FloorResidentsDb.GetFloor(floorIndex);

        PreventDuplicates(floorResidents, slotDataSet);
        units.FloorResidentsDb.RegisterSlotsForFloor(floorIndex, slotDataSet.ToList());

        units.Installer.InstallStructures(units.FloorDb.GetFloorData(floorIndex), floorResidents, slotDataSet.ToList());

        EditorUtility.SetDirty(units.FloorResidentsDb);
        EditorUtility.SetDirty(units.Installer);
#endif
    }

    public void RandomizeSelectedFloor(int floorIndex)
    {
        int floorAmount = units.FloorDb.FloorDatas.Count;
        if (floorIndex > floorAmount || floorIndex < 0)
        {
            Debug.LogWarning("Selected floor index is out of range.");
            return;
        }
#if UNITY_EDITOR

        Undo.RecordObject(units.FloorResidentsDb, "Installment From Cell" + floorIndex);
        Undo.RecordObject(units.Installer, "Installment From Cell" + floorIndex);

        var floorResidents = units.FloorResidentsDb.GetFloor(floorIndex);
        var cells = floorResidents.Cells.ToHashSet();

        if(!TryCellToSlotData(cells, out var slotDataSet)) //var slotDataSet = TryCellToSlotData(cells);
            return;
        
        var floorData = units.FloorDb.GetFloorData(floorIndex);
        var structures = units.FloorResidentsDb.ClearStructuresDataKeepCells(floorIndex);
        units.Installer.ClearStructuresPhysically(structures);

        units.FloorResidentsDb.RegisterSlotsForFloor(floorIndex,
            slotDataSet.ToList()); //units.MapOrganizer.ToSlotData(cells, cityData)
        units.Installer.InstallStructures(floorData, units.FloorResidentsDb.GetFloor(floorData.Index));

        EditorUtility.SetDirty(units.FloorResidentsDb);
        EditorUtility.SetDirty(units.Installer);
#endif
    }

    public void RandomizeAndInstallTotalZone()
    {
        foreach (var floorData in units.FloorDb.FloorDatas)
        {
            var floorIndex = floorData.Index;
            RandomizeSelectedFloor(floorIndex);
        }
    }

    private void AddFloorResidentsData()
    {
        units.FloorResidentsDb.AddFloorResidentsData();
    }

    public void ClearStructuresOnFloor(int floorIndex)
    {
        var structures = units.FloorResidentsDb.ClearResidentsData(floorIndex);
        units.Installer.ClearStructuresPhysically(structures);
    }

    public void ClearFloorResidentsData(int floorIndex)
    {
        ClearStructuresOnFloor(floorIndex);
        units.FloorResidentsDb.ClearResidentsData(floorIndex);
        if (floorIndex > 0)
        {
            units.FloorDb.OnFloorClearRequest?.Invoke(floorIndex);
        }
    }

    private void RemoveLastFloorResidentsData()
    {
        units.FloorResidentsDb.RemoveLastFloor();
    }

    public void RestoreMatrixSizeIfNeeded()
    {
        var typesCount = cityData.GetStructureTypes().Count;
        int matrixSize = Mathf.RoundToInt(Mathf.Pow(typesCount, 2));

        if (cityData.matrix == null)
        {
            Debug.LogWarning("MATRIX IS NULL");
            cityData.matrix = new bool[matrixSize];
        }
    }

    private bool TryCellToSlotData(HashSet<Vector2Int> cells, out HashSet<SlotData> slotDataSet)
    {
        HashSet<StructureType> cityTypes = cityData.GetStructureTypes();
        var pools = units.Installer.pools;
        slotDataSet = null;
        
        HashSet<StructureType> poolTypes = new();
        pools.ForEach(pool => poolTypes.Add(pool.poolData.StructureType));

        foreach (var cityType in cityTypes)
        {
            if (!poolTypes.Contains(cityType))
            {
                Debug.LogWarning(cityType + " structure type can't be found in any pool");
                return false;
                //cityData.RemoveStructureType(cityType);
            }
        }
        
        //units.MapOrganizer.MatchRandomizerWithPool(ref cityData, activeTypes);
        slotDataSet = units.MapOrganizer.ToSlotData(cells, cityData);
        return true;
    }

    private void PreventDuplicates(FloorResidentsData floorResidents, HashSet<SlotData> slotDataSet)
    {
        foreach (var slot in floorResidents.Slots)
        {
            if (slotDataSet.Contains(slot))
                slotDataSet.Remove(slot);
        }
    }

    private void RemoveStructureDataFromAllFloors(StructureType structureType)
    {
        var structures = units.FloorResidentsDb.RemoveStructureDataFrom_AllFloors(structureType);

        Debug.Log("STRUCTURES TO REMOVE: " + structures.Count());
        units.Installer.ClearStructuresPhysically(structures);
    }
}