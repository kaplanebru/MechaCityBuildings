using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReplacementData", menuName = "Scriptable Objects/ReplacementData")]
public class ReplacementData : ScriptableObject
{
    public ReplacementType Type;
    public int HeightTier;
    public int PatternTier;
    public Color Color; //todo enum
}

public class ReplacementDataHolder
{
    public Dictionary<ReplacementType, ReplacementData> _datasByType = new Dictionary<ReplacementType, ReplacementData>();
    public ReplacementData ResolvedDataFromType(ReplacementType type)
    {
        if (!_datasByType.ContainsKey(type))
        {
            Debug.LogError($"No replacement data found for type {type}");
            return null;
        }
        return _datasByType[type];
    }
}