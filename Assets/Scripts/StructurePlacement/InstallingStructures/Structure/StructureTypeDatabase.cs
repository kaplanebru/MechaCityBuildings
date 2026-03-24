using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StructureDataByType
{
    public StructureType Type;
    public StructureTypeData typeData;
}

[CreateAssetMenu(fileName = "ReplacementDataBase", menuName = "Scriptable Objects/ReplacementDataBase")]
public class StructureTypeDatabase : ScriptableObject
{
    [SerializeField] private List<StructureDataByType> datas = new();
    private static Dictionary<StructureType, StructureTypeData> _datasByType = new ();
    private static Dictionary<StructureType, int> _heightTierByType = new ();

    public StructureTypeData GetData(StructureType type)
    {
        EnsureBuilt();
        _datasByType.TryGetValue(type, out var data);
        return data;
    }

    public int GetHeightTierByType(StructureType type)
    {
        EnsureBuilt();
        return _heightTierByType[type];
    }

    private void EnsureBuilt()
    {
        if (_datasByType.Count == datas.Count && 
            _heightTierByType.Count == datas.Count) return;
        Rebuild();
    }

    private void Rebuild()
    {
        _datasByType.Clear();
        _heightTierByType.Clear();

        foreach (var d in datas)
        {
            if (d.typeData == null)
            {
                Debug.LogWarning($"Rebuilding {GetType().Name} due to null data");
                return;
            }

            if (_datasByType.ContainsKey(d.Type))
            {
                Debug.LogWarning($"Duplicate replacement type: {d.Type}");
                return;
            }

            if (d.typeData.Type != d.Type)
            {
                Debug.LogWarning("Data type doesn't match the type");
                return;
            }

            _datasByType[d.Type] = d.typeData;
            _heightTierByType[d.Type] = d.typeData.HeightTier;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) return;
        Rebuild();
    }
#endif

    private void Awake()
    {
        Rebuild();
    }
}