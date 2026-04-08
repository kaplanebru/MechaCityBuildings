using System;
using UnityEngine;

[Serializable]
public class QuadData
{
    [Range(1, 16)] public Vector2Int WidthHeight;

    //[HideInInspector]
    public Vector2Int[] Coords;

    //[HideInInspector]
    public Vector2Int[] Neighbors;
    
}