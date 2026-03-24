using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorIntersectionMasker
{
    //Todo: after that call deconstruct dummies at intersections on the lower floor
    //tODO: bir floor silinince alttaki intersectionların da recover olması lazım: keyler dursun, hidden diye liste de tutulabilir
    public static HashSet<Structure> GetIntersectionsUnderFloor(
        FloorData upperFloor, 
        FloorData lowerFloor,
        Dictionary<Vector2Int, Structure> lowerFloorStructures)
    {
        var upperCells = upperFloor.GetCells();
        var lowerCells = lowerFloor.GetCells();

        var intersections = FindIntersections(upperCells, lowerCells);
        RemoveIntersectionBoundary(intersections);
        return GetItemsOnIntersectionPoints(intersections, lowerFloorStructures);
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
    
    private static HashSet<Structure> GetItemsOnIntersectionPoints
        (HashSet<Vector2Int> intersections, Dictionary<Vector2Int, Structure> lowerFloorStructures)
    {
        HashSet<Structure> structuresToRemove = new HashSet<Structure>();
        foreach (var intersection in intersections)
        {
            if (lowerFloorStructures.TryGetValue(intersection, out var structure))
            {
                structuresToRemove.Add(structure);
            }
        }
        return structuresToRemove;
    }
}