using System;
using UnityEngine;

[Serializable]
public class CityBuilderUnits
{
    public Painter painter;
    public Builder builder;
    public GridSystem gridSystem;
    public FloorManagement floorManagement;
}
public class CityBuilder : MonoBehaviour
{
    public CityBuilderUnits units;

    public void UpdateAverageBuildingHeight()
    {
        units.floorManagement.OnFloorHeightUpdate();
        
        var activeFloor = units.floorManagement.db.GetActiveFloorData();
        units.gridSystem.OnFloorHeightUpdate(activeFloor);
    }
}

public enum UserStates
{
    Empty,
    Drawing,
    Construction,
    Randomizing
}