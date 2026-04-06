using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class FloorResidentsData
{
    public List<CellData> OccupiedCells = new();
    public List<Structure> Structures = new();
}


[System.Serializable]
public class FloorData
{
    public int Index;
    public Transform Root;

    public float GetFloorHeight(int averageBuildingHeight) => Index * averageBuildingHeight;

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