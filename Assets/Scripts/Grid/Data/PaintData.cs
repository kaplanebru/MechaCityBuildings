using UnityEngine;

[CreateAssetMenu(fileName = "PaintData", menuName = "CityBuilder/PaintData")]
public class PaintData: ScriptableObject
{
    [Tooltip("Only colliders on these layers will be paintable.")]
    public LayerMask GroundLayerMask;

    [Header("Brush")]
    [Min(0f)]
    public float BrushRadiusInWorldUnits = 2f;

    [Tooltip("This must match the cell size used by your GridSearcher/GridData.")]
    [Min(0.0001f)]
    public float CellSizeInWorldUnits = 1f;

    [Tooltip("Hold left mouse button to paint.")]
    public bool PaintWithLeftMouse = true;

    [Tooltip("Hold right mouse button to erase.")]
    public bool EraseWithRightMouse = true;

}