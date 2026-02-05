using System;
using System.Collections;
using UnityEngine;


public sealed class SearcherPainter : IGridTool
{
    private PaintData _paintData;
    private GridSearcher _searcher;
    private GridPainter _painter;
    private Camera cam;

    public void SetGridRelatedData(IGridRelatedData[] gridRelatedData)
    {
        _paintData = gridRelatedData[1] as PaintData;
        cam = Camera.main;
    }

    public void SetSecondaryTools(params IGridTool[] secondaryTools)
    {
        foreach (var tool in secondaryTools)
        {
            if (tool is GridPainter painter)
                _painter = painter;
            
            else if (tool is GridSearcher searcher)
                _searcher = searcher;
        }
    }

    public IEnumerator PaintRoutine()
    {
        while (true)
        {
            bool isPainting = _paintData.PaintWithLeftMouse && Input.GetMouseButton(0);
            bool isErasing = _paintData.EraseWithRightMouse && Input.GetMouseButton(1);

// No input this frame -> do nothing.
            if (!isPainting && !isErasing)
            {
                yield return null;
                continue;
            }

// Raycast to ground.
            if (!TryGetMouseGroundHitPoint(out Vector3 hitPointWorld))
            {
                yield return null;
                continue;
            }

// Convert hit point to grid cell.
            if (!_searcher.TryWorldPositionToCellIndex(hitPointWorld, out Vector2Int centerCellIndex))
            {
                yield return null;
                continue;
            }

// Left mouse = paint, Right mouse = erase.
            bool paintValue = isPainting;

// IMPORTANT:
// Painter must know WHICH cell is the center.
// If your painter method signature is different, adjust accordingly.
            _painter.PaintSelectedCellsInBrush(paintValue, centerCellIndex); //centerCellIndex,

            yield return null;
        }
    }
    

    private bool TryGetMouseGroundHitPoint(out Vector3 hitPointWorld)
    {
        hitPointWorld = default;

        if (cam == null)
            return false;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        float maxDistance = 10000f;

        if (!Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, _paintData.GroundLayerMask))
            return false;

        hitPointWorld = hitInfo.point;
        return true;
    }
}