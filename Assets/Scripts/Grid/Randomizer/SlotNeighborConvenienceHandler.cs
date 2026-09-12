using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class SlotNeighborConvenienceHandler
{
    private Dictionary<StructureType, StructureTypeSearchData> inconvenientsByStructureType = new();
    private Dictionary<Vector2Int, StructureType> _filledCells = new();

    public SlotNeighborConvenienceHandler(HashSet<StructureTypeSearchData> structureTypeDatas)
    {
        foreach (var structureTypeData in structureTypeDatas)
        {
            inconvenientsByStructureType.Add(structureTypeData.Type, structureTypeData);
        }
    }

    public void UpdateFilledCells(QuadOnMap quadOnMap, StructureType quadType)
    {
        foreach (var cell in quadOnMap.data.Coords)
        {
            if (_filledCells.ContainsKey(cell))
            {
                Debug.Log($"Filled cell repetition: {_filledCells[cell]}");
                continue;
            }

            _filledCells.Add(cell, quadType);
        }
    }


    public bool IsTypeConvenient2(StructureType currentType, Vector2Int[] neighbors)
    {
        foreach (var neighbor in neighbors)
        {
            if (_filledCells.TryGetValue(neighbor, out var filledCellType))
            {
                //Debug.Log($"Neighbor {neighbor} has type {determinedCellType}, impossibleTypes: {string.Join(",", impossibleTypes)}");
                // Şu an yerleştirilen tip, komşunun impossible listesinde mi?
                var inconvenientTypes = inconvenientsByStructureType[currentType].ImpossibleStructureTypes;
                if (inconvenientTypes.Contains(filledCellType))
                    return false;

                // Komşu, şu an yerleştirilen tipi impossible olarak görüyor mu?
                var neighborInconvenientTypes =
                    inconvenientsByStructureType[filledCellType].ImpossibleStructureTypes;
                if (neighborInconvenientTypes.Contains(currentType))
                    return false;
            }
        }

        return true;
    }
}