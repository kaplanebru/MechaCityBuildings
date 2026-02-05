using System;
using UnityEngine;

[Serializable]
public class GridData: IGridRelatedData
{
    public Transform OriginWorldTransform;
    public float CellSize;           // kare hücre
    public Vector2Int GridSize;      // (width, height) => xCount, yCount
}

public interface IGridTool
{ 
    public void SetGridRelatedData(IGridRelatedData[] gridRelatedData);
    
    public void SetSecondaryTools(params IGridTool[] secondaryTools);
}

public interface IGridRelatedData {}