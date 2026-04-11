using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CityData", menuName = "CityBuilder/CityData")]
public class CityData: ScriptableObject
{
    public float HeightGap = 2;
    public JuxtapositionData[] JuxtapositionDataSet;
    //public DistanceData[] HorizontalDistanceBetweenBuildings;
    
    public RandomizerData[] RandomizerDataSet;


    private Dictionary<int, int> QuotaByHeigt = new();

    public Dictionary<int, int> GetQuotaByHeight()
    {
        foreach (var juxtapositionData in JuxtapositionDataSet)
        {
            QuotaByHeigt[juxtapositionData.HeightTier] =  juxtapositionData.MaxJuxtapositionQuota;
        }
        return QuotaByHeigt;
    }
    
    public StructureType[] GetSelectedStructureTypes()=>RandomizerDataSet.Select(t=>t.Type).ToArray();
}

[Serializable]
public class JuxtapositionData
{
    public int HeightTier;
    [Range(1, 20)] public int MaxJuxtapositionQuota = 2;
}