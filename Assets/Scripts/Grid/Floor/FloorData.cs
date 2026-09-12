using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class FloorResidentsData
{
    public List<Vector2Int> Cells = new();
    public List<SlotData> Slots = new(); //aynı düzenin tekrarı için gerekecektir, save disposition için
    public List<Structure> Structures = new();
    
    public void RegisterStructures(List<Structure> structures) => Structures.AddRange(structures);
    public void RegisterCells(List<Vector2Int> cells)=>  Cells.AddRange(cells);
    public void RegisterSlots(List<SlotData> slots) => Slots.AddRange(slots);
   
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