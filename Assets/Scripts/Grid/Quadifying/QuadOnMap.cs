using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class QuadOnMap
{
    public QuadData data = new();
    public Vector2Int StartPoint;
    public Vector2 Center;

    public QuadOnMap(Vector2Int widthHeight, Vector2Int startPoint)
    {
        data.WidthHeight = widthHeight;
        StartPoint = startPoint;
    }

    public void SetPoints(Vector2Int[] points, Vector2Int[] neighbors)
    {
        data.Coords = points;
        data.Neighbors = neighbors;
        SetCenter();
    }

    private void SetCenter()
    {
        Vector2 sum = data.Coords.Aggregate(Vector2.zero, (current, slot) => current + slot);
        Center = sum / data.Coords.Length;
    }
}

public class SquareOnMap : QuadOnMap
{
    public SquareOnMap(int pow, Vector2Int startPoint) : base(new Vector2Int(pow, pow), startPoint)
    {
    }
}