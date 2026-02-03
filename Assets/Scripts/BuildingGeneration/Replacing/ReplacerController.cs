using System;
using System.Collections.Generic;
using UnityEngine;

public class ReplacerController : MonoBehaviour
{
    public List<ReplacerAuto> autoReplacers = new();
    [SerializeField] private Transform placeholderParent;
    
    private readonly Dictionary<ReplacementType, ReplacerBase> _byType = new();
    public IReadOnlyDictionary<ReplacementType, ReplacerBase> ByType => _byType;

    public bool TryGet(ReplacementType type, out ReplacerBase replacer)
    {
        EnsureBuilt();
        return _byType.TryGetValue(type, out replacer);
    }

    private void EnsureBuilt()
    {
        if (_byType.Count == autoReplacers.Count) return;
        Rebuild();
    }

    private void Rebuild()
    {
        _byType.Clear();

        foreach (var r in autoReplacers)
        {
            if (!r) continue;
            _byType[r.replacementType] = r;
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