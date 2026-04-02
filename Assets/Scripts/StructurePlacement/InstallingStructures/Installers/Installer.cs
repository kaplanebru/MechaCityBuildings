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
    private Dictionary<StructureType, List<CellData>> _cellDatasByType = new();
    [SerializeField] private GridData gridData;
    public StructurePool[] pools;
    
    public FloorResidentsData floorToInstall;
    
    public void InstallStructures(FloorData floorData, FloorResidentsData floorResidentsData)
    {
        ClassifyPlacementDatasOnFloor(floorResidentsData);
        InstallStructuresFromMultiplePools(floorData.Root);
    }
    
    private void InstallStructuresFromMultiplePools(Transform floorRoot)
    {
        List<Structure> structuresByType = new ();
        foreach (var pool in pools)
        {
            structuresByType.AddRange(InstallStructuresFromPool(pool, floorRoot));
        }
        
        floorToInstall.Structures = structuresByType;
    }
    
    private Structure[]  InstallStructuresFromPool(StructurePool pool, Transform floorRoot)
    {
        RestorePoolIfNeeded(pool);
        _cellDatasByType.TryGetValue(pool.poolData.StructureType, out List<CellData> cellDataSet);

        if (cellDataSet == null) return Array.Empty<Structure>();

        if (pool.poolData.PoolSize < cellDataSet.Count)
        {
            Debug.LogWarning("Pool size is too small for " + pool.poolData.StructureType);
            return Array.Empty<Structure>();
        }

        var structuresByType = InstallerHelper.Install(
            cellDataSet.ToArray(), 
            floorRoot, 
            pool,
            gridData);
        
        InstallerHelper.SealCellMetadataToStructure(structuresByType, gridData);

        return structuresByType;
    }
    
    private void ClassifyPlacementDatasOnFloor(FloorResidentsData floorResidentsData)
    {
        floorToInstall = floorResidentsData;
        if (floorToInstall.OccupiedCells.Count == 0)
        {
            Debug.Log("No placement dataset found");
            return;
        }

        _cellDatasByType.Clear();
        _cellDatasByType = floorToInstall.OccupiedCells
            .GroupBy(p => p.GetStructureType())
            .ToDictionary(g =>
                g.Key, g => g.ToList());
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
        {
            if (floorToInstall.Structures != null)
            {
                ReleaseItemsToPool(floorToInstall.Structures.ToHashSet()); //dunno
            }
        }
    }

    public void ReleaseItemsToPool(HashSet<Structure> structures)
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