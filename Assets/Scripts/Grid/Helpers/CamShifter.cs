using UnityEngine;

public class CamShifter : MonoBehaviour
{
    [SerializeField] private Transform[] camTransforms;
    [SerializeField] private Camera cam;
    private int currentCamIndex = 0;
    private float startHeightTopdownCam;

    public void Initialize()
    {
        startHeightTopdownCam = camTransforms[0].position.y;
    }

    public void OnShiftCamButtonClicked()
    {
        ShiftCamSetting();
        ApplyTransform();
    }

    public void AlignRelativeHeightByFloor(float heightOffset)
    {
        var pos = camTransforms[0].position;
        pos.y = startHeightTopdownCam + heightOffset;
        camTransforms[0].position = pos;
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