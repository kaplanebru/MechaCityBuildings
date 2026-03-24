using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementOrderRegulator
{
    private float heightGap;
    public PlacementOrderRegulator(float heightGap)
    {
        this.heightGap = heightGap;
    }

    private PlacementData[] placeholderDataSet;
    private Dictionary<int, List<PlacementData>> placeholdersByColumn = new();

    //todo: call after starting the provider & before randomizer
    public PlacementData[] GetRegulatedPlacements(HashSet<CellWorldData> cellDatas)
    {
        Regulate(cellDatas);
        var placeholders = placeholdersByColumn.Values.SelectMany(d => d).ToArray();
        placeholders = placeholders.OrderBy(p=>p.OrderIndex).ToArray();
        return placeholders;
    }
    private void Regulate(HashSet<CellWorldData> cellDatas)
    {
        SetPlaceholderData(cellDatas);//todo: ca peut se situer dans la lieu auquel regulate a ete appelle
        
        SortPlaceholdersByColumn();
        SortPlaceholdersByRow();
        SetOrderIndexes();
    }
    
    private void SetPlaceholderData(HashSet<CellWorldData> cellDatas)
    {
        placeholderDataSet = CellToPlacementData.GetCreatedPlacement(cellDatas).ToArray();
    }
    
    private void SortPlaceholdersByColumn()
    {
        placeholderDataSet = placeholderDataSet.OrderBy(p => p.Position.y).ToArray();
        var smallestHeight = placeholderDataSet[0].Position.y;

        float maxHeight = smallestHeight + heightGap;
        int columnIndex = 0;
        foreach (var placeholder in placeholderDataSet)
        {
            if (placeholder.Position.y > maxHeight)
            {
                columnIndex++;
                maxHeight += heightGap;
            }

            if (!placeholdersByColumn.ContainsKey(columnIndex))
                placeholdersByColumn.Add(columnIndex, new List<PlacementData>());
            
            placeholdersByColumn[columnIndex].Add(placeholder);
        }
    }

    private void SortPlaceholdersByRow()
    {
        foreach (var key in placeholdersByColumn.Keys.ToList())
        {
            placeholdersByColumn[key] = placeholdersByColumn[key]
                .OrderBy(p => p.Position.x)
                .ToList();
        }
    }
    
    private void SetOrderIndexes()
    {
        int counter = 0;
        foreach (var placeholders in placeholdersByColumn.Values)
        {
            foreach (var placeholder in placeholders)
            {
                placeholder.SetOrderIndex(counter);
                counter++;
            }
        }
    }

}
