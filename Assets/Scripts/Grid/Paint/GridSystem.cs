using System;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public GridData gridData;
    public PaintData paintData;
    [SerializeField] private OverlayGridPainter overlayGridPainter;
    
    private IGridRelatedData[] gridRelatedData;
    private IGridTool[] tools;
    
    private GridPainter painter = new();
    private GridSearcher searcher = new();
    private GridMasker masker = new();
    private SearcherPainter searcherPainter = new();

    private void Start()
    {
        SetTools();
        StartCoroutine(searcherPainter.PaintRoutine());
    }

    private void SetTools()
    {
        tools = new IGridTool[] { painter, searcher, masker, searcherPainter };
        masker.SetOverlayPainter(overlayGridPainter);
        InjectSecondaryTools();
        DistributeData();
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
