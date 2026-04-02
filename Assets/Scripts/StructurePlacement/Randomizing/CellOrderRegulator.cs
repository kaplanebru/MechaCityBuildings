using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellOrderRegulator
{
    private float heightGap;
    public CellOrderRegulator(float heightGap)
    {
        this.heightGap = heightGap;
    }

    private CellData[] cellDataSet;
    private Dictionary<int, List<CellData>> cellsByColumn = new();

    //todo: call after starting the provider & before randomizer
    public CellData[] GetRegulatedPlacements(HashSet<CellData> cellDatas)
    {
        Regulate(cellDatas);
        var placeholders = cellsByColumn.Values.SelectMany(d => d).ToArray();
        placeholders = placeholders.OrderBy(p=>p.OrderIndex).ToArray();
        return placeholders;
    }
    private void Regulate(HashSet<CellData> cellDatas)
    {
        SetPlaceholderData(cellDatas);//todo: ca peut se situer dans la lieu auquel regulate a ete appelle
        
        SortPlaceholdersByColumn();
        SortPlaceholdersByRow();
        SetOrderIndexes();
    }
    
    private void SetPlaceholderData(HashSet<CellData> cellDatas)
    {
        cellDataSet = cellDatas.ToArray();
    }
    
    private void SortPlaceholdersByColumn()
    {
        cellDataSet = cellDataSet.OrderBy(p => p.CellIndex.y).ToArray();
        var smallestHeight = cellDataSet[0].CellIndex.y;

        float maxHeight = smallestHeight + heightGap;
        int columnIndex = 0;
        foreach (var placeholder in cellDataSet)
        {
            if (placeholder.CellIndex.y > maxHeight)
            {
                columnIndex++;
                maxHeight += heightGap;
            }

            if (!cellsByColumn.ContainsKey(columnIndex))
                cellsByColumn.Add(columnIndex, new List<CellData>());
            
            cellsByColumn[columnIndex].Add(placeholder);
        }
    }

    private void SortPlaceholdersByRow()
    {
        foreach (var key in cellsByColumn.Keys.ToList())
        {
            cellsByColumn[key] = cellsByColumn[key]
                .OrderBy(p => p.CellIndex.x)
                .ToList();
        }
    }
    
    private void SetOrderIndexes()
    {
        int counter = 0;
        foreach (var placeholders in cellsByColumn.Values)
        {
            foreach (var placeholder in placeholders)
            {
                placeholder.SetOrderIndex(counter);
                counter++;
            }
        }
    }

}
