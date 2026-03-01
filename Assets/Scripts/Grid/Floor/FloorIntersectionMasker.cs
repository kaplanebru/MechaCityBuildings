using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorIntersectionMasker
{
    //Todo: after that call deconstruct dummies at intersections on the lower floor
    //tODO: bir floor silinince alttaki intersectionların da recover olması lazım: keyler dursun, hidden diye liste de tutulabilir
    public static HashSet<Transform> GetIntersectionsUnderFloor(FloorData upperFloor, FloorData lowerFloor)
    {
        var upperCells = upperFloor.GetTotalItemsByCell().Keys.ToHashSet();
        var lowerCells = lowerFloor.GetTotalItemsByCell().Keys.ToHashSet();

        var intersections = FindIntersections(upperCells, lowerCells);
        RemoveIntersectionBoundary(intersections);
        return GetItemsOnIntersectionPoints(intersections, lowerFloor);
    }

    private static HashSet<Vector2Int> FindIntersections(HashSet<Vector2Int> upperCells, HashSet<Vector2Int> lowerCells)
    {
        HashSet<Vector2Int> intersections = new();

        foreach (var upperCell in upperCells)
        {
            if (lowerCells.Contains(upperCell))
            {
                intersections.Add(upperCell);
            }
        }
        return intersections;
    }

    

    private static void RemoveIntersectionBoundary(HashSet<Vector2Int> intersections)
    {
        var intersectionBounds = BoundaryFinder.GetBoundsWithInner(1, intersections);
        intersections.RemoveWhere(intersection => intersectionBounds.Contains(intersection));
    }
    
    private static HashSet<Transform> GetItemsOnIntersectionPoints(HashSet<Vector2Int> intersections, FloorData lowerFloor)
    {
        HashSet<Transform> buildings = new HashSet<Transform>();
        foreach (var intersection in intersections)
        {
            if (lowerFloor.GetTotalItemsByCell().TryGetValue(intersection, out var value))
            {
                buildings.Add(value);
            }
        }
        return buildings;
    }
}