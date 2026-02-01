public class HeightTierData
{
    public int Tier;
    public int MaxQuota;
    public int UsedQuota = 0;

    public HeightTierData(int tier, int maxQuota)
    {
        Tier = tier;
        MaxQuota = maxQuota;
    }
    public void ResetUsedQuota()
    {
        UsedQuota = 0;
    }
    public bool HasQuota()
    {
        return UsedQuota < MaxQuota;
    }
}