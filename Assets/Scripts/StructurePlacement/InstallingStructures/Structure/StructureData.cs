using UnityEngine;

[CreateAssetMenu(fileName = "ReplacementData", menuName = "Scriptable Objects/ReplacementData")]
public class StructureData : ScriptableObject
{
    public int Id;
    public string Name;
    public StructureType Type;
    public QuadSample QuadSample;
    public int HeightTier;
    public Color Color; //todo enum
}