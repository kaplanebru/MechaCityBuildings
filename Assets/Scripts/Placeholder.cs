using System;
using UnityEngine;

public class Placeholder : MonoBehaviour
{
    public ReplacementType replacementType = ReplacementType.RightBatiment;
    public bool canBeCollectedRandomly = true;
    public bool canBeOrderedRandomly = true;
    
    public string UniqId { get; private set; }

    private void SetUniqID()
    {
        UniqId = Guid.NewGuid().ToString("N");
    }
    
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(UniqId))
        {
            SetUniqID();
            print("id set");
        }
    }
}

