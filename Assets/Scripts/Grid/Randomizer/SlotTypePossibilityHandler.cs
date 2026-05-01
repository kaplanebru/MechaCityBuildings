using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class SlotTypePossibilityHandler
{
    private Dictionary<StructureType, StructureTypeSearchData> impossibilitiesByStructureType = new();
    private Dictionary<Vector2Int, StructureType> _filledCells = new();

    public SlotTypePossibilityHandler(HashSet<StructureTypeSearchData> structureTypeDatas)
    {
        foreach (var structureTypeData in structureTypeDatas)
        {
            impossibilitiesByStructureType.Add(structureTypeData.Type, structureTypeData);
        }
    }

    public void UpdateFilledCells(QuadOnMap quadOnMap, StructureType quadType)
    {
        foreach (var cell in quadOnMap.data.Coords)
        {
            if (_filledCells.ContainsKey(cell))
            {
                Debug.Log($"Filled cell repetition: {_filledCells[cell]}");
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
                var impossibleTypes = impossibilitiesByStructureType[currentType].ImpossibleStructureTypes;
                if (impossibleTypes.Contains(filledCellType))
                    return false;

                // Komşu, şu an yerleştirilen tipi impossible olarak görüyor mu?
                var neighborImpossibleTypes =
                    impossibilitiesByStructureType[filledCellType].ImpossibleStructureTypes;
                if (neighborImpossibleTypes.Contains(currentType))
                    return false;
            }
        }

        return true;
    }
}