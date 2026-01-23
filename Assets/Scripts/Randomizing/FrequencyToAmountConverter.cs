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
    }

    public void CheckForRest(int rest, Action<int> applyValueCallback)
    {
        if (rest == _totalBodyCount) return;

        for (int i = _totalBodyCount - 1; i >= rest; i--)
        {
            applyValueCallback(i);
           // _placeholderDataSet[i].SetType(randomizerDataSet.Last().Type);
        }
    }
}
