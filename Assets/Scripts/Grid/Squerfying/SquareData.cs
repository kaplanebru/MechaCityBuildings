using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class QuadData
{
    public Vector2Int WidthHeight;
    public Vector2Int IndexPoint;
    public Vector2Int[] Slots;
    public Vector2 Center;

    public QuadData(Vector2Int widthHeight, Vector2Int indexPoint)
    {
        WidthHeight = widthHeight;
        IndexPoint = indexPoint;
        Slots = Quadifyer.GetQuadSlotsByPoint(WidthHeight, IndexPoint).ToArray();
        SetCenter();
    }

    private void SetCenter()
    {
        Vector2 sum = Slots.Aggregate(Vector2.zero, (current, slot) => current + slot);
        Center = sum / Slots.Length;
    }
}

public class SquareData : QuadData
{
    public SquareData(int pow, Vector2Int indexPoint) : base(new Vector2Int(pow, pow), indexPoint)
    {
    }
}