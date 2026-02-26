using System;
using UnityEngine;

//relation class between overlay painter and paint detector
public class Painter : MonoBehaviour
{
    [SerializeField] private GridData gridData;
    [SerializeField] private PaintData paintData;
    [SerializeField] private OverlayPainter overlayPainter;


    public void ExecutePainting() //todo: to call with editor update that triggered by Start Painting Button
    {
        if (PaintDetector.TryDetectAvailableCell(gridData, paintData, out SelectedCellData selectedCellData))
        {
            GridBrusher.BrushSelectedCells(selectedCellData, overlayPainter, gridData, paintData);
        }
    }
    
}
