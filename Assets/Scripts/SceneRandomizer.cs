using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class RandomizerData
{
    public ReplacementType Type;
    [Range(0,100)] public int Ratio = 50;
    public int Amount { get; set; }
}

public class SceneRandomizer : MonoBehaviour
{
    [SerializeField] private RandomizerData[] datas;

    private List<Placeholder> _placeholders = new List<Placeholder>();
    private Placeholder[] _shuffled;

    private void GetAllPlaceholders()
    {
        _placeholders.Clear();
        _placeholders = FindObjectsByType<Placeholder>(FindObjectsSortMode.None)
            .Where(p=>p.canBeOrderedRandomly).ToList();
    }

    public void MixAndApply()
    {
        if (_placeholders.Count == 0)
            GetAllPlaceholders();

        _shuffled = _placeholders.OrderBy(_ => UnityEngine.Random.value).ToArray();
        
        SetAmountsByRatio();
        ApplyTypes();
    }

    private void ApplyTypes()
    {
        int amount = 0;
        int rest = 0;
        foreach (var data in datas)
        {
            amount += data.Amount;
            for (int i = rest; i < amount; i++)
            {
                _shuffled[i].replacementType = data.Type;
            }
            rest += amount;
        }
        
        CheckForRest(rest);
    }

    private int GetRatioSum()
    {
        return datas.Sum(d=>d.Ratio);
    }
    private void SetAmountsByRatio()
    {
        int totalAmount = _placeholders.Count;
        float ratioSum = GetRatioSum();
        
        foreach (var data in datas)
        {
            data.Amount = Mathf.FloorToInt(totalAmount * data.Ratio / ratioSum);
            Debug.Log("Amount: " + data.Amount);
        }
    }

    private void CheckForRest(int rest)
    {
        if(rest == _placeholders.Count) return;
        
        for (int i = _placeholders.Count - 1; i >= rest; i--)
        {
            _shuffled[i].replacementType = datas.Last().Type;
        }
    }
    
    public void ResetAllToGivenType(ReplacementType resetType)
    {
        _placeholders.ForEach(p => { p.replacementType = resetType;});
    }
}