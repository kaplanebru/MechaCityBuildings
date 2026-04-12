using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlotTypePossibilityHandler
{
    private Dictionary<Vector2Int, HashSet<StructureType>> _typesByCell = new();
    private Dictionary<StructureType, StructureTypeSearchData> _searchDatasByType = new();
    private HashSet<Vector2Int> determinedCells = new();
    public SlotTypePossibilityHandler(HashSet<Vector2Int> cells, HashSet<StructureTypeSearchData> searchDatas)
    {
        var allTypes = searchDatas.Select(s => s.Type).ToHashSet();
        
        _typesByCell = cells.ToDictionary(
            cell => cell,
            cell => new HashSet<StructureType>(allTypes));

        foreach (var searchData in searchDatas)
        {
            _searchDatasByType.Add(searchData.Type, searchData);
        }
    }

    public void UpdateNeighbourPossibilities(QuadOnMap quadOnMap, StructureType quadType)
    {
        foreach (var cell in quadOnMap.data.Coords)
        {
            _typesByCell[cell].Clear();
            _typesByCell[cell].Add(quadType);
            determinedCells.Add(cell);
        }

        if(quadType == StructureType.RightBatiment)
            Debug.Log("small cell");
        
       
        foreach (var neighbor in quadOnMap.data.Neighbors)
        {
            EliminatePossibleStructuresOfGivenCell(neighbor, _searchDatasByType[quadType]);
        }
    }

    private void EliminatePossibleStructuresOfGivenCell(Vector2Int neighborCell, StructureTypeSearchData searchData)
    {
        if(determinedCells.Contains(neighborCell)) return;
        if (_typesByCell.TryGetValue(neighborCell, out var neighborCellTypes))
        {
            foreach (var impossibleType in searchData.AdjacencyImpossibilities)
            {
                neighborCellTypes.Remove(impossibleType);
            }
        }

        /*if(searchData.Type != StructureType.RightBatiment) return;
        if (_typesByCell.TryGetValue(neighborCell, out var types))        {
            foreach (var type in types)
                Debug.Log("possible type: " + neighborCell + " " + type);
        }*/
    }

    public bool IsTypeConvenient(StructureType examiningType, Vector2Int[] cells)
    {
        if (cells.Length == 1)
        {
            var possibleStructureTypes = _typesByCell[cells[0]];
            return possibleStructureTypes.Contains(examiningType);
        }
        
        Dictionary<StructureType, int> frequencyInCoords = new();
        foreach (var cell in cells)
        {
            foreach (var type in _typesByCell[cell])
            {
                if (frequencyInCoords.TryGetValue(type, out int count))
                    frequencyInCoords[type] = count + 1;
                else
                    frequencyInCoords[type] = 1; //1
            }
        }
        int maxCount = frequencyInCoords.Values.Max();

       /* Debug.Log(maxCount);
        if (maxCount == 0)
        {
            Debug.LogWarning("No possible structure type found");
        }*/
        
        var topCandidates = frequencyInCoords
            .Where(kvp => kvp.Value == maxCount)
            .Select(kvp => kvp.Key)
            .ToList();
        
        return topCandidates.Contains(examiningType);
    }
    
}