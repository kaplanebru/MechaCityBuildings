using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public sealed class PainterProjected : IGridTool
{
    private PaintData _paintData;
    private GridProjector _projector;
    private PainterInGrid _painterInGrid;
    private Camera cam;

    public void SetGridRelatedData(Dictionary<GridDataType, IGridRelatedData> gridRelatedData)
    {
        _paintData = gridRelatedData[GridDataType.PaintData] as PaintData;
        cam = Camera.main;
    }

    public void SetSecondaryTools(params IGridTool[] secondaryTools)
    {
        foreach (var tool in secondaryTools)
        {
            if (tool is PainterInGrid painter)
                _painterInGrid = painter;
            
            else if (tool is GridProjector searcher)
                _projector = searcher;
        }
    }

    public IEnumerator PaintRoutine()
    {
        while (true)
        {
            bool isPainting = _paintData.PaintWithLeftMouse && Input.GetMouseButton(0);
            bool isErasing = _paintData.EraseWithRightMouse && Input.GetMouseButton(1);

            if (!isPainting && !isErasing)
            {
                yield return null;
                continue;
            }

            if (!TryGetMouseGroundHitPoint(out Vector3 hitPointWorld))
            {
                yield return null;
                continue;
            }

            if (!_projector.TryWorldPositionToCellIndex(hitPointWorld, out Vector2Int centerCellIndex))
            {
                yield return null;
                continue;
            }

// Left mouse = paint, Right mouse = erase.
            bool paintValue = isPainting;

// IMPORTANT:
// Painter must know WHICH cell is the center.
// If your painter method signature is different, adjust accordingly.
            _painterInGrid.PaintSelectedCellsInBrush(paintValue, centerCellIndex); //centerCellIndex,

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