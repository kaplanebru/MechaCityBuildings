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
    public StructureTypeData[] structureTypeDatas; //TEMP: test
    public CityDrawerUnits units;
    public Action<int, HashSet<SlotData>> OnSlotsReady;
    
    [HideInInspector]
    public bool[] adjacency;

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
        var activeFloor = units.floorDatabase.GetActiveFloorData(); //register as SlotData
        
        //var cellDataSet = SlotDataCreator.CreateCellDataFromSinglePoints(cells.ToHashSet(), units.gridData.MinBuildingCellSize); //units.gridSystem.cellRecorderCache
        
        //TEMPORARY: TEST
        
        var adjacencyPossibilities = 
            AdjacencyHelper.GetPossibleAdjacencyDB(structureTypeDatas.Select(s=>s.Type).ToArray(), adjacency);
        Dictionary<StructureTypeData, int> quadSamplesAndAmounts = new ();
        quadSamplesAndAmounts.Add(structureTypeDatas[0], int.MaxValue);
        quadSamplesAndAmounts.Add(structureTypeDatas[1], 2); //averageFrequency
        var cellDataSet = MapOrganizer.ToSlotData(
            units.gridSystem.cellRecorderCache.ToHashSet(), 
            quadSamplesAndAmounts,
            adjacencyPossibilities);
        
        OnSlotsReady?.Invoke(activeFloor.Index, cellDataSet);
        GridMasker.ResetSelectedCells(units.gridSystem.overlayPainter, units.gridSystem.gridData);

    }
    
    public void InitiateMatrixIfNeeded()
    {
        int matrixSize = Mathf.RoundToInt(Mathf.Pow(structureTypeDatas.Length, 2));
        if (adjacency == null || adjacency.Length != matrixSize)
        {
            adjacency = new bool[matrixSize];
        }
    }
}

public enum UserStates
{
    Empty,
    Drawing,
    Construction,
    Randomizing
}