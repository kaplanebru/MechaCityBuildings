using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GridSystem : MonoBehaviour
{
    [SerializeField] private UserPreferences userPreferences;
    [SerializeField] private GridData gridData;
    [SerializeField] private PaintData paintData;
    [SerializeField] private MapSizeToGridSize mapSizeToGridSize;
    
    public void Initialize()
    {
        Configurations.SetData(userPreferences);
        AdaptGridSizeToUserCellSize();
    }
    
    private void AdaptGridSizeToUserCellSize()
    {
        if (userPreferences.UseMapSizeForGridSize)
            userPreferences.ProjectedGridSize = mapSizeToGridSize.GetGridSizeFromMesh();

        userPreferences.ProjectedGridSize.x =
            Mathf.RoundToInt(userPreferences.ProjectedGridSize.x / userPreferences.BuildingCellSize);
        userPreferences.ProjectedGridSize.y =
            Mathf.RoundToInt(userPreferences.ProjectedGridSize.y / userPreferences.BuildingCellSize);

        gridData.AdaptiveGridSize = userPreferences.ProjectedGridSize;
        gridData.CellSize = userPreferences.BuildingCellSize;
    }

   
}