using UnityEngine;

public class CellItem : MonoBehaviour
{
    public Vector2Int cellMetadata = new Vector2Int(-1, -1);

    public void SetCellMetaData(Vector2Int cell)
    {
       cellMetadata = cell;
    }

    public void ResetCellMetaData()
    {
        cellMetadata = new Vector2Int(-1, -1);
    }
}
