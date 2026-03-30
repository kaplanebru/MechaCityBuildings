using UnityEngine;

[CreateAssetMenu(fileName = "PaintData", menuName = "CityBuilder/PaintData")]
public class PaintData: ScriptableObject
{
    [Tooltip("Only colliders on these layers will be paintable.")]
    public LayerMask GroundLayerMask;

    [Header("Brush")]
    [Min(0f)]
    public int BrushRadius = 1;
    
    [Tooltip("Hold left mouse button to paint.")]
    public bool PaintWithLeftMouse = true;

    [Tooltip("Hold right mouse button to erase.")]
    public bool EraseWithRightMouse = true;

}