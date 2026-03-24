using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class FloorResidentsDatabase : MonoBehaviour
{
    public List<FloorResidentsData> residentsFloor = new List<FloorResidentsData>();
    private ArrangementCache arrangementCache = new(); 
    
    public FloorResidentsData GetFloor(int floorIndex) => residentsFloor[floorIndex];
    public void RegisterCells(int floorIndex, List<Vector2Int> occupiedCells)
    {
        residentsFloor[floorIndex].OccupiedCells = occupiedCells.ToList();
    }
    
    public void AddFloorResidentsData(FloorData floorData)
    {
        residentsFloor.Add(new FloorResidentsData(floorData.Index));
    }
    
    public HashSet<Structure> ClearResidents(int floorIndex)
    {
        var occupant =  residentsFloor[floorIndex];
        
            occupant.OccupiedCells.Clear();
            occupant.PlacementDataset.Clear();

            HashSet<Structure> tempStructures = new(occupant.Structures);
            occupant.Structures = null;
            return tempStructures;
    }
    
    public void RemoveLastFloor()
    {
        residentsFloor.RemoveAt(residentsFloor.Count - 1);
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
                residentsFloor[activeFloorIndex].OccupiedCells.ToHashSet(), 
                residentsFloor[lowerFloorData.Index].OccupiedCells.ToHashSet(),
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
        arrangementCache.Add(arrangementName, residentsFloor[floorIndex].PlacementDataset.ToArray());
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
