using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiceRoller
{
    public ReplacementType RollDices(Dictionary<ReplacementType, int> amountsByType)
    {
        if(amountsByType.Count == 1)
            return amountsByType.First().Key;
        
        
        
        var sum = amountsByType.Values.Sum();
        if (sum == 0)
        {
            Debug.LogWarning("roll sum is zero");
        }
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
    
   /* public class DiceData
    {
        public ReplacementType Type;
        public int Amount;
        public DiceData(ReplacementType type, int amount, float multiplier)
        {
            Type = type;
            Amount = Mathf.FloorToInt(amount/multiplier);
        }
    
    }
    public class DiceRoller
    {
        public ReplacementType RollDices(DiceData[] dices)
        {
            if(dices.Length == 1)
                return dices.First().Type;
        
            var sum = dices.Sum(d => d.Amount);
            if (sum == 0)
            {
                Debug.LogWarning("roll sum is zero");
            }
            int roll = Random.Range(0, sum);
            int cumulative = 0;

            foreach (var dice in dices)
            {
                cumulative += dice.Amount;
                if (roll < cumulative)
                    return dice.Type;
            }
        
            Debug.LogError("RollDices don't match");
            return dices.First().Type;
        }*/
}
