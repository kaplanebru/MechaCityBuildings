using UnityEngine;

public class CellItem : MonoBehaviour
{
    public Vector2Int cellCache = new Vector2Int(-1, -1);

    public void SetCellCache(Vector2Int cell)
    {
       cellCache = cell;
    }
}
