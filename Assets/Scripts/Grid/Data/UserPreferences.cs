using System;
using UnityEngine;

[Serializable]
public class UserPreferences
{
    [Header("Floor Settings")] public int AverageFloorHeight = 2;
    [Header("Grid Settings")] public int BuildingCellSize = 2;
    public bool UseMapSizeForGridSize = true;
    public Vector2Int ProjectedGridSize = new(100, 50);
    [Header("Dummy")] public Transform Dummy;
}

public static class Configurations
{
    public static UserPreferences UserPreferences {get; private set;}
    public static void SetData(UserPreferences userPref)
    {
        UserPreferences = userPref;
    }
}
