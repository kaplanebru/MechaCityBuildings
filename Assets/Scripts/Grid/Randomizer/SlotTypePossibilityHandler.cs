using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlotTypePossibilityHandler
{
    private Dictionary<Vector2Int, HashSet<StructureType>> _possibleTypesBySlots = new();
    private Dictionary<StructureType, StructureType[]> _adjacencyImpossibilities = new();

    public SlotTypePossibilityHandler( HashSet<Vector2Int> cells,
        HashSet<StructureType> allTypesInQuestion,
        Dictionary<StructureType, StructureType[]> adjacencyImpossibilities)
    {
        _possibleTypesBySlots = cells.ToDictionary(
            cell => cell,
            cell => new HashSet<StructureType>(allTypesInQuestion)
        );
        
        _adjacencyImpossibilities = adjacencyImpossibilities;
    }

    public void UpdateNeighbourPossibilities(QuadOnMap quadOnMap, StructureType currentType)
    {
        foreach (var coord in quadOnMap.data.Coords)
        {
            _possibleTypesBySlots.Remove(coord);
        }

        foreach (var neighbor in quadOnMap.data.Neighbors)
        {
            EliminatePossibleStructuresOfGivenCell(neighbor, _adjacencyImpossibilities[currentType]);
        }
    }

    private void EliminatePossibleStructuresOfGivenCell(Vector2Int cell, StructureType[] structureTypesToEliminate)
    {
        if (_possibleTypesBySlots.TryGetValue(cell, out var possibleStructureTypes))
        {
            foreach (var type in structureTypesToEliminate)
            {
                possibleStructureTypes.Remove(type);
            }
        }
    }

    public bool IsTypeConvenient(StructureType givenType, Vector2Int[] coords)
    {
        if (coords.Length == 1)
        {
            var possibleStructureTypes = _possibleTypesBySlots[coords[0]];
            return possibleStructureTypes.Contains(givenType);
        }
        
        Dictionary<StructureType, int> frequencyInCoords = new();
        foreach (var coord in coords)
        {
            foreach (var type in _possibleTypesBySlots[coord])
            {
                if (frequencyInCoords.TryGetValue(type, out int count))
                    frequencyInCoords[type] = count + 1;
                else
                    frequencyInCoords[type] = 1;
            }
        }
        int maxCount = frequencyInCoords.Values.Max();
        
        var topCandidates = frequencyInCoords
            .Where(kvp => kvp.Value == maxCount)
            .Select(kvp => kvp.Key)
            .ToList();
        
        return topCandidates.Contains(givenType); // || givenType == StructureType.RightBatiment;
    }
    
}