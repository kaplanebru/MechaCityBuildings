using UnityEngine;

[CreateAssetMenu(fileName = "ReplacementData", menuName = "Scriptable Objects/ReplacementData")]
public class StructureData : ScriptableObject
{
    public string Name;
    public StructureType Type; //hide in inspector
    public QuadSample QuadSample;
}