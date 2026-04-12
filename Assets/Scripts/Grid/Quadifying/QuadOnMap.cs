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

    public QuadOnMap(Vector2Int widthHeight)
    {
        data.WidthHeight = widthHeight;
    }

    public void Setup(Vector2Int[] points, Vector2Int[] neighbors)
    {
        data.Coords = points;
        data.Neighbors = neighbors;
        SetCenter();
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
}

public class SquareOnMap : QuadOnMap
{
    public SquareOnMap(int pow) : base(new Vector2Int(pow, pow))
    {
    }
}