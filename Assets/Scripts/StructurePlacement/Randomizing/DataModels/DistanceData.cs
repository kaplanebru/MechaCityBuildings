using System;
using UnityEngine;

[Serializable]
public class DistanceData
{
    public float Distance;
    public FrequencyData FrequencyData;
}

[Serializable]
public class FrequencyData
{
    [Range(0, 100)] public int Frequency = 30;
    public int Amount { get; set; }
}