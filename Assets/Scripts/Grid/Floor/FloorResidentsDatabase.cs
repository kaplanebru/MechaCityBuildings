using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class FloorResidentsDatabase : MonoBehaviour
{
    public List<FloorResidentsData> floorResidents = new List<FloorResidentsData>();
    private DispositionRecorder _dispositionRecorder = new();

    public FloorResidentsData GetFloor(int floorIndex) => floorResidents[floorIndex];

    public void RegisterCells(int floorIndex, List<Vector2Int> cells)
    {
        GetFloor(floorIndex).Cells = cells;
    }
    public void RegisterSlots(int floorIndex, List<SlotData> slots)
    {
        GetFloor(floorIndex).Slots.Clear();
        GetFloor(floorIndex).Slots = slots.ToList();
    }

    public void AddFloorResidentsData()
    {
        floorResidents.Add(new FloorResidentsData());
    }

    public HashSet<Structure> ClearResidents(int floorIndex)
    {
        var floorResidentsData = floorResidents[floorIndex];

        floorResidentsData.Slots.Clear();

        HashSet<Structure> tempStructures = new(floorResidentsData.Structures);
        floorResidentsData.Structures.Clear();
        return tempStructures;
    }

    public void RemoveLastFloor()
    {
        floorResidents.RemoveAt(floorResidents.Count - 1);
    }


    //TODO
    private bool TryDeconstructInvisibleIntersections(
        int activeFloorIndex,
        FloorDatabase floorDb,
        Structure[] structuresOnFloor,
        out HashSet<Structure> intersectingBuildings)
    {
        intersectingBuildings = null;

        if (floorDb.TryGetLowerFloorData(activeFloorIndex, out var lowerFloorData))
        {
            intersectingBuildings = FloorIntersectionMasker.GetIntersectionsUnderFloor(
                GetFloor(activeFloorIndex).Slots.SelectMany(cd=>cd.Cells).ToHashSet(),
                GetFloor(lowerFloorData.Index).Slots.SelectMany(cd=>cd.Cells).ToHashSet(),
                GetStructuresByCell(structuresOnFloor) //placementFloors[floorIndex].Structures;
            );
            return true;
        }

        return false;
    }


    private static Dictionary<Vector2Int, Structure> GetStructuresByCell(Structure[] structures)
    {
        Dictionary<Vector2Int, Structure> structuresByCell = new();

        foreach (var structure in structures)
        {
            structuresByCell.TryAdd(structure.slotMetadata, structure);
        }

        return structuresByCell;
    }

    public void SaveCurrentArrangement(string dispositionName)
    {
        //TODO: dictionart kaydedemiyor zaten
        List<List<SlotData>> slotDatasList = floorResidents.Select(floorResidentData => floorResidentData.Slots).ToList();
        DispositionData dispositionData = new DispositionData(dispositionName, slotDatasList);
        _dispositionRecorder.Add(dispositionData);
    }


    public void ResurrectArrangement(string arrangementName)
    {
        _dispositionRecorder.ResurrectArrangement(arrangementName);
    }


    /*public static void RestoreBuildingsOnFloor(FloorData floorData,GridData gridData)
    {
        HashSet<Vector2Int> keys = floorData.Slots.ToHashSet();
        foreach (var key in keys)
        {
            if (floorData.HasStructureOnCell(key, out var item)) continue;

            //floorData.SetItemOnCell(key, ConstructItem(key, floorData, gridData));
        }
    }*/
}