using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CityRegulator
{
    private float heightGap = 2;
    private PlaceholderData[] placeholderDataSet;
    private Dictionary<int, List<PlaceholderData>> placeholdersByColumn = new();

    //todo: call after starting the provider & before randomizer
    public PlaceholderData[] GetRegulatedPlaceholdersData()
    {
        Regulate();
        return placeholdersByColumn.Values.SelectMany(v => v).ToArray();
    }
    private void Regulate()
    {
        SetPlaceholderData();
        SortPlaceholdersByColumn();
        SortPlaceholdersByRow();
        SetOrderIndexes();
    }
    private void SetPlaceholderData()
    {
        placeholderDataSet = PlaceholderProvider.GetPlaceholderDataSet().ToArray();
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
                placeholdersByColumn.Add(columnIndex, new List<PlaceholderData>());
            
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
