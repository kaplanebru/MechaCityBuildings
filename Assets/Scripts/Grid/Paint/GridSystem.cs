using System;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public GameObject dummy;
    public GridData gridData;
    public PaintData paintData;
    [SerializeField] private OverlayGridPainter overlayGridPainter;
    
    private IGridRelatedData[] gridRelatedData;
    private IGridTool[] tools;
    
    private GridPainter painter = new();
    private GridSearcher searcher = new();
    private GridMasker masker = new();
    private SearcherPainter searcherPainter = new();
    private GridToConstruction contstructor = new();
    
    private GridOptimizer optimizer = new();

    private void Start()
    {
        SetTools();
        StartCoroutine(searcherPainter.PaintRoutine());
    }

    private void SetTools()
    {
        tools = new IGridTool[] { painter, searcher, masker, searcherPainter, contstructor};
        masker.SetOverlayPainter(overlayGridPainter);
        InjectSecondaryTools();
        DistributeData();
    }

    List<Vector3> trackedCellsInWorld = new();
    public void ConstructBuildingsOnCells()
    {
        var trackedCells = masker.GetTrackedCells();

        if (trackedCells.Count == 0)
        {
            print("No tracked cells found");
            return;
        }
        
        trackedCellsInWorld.Clear();
        
        trackedCells = BoundaryFinder.GetBoundsWithInner(1, trackedCells);

        foreach (var trackedCell in trackedCells)
        {
            Vector3 pos = contstructor.GetCellIndexToWorldPositionCenter(trackedCell.x, trackedCell.y);
            trackedCellsInWorld.Add(pos);
            Instantiate(dummy, pos, Quaternion.identity);
        }

        
        //var edgeCells = optimizer.GetEdges(trackedCellsInWorld, 1);
        /*foreach (var edgeCell in edgeCells)
        {
            Instantiate(dummy, edgeCell, Quaternion.identity);
        }*/
    }

    private void InjectSecondaryTools()
    {
        painter.SetSecondaryTools(searcher, masker);
        searcherPainter.SetSecondaryTools(searcher, painter);
    }
    
    public void DistributeData()
    {
        gridRelatedData = new IGridRelatedData[] { gridData, paintData };

        foreach (var tool in tools)
        {
            tool.SetGridRelatedData(gridRelatedData);
        }
    }
}
