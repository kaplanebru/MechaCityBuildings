using System;
using UnityEngine;

public class Placeholder : MonoBehaviour
{
    public PlaceholderData data = new();
    public bool canBeCollectedRandomly = true;
    public bool canBeOrderedRandomly = true;

    public void SetDataTransformValues()
    {
        data.SetTransformValues(transform.position, transform.rotation, transform.localScale);
    }
}

