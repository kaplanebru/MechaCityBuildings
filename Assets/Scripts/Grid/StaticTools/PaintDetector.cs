using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
#if UNITY_EDITOR
    /// <summary>
    /// Editor version: call this from OnSceneGUI() (CustomEditor) or OnToolGUI() (EditorTool).
    /// Pass Event.current.
    /// </summary>
    public static bool TryDetectAvailableCell_Editor(
        GridData gridData,
        PaintData paintData,
        Event e,
        out SelectedCellData selectedCellData)
    {
        selectedCellData = null;

        if (e == null)
            return false;

        // You want paint/erase while dragging too.
        if (e.type != EventType.MouseDown && e.type != EventType.MouseDrag)
            return false;

        bool isPainting = paintData.PaintWithLeftMouse && e.button == 0;
        bool isErasing = paintData.EraseWithRightMouse && e.button == 1;

        if (!isPainting && !isErasing)
            return false;

        if (!TryGetMouseGroundHitPoint_Editor(paintData, e, out Vector3 hitPointWorld))
            return false;

        if (!GridProjector.TryWorldPositionToCellIndex(hitPointWorld, gridData, out Vector2Int selectedCellCenterIndex))
            return false;

        selectedCellData = new SelectedCellData(selectedCellCenterIndex, isPainting);

        //Debug.Log(selectedCellData.CellCenterIndex + " " + selectedCellData.IsPainting);
        // Prevent Scene selection / manipulation tools from also consuming the click.
        e.Use();

        return true;
    }

    private static bool TryGetMouseGroundHitPoint_Editor(
        PaintData paintData,
        Event e,
        out Vector3 hitPointWorld)
    {
        hitPointWorld = default;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        float maxDistance = 10000f;
        
        if (!Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, paintData.GroundLayerMask,
                QueryTriggerInteraction.Ignore))
            return false;

        hitPointWorld = hitInfo.point;
        
        //Debug.Log(hitPointWorld);
        //Debug.DrawRay(ray.origin, ray.direction * 50f, Color.magenta, 2f);
        SceneView.RepaintAll();
        return true;
    }

    /// <summary>
    /// Optional helper: call at the top of OnSceneGUI/OnToolGUI
    /// so Unity doesn't select objects when you click-drag to paint.
    /// </summary>
    public static void CaptureSceneViewControl()
    {
        UnityEditor.HandleUtility.AddDefaultControl(UnityEngine.GUIUtility.GetControlID(UnityEngine.FocusType.Passive));
    }
#endif

  
    /*private static Camera Cam = Camera.main;

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
    }*/
}