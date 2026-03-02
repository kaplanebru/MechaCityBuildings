using System;
using UnityEngine;

//relation class between overlay painter and paint detector
public class Painter : MonoBehaviour
{
    [SerializeField] private GridData gridData;
    [SerializeField] private PaintData paintData;
    [SerializeField] private OverlayPainter overlayPainter;


    public void ExecutePainting(Event e) //todo: to call with editor update that triggered by Start Painting Button
    {
        
        if (PaintDetector.TryDetectAvailableCell_Editor(gridData, paintData, e, out var selectedCellData))
        {
            GridBrusher.BrushSelectedCells(selectedCellData, overlayPainter, gridData, paintData);

        }
    }

    public void RestorePaintedAreas()
    {
        //GridMasker.
    }
    
}
