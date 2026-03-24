using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DiceRoller
{
    public static StructureType RollDices(PendingPlacements[] pendingReplacements)
    {
        if (pendingReplacements.Length == 1)
            return pendingReplacements.First().Type;

        foreach (var pending in pendingReplacements)
        {
            pending.Amount += Mathf.RoundToInt(100f/(pending.RemainingQuota + 1));
        }
        
        var sum = pendingReplacements.Sum(p=>p.Amount);
        if (sum == 0)
        {
            Debug.LogWarning("roll sum is zero");
        }
        int roll = Random.Range(0, sum);
        int cumulative = 0;

        foreach (var pending in pendingReplacements)
        {
            cumulative += pending.Amount;
            if (roll < cumulative)
                return pending.Type;
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
