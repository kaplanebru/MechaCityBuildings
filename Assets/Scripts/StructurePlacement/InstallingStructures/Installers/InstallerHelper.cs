using System.Collections.Generic;
using UnityEngine;

public static class InstallerHelper
{
    public static Structure[] Install(PlacementData[] placementDataset, Transform parent, StructurePool pool)
    {
        List<Structure> structures = new List<Structure>();
        foreach (var placementData in placementDataset)
        {
            var structure = pool.GetItem();
            structures.Add(structure);

            structure.transform.position = placementData.Position;
            structure.transform.rotation = placementData.Rotation;
            structure.transform.localScale = placementData.Scale;
            structure.type = placementData.GetStructureType();

            structure.transform.SetParent(parent);
            // structure.cellMetadata = GridToConstruction. todo: bunu reverse eden func vardı
        }

        return structures.ToArray();
    }

    public static void SealCellMetadataToStructure(Structure[] structures, GridData gridData)
    {
        foreach (var structure in structures)
        {
            var cellIndex = CellConverter.
                GetCellIndexFromWorldPosition(structure.transform.position, gridData);
            
            structure.cellMetadata = cellIndex;
            
            //todo: ya da structure positionunu convert ederiz direkt
        }
    }
}