using UnityEngine;

public class CamShifter : MonoBehaviour
{
    [SerializeField] private Transform[] camTransforms;
    [SerializeField] private Camera cam;
    private int currentCamTransform = 0;
    
    public void OnShiftCamButtonClicked()
    {
        ShiftPos();
        ApplyTransform();
    }

    private void ShiftPos()
    {
        currentCamTransform = (currentCamTransform + 1) % camTransforms.Length;
    }

    private void ApplyTransform()
    {
        cam.transform.position = camTransforms[currentCamTransform].position;
        cam.transform.rotation = camTransforms[currentCamTransform].rotation;
    }
}
