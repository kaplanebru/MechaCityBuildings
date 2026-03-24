using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]

public class CellRegistry
{
    
    public static HashSet<CellWorldData> RegisterCellsOnFloorAndSendWorldCells(List<Vector2Int> cellRecorderCache, FloorData floorData, GridData gridData)
    {
        var registeredCells = cellRecorderCache.ToHashSet();
        
        if (registeredCells.Count == 0)
        {
            Debug.Log("No tracked cells found");
            return null;
        }
        
        HashSet<CellWorldData> cellWorldDataset = new();
        foreach (var cell in registeredCells)
        {
            floorData.AddCell(cell);

            Vector3 worldPos = CellConverter.GetWorldPositionCenterFromCellIndex(cell.x, cell.y, gridData);
            //worldPos.y += gridData.AverageBuildingHeight;
            
            var cellWorldData = new CellWorldData(
                worldPos, 
                Quaternion.identity, 
                Vector3.one * gridData.BuildingCellSize); 
            
            //Vector3 cellScale = paintData.CellSizeInWorldUnits
            //todo: brushdatadan zemini çek, aslında hiç sizinge gerek yok, cell'i binaya göre sizelıyoruz
            
            cellWorldDataset.Add(cellWorldData);
        }
        
        return cellWorldDataset;
        //OnCellsReady?.Invoke(cellWorldDataset, floorData.FloorIdentifier.Index);
    }

}