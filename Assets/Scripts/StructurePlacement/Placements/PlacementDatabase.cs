using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[ExecuteInEditMode]
public class PlacementDatabase: MonoBehaviour
{
    [SerializeField] private FloorManagement floorManagement;
    public List<PlacementFloor> placementFloors = new();
    
    private ArrangementCache arrangementCache = new(); 
    //todo: all data will be lost, if its not writing in json,
    //or using parameters instead of variable caches

    
    public void OnEnable()
    {
        floorManagement.OnFloorCreated += IncreasePlacementFloors;
        floorManagement.OnLastFloorRemoved += DecreaseLastPlacementFloor;
    }

    public void OnDisable()
    {
        floorManagement.OnFloorCreated -= IncreasePlacementFloors;
        floorManagement.OnLastFloorRemoved -= DecreaseLastPlacementFloor;
    }
    
    private void DecreaseLastPlacementFloor()
    {
        placementFloors.Remove(placementFloors.Last());
    }

    private void IncreasePlacementFloors(FloorData floorData)
    {
        placementFloors.Add(new PlacementFloor(floorData));
    }
    
    public int GetPlacementFloorCount() => placementFloors.Count;

    public PlacementFloor GetPlacementFloor(int floorIndex)
    {
        if (placementFloors[floorIndex] == null)
        {
            Debug.LogError("No placement floor was found with index " + floorIndex);
            return null;
        }
        return placementFloors[floorIndex];
    }
    
    public void SaveCurrentArrangement(string arrangementName, int floorIndex)
    {
        arrangementCache.Add(arrangementName, placementFloors[floorIndex].PlacementDataset.ToArray());
    }


    public void ResurrectArrangement(string arrangementName)
    {
        arrangementCache.ResurrectArrangement(arrangementName);
    }
    
    private static bool TryDeconstructInvisibleIntersections(
        FloorData activeFloorData, 
        FloorDatabase floorDb,
        Structure[] structuresOnFloor,
        out HashSet<Structure> intersectingBuildings)
    {
        intersectingBuildings = null;
        int floorIndex = activeFloorData.FloorIdentifier.Index;
        if (floorDb.TryGetLowerFloorData(floorIndex, out var lowerFloorData))
        {
            intersectingBuildings = FloorIntersectionMasker.GetIntersectionsUnderFloor(
                activeFloorData, 
                lowerFloorData,
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