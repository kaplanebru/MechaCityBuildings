using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]

public class Builder : MonoBehaviour
{
    [SerializeField] private OverlayPainter overlayPainter;
    [SerializeField] private FloorManagement floorManagement;
    [SerializeField] private GridData gridData;
    [SerializeField] private PaintData paintData;
    
    private void OnEnable()
    {
        floorManagement.db.OnActiveFloorUpdate += SetOverlayMeshHeight;
    }

    private void OnDisable()
    {
        floorManagement.db.OnActiveFloorUpdate -= SetOverlayMeshHeight;
    }

    private void SetOverlayMeshHeight(FloorData activeFloor)
    {
        overlayPainter.SetOverlayMeshHeight(activeFloor, gridData.AverageBuildingHeight);
        Debug.Log("on active floor: " + activeFloor);
    }
    
    public void ConstructBuildingsOnCells(List<Vector2Int> cellRecorderCache)
    {
        var registeredCells = cellRecorderCache.ToHashSet();//GridMasker.RegisterTrackedCells();
        GridToConstruction.Construct(registeredCells, gridData, floorManagement.db);
        
    }

    public void DestroyBuildingsOnCells()
    {
        GridToConstruction.DeconstructBuildingsOnCells(floorManagement.db);
    }

}
