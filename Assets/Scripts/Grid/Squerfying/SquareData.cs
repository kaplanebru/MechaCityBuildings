using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class SquareData
{
    public int Pow;
    public Vector2Int IndexPoint;
    public Vector2Int[] Slots;
    public Vector2 Center;

    public SquareData(int pow, Vector2Int indexPoint)
    {
        Pow = pow;
        IndexPoint = indexPoint;
        Slots = Squarefyer.GetSquareSlotsByPoint(Pow, IndexPoint).ToArray();
        SetCenter();
    }

    private void SetCenter()
    {
        Vector2 sum = Slots.Aggregate(Vector2.zero, (current, slot) => current + slot);
        Center = sum / Slots.Length;
    }
}