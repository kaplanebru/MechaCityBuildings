using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlotTypePossibilityHandler
{
    private Dictionary<Vector2Int, HashSet<StructureType>> _possibleTypesOfSlot = new();
    
    public void Initiate(HashSet<Vector2Int> cells, HashSet<StructureType> allTypesInQuestion)
    {
        _possibleTypesOfSlot = cells.ToDictionary(
            cell => cell,
            cell => new HashSet<StructureType>(allTypesInQuestion)
        );
    }

    public void UpdateNeighbourPossibilities(QuadOnMap quadOnMap, StructureType currentType)
    {
        foreach (var coord in quadOnMap.data.Coords)
        {
            _possibleTypesOfSlot.Remove(coord);
        }

        foreach (var neighbor in quadOnMap.data.Neighbors)
        {
            EliminatePossibilitiesOfGivenCell(neighbor, currentType);
        }
    }

    private void EliminatePossibilitiesOfGivenCell(Vector2Int cell, params StructureType[] possibilitiesToEliminate)
    {
        if (_possibleTypesOfSlot.TryGetValue(cell, out var possibleTypes))
        {
            foreach (var type in possibilitiesToEliminate)
            {
                possibleTypes.Remove(type);
            }
        }
    }

    public bool IsTypeConvenient(StructureType givenType, Vector2Int[] coords)
    {
        if (coords.Length == 1)
        {
            var possibilities = _possibleTypesOfSlot[coords[0]];
            return possibilities.Contains(givenType) || givenType == StructureType.RightBatiment;
            //todo: || sonrası test amaçlı. yanyana gelebilenler olarak eleriz daha sonra
        }
        
        Dictionary<StructureType, int> frequencyInCoords = new();
        foreach (var coord in coords)
        {
            foreach (var type in _possibleTypesOfSlot[coord])
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
        
        return topCandidates.Contains(givenType) || givenType == StructureType.RightBatiment;
        //todo later: or yanyana olabilirler listesindeyse contains'de olmasa da olur

    }
    
    public StructureType GetHighestPossibilityOnQuad(Vector2Int[] coords)
    {
        if (coords.Length == 1)
        {
            var possibilities = _possibleTypesOfSlot[coords[0]];
            return possibilities.ElementAt(Random.Range(0, possibilities.Count));
        }
        
        Dictionary<StructureType, int> frequencyInCoords = new();
        foreach (var coord in coords)
        {
            foreach (var type in _possibleTypesOfSlot[coord])
            {
                if (frequencyInCoords.TryGetValue(type, out int count))
                {
                    frequencyInCoords[type] = count + 1;
                }
                else
                {
                    frequencyInCoords[type] = 1;
                }
            }
        }
        
        int maxCount = frequencyInCoords.Values.Max();
        
        if (maxCount == 0)
            throw new InvalidOperationException("No possible types found for given coords.");

        var topCandidates = frequencyInCoords
            .Where(kvp => kvp.Value == maxCount)
            .Select(kvp => kvp.Key)
            .ToList();
        

       return topCandidates.Count == 1
            ? topCandidates[0]
            : topCandidates[Random.Range(0, topCandidates.Count)];
    }
}