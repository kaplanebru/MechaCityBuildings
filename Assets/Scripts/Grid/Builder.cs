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
            floorManagement.db.OnActiveFloorUpdate += SetOverlayPainterHeight;
    }

    private void OnDisable()
    {
        if(floorManagement != null && overlayPainter != null)
            floorManagement.db.OnActiveFloorUpdate -= SetOverlayPainterHeight;
    }
    
    private void SetOverlayPainterHeight(FloorData floorData) => overlayPainter.SetPainterHeight(floorData);
    
    public void ConstructBuildingsOnCells()
    {
        var registeredCells = GridMasker.RegisterTrackedCells();
        GridToConstruction.Construct(registeredCells, gridData, floorManagement.db);
    }

    public void DestroyBuildingsOnCells()
    {
        GridToConstruction.DeconstructBuildingsOnCells(floorManagement.db);
    }

}
