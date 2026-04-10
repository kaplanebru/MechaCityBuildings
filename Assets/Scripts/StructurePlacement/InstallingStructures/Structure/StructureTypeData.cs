using UnityEngine;

[CreateAssetMenu(fileName = "ReplacementData", menuName = "Scriptable Objects/ReplacementData")]
public class StructureTypeData : ScriptableObject
{
    public int Id;
    public string Name;
    public StructureType Type;
    public QuadSample QuadSample;
    public int HeightTier;
    public Color Color; //todo enum
}