using System.Collections.Generic;
using UnityEngine;

public static class InstallerHelper
{
    public static Structure[] Install(SlotData[] cellDataSet, Transform parent, StructurePool pool, GridData gridData)
    {
        List<Structure> structures = new List<Structure>();
        foreach (var cellData in cellDataSet)
        {
            var structure = pool.GetItem();
            structures.Add(structure);
            
            structure.transform.SetParent(parent);

            Vector3 worldPos = CellConverter.GetWorldPositionFromCellCenter(cellData, gridData);
                //GetWorldPositionCenterFromCellIndex(cellData.Cells.x, cellData.Cells.y, gridData);
            
            structure.transform.localPosition = worldPos;
            structure.transform.localRotation = cellData.Rotation;
            structure.type = cellData.GetStructureType();
        }

        return structures.ToArray();
    }

    public static void SealCellMetadataToStructure(Structure[] structures, GridData gridData)
    {
        foreach (var structure in structures)
        {
            var cellIndex = CellConverter.
                GetCellIndexFromWorldPosition(structure.transform.position, gridData);
            
            structure.slotMetadata = cellIndex;
            
            //todo: ya da structure positionunu convert ederiz direkt
            //todo: cells[0]
        }
    }
}