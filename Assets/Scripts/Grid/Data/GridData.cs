using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Grid Data", menuName = "CityBuilder/Grid Data")]
public class GridData: ScriptableObject, IGridRelatedData
{
    [HideInInspector] public Transform OriginWorldTransform;
    [HideInInspector] public float CellSize;
    [HideInInspector] public Vector2Int AdaptiveGridSize;
}


public interface IGridRelatedData {}