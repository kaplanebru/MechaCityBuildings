using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class FloorResidentsDatabase : MonoBehaviour
{
    public List<FloorResidentsData> floorResidents = new List<FloorResidentsData>();
    private ArrangementCache arrangementCache = new();

    public FloorResidentsData GetFloor(int floorIndex) => floorResidents[floorIndex];

    public void RegisterCells(int floorIndex, List<CellData> occupiedCells)
    {
        GetFloor(floorIndex).OccupiedCells = occupiedCells.ToList();
    }

    public void AddFloorResidentsData()
    {
        floorResidents.Add(new FloorResidentsData());
    }

    public HashSet<Structure> ClearResidents(int floorIndex)
    {
        var floorResidentsData = floorResidents[floorIndex];

        floorResidentsData.OccupiedCells.Clear();

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
                GetFloor(activeFloorIndex).OccupiedCells.Select(cd=>cd.CellIndex).ToHashSet(),
                GetFloor(lowerFloorData.Index).OccupiedCells.Select(cd=>cd.CellIndex).ToHashSet(),
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
            structuresByCell.TryAdd(structure.cellMetadata, structure);
        }

        return structuresByCell;
    }

    public void SaveCurrentArrangement(string arrangementName, int floorIndex)
    {
        arrangementCache.Add(arrangementName, floorResidents[floorIndex].OccupiedCells.ToArray());
    }


    public void ResurrectArrangement(string arrangementName)
    {
        arrangementCache.ResurrectArrangement(arrangementName);
    }


    /*public static void RestoreBuildingsOnFloor(FloorData floorData,GridData gridData)
    {
        HashSet<Vector2Int> keys = floorData.OccupiedCells.ToHashSet();
        foreach (var key in keys)
        {
            if (floorData.HasStructureOnCell(key, out var item)) continue;

            //floorData.SetItemOnCell(key, ConstructItem(key, floorData, gridData));
        }
    }*/
}