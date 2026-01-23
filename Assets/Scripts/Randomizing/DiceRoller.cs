using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class DiceRoller
{
    public ReplacementType RollDices(Dictionary<ReplacementType, int> amountsByType)
    {
        var sum = amountsByType.Values.Sum();
        int roll = Random.Range(0, sum);
        int cumulative = 0;

        foreach (var amountByType in amountsByType)
        {
            cumulative += amountByType.Value;
            if (roll < cumulative)
                return amountByType.Key;
        }
        
        Debug.LogError("RollDices don't match");
        return amountsByType.First().Key;
    }
}
