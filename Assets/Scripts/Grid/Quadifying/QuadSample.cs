using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//todo: Generate butonu ekle! ya da change olunca generate etsin

[CreateAssetMenu(fileName = "QuadSample", menuName = "CityBuilder/QuadSample")]
public class QuadSample : ScriptableObject
{
    public QuadData data;
    

    public void Generate()
    {
        /*if (data.WidthHeight.x < 2 && data.WidthHeight.y < 2)
        {
            Debug.LogAssertion("Quad with that size is not possible: " + data.WidthHeight + " please change size");
            return;
        }*/
        GenerateQuad();
        data.Neighbors = GenerateNeighbors();
    }

    private void GenerateQuad()
    {
        List<Vector2Int> sampleCoords = new();

        for (int row = 0; row < data.WidthHeight.y; row++)
        {
            for (int col = 0; col < data.WidthHeight.x; col++)
            {
                sampleCoords.Add(new Vector2Int(col, row));
            }
        }

        data.Coords = sampleCoords.ToArray();
    }

    private Vector2Int[] GenerateNeighbors()
    {
        int column = data.WidthHeight.x;
        int row = data.WidthHeight.y;

        int lastColumn = column - 1;
        int lastRow = row - 1;

        HashSet<Vector2Int> neighbors = new();

        for (int c = 0; c < column; c++)
        {
            var upper = new Vector2Int(c, 0) + Vector2Int.down;
            neighbors.Add(upper);

            var lower = new Vector2Int(c, lastRow) + Vector2Int.up;
            neighbors.Add(lower);
        }

        for (int r = 0; r < row; r++)
        {
            var left = new Vector2Int(0, r) + Vector2Int.left;
            neighbors.Add(left);

            var right = new Vector2Int(lastColumn, r) + Vector2Int.right;
            neighbors.Add(right);
        }

        return neighbors.ToArray();
    }

    public Vector2Int[] GetNeighborEdges()
    {
        Vector2Int[] edges = new Vector2Int[4];
        int column = data.WidthHeight.x;
        int row = data.WidthHeight.y;
        
        int lastColumn = column - 1;
        int lastRow = row - 1;

        if (data.Coords.Length == 0)
            Debug.Log("No points found to create edges");
        
        var leftUp = Vector2Int.zero;
        edges[0] = leftUp + new Vector2Int(-1, -1);

        var rightUp = new Vector2Int(lastColumn, 0);
        edges[1] = rightUp + new Vector2Int(1, -1);

        var leftDown = new Vector2Int(0, lastRow);
        edges[2] = leftDown + new Vector2Int(-1, 1);
        
        var rightDown = new Vector2Int(lastColumn, lastRow);
        edges[3] = rightDown + new Vector2Int(1, 1);
        
        return edges;
    }
}