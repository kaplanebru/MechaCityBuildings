using System;
using UnityEngine;

[Serializable]
public class GridData: IGridRelatedData
{
    public Vector3 OriginWorld;      // x,z önemli; y referans (0 olabilir)
    public float CellSize;           // kare hücre
    public Vector2Int GridSize;      // (width, height) => xCount, yCount
}

public interface IGridTool
{ 
    public void SetGridRelatedData(IGridRelatedData[] gridRelatedData);
    
    public void SetSecondaryTools(params IGridTool[] secondaryTools);
}

public interface IGridRelatedData {}

[Serializable]
public class PaintData: IGridRelatedData
{
    [Header("Raycast")]
    public Camera CameraToUse;

    [Tooltip("Only colliders on these layers will be paintable.")]
    public LayerMask GroundLayerMask;

    [Header("Brush")]
    [Min(0f)]
    public float BrushRadiusInWorldUnits = 2f;

    [Tooltip("This must match the cell size used by your GridSearcher/GridData.")]
    [Min(0.0001f)]
    public float CellSizeInWorldUnits = 1f;

    [Tooltip("Hold left mouse button to paint.")]
    public bool PaintWithLeftMouse = true;

    [Tooltip("Hold right mouse button to erase.")]
    public bool EraseWithRightMouse = true;

}
