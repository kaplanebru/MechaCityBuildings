using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class QuadData
{
    public Vector2Int WidthHeight;
    public Vector2Int StartPoint;
    public Vector2Int[] Points;
    public Vector2 Center;
    public Vector2Int[] Neighbors;

    public QuadData(Vector2Int widthHeight)
    {
        WidthHeight = widthHeight;
    }

    public void Setup(
        Vector2Int startPoint,
        Vector2Int[] points,
        Vector2Int[] neighbors)
    {
        StartPoint = startPoint;
        Points = points;
        Neighbors = neighbors;
        SetCenter();
    }

    private void SetCenter()
    {
        Vector2 sum = Points.Aggregate(Vector2.zero, (current, slot) => current + slot);
        Center = sum / Points.Length;
    }
}

public class SquareData : QuadData
{
    public SquareData(int pow) : base(new Vector2Int(pow, pow))
    {
    }
}