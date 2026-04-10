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

    private SlotData[] slotDataSet;
    private Dictionary<int, List<SlotData>> cellsByColumn = new();

    //todo: call after starting the provider & before randomizer
    public SlotData[] GetRegulatedPlacements(HashSet<SlotData> slotDatas)
    {
        return slotDatas.ToArray();

        Regulate(slotDatas);
        var placeholders = cellsByColumn.Values.SelectMany(d => d).ToArray();
        placeholders = placeholders.OrderBy(p=>p.OrderIndex).ToArray();
        return placeholders;
    }
    private void Regulate(HashSet<SlotData> slotDatas)
    {
        SetPlaceholderData(slotDatas);//todo: ca peut se situer dans la lieu auquel regulate a ete appelle
        
       // SortPlaceholdersByColumn();
        //SortPlaceholdersByRow();
       // SetOrderIndexes();
    }
    
    private void SetPlaceholderData(HashSet<SlotData> slotDatas)
    {
        slotDataSet = slotDatas.ToArray();
    }
    
    private void SortPlaceholdersByColumn()
    {
        /*slotDataSet = slotDataSet.OrderBy(p => p.Cells.y).ToArray();
        var smallestHeight = slotDataSet[0].Cells.y;

        float maxHeight = smallestHeight + heightGap;
        int columnIndex = 0;
        foreach (var placeholder in slotDataSet)
        {
            if (placeholder.Cells.y > maxHeight)
            {
                columnIndex++;
                maxHeight += heightGap;
            }

            if (!cellsByColumn.ContainsKey(columnIndex))
                cellsByColumn.Add(columnIndex, new List<SlotData>());
            
            cellsByColumn[columnIndex].Add(placeholder);
        }*/
    }

    private void SortPlaceholdersByRow()
    {
        /*foreach (var key in cellsByColumn.Keys.ToList())
        {
            cellsByColumn[key] = cellsByColumn[key]
                .OrderBy(p => p.Cells.x)
                .ToList();
        }*/
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
