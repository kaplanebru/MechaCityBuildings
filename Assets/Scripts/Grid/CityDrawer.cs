using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CityDrawerUnits
{
    public GridData gridData;
    public PaintData paintData;
    
    public GridSystem gridSystem;
    public FloorManagement floorManagement;
}

[ExecuteInEditMode]
public class CityDrawer : MonoBehaviour
{
   
    public CityDrawerUnits units;
    public Action<HashSet<CellWorldData>, int> OnCellsReady;

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
        units.floorManagement.OnFloorHeightUpdate();

        var activeFloor = units.floorManagement.db.GetActiveFloorData();
        units.gridSystem.OnFloorHeightUpdate(activeFloor);
    }

    public void ConstructBuildingsRequest()
    {
        var activeFloor = units.floorManagement.db.GetActiveFloorData();
        var worldCells = CellRegistry.RegisterCellsOnFloorAndSendWorldCells(
            units.gridSystem.cellRecorderCache,
            activeFloor,
            units.gridSystem.gridData);

        GridMasker.ResetSelectedCells(units.gridSystem.overlayPainter, units.gridSystem.gridData);
        
        print("world cells" + worldCells.Count);
        OnCellsReady?.Invoke(worldCells, activeFloor.Index);
    }
}

public enum UserStates
{
    Empty,
    Drawing,
    Construction,
    Randomizing
}