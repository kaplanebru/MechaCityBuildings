using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class QuadOnMap
{
    public QuadData data = new();
    public Vector2 Center;
    public int Perimeter;
    public StructureType StructureType;
    public bool IsBoundary;
    private Vector2Int MissingNeighborSum;

    public QuadOnMap(Vector2Int widthHeight)
    {
        data.WidthHeight = widthHeight;
    }

    public void Setup(Vector2Int[] points, Vector2Int[] neighbors, StructureType structureType, Vector2Int missingNeighborSum)
    {
        data.Coords = points;
        data.Neighbors = neighbors;
        StructureType = structureType;
        MissingNeighborSum = missingNeighborSum;
        IsBoundary = !missingNeighborSum.Equals(Vector2Int.zero);
        SetCenter();
        //SetPerimeter();
    }

    private void SetCenter()
    {
        Vector2 sum = data.Coords.Aggregate(Vector2.zero, (current, slot) => current + slot +  Vector2.one/2f);
        Center = sum / data.Coords.Length;
    }

    private void SetPerimeter()
    {
        Perimeter = (data.WidthHeight.x + data.WidthHeight.y) * 2;
    }

    public Vector3 GetForwardDirection()
    {
        var outwardNormal = new Vector2Int(
            Math.Sign(MissingNeighborSum.x),
            Math.Sign(MissingNeighborSum.y));
        
        Vector2Int tangent = new Vector2Int(-outwardNormal.y, outwardNormal.x); //perpendicular
        Vector3 forward = new Vector3(tangent.x, 0f, tangent.y);

        return forward;
    }
}

public class SquareOnMap : QuadOnMap
{
    public SquareOnMap(int pow) : base(new Vector2Int(pow, pow))
    {
    }
}