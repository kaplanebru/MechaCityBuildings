using System;
using UnityEngine;

public class CamShifter : MonoBehaviour
{
    [SerializeField] private Transform[] camTransforms;
    [SerializeField] private Camera cam;
    [SerializeField] private FloorManagement floorManagement;
    [SerializeField] private int floorChangeOffset = 5;

    private int currentCamIndex = 0;
    private float startHeightTopdownCam;

    private void OnEnable()
    {
        Initialize();
        floorManagement.db.OnActiveFloorUpdate += AlignHeightByFloor;
    }

    private void OnDisable()
    {
        floorManagement.db.OnActiveFloorUpdate -= AlignHeightByFloor;
    }
    
    public void Initialize()
    {
        startHeightTopdownCam = camTransforms[0].position.y;
    }

    public void OnShiftCamButtonClicked()
    {
        ShiftCamSetting();
        ApplyTransform();
    }
    
    private void AlignHeightByFloor(FloorData floorData)
    {
        var pos = camTransforms[0].position;
        pos.y = startHeightTopdownCam + floorData.FloorGroundHeight + floorChangeOffset;
        
        camTransforms[0].position = pos;
        ApplyTransform();
    }

    private void ShiftCamSetting()
    {
        currentCamIndex = (currentCamIndex + 1) % camTransforms.Length;
    }

    private void ApplyTransform()
    {
        cam.transform.position = camTransforms[currentCamIndex].position;
        cam.transform.rotation = camTransforms[currentCamIndex].rotation;
    }
}