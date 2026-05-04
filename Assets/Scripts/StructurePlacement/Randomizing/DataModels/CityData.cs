using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CityData", menuName = "CityBuilder/CityData")]
public class CityData : ScriptableObject
{
    public float HeightGap = 2;


    public RandomizerData[] RandomizerDataSet;

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


    public StructureType[] GetSelectedStructureTypes() => RandomizerDataSet.Select(t => t.Type).ToArray();
}

