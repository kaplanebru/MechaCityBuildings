using System;
using System.Collections.Generic;

public class AdjacencyHelper
{
    public static IEnumerable<StructureType> GetImpossibleAdjacencyForGivenType(StructureType a,
        StructureType[] selectedStructureTypes, bool[] adjacencyData)
    {
        int size = selectedStructureTypes.Length;
        int row = Array.IndexOf(selectedStructureTypes, a);
        

        for (int col = 0; col < size; col++)
        {
            if (!adjacencyData[row * size + col])
                yield return selectedStructureTypes[col];
        }
    }

    public static Dictionary<StructureType, StructureType[]> GetPossibleAdjacencyDB(
        StructureType[] selectedStructureTypes,
        bool[] adjacency)
    {
        int size = selectedStructureTypes.Length;
        var db = new Dictionary<StructureType, StructureType[]>();

        for (int row = 0; row < size; row++)
        {
            var compatible = new List<StructureType>();

            for (int col = 0; col < size; col++)
            {
                if (adjacency[row * size + col])
                    compatible.Add(selectedStructureTypes[col]);
            }

            db[selectedStructureTypes[row]] = compatible.ToArray();
        }

        return db;
    }

    public static Dictionary<StructureType, StructureType[]> GetImpossibleAdjacencyDB(
        StructureType[] selectedStructureTypes,
        bool[] adjacency)
    {
        int size = selectedStructureTypes.Length;
        var db = new Dictionary<StructureType, StructureType[]>();

        for (int row = 0; row < size; row++)
        {
            var incompatible = new List<StructureType>();

            for (int col = 0; col < size; col++)
            {
                if (!adjacency[row * size + col])
                    incompatible.Add(selectedStructureTypes[col]);
            }

            db[selectedStructureTypes[row]] = incompatible.ToArray();
        }

        return db;
    }
    
    public static bool CanBeAdjacent(
        StructureType a,
        StructureType b,
        StructureType[] selectedStructureTypes,
        bool[] adjacency)
    {
        int row = Array.IndexOf(selectedStructureTypes, a);
        int col = Array.IndexOf(selectedStructureTypes, b);

        if (row < 0 || col < 0) return false;

        return adjacency[row * selectedStructureTypes.Length + col];
    }

    public static IEnumerable<StructureType> GetAdjacencyForGivenType(StructureType a,
        StructureType[] selectedStructureTypes, bool[] adjacencyData)
    {
        int size = selectedStructureTypes.Length;
        int row = Array.IndexOf(selectedStructureTypes, a);
        

        for (int col = 0; col < size; col++)
        {
            if (adjacencyData[row * size + col])
                yield return selectedStructureTypes[col];
        }
    }
}