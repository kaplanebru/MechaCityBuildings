using System;
using System.Collections.Generic;

[Serializable]
public class CityData
{
    public float HeightGap = 2;
    public JuxtapositionData[] JuxtapositionDataSet;
    public DistanceData[] HorizontalDistanceBetweenBuildings;

    private Dictionary<int, int> QuotaByHeigt = new();

    public Dictionary<int, int> GetQuotaByHeight()
    {
        foreach (var juxtapositionData in JuxtapositionDataSet)
        {
            QuotaByHeigt[juxtapositionData.HeightTier] =  juxtapositionData.MaxJuxtapositionQuota;
        }
        return QuotaByHeigt;
    }
}

[Serializable]
public class JuxtapositionData
{
    public int HeightTier;
    public int MaxJuxtapositionQuota = 2;
}