using System;
using System.Collections.Generic;

[Serializable]
public class PlacementFloor
{
    public List<PlacementData> PlacementDataset = new();
    public Structure[] Structures;
    public FloorData FloorData;

    public PlacementFloor(FloorData floorData)
    {
        FloorData = floorData;
    }
}