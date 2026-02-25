using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCellData
{
    public SelectedCellData(Vector2Int cellCenterIndex, bool isPainting)
    {
        CellCenterIndex = cellCenterIndex;
        IsPainting = isPainting;
    }

    public Vector2Int CellCenterIndex;
    public bool IsPainting;
}
public static class PaintDetector
{
    private static Camera Cam = Camera.main;
    
    public static bool TryDetectAvailableCell(GridData gridData, PaintData paintData, out SelectedCellData selectedCellData)//todo to call in update
    {
        selectedCellData = null;
        bool isPainting = paintData.PaintWithLeftMouse && Input.GetMouseButton(0);
        bool isErasing = paintData.EraseWithRightMouse && Input.GetMouseButton(1);

        if (!isPainting && !isErasing)
            return false;

        if (!TryGetMouseGroundHitPoint(paintData, out Vector3 hitPointWorld))
            return false;

        if (!GridProjector.TryWorldPositionToCellIndex(hitPointWorld, gridData, out Vector2Int selectedCellCenterIndex))
            return false;

        selectedCellData = new SelectedCellData(selectedCellCenterIndex, isPainting);
        
        /*bool paintValue = isPainting;
        
        PaintHelperInGrid.BrushSelectedCells(
            paintValue,
            selectedCellCenterIndex, 
            gridData,
            paintData);*/
        return true;
    }
    private static bool TryGetMouseGroundHitPoint(PaintData paintData, out Vector3 hitPointWorld)
    {
        hitPointWorld = default;

        if (Cam == null)
            return false;

        Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
        float maxDistance = 10000f;

        if (!Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, paintData.GroundLayerMask))
            return false;

        hitPointWorld = hitInfo.point;
        return true;
    }
}