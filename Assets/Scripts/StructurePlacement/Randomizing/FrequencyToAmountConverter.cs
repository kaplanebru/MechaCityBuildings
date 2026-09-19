using System;
using System.Linq;
using UnityEngine;

public static class FrequencyToAmountConverter
{
    private static int GetRatioSum(FrequencyData[] frequencyDataSet)
    {
        return frequencyDataSet.Sum(d => d.Frequency * d.Volume);
    }

    public static void SetAmountsByRatio(FrequencyData[] frequencyDataSet, int totalCellAmount)
    {
        float ratioSum = GetRatioSum(frequencyDataSet);

        foreach (var data in frequencyDataSet)
        {
            data.Amount = Mathf.FloorToInt(totalCellAmount * (data.Frequency * data.Volume / ratioSum));
        }

        HandleRest(frequencyDataSet, totalCellAmount);
    }

    private static void HandleRest(FrequencyData[] frequencyDataSet, int totalCellCount)
    {
        var totalAmount = frequencyDataSet.Sum(d => d.Amount);
        int rest = totalCellCount - totalAmount;
        
        Debug.Log("rest: " + rest + " total cell count: " + totalCellCount);

        if (rest == 0) return;
        
        /*var smallest = frequencyDataSet.OrderBy(d => d.Volume).First();

        while (rest >= smallest.Volume)
        {
            rest -= smallest.Volume;
            smallest.Amount++;
        }*/
    }
}