using System;
using UnityEngine;

//relation class between overlay painter and paint detector
public class Painter : MonoBehaviour
{
    [SerializeField] private GridData gridData;
    [SerializeField] private PaintData paintData;
    [SerializeField] private OverlayPainter overlayPainter;


//todo: Grid'i başlattıktan sonra burayı çalıştır
    public void CreateOverlayMeshIfNeeded()
    {
        overlayPainter.isInitialized = false; //todo:her wire'da baştan kurmamalı ama valuelar değiştiyse baştan kurmalı
        overlayPainter.CreateOverlayMeshIfNeeded(gridData);
    }

    public void StartPainting() //todo: to call with editor update that triggered by Start Painting Button
    {
        if (PaintDetector.TryDetectAvailableCell(gridData, paintData, out SelectedCellData selectedCellData))
        {
            GridBrusher.BrushSelectedCells(selectedCellData, overlayPainter, gridData, paintData);
        }
    }
    
}
