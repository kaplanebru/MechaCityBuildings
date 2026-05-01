using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlotTypePossibilityHandler
{
    //private Dictionary<Vector2Int, HashSet<StructureType>> _possibleTypesByCell = new();
    private Dictionary<StructureType, StructureTypeSearchData> _structureTypeDatas = new();
    private Dictionary<Vector2Int, StructureType> _determinedCells = new();

    public SlotTypePossibilityHandler(HashSet<Vector2Int> cells, HashSet<StructureTypeSearchData> structureTypeDatas)
    {
        var allTypes = structureTypeDatas.Select(s => s.Type).ToHashSet();

        /*_possibleTypesByCell = cells.ToDictionary(
            cell => cell,
            cell => new HashSet<StructureType>(allTypes));*/

        foreach (var structureTypeData in structureTypeDatas)
        {
            _structureTypeDatas.Add(structureTypeData.Type, structureTypeData);
        }
    }

    public void UpdateNeighbourPossibilities(QuadOnMap quadOnMap, StructureType quadType)
    {
        //POSSİBLE BUG: AYNI CELL'E TEKRAR TEKRAR GİDİLDİĞİNDE UPDATE EDİLİYOR OLABİLİR Mİ
        foreach (var cell in quadOnMap.data.Coords)
        {
            //_possibleTypesByCell[cell].Clear();
            //_possibleTypesByCell[cell].Add(quadType);

            _determinedCells.Add(cell, quadType);
        }

        /*foreach (var neighbor in quadOnMap.data.Neighbors)
        {
            EliminatePossibleStructuresOfGivenCell(neighbor, _structureTypeDatas[quadType]);
            //Debug.Log("eliminated neighbor with neighbor type = " + quadOnMap + "possible type: " + _searchDatasByType[quadType].Amount);
        }*/
    }

    private void EliminatePossibleStructuresOfGivenCell(Vector2Int neighborCell,
        StructureTypeSearchData structureTypeData)
    {
       /* if (_determinedCells.ContainsKey(neighborCell)) return;
        if (_possibleTypesByCell.TryGetValue(neighborCell, out var neighborCellTypes))
        {
            foreach (var impossibleType in structureTypeData.ImpossibleStructureTypes)
            {
                neighborCellTypes.Remove(impossibleType);
            }
        }*/
    }

    public bool IsTypeConvenient2(HashSet<StructureType> impossibleTypes, Vector2Int[] neighbors)
    {
        foreach (var neighbor in neighbors)
        {
            if (_determinedCells.TryGetValue(neighbor, out var determinedCellType))
            {
                if (impossibleTypes.Contains(determinedCellType))
                {
                    return false;
                }
            }
        }
        return true;
    }

    /*public bool IsTypeConvenient(StructureType examiningType, Vector2Int[] quadCells)
    {
        /*if (cells.Length == 1)
        {
            //what about neighbors?
            var possibleStructureTypes = _possibleTypesByCell[cells[0]];
            return possibleStructureTypes.Contains(examiningType);
        }*/

        /*Dictionary<StructureType, int> frequencyInCoords = new();
        foreach (var quadCell in quadCells)
        {
            if (_possibleTypesByCell.TryGetValue(quadCell, out var possibleTypes))
            {
                foreach (var type in _possibleTypesByCell[quadCell])
                {
                    if (frequencyInCoords.TryGetValue(type, out int count))
                        frequencyInCoords[type] = count + 1;
                    else
                        frequencyInCoords[type] = 1;
                }
            }
        }

        int maxCount = frequencyInCoords.Values.Max();

        /* Debug.Log(maxCount);
         if (maxCount == 0)
         {
             Debug.LogWarning("No possible structure type found");
         }*/

       /* var topCandidates = frequencyInCoords
            .Where(kvp => kvp.Value == maxCount)
            .Select(kvp => kvp.Key)
            .ToList();

        return topCandidates.Contains(examiningType);
    }*/
}