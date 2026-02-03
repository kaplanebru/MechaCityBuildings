using System.Collections.Generic;
using System.Linq;

public class HeightTierHelper
{
    private Dictionary<int, HeightTierData> DataByTier { get; } = new();
    public HeightTierData GetDataByTier(int tier)=> DataByTier[tier];
    
    public int GetMaxQuotaByTier(int tier)=> GetDataByTier(tier).MaxQuota;

    public void SetHeightTierDatas(Dictionary<int, int> quotasByHeightTier)
    {
        foreach (var quotaByTier in quotasByHeightTier)
        {
            DataByTier.Add(quotaByTier.Key, new HeightTierData(quotaByTier.Key, quotaByTier.Value));
        }
    }
    public void UpdateUsedQuota(int tier)
    {
        GetDataByTier(tier).UsedQuota++;
    }
    
    public void ResetQuotasExcept(int selectedHeightTier)
    {
        foreach (var data in DataByTier.Values)
        {
            if (data.Tier == selectedHeightTier) continue;
            data.ResetUsedQuota();
        }
    }
    public void ResetQuotas()
    {
        DataByTier.Values.ToList().ForEach(x => x.ResetUsedQuota());
    }
    
    public bool HasQuota(int heightTier) => GetDataByTier(heightTier).HasQuota();

    public int GetRemainingQuota(int heightTier)
    {
        var data = GetDataByTier(heightTier);
        return data.MaxQuota - data.UsedQuota;
    }

}