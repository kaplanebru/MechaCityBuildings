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
        if (data.WidthHeight.x < 2 && data.WidthHeight.y < 2)
        {
            Debug.LogAssertion("Quad with that size is not possible: " + data.WidthHeight + " please change size");
            return;
        }
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

        HashSet<Vector2Int> edgeNeighbors = new();

        for (int c = 0; c < column; c++)
        {
            var upper = new Vector2Int(c, 0) + Vector2Int.up;
            edgeNeighbors.Add(upper);

            var lower = new Vector2Int(c, lastRow) + Vector2Int.down;
            edgeNeighbors.Add(lower);
        }

        for (int r = 0; r < row; r++)
        {
            var left = new Vector2Int(0, r) + Vector2Int.left;
            edgeNeighbors.Add(left);

            var right = new Vector2Int(lastColumn, r) + Vector2Int.right;
            edgeNeighbors.Add(right);
        }

        return edgeNeighbors.ToArray();
    }
}