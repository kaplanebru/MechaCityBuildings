using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlotCreator
{
    public static HashSet<SlotData> CreateSlotDataFromQuads(QuadOnMap[] randomQuads, HashSet<Vector2Int> map)
    {
        HashSet<SlotData> slotDatas = new();

        foreach (var quad in randomQuads)
        {
            var slotData = new SlotData(quad.data.Coords);

            foreach (var point in quad.data.Neighbors)
            {
                if(map.Contains(point))
                    slotData.Neighbors.Add(new NeighborCell(point));
            }

            //çevresi kadar neighbor'u olur max
            slotData.Type = quad.data.Neighbors.Length < quad.Perimeter ? SlotType.Boundary : SlotType.Regular;
            slotData.SlotSize = quad.data.WidthHeight;
            slotData.Center = quad.Center;
            slotData.StructureType = quad.StructureType;

            slotDatas.Add(slotData);
        }
        
        //FindOrientationsForSingleCells(singleCellDatas.ToHashSet());
        
        return slotDatas;
    }
    
    
    private static void FindOrientationsForSingleCells(HashSet<SlotData> cellDataSet)
    {
        var boundaryCells = CellRegistry.GetBoundaries(cellDataSet);

        foreach (var boundaryCell in boundaryCells)
        {
            if (boundaryCell.OutwardNormal == Vector2Int.zero)
            {
                continue;
            }
            
            Vector2Int tangent = new Vector2Int(
                -boundaryCell.OutwardNormal.y, 
                boundaryCell.OutwardNormal.x); //perpendicular
            
            Vector3 forward = new Vector3(tangent.x, 0f, tangent.y);
            boundaryCell.Rotation = Quaternion.LookRotation(forward, Vector3.up);
        }
    }

    
}