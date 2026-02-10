using System.Collections.Generic;
using UnityEngine;

public class FloorIntersectionMasker
{
    private GridMasker masker;
    private HashSet<Vector2Int> intersections = new();
    
    //Todo: after that call deconstruct dummies at intersections on the lower floor
    private void DetectCellsUnderFloor(int givenFloor) 
    {
        if(givenFloor <= 0) return;
        if(!masker.CellTrackByFloor.ContainsKey(givenFloor)) return;
        if(!masker.CellTrackByFloor.ContainsKey(givenFloor-1)) return;

        var upperCells = masker.CellTrackByFloor[givenFloor];
        var lowerCells = masker.CellTrackByFloor[givenFloor-1];
        
        FindIntersections(upperCells, lowerCells);
    }

    private void FindIntersections(HashSet<Vector2Int> upperCells, HashSet<Vector2Int> lowerCells)
    {
        intersections.Clear();
        foreach(var upperCell in upperCells)
        {
            if (lowerCells.Contains(upperCell))
            {
                intersections.Add(upperCell);
            }
        }

        RemoveIntersectionBoundary();
    }

    private void RemoveIntersectionBoundary()
    {
        var intersectionBounds = BoundaryFinder.GetBoundsWithInner(1, intersections);
        intersections.RemoveWhere(intersection => intersectionBounds.Contains(intersection));
    }

}
