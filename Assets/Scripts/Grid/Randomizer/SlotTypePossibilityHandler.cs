using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlotTypePossibilityHandler
{
    private Dictionary<Vector2Int, HashSet<StructureType>> _possibleTypesBySlot = new();
    private Dictionary<StructureType, StructureType[]> _adjacencyImpossibilities = new();

    public SlotTypePossibilityHandler( HashSet<Vector2Int> cells,
        Dictionary<StructureType, StructureType[]> adjacencyImpossibilities)
    {
        _possibleTypesBySlot = cells.ToDictionary(
            cell => cell,
            cell => new HashSet<StructureType>(adjacencyImpossibilities.Keys.ToArray())
        );
        
        _adjacencyImpossibilities = adjacencyImpossibilities;
    }

    public void UpdateNeighbourPossibilities(QuadOnMap quadOnMap, StructureType currentType)
    {
        foreach (var coord in quadOnMap.data.Coords)
        {
            _possibleTypesBySlot.Remove(coord);
        }

        foreach (var neighbor in quadOnMap.data.Neighbors)
        {
            EliminatePossibleStructuresOfGivenCell(neighbor, _adjacencyImpossibilities[currentType]);
        }
    }

    private void EliminatePossibleStructuresOfGivenCell(Vector2Int cell, StructureType[] typesToEliminate)
    {
        if (_possibleTypesBySlot.TryGetValue(cell, out var possibleStructureTypes))
        {
            foreach (var type in typesToEliminate)
            {
                possibleStructureTypes.Remove(type);
            }
        }
    }

    public bool IsTypeConvenient(StructureType examiningType, Vector2Int[] coords)
    {
        if (coords.Length == 1)
        {
            var possibleStructureTypes = _possibleTypesBySlot[coords[0]];
            return possibleStructureTypes.Contains(examiningType);
        }
        
        Dictionary<StructureType, int> frequencyInCoords = new();
        foreach (var coord in coords)
        {
            foreach (var type in _possibleTypesBySlot[coord])
            {
                if (frequencyInCoords.TryGetValue(type, out int count))
                    frequencyInCoords[type] = count + 1;
                else
                    frequencyInCoords[type] = 1;
            }
        }
        int maxCount = frequencyInCoords.Values.Max();

        if (maxCount == 0)
        {
            Debug.LogWarning("No possible structure type found");
        }
        
        var topCandidates = frequencyInCoords
            .Where(kvp => kvp.Value == maxCount)
            .Select(kvp => kvp.Key)
            .ToList();
        
        return topCandidates.Contains(examiningType);
    }
    
}