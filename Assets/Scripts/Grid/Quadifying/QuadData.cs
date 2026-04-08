using System;
using UnityEngine;

[Serializable]
public class QuadData
{
    public Vector2Int WidthHeight = new Vector2Int(2,2);

    //[HideInInspector]
    public Vector2Int[] Coords;

    //[HideInInspector]
    public Vector2Int[] Neighbors;
    
    public int GetPointAmount => WidthHeight.x * WidthHeight.y;
    
}