using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellDataCreator
{
    public static HashSet<CellData> CreateCellDataFromQuads(QuadOnMap[] randomQuads, HashSet<Vector2Int> map)
    {
        HashSet<CellData> quadCellDatas = new();

        foreach (var quad in randomQuads)
        {
            var cellData = new CellData(quad.StartPoint);

            foreach (var point in quad.data.Neighbors)
            {
                if(map.Contains(point))
                    cellData.Neighbors.Add(new NeighborCellPoint(point));
            }

            //çevresi kadar neighbor'u olur max
            cellData.Type = quad.data.Neighbors.Length < quad.Perimeter ? CellType.Boundary : CellType.Regular;
            cellData.CellSize = quad.data.WidthHeight;
            cellData.Center = quad.Center;

            quadCellDatas.Add(cellData);
        }
        
        return quadCellDatas;
    }
    
    public static HashSet<CellData> CreateCellDataFromSinglePoints(HashSet<Vector2Int> singleCells, HashSet<Vector2Int> map, int cellUnit)
    {
        HashSet<CellData> singleCellDatas = new HashSet<CellData>();
        foreach (Vector2Int cellIndex in singleCells)
        {
            var cellData = new CellData(cellIndex);
            cellData.SetNeighbors(cellUnit, map);

            singleCellDatas.Add(cellData);
        }

        FindOrientationsForSingleCells(singleCellDatas.ToHashSet());

        return singleCellDatas.ToHashSet();
    }
    
    
    private static void FindOrientationsForSingleCells(HashSet<CellData> cellDataSet)
    {
        var boundaryCells = CellRegistry.GetBoundaries(cellDataSet);

        foreach (var boundaryCell in boundaryCells)
        {
            if (boundaryCell.OutwardNormal == Vector2Int.zero)
            {
                continue;
            }
            
            Vector2Int tangent = new Vector2Int(
                -boundaryCell.OutwardNormal.y, 
                boundaryCell.OutwardNormal.x); //perpendicular
            
            Vector3 forward = new Vector3(tangent.x, 0f, tangent.y);
            boundaryCell.Rotation = Quaternion.LookRotation(forward, Vector3.up);
        }
    }

    
}