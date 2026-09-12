using System;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "ReplacementDataBase", menuName = "Scriptable Objects/ReplacementDataBase")]
public class StructureDatabase : ScriptableObject
{
    public List<StructureData> datas = new();
    private static Dictionary<StructureType, StructureData> _datasByType = new ();

    public StructureData GetData(StructureType type)
    {
        EnsureBuilt();
        _datasByType.TryGetValue(type, out var data);
        return data;
    }

    public void CreateEnums(int amount = 10)
    {
        for (int i = 0; i < amount; i++)
        {
            
        }
    }
   

    private void EnsureBuilt()
    {
        if (_datasByType.Count == datas.Count) return;
        Rebuild();
    }

    private void Rebuild()
    {
        _datasByType.Clear();
        foreach (var data in datas)
        {
            if (data == null)
            {
                Debug.LogWarning($"Rebuilding {GetType().Name} due to null data");
                return;
            }

            if (_datasByType.ContainsKey(data.Type))
            {
                Debug.LogWarning($"Duplicate replacement type: {data.Type}");
                return;
            }
            
            _datasByType.Add(data.Type, data);
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