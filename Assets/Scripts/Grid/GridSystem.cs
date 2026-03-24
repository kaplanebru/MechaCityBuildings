using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridSystem : MonoBehaviour
{
    public GridData gridData;
    [SerializeField] private MapSizeToGridSize mapSizeToGridSize;
    public OverlayPainter overlayPainter;
    public Transform originWorldTransform;
    
    [HideInInspector] public List<Vector2Int> cellRecorderCache = new(); //kaydedilmesi lazım

    private void OnEnable()
    {
        Eventbus.OnReloadCall += ReloadGrid;
    }

    private void OnDisable()
    {
        Eventbus.OnReloadCall -= ReloadGrid;
    }

    public void RecalculateGrid()
    {
        AdaptGridByMeshAndCellSize(); //if map changes or cell size changes
        GridMasker.SetGridWithinCells(gridData, cellRecorderCache);

        overlayPainter.RebuildAll(gridData);
    }

    public void ReloadGrid()
    {
        gridData.OriginWorldTransform = originWorldTransform;
        
        overlayPainter.RecoverMeshIfNecessary(gridData);
        //todo: recover edince de grid mask çalışmalı
        
        GridMasker.SetGridWithinCells(gridData, cellRecorderCache);
    }

    private void AdaptGridByMeshAndCellSize()
    {
        gridData.AdaptiveGridSize = mapSizeToGridSize.GetGridSizeFromMesh();

        gridData.AdaptiveGridSize.x =
            Mathf.RoundToInt(gridData.AdaptiveGridSize.x / gridData.BuildingCellSize);
        gridData.AdaptiveGridSize.y =
            Mathf.RoundToInt(gridData.AdaptiveGridSize.y / gridData.BuildingCellSize);

        
        //todo: user pref değil de grid dataya işlenmeli direkt.
        //çünkü başka bir gridbuilderınkiler bunlara yazılır!!!!
    }
    
    public void OnFloorHeightUpdate(FloorData activeFloor)
    {
        overlayPainter.SetOverlayMeshHeight(activeFloor);
    }
}