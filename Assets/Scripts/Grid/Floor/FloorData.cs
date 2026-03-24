using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class FloorResidentsData
{
    public int Index;
    public List<Vector2Int> OccupiedCells = new();
    public List<PlacementData> PlacementDataset = new();
    public Structure[] Structures;

    public FloorResidentsData(int index)
    {
        Index = index;
    }
}


[System.Serializable]
public class FloorData
{
    public int Index;
    public Transform Root;
    //public List<Vector2Int> OccupiedCells = new();

    public float GetFloorHeight(int averageBuildingHeight) => Index * averageBuildingHeight;

    //public HashSet<Vector2Int> GetCells() => OccupiedCells.ToHashSet();
    public FloorData(int index, Transform root, int averageBuildingHeight)
    {
        Index = index;
        Root = root;

        ImplementFloorHeight(averageBuildingHeight);
    }

    public void ImplementFloorHeight(int averageBuildingHeight)
    {
        var pos = Root.localPosition;
        pos.y = GetFloorHeight(averageBuildingHeight);
        Root.localPosition = pos;
    }

   

}