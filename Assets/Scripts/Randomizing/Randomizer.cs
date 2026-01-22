using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Randomizer
{
    private RandomizerData[] randomizerDataSet;
    private List<PlaceholderData> _placeholderDataSet = new();


    public void Setup(RandomizerData[] randomizerDatas, List<PlaceholderData> placeholderDatas)
    {
        randomizerDataSet = randomizerDatas;
        _placeholderDataSet = placeholderDatas;
    }
    private int GetRatioSum()
    {
        return randomizerDataSet.Sum(d => d.Ratio);
    }

    public void SetAmountsByRatio()
    {
        int totalAmount = _placeholderDataSet.Count;
        float ratioSum = GetRatioSum();

        foreach (var data in randomizerDataSet)
        {
            data.Amount = Mathf.FloorToInt(totalAmount * data.Ratio / ratioSum);
        }
    }

    public void CheckForRest(int rest)
    {
        if (rest == _placeholderDataSet.Count) return;

        for (int i = _placeholderDataSet.Count - 1; i >= rest; i--)
        {
            _placeholderDataSet[i].SetType(randomizerDataSet.Last().Type);
        }
    }
}
