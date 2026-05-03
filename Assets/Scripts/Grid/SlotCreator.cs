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

            foreach (var point in quad.data.Neighbors) //missinglere burdan da bakılabilir
            {
                if(map.Contains(point))
                    slotData.Neighbors.Add(new NeighborCell(point));
            }

            slotData.SetType(quad.IsBoundary);

            if (quad.IsBoundary)
                slotData.Rotation = Quaternion.LookRotation(quad.GetForwardDirection(), Vector3.up);
            
            slotData.SetForwardDirection(quad.GetForwardDirection());
            slotData.SlotSize = quad.data.WidthHeight;
            slotData.Center = quad.Center;
            slotData.StructureType = quad.StructureType;

            slotDatas.Add(slotData);
        }
        
        return slotDatas;
    }
    
    
    private static void FindOrientationsForSlots(HashSet<SlotData> slotDatas)
    {
        var boundarySlots = CellRegistry.GetBoundaries(slotDatas);

        foreach (var boundarySlot in boundarySlots)
        {
            if (boundarySlot.OutwardNormal == Vector2Int.zero)
            {
                Debug.LogWarning("OutwardNormal is zero");
                continue;
            }
            
            Vector2Int tangent = new Vector2Int(
                -boundarySlot.OutwardNormal.y, 
                boundarySlot.OutwardNormal.x); //perpendicular
            
            Vector3 forward = new Vector3(tangent.x, 0f, tangent.y);
            boundarySlot.Rotation = Quaternion.LookRotation(forward, Vector3.up);
        }
    }

    
}