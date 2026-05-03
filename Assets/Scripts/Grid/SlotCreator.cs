using System;
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

            var missingNeighborSum = Vector2Int.zero;
            foreach (var point in quad.data.Neighbors) //missinglere burdan da bakılabilir
            {
                if(map.Contains(point))
                    slotData.Neighbors.Add(new NeighborCell(point));
                else
                    missingNeighborSum += point;
            }

            slotData.SetType(missingNeighborSum != Vector2Int.zero);

            if (slotData.Type == SlotType.Boundary)
                slotData.Rotation = Quaternion.LookRotation(GetForwardDirection(missingNeighborSum), Vector3.up);
            
            slotData.SlotSize = quad.data.WidthHeight;
            slotData.Center = quad.Center;
            slotData.StructureType = quad.StructureType;

            slotDatas.Add(slotData);
        }
        
        return slotDatas;
    }
    
    public static Vector3 GetForwardDirection(Vector2Int missingNeighborSum)
    {
        var outwardNormal = new Vector2Int(
            Math.Sign(missingNeighborSum.x),
            Math.Sign(missingNeighborSum.y));
        
        Vector2Int tangent = new Vector2Int(-outwardNormal.y, outwardNormal.x); //perpendicular
        Vector3 forward = new Vector3(tangent.x, 0f, tangent.y);

        return forward;
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