public class PendingPlacements
{
    public StructureType Type;
    public int Amount;
    public int RemainingQuota;

    public PendingPlacements(StructureType type, int amount)
    {
        Type = type;
        Amount = amount;
    }

    public void SetRemainingQuota(int remainingQuota)
    {
        RemainingQuota = remainingQuota;
    }
}