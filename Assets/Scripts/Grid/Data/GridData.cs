using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Grid Data", menuName = "CityBuilder/Grid Data")]
public class GridData: ScriptableObject
{
    public int MinBuildingCellSize = 2;
    public int AverageBuildingHeight = 2;
    
    [HideInInspector] public Transform OriginWorldTransform;
    [HideInInspector] public Vector2Int AdaptiveGridSize;

}


