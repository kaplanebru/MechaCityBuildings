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
    public bool OnPaintingState = false;
    public CityBuilderUnits units;
}
