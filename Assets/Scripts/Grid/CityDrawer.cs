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
    public CityDrawerUnits units;
    public Action<int, HashSet<Vector2Int>, HashSet<CellWorldData>> OnCellsReady;

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
    public void UpdateAverageBuildingHeight()
    {
        FloorManagement.OnFloorHeightUpdate(units.floorDatabase);

        var activeFloor = units.floorDatabase.GetActiveFloorData();
        units.gridSystem.OnFloorHeightUpdate(activeFloor);
    }

    public void ConstructBuildingsRequest()
    {
        var activeFloor = units.floorDatabase.GetActiveFloorData();
        var cells = units.gridSystem.cellRecorderCache;
            
        var worldCells = CellRegistry.RegisterCellsOnFloorAndSendWorldCells(
            cells,
            activeFloor,
            units.gridSystem.gridData);

        GridMasker.ResetSelectedCells(units.gridSystem.overlayPainter, units.gridSystem.gridData);
        
        print("world cells" + worldCells.Count);
        OnCellsReady?.Invoke(activeFloor.Index, cells.ToHashSet(), worldCells);
    }
}

public enum UserStates
{
    Empty,
    Drawing,
    Construction,
    Randomizing
}