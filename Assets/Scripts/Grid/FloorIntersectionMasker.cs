using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorIntersectionMasker
{
    private GridMasker masker;
    private HashSet<Vector2Int> intersections = new();
    
    //Todo: after that call deconstruct dummies at intersections on the lower floor
    private void DetectCellsUnderFloor(FloorData upperFloor, FloorData lowerFloor) 
    {
        //if(givenFloor <= 0) return;
        
        var upperCells = upperFloor.ItemsByCell.Keys.ToHashSet();
        var lowerCells = lowerFloor.ItemsByCell.Keys.ToHashSet();
        
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
