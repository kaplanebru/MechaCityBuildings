using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ReplacementDataByType
{
    public ReplacementType Type;
    public ReplacementData Data;
}

[CreateAssetMenu(fileName = "ReplacementDataBase", menuName = "Scriptable Objects/ReplacementDataBase")]
public class ReplacementDataBase: ScriptableObject
{
    [SerializeField] private List<ReplacementDataByType> datas = new();
    private static Dictionary<ReplacementType, ReplacementData> _datasByType = new Dictionary<ReplacementType, ReplacementData>();
    
    public ReplacementData Get(ReplacementType type)
    {
        EnsureBuilt();
         _datasByType.TryGetValue(type, out var data);
         return data;
    }

    private void EnsureBuilt()
    {
        if (_datasByType.Count == datas.Count) return;
        Rebuild();
    }
    
    private void Rebuild()
    {
        _datasByType.Clear();
        
        foreach (var d in datas)
        {
            if (d.Data == null)
            {
                Debug.LogWarning($"Rebuilding {GetType().Name} due to null data");
                return;
            }
            if (_datasByType.ContainsKey(d.Type))
            {
                Debug.LogWarning($"Duplicate replacement type: {d.Type}");
                return;
            }
            
            if (d.Data.Type != d.Type)
            {
               Debug.LogWarning("Data type doesn't match the type");
               return;
            }
            
            _datasByType[d.Type] = d.Data;
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