using System.Collections.Generic;
using UnityEngine;

public static class InstallerHelper
{
    public static Structure[] Install(SlotData[] slotDatas, Transform parent, StructurePool pool, GridData gridData)
    {
        List<Structure> structures = new List<Structure>();
        foreach (var slotData in slotDatas)
        {
            var structure = pool.GetItem();
            structures.Add(structure);
            
            structure.transform.SetParent(parent);

            Vector3 worldPos = CellConverter.GetWorldPositionFromCellCenter(slotData, gridData);
                //GetWorldPositionCenterFromCellIndex(cellData.Cells.x, cellData.Cells.y, gridData);
            
            structure.transform.localPosition = worldPos;
            structure.transform.localRotation = slotData.Rotation;
            structure.type = slotData.GetStructureType();
        }

        //DebugTheHelper(slotDatas, gridData);
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

    private static void DebugTheHelper(SlotData[] slotDatas, GridData gridData)
    {
        if (slotDatas[0].StructureType == StructureType.LeftBatiment)
        {
            var neighbors = slotDatas[0].Neighbors;
            foreach (var neighbor in neighbors)
            {
                var worldPos = CellConverter.GetWorldPositionCenterFromCellIndex(neighbor.Coords.x, neighbor.Coords.y, gridData);
               
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.SetPositionAndRotation(worldPos, Quaternion.identity);
            }
        }
    }
}