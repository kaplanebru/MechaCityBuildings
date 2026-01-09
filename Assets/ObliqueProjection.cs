using UnityEngine;
using System.Collections;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class ObliqueProjection : MonoBehaviour
{
    public float angle = 45.0f;
    public float zScale = 0.5f;
    public float zOffset = 0.0f;

    public void Apply ()
    {
        GetComponent<Camera>().orthographic = true;
        var orthoHeight = GetComponent<Camera>().orthographicSize;
        var orthoWidth = GetComponent<Camera>().aspect * orthoHeight;
        var m = Matrix4x4.Ortho (-orthoWidth, orthoWidth, -orthoHeight, orthoHeight, GetComponent<Camera>().nearClipPlane, GetComponent<Camera>().farClipPlane);
        var s = zScale / orthoHeight;
        m [0, 2] = +s * Mathf.Sin (Mathf.Deg2Rad * -angle);
        m [1, 2] = -s * Mathf.Cos (Mathf.Deg2Rad * -angle);
        m [0, 3] = -zOffset * m [0, 2];
        m [1, 3] = -zOffset * m [1, 2];
        GetComponent<Camera>().projectionMatrix = m;
    }

    void OnEnable ()
    {
        Apply ();
    }

    void OnDisable ()
    {
        GetComponent<Camera>().ResetProjectionMatrix ();
    }
}