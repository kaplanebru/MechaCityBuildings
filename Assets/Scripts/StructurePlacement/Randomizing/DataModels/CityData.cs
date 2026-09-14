using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CityData
{
    public List<RandomizerData> RandomizerDataSet = new();
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

    public void RemoveStructureType(StructureType structureType)
    {
        RandomizerDataSet.Remove(RandomizerDataSet.FirstOrDefault(t => t.Type == structureType));
    }
    private void Reset()
    {
        matrix = new bool[RandomizerDataSet.Count];
        Array.Fill(matrix, true);  
    }
    
}