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
    public FloorResidentsDatabase floorResidentsDatabase;
    [SerializeField] private GridData gridData;
    public StructurePool[] pools;
    
    public FloorResidentsData floorToInstall;
    
    public void InstallStructures(FloorData floorData)
    {
        ClassifyPlacementDatasOnFloor(floorData.Index);
        InstallStructuresFromMultiplePools(floorData.Root);
    }
    
    private void InstallStructuresFromMultiplePools(Transform floorRoot)
    {
        List<Structure> structuresByType = new ();
        foreach (var pool in pools)
        {
            structuresByType.AddRange(InstallStructuresFromPool(pool, floorRoot));
        }
        
        floorToInstall.Structures = structuresByType.ToArray();
    }
    
    private Structure[]  InstallStructuresFromPool(StructurePool pool, Transform floorRoot)
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
            floorRoot, 
            pool);
        
        InstallerHelper.SealCellMetadataToStructure(structuresByType, gridData);

        return structuresByType;
    }
    
    


    private void ClassifyPlacementDatasOnFloor(int floorIndex)
    {
        floorToInstall = floorResidentsDatabase.GetFloor(floorIndex);
        
        if (floorToInstall.PlacementDataset.Count == 0)
        {
            Debug.Log("No placement dataset found");
            return;
        }

        /*_placementDatasByType.Clear();
        _placementDatasByType = floorToInstall.PlacementDataset
            .GroupBy(p => p.GetStructureType())
            .ToDictionary(g =>
                g.Key, g => g.ToList());*/
        
        _placementDatasByType.Clear();
        foreach (var pool in pools)
        {
            _placementDatasByType[pool.poolData.StructureType] = new List<PlacementData>();
        }
        foreach (var placementData in floorToInstall.PlacementDataset)
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
            ReleaseItemsToPool(floorToInstall.Structures.ToHashSet()); //dunno
    }

    public void ReleaseItemsToPool(HashSet<Structure> structures) //PlacementFloor placementFloor
    {
       if (structures == null || structures.Count == 0) return;
       
        foreach (var pool in pools)
        {
            pool.ReleaseItemsToPool(structures.Where
                (s => s.type == pool.poolData.StructureType).ToArray());
        }
    }
    
    public void ClearStructures(HashSet<Structure> structures)
    {
        ReleaseItemsToPool(structures);
    }
}