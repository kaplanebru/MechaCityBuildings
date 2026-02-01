using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiceRoller
{
    public ReplacementType RollDices(PendingReplacement[] pendingReplacements)
    {
        if (pendingReplacements.Length == 1)
            return pendingReplacements.First().Type;

        foreach (var replacement in pendingReplacements)
        {
            /*if (replacement.RemainingQuota == 0)
            {
                Debug.LogError("zero remaining quota");
            }*/
            replacement.Amount += Mathf.RoundToInt(100/(replacement.RemainingQuota + 1));
        }
        
        var sum = pendingReplacements.Sum(p=>p.Amount);
        if (sum == 0)
        {
            Debug.LogWarning("roll sum is zero");
        }
        int roll = Random.Range(0, sum);
        int cumulative = 0;

        foreach (var replacement in pendingReplacements)
        {
            cumulative += replacement.Amount;
            if (roll < cumulative)
                return replacement.Type;
        }
        
        Debug.LogError("RollDices don't match");
        return pendingReplacements.First().Type;
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
