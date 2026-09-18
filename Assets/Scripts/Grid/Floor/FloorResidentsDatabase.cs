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

    public HashSet<Structure> ClearStructuresDataKeepCells(int floorIndex)
    {
        var floorResidentsData = floorResidents[floorIndex];
        
        floorResidentsData.Slots.Clear();
        HashSet<Structure> tempStructures = new(floorResidentsData.Structures);
        floorResidentsData.Structures.Clear();
        return tempStructures;
    }

    public HashSet<Structure> ClearResidentsData(int floorIndex)
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
            structuresByCell.TryAdd(structure.cellMetaData, structure);
        }
        return structuresByCell;
    }

    
    public HashSet<Structure> RemoveStructureDataFrom_AllFloors(StructureType structureType)
    {
        HashSet<Structure> cachedStructures = new();
        foreach (var floorResidentData in floorResidents)
        {
            cachedStructures.UnionWith(RemoveStructureDataFrom_SingleFloor(structureType, floorResidentData));
        }
        
        return cachedStructures;
    }

    public HashSet<Structure> GetSelectedStructuresFromAllFloors(StructureType structureType, bool clear = true)
    {
        HashSet<Structure> totalStructures = new();
        for (var i = 0; i < floorResidents.Count; i++)
        {
            var floorResidentData = floorResidents[i];
            var relatedStructures = floorResidentData.Structures.Where(sd => sd.type == structureType).ToHashSet();
            totalStructures.UnionWith(relatedStructures);

            for (var j = relatedStructures.Count - 1; j >= 0; j--)
            {
                var relatedStructure = floorResidentData.Structures[j];
                
                if(clear)
                    floorResidentData.Structures.Remove(relatedStructure);
            }
        }

        return totalStructures;
    }

    public HashSet<Structure> RemoveStructureDataFrom_SingleFloor(StructureType structureType, FloorResidentsData floorResidentData)
    {
        var relatedSlots = floorResidentData.Slots.Where(f => f.StructureType == structureType).ToHashSet();
        var relatedStructures = floorResidentData.Structures.Where(sd => sd.type == structureType).ToHashSet();
        HashSet<Vector2Int> relatedCells = new();
        HashSet<Structure> cachedStructures = new();

        foreach (var relatedSlot in relatedSlots)
        {
            relatedCells.UnionWith(relatedSlot.Cells);
        }

        foreach (var relatedCell in relatedCells)
        {
            floorResidentData.Cells.Remove(relatedCell);
        }
        
        foreach (var slot in relatedSlots)
        {
            floorResidentData.Slots.Remove(slot);
        }
        
        foreach (var structure in relatedStructures)
        {
            cachedStructures.Add(structure);
            floorResidentData.Structures.Remove(structure);
        }
        Debug.Log("cached structutrues: " + cachedStructures.Count);
        return cachedStructures;
    }
}