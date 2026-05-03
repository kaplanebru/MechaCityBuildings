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

            slotData.Rotation *= Quaternion.Euler(0, -90, 0);
            
            if (TryGetMissingNeighborNormalSum(quad, map, slotData, out var missingNeighborNormalSum))
            {
                slotData.SetType(true);
                slotData.Rotation = Quaternion.LookRotation(GetForwardDirection(missingNeighborNormalSum), Vector3.up);
            }
            else
                slotData.SetType(false);
            
            slotData.SlotSize = quad.data.WidthHeight;
            slotData.Center = quad.Center;
            slotData.StructureType = quad.StructureType;

            slotDatas.Add(slotData);
        }

        return slotDatas;
    }

    private static bool TryGetMissingNeighborNormalSum(QuadOnMap quad, HashSet<Vector2Int> map, SlotData slotData, out Vector2Int missingNeighborNormalSum)
    {
        missingNeighborNormalSum = Vector2Int.zero;
        foreach (var point in quad.data.Neighbors) //missinglere burdan da bakılabilir
        {
            if (map.Contains(point))
                slotData.Neighbors.Add(new NeighborCell(point));
            else
            {
                var pointCenter = new Vector2(point.x + 0.5f, point.y + 0.5f);
                var direction = pointCenter - quad.Center;
                
                var normal = new Vector2Int(Math.Sign(direction.x), Math.Sign(direction.y));
                missingNeighborNormalSum += normal;

                //Debug.Log($"point: {pointCenter}, center: {quad.Center}, direction: {direction}");
            }
        }
        return missingNeighborNormalSum != Vector2Int.zero;
    }

    private static Vector3 GetForwardDirection(Vector2Int missingNeighborSum)
    {
        var outwardNormal = new Vector2Int(
            Math.Sign(missingNeighborSum.x),
            Math.Sign(missingNeighborSum.y));
        
        bool isCorner = outwardNormal.x != 0 && outwardNormal.y != 0;
        if(isCorner)
            outwardNormal = GetFacingDirectionForCorners(missingNeighborSum);
        
        Vector2Int tangent = new Vector2Int(outwardNormal.y, -outwardNormal.x); //perpendicular
        var forward = new Vector3(tangent.x, 0f, tangent.y);
        //var forward = new Vector3(outwardNormal.y, 0f, -outwardNormal.x);
        
        //TODO: haritanın tersliğini düzelt, ve haritanın tersliğine göre rotation optionları ekle
        
        return forward;
    }
    
    private static Vector2Int GetFacingDirectionForCorners(Vector2Int outwardNormal)
    {
        if (Math.Abs(outwardNormal.x) >= Math.Abs(outwardNormal.y))
            return new Vector2Int(Math.Sign(outwardNormal.x), 0);
        
        return new Vector2Int(0, Math.Sign(outwardNormal.y));
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