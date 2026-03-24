using System;
using System.Linq;
using UnityEngine;

public static class FrequencyToAmountConverter
{
    private static int GetRatioSum(FrequencyData[] frequencyDataSet)
    {
        return frequencyDataSet.Sum(d => d.Frequency);
    }

    public static void SetAmountsByRatio(FrequencyData[] frequencyDataSet, int totalBodyCount)
    {
        int totalAmount = totalBodyCount;
        float ratioSum = GetRatioSum(frequencyDataSet);

        foreach (var data in frequencyDataSet)
        {
            data.Amount = Mathf.FloorToInt(totalAmount * data.Frequency / ratioSum);
        }
        
        HandleRest(frequencyDataSet, totalAmount);
    }

    private static void HandleRest(FrequencyData[] frequencyDataSet, int totalBodyCount)
    {
        var totalAmount = frequencyDataSet.Sum(d => d.Amount);
        int rest = totalBodyCount - totalAmount;
        
        if(rest == 0) return;
        frequencyDataSet = frequencyDataSet.OrderBy(d => d.Amount).ToArray();
        
        while (rest > 0)
        {
            foreach (var data in frequencyDataSet)
            {
                data.Amount++;
                rest--;
                if(rest == 0) return;
            }
        }
    }
}
