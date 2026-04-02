using System;
using System.Collections.Generic;

[Serializable]
public class FloorWithResidents
{
    public List<CellData> CellDataSet = new();
    public Structure[] Structures;
    
    //todo: public FloorResidentsData ResidentsData = new();
    public FloorData FloorData;

    public FloorWithResidents(FloorData floorData)
    {
        FloorData = floorData;
    }
}