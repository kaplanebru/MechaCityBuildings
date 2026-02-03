using System;
using System.Linq;
using UnityEngine;

public class FrequencyToAmountConverter
{
    private FrequencyData[] _frequencyDataSet;
    private int _totalBodyCount;

    public void Setup(FrequencyData[] frequencyDatas, int totalBodyCount)
    {
        _frequencyDataSet = frequencyDatas;
        _totalBodyCount = totalBodyCount;
    }
    private int GetRatioSum()
    {
        return _frequencyDataSet.Sum(d => d.Frequency);
    }

    public void SetAmountsByRatio()
    {
        int totalAmount = _totalBodyCount;
        float ratioSum = GetRatioSum();

        foreach (var data in _frequencyDataSet)
        {
            data.Amount = Mathf.FloorToInt(totalAmount * data.Frequency / ratioSum);
        }
        
        HandleRest();
    }

    private void HandleRest()
    {
        var totalAmount = _frequencyDataSet.Sum(d => d.Amount);
        int rest = _totalBodyCount - totalAmount;
        
        if(rest == 0) return;
        _frequencyDataSet = _frequencyDataSet.OrderBy(d => d.Amount).ToArray();
        
        while (rest > 0)
        {
            foreach (var data in _frequencyDataSet)
            {
                data.Amount++;
                rest--;
                if(rest == 0) return;
            }
        }
    }
}
