using UnityEngine;

[CreateAssetMenu(fileName = "ReplacementData", menuName = "Scriptable Objects/ReplacementData")]
public class StructureTypeData : ScriptableObject
{
    public StructureType Type;
    public int HeightTier;
    public int PatternTier;
    public Color Color; //todo enum
    
}