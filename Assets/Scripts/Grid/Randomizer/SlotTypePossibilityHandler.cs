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

    public void UpdateNeighbourPossibilities(SlotData slotData)
    {
        var slotDataType = slotData.StructureType;

        foreach (var slot in slotData.Cells)
        {
            _possibleTypesOfSlot.Remove(slot);
        }
        
        slotData.Neighbors.ForEach(n=> EliminatePossibilitiesOfGivenCell(n.Coords, slotDataType));
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

    public StructureType GetHighestPossibilityOnQuad(Vector2Int[] slotCells)
    {
        if (slotCells.Length == 1)
        {
            var possibilities = _possibleTypesOfSlot[slotCells[0]];
            return possibilities.ElementAt(Random.Range(0, possibilities.Count));
        }
        
        Dictionary<StructureType, int> frequencyInQuadCells = new();
        foreach (var slotCell in slotCells)
        {
            foreach (var type in _possibleTypesOfSlot[slotCell])
            {
                if (frequencyInQuadCells.TryGetValue(type, out int count))
                {
                    frequencyInQuadCells[type] = count + 1;
                }
                else
                {
                    frequencyInQuadCells[type] = 1;
                }
            }
        }
        
        int maxCount = frequencyInQuadCells.Values.Max();
        
        if (maxCount == 0)
            throw new InvalidOperationException("No possible types found for given cells.");

        var topCandidates = frequencyInQuadCells
            .Where(kvp => kvp.Value == maxCount)
            .Select(kvp => kvp.Key)
            .ToList();
        

       return topCandidates.Count == 1
            ? topCandidates[0]
            : topCandidates[Random.Range(0, topCandidates.Count)];
    }
}