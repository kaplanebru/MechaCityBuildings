using UnityEngine;

public class Structure : MonoBehaviour
{
    public StructureType type;
    
    public Vector2Int slotMetadata = new Vector2Int(-1, -1);
    
    public int floorIndex;
    public void SetCellMetaData(Vector2Int cell)
    {
        slotMetadata = cell;
    }
}
