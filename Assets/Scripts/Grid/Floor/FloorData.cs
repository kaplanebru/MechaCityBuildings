using System.Collections.Generic;
using UnityEngine;

public class FloorData
{
    public int Index;
    public Transform Root;
    public Dictionary<Vector2Int, Transform> ItemsByCell = new();
    public float FloorGroundHeight => Index * Configurations.UserPreferences.AverageBuildingHeight;

    public FloorData(int index, Transform root)
    {
        Index = index;
        Root = root;
    }
}