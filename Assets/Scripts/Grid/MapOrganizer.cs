using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapOrganizer
{

    public static HashSet<CellData> ToCellData(List<Vector2Int> cellPoints, QuadSample quadSample, int quadAmount)
    {
        List<Vector2Int> points = new();
        points.AddRange(cellPoints);
        
        Shuffle(points);
        var randomQuads = QuadSearcher.SearchQuads(quadSample, points.ToHashSet(), quadAmount);

        
        HashSet<CellData> cells = new ();
        foreach (var quad in randomQuads)
        {
            var quadCell = new CellData(quad.StartPoint);
            
            quadCell.Neighbors = quad.data.Neighbors.ToList();
            
            //çevresi kadar neighbor'u olur max
            if (quad.data.Neighbors.Length < quad.Perimeter)
                quadCell.Type = CellType.Boundary;
            
            quadCell.CellSize = quad.data.WidthHeight;
            quadCell.Center = quad.Center;
            Debug.Log(quad.Center);
            
            cells.Add(quadCell);

            foreach (var quadPoint in  quad.data.Coords)
            {
                points.Remove(quadPoint);
            }

        }

        cells.UnionWith(CellDataCreator.ConvertToCellData(points.ToHashSet(), 1));
        return cells;
    }
    
    public static void Shuffle<T>(IList<T> collection) //T[] //IList
    {
        int n = collection.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (collection[n], collection[k]) = (collection[k], collection[n]);
        }
    }

}
