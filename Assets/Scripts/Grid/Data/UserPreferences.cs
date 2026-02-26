using System;
using UnityEngine;

[Serializable]
public class UserPreferences
{
    [Header("Floor Settings")]
    public int AverageBuildingHeight = 2;
    [Header("Grid Settings")]   
    public int BuildingCellSize = 2;
    public Transform OriginWorldTransform;
    
    [Header("Dummy")] 
    public Transform Dummy;
    
    [HideInInspector]public bool UseMapSizeForGridSize = true;
    [HideInInspector]public Vector2Int ProjectedGridSize = new(100, 50);
}

public static class Configurations
{
    public static UserPreferences UserPreferences { get; private set; }

    public static void SetData(UserPreferences userPref)
    {
        UserPreferences = userPref;
    }
}