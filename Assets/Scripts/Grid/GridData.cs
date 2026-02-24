using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridData: IGridRelatedData
{
    public Transform OriginWorldTransform;
    public float CellSize { get; set; }        
    public Vector2Int AdaptiveGridSize { get; set; }             
}

public interface IGridTool
{ 
    public void SetGridRelatedData(Dictionary<GridDataType, IGridRelatedData> gridRelatedData);
    
    public void SetSecondaryTools(params IGridTool[] secondaryTools);
}

public interface IGridRelatedData {}