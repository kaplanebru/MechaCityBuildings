using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum StructureType
{
    RightBatiment,
    LeftBatiment,
    Stairs,
    VariedBatiment,
    Undefined
}

[ExecuteInEditMode]
public class Installer : MonoBehaviour
{
    private Dictionary<StructureType, List<PlacementData>> _placementDatasByType = new();
    public PlacementDatabase placementDatabase;
    [SerializeField] private FloorManagement floorManagement;
    [SerializeField] private GridData gridData;
    public StructurePool[] pools;
    
    public PlacementFloor _placementFloorToInstall;


    private void OnEnable()
    {
        floorManagement.OnFloorClearRequest += ClearStructuresOnFloor;
    }

    private void OnDisable()
    {
       floorManagement.OnFloorClearRequest -= ClearStructuresOnFloor;
    }

    private void ClassifyPlacementDatasOnFloor(int floorIndex)
    {
        _placementFloorToInstall = placementDatabase.GetPlacementFloor(floorIndex);
        
        if (_placementFloorToInstall.PlacementDataset.Count == 0)
        {
            Debug.Log("No placement dataset found");
            return;
        }

        /*_placementDatasByType.Clear();
        _placementDatasByType = _placementFloorToInstall.PlacementDataset
            .GroupBy(p => p.GetStructureType())
            .ToDictionary(g =>
                g.Key, g => g.ToList());*/
        
        _placementDatasByType.Clear();
        foreach (var pool in pools)
        {
            _placementDatasByType[pool.poolData.StructureType] = new List<PlacementData>();
        }
        foreach (var placementData in _placementFloorToInstall.PlacementDataset)
        {
            _placementDatasByType[placementData.GetStructureType()].Add(placementData);
        }
    }


    public void InitiatePools()
    {
        //todo destroy immediate
        foreach (var pool in pools)
        {
            pool.InitializePool();
        }
    }
    
    private void RestorePoolIfNeeded(StructurePool pool)
    {
        pool.CheckPoolActivity();
        if (!pool.IsInitialized()) 
            pool.InitializePool();
        else
            ReleaseItemsToPool(_placementFloorToInstall.FloorData.FloorIdentifier.Index); //dunno
    }
    
    public void InstallStructures(int floorIndex)
    {
        ClassifyPlacementDatasOnFloor(floorIndex);
        InstallStructuresFromMultiplePools();
    }
    
    private void InstallStructuresFromMultiplePools()
    {
        List<Structure> structuresByType = new ();
        foreach (var pool in pools)
        {
            structuresByType.AddRange(InstallStructuresFromPool(pool));
        }
        
        _placementFloorToInstall.Structures = structuresByType.ToArray();

    }
   
    private Structure[]  InstallStructuresFromPool(StructurePool pool)
    {
        RestorePoolIfNeeded(pool);
        var placementDataset = _placementDatasByType[pool.poolData.StructureType];

        if (pool.poolData.PoolSize < placementDataset.Count)
        {
            Debug.LogWarning("Pool size is too small for " + pool.poolData.StructureType);
            return Array.Empty<Structure>();
        }

        var structuresByType = InstallerHelper.Install(
            placementDataset.ToArray(), 
            _placementFloorToInstall.FloorData.FloorIdentifier.Root, 
            pool);
        
        InstallerHelper.SealCellMetadataToStructure(structuresByType, gridData);

        return structuresByType;
    }

   

    public void ReleaseItemsToPool(int floorIndex) //PlacementFloor placementFloor
    {
       //belki de delete floor deyince hiç floor datadan gitmeyip burdan sileriz

       var placementFloor = placementDatabase.GetPlacementFloor(floorIndex);
       
       if (placementFloor.Structures == null || placementFloor.Structures.Length == 0) return;
       
        foreach (var pool in pools)
        {
            pool.ReleaseItemsToPool(placementFloor.Structures.Where
                (s => s.type == pool.poolData.StructureType).ToArray());
        }
        
        placementFloor.Structures = null;
    }

    public void UninstallStructures(int floorIndex, Action<int> onUninstall)
    {
        ReleaseItemsToPool(floorIndex);

        var placementFloor = placementDatabase.GetPlacementFloor(floorIndex);
        placementFloor.FloorData.ClearCells();
        placementFloor.PlacementDataset.Clear();
        
        onUninstall?.Invoke(floorIndex);
    }
    
    private void ClearStructuresOnFloor(int floorIndex)
    {
        var placementFloor = placementDatabase.GetPlacementFloor(floorIndex);
        
        ReleaseItemsToPool(floorIndex);
        placementDatabase.placementFloors.Remove(placementFloor);
        
    }
}