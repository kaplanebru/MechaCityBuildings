using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Grid Data", menuName = "CityBuilder/Grid Data")]
public class GridData: ScriptableObject, IGridRelatedData
{
    public int BuildingCellSize = 2;
    public int AverageBuildingHeight = 2;
    
    [HideInInspector] public Transform OriginWorldTransform;
    [HideInInspector] public Vector2Int AdaptiveGridSize;

}


public interface IGridRelatedData {}