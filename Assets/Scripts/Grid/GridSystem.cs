using System.Collections.Generic;
using UnityEngine;


public class GridSystem : MonoBehaviour
{
    public GridData gridData;
    [SerializeField] private MapSizeToGridSize mapSizeToGridSize;
    public OverlayPainter overlayPainter;
    public Transform originWorldTransform;
    
    [HideInInspector] public List<Vector2Int> cellRecorderCache = new(); //kaydedilmesi lazım

    

    //todo: hard reset if needed: yani normal reset gibi gidip tek tek bulup silmeyecek,
    //loop ile her celli dolaşıp silecek hem masktan hem overlayden
    public void RecalculateGrid()
    {
        AdaptGridByMeshAndCellSize(); //if map changes or cell size changes
        GridMasker.SetGridWithinCells(gridData, cellRecorderCache);

        overlayPainter.RecoverMeshIfNecessary(gridData);
    }

    public void ReloadGrid()
    {
        gridData.OriginWorldTransform = originWorldTransform;
        GridMasker.SetGridWithinCells(gridData, cellRecorderCache);

        overlayPainter.CreateOverlayMesh(gridData);
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
}