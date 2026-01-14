using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BatimentRandomizer : MonoBehaviour
{
    [SerializeField] private ReplacementType currentType = ReplacementType.RightBatiment;
    [SerializeField] private ReplacementType intruderType = ReplacementType.LeftBatiment;
    
    private List<Placeholder> _currentBatiments = new List<Placeholder>();
    [SerializeField] private float mixRatio100 = 40;
    
    private void GetAllBatimentsInCurrentType()
    {
        _currentBatiments.Clear();
        _currentBatiments = FindObjectsByType<Placeholder>(FindObjectsSortMode.None).
            Where(p=> p.canBeCollectedRandomly && p.replacementType == currentType).ToList();
    }

    public void MixBatiments()
    {
        if(_currentBatiments.Count == 0)
            GetAllBatimentsInCurrentType();
        else
            ResetBatimentsToCurrentType();

        var shuffled = _currentBatiments.OrderBy(_ => UnityEngine.Random.value).ToArray();
        int mixAmount = GetCalculatedMixAmount();
        for (int i = 0; i < mixAmount; i++)
        {
            shuffled[i].replacementType = intruderType;
        }
    }

    private int GetCalculatedMixAmount()
    {
        int total = _currentBatiments.Count;
        float floatAmount = total * mixRatio100 / 100;
        return Mathf.FloorToInt(floatAmount);
     
    }
    public void ResetBatimentsToCurrentType()
    {
        _currentBatiments.ForEach(p => { p.replacementType = currentType; });
    }
}
