using UnityEngine;

[CreateAssetMenu(fileName = "ReplacementData", menuName = "Scriptable Objects/ReplacementData")]
public class ReplacementData : ScriptableObject
{
    public ReplacementType Type;
    public int HeightTier;
    public int PatternTier;
    public Color Color; //todo enum
}