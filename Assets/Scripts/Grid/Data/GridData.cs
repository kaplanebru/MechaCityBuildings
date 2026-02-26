using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridData: IGridRelatedData
{
    public Transform OriginWorldTransform;
    [HideInInspector] public float CellSize;
    [HideInInspector] public Vector2Int AdaptiveGridSize;
}


public interface IGridRelatedData {}