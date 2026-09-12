using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class FloorResidentsDatabase : MonoBehaviour
{
    public List<FloorResidentsData> floorResidents = new List<FloorResidentsData>();

    public FloorResidentsData GetFloor(int floorIndex) => floorResidents[floorIndex];

    public void RegisterCellsForFloor(int floorIndex, List<Vector2Int> cells)
    {
        GetFloor(floorIndex).RegisterCells(cells);

    }
    public void RegisterSlotsForFloor(int floorIndex, List<SlotData> slots)
    {
        GetFloor(floorIndex).RegisterSlots(slots);
    }

    public void AddFloorResidentsData()
    {
        floorResidents.Add(new FloorResidentsData());
    }

    public HashSet<Structure> ClearStructuresKeepCells(int floorIndex)
    {
        var floorResidentsData = floorResidents[floorIndex];
        
        floorResidentsData.Slots.Clear();
        HashSet<Structure> tempStructures = new(floorResidentsData.Structures);
        floorResidentsData.Structures.Clear();
        return tempStructures;
    }

    public HashSet<Structure> ClearResidents(int floorIndex)
    {
        var floorResidentsData = floorResidents[floorIndex];

        floorResidentsData.Slots.Clear();
        floorResidentsData.Cells.Clear();

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