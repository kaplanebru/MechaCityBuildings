using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public class CityDrawerUnits
{
    public GridData gridData;
    public PaintData paintData;
    
    public GridSystem gridSystem;
    public FloorDatabase floorDatabase;
}

[ExecuteInEditMode]
public class CityDrawer : MonoBehaviour
{
    public QuadSample quadSample;
    public CityDrawerUnits units;
    public Action<int, HashSet<CellData>> OnCellsReady;

     public void ExecutePainting(Event e) //todo: to call with editor update that triggered by Start Painting Button
    {
        if (PaintDetector.TryDetectAvailableCell_Editor(units.gridData,
                units.paintData,
                e,
                out var selectedCellData))
        {
            GridBrusher.BrushSelectedCells(selectedCellData, 
                units.gridSystem.overlayPainter, 
                units.gridData, 
                units.paintData);

        }
    }
    public void UpdateAverageStructureHeight()
    {
        FloorManagement.OnFloorHeightUpdate(units.floorDatabase);

        var activeFloor = units.floorDatabase.GetActiveFloorData();
        units.gridSystem.OnFloorHeightUpdate(activeFloor);
    }

    public void ConstructionRequest()
    {
        var activeFloor = units.floorDatabase.GetActiveFloorData(); //register as CellData
        
        //var cellDataSet = CellDataCreator.CreateCellDataFromSinglePoints(cells.ToHashSet(), units.gridData.MinBuildingCellSize); //units.gridSystem.cellRecorderCache
        var cellDataSet = MapOrganizer.ToCellData(units.gridSystem.cellRecorderCache.ToHashSet(), quadSample, 1);
        
        OnCellsReady?.Invoke(activeFloor.Index, cellDataSet);
        GridMasker.ResetSelectedCells(units.gridSystem.overlayPainter, units.gridSystem.gridData);

    }
}

public enum UserStates
{
    Empty,
    Drawing,
    Construction,
    Randomizing
}