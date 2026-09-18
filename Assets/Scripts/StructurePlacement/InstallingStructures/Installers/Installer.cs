using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



[ExecuteInEditMode]
public class Installer : MonoBehaviour
{
    public Action<StructureType> OnStructureRemovalRequest;
    public Func<StructureType, StructurePool, HashSet<Structure>> OnStructureReplacementRequest;
    private Dictionary<StructureType, List<SlotData>> _slotDatasByType = new();
    [SerializeField] private GridData gridData;
    public Transform root;
    public List<StructurePool> pools = new();

    public FloorResidentsData floorToInstall; //TODO MAKE THIS PRIVATE

    public IEnumerable<StructureType> GetStructureTypesFromPools()
    {
        foreach (var pool in pools)
        {
            yield return pool.poolData.StructureType;
        }
    }
    public void InstallStructures(FloorData floorData, FloorResidentsData floorResidentsData, List<SlotData> slots = null)
    {
        slots ??= floorResidentsData.Slots;
        ClassifyPlacementDatasOnFloor(floorResidentsData, slots);
        InstallStructuresFromMultiplePools(floorData.Root);
    }

    private void InstallStructuresFromMultiplePools(Transform floorRoot)
    {
        List<Structure> structuresByType = new();
        foreach (var pool in pools)
        {
            structuresByType.AddRange(InstallStructuresFromPool(pool, floorRoot));
        }

        floorToInstall.RegisterStructures(structuresByType);
    }

    public Structure[] InstallStructuresFromPool(StructurePool pool, Transform floorRoot)
    {
        RestorePoolIfNeeded(pool);
        _slotDatasByType.TryGetValue(pool.poolData.StructureType, out List<SlotData> slotDatas);

        if (slotDatas == null) return Array.Empty<Structure>();

        if (pool.poolData.PoolSize < slotDatas.Count)
        {
            Debug.LogWarning("Pool size is too small for " + pool.poolData.StructureType);
            return Array.Empty<Structure>();
        }

        var structuresByType = InstallerHelper.Install(
            slotDatas.ToArray(),
            floorRoot,
            pool,
            gridData);

        InstallerHelper.SealCellMetadataToStructure(structuresByType, gridData);

        return structuresByType;
    }

    private void ClassifyPlacementDatasOnFloor(FloorResidentsData floorResidentsData, List<SlotData> slots)
    {
        floorToInstall = floorResidentsData;

        _slotDatasByType.Clear();
        _slotDatasByType = slots
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
        /*else
        {
            if (floorToInstall.Structures != null)
            {
                ReleaseItemsToPool(floorToInstall.Structures.ToHashSet()); //dunno
            }
        }*/
    }

    private void ReleaseItemsToPool(HashSet<Structure> mixedStructures)
    {
        if (mixedStructures == null || mixedStructures.Count == 0)
        {
            Debug.LogWarning("structures null or empty");
            return;
        }

        Debug.Log("pool count: " + pools.Count);
        foreach (var pool in pools)
        {
            var items = mixedStructures.Where
                (s => s.type == pool.poolData.StructureType).ToArray();
            
            pool.ReleaseItemsToPool(items);
        }
    }

    public void ClearStructuresPhysically(HashSet<Structure> mixedStructures)
    {
        ReleaseItemsToPool(mixedStructures);
    }

    public void AddNewPool(StructurePool pool)
    {
        pools.Add(pool);
    }
}