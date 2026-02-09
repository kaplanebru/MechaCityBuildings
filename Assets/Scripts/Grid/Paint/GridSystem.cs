using System;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public GridData gridData;
    public PaintData paintData;
    public ConstructionData constructionData;
    [SerializeField] private OverlayGridPainter overlayGridPainter;
    
    private IGridRelatedData[] gridRelatedData;
    private IGridTool[] tools;
    
    private GridPainter painter = new();
    private GridSearcher searcher = new();
    private GridMasker masker = new();
    private SearcherPainter searcherPainter = new();
    private GridToConstruction contstructor = new();
    
    private void Start()
    {
        SetTools(); 
        overlayGridPainter.Setup(gridData);
        StartCoroutine(searcherPainter.PaintRoutine());
    }

    private void SetTools()
    {
        tools = new IGridTool[] { painter, searcher, masker, searcherPainter, contstructor};
        masker.SetOverlayPainter(overlayGridPainter);
        InjectSecondaryTools();
        DistributeData();
    }

    public void ConstructBuildingsOnCells()
    {
        var trackedCells = masker.GetTrackedCells();
        if (trackedCells.Count == 0)
        {
            print("No tracked cells found");
            return;
        }
        
        contstructor.ConstructBuildingsOnCells(trackedCells, constructionData);
    }

    public void DestroyBuildingsOnCells()
    {
        masker.RestoreSelectedCells();
        contstructor.DeconstructBuildingsOnCells();
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
