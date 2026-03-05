using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Builder : MonoBehaviour
{
    [SerializeField] private OverlayPainter overlayPainter;
    [SerializeField] private FloorManagement floorManagement;
    [SerializeField] private GridData gridData;
    [SerializeField] private PaintData paintData;
    
    private void OnEnable()
    {
        if(floorManagement != null && overlayPainter != null)
            floorManagement.db.OnActiveFloorUpdate += SetOverlayMeshHeight;
    }

    private void OnDisable()
    {
        if(floorManagement != null && overlayPainter != null)
            floorManagement.db.OnActiveFloorUpdate -= SetOverlayMeshHeight;
    }
    
    private void SetOverlayMeshHeight(FloorData floorData) => overlayPainter.SetOverlayMeshHeight(floorData, gridData.AverageBuildingHeight);
    
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
