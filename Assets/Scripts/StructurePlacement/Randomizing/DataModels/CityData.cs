using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CityData
{
    public RandomizerData[] RandomizerDataSet;
    [HideInInspector] public bool[] matrix;

    public StructureType[] GetSelectedStructureTypes() => RandomizerDataSet.Select(t => t.Type).ToArray();

    public bool TryGetAmountByType(StructureType structureType, out int amount)
    {
        amount = -1;
        var data = RandomizerDataSet.FirstOrDefault(t => t.Type == structureType);
        if (data != null)
        {
            amount = data.FrequencyData.Amount;
            return true;
        }

        return false;
    }
    
    void Reset()
    {
        matrix = new bool[RandomizerDataSet.Length];
        Array.Fill(matrix, true);  
    }
}