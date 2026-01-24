using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class PlaceholderProvider : MonoBehaviour
{
    //TODO: compare child count before (> 0)
    [SerializeField] private List<Placeholder> Placeholders = new();
    private static List<PlaceholderData> PlaceholderDatas { get; set; } = new();
    
    private void OnEnable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.delayCall += EditorInit;
            return;
        }
#endif
        RebuildCache();
    }
    
#if UNITY_EDITOR
    private void OnDisable()
    {
        UnityEditor.EditorApplication.delayCall -= EditorInit;
    }
    
    private void OnValidate()
    {
// Inspector'da değer değişince / child ekle-çıkar olunca editörde güncel tut
        if (Application.isPlaying) return;
        UnityEditor.EditorApplication.delayCall -= EditorInit;
        UnityEditor.EditorApplication.delayCall += EditorInit;
    }


    private void EditorInit()
    {
        if (this == null) return;
        RebuildCache();
        UnityEditor.EditorUtility.SetDirty(this);
        if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(this))
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);

        UnityEditor.EditorApplication.delayCall -= EditorInit;
    }
#endif

    private void RebuildCache()
    {
        TryFillPlaceholders();
        TryFillPlaceholdersData();
    }

    public static List<PlaceholderData> GetPlaceholderDataSetByType(ReplacementType type)
    {
        return SetPlaceholderDataSetByType(type, PlaceholderDatas);
    }

    public static List<PlaceholderData> GetPlaceholderDataSetByGivenPlaceholders(List<Placeholder> placeholders)
    {
        return SetPlaceholderDataSet(placeholders);
    }

    public static List<PlaceholderData> GetPlaceholderDataSet() => PlaceholderDatas.ToList();

    private static List<PlaceholderData> SetPlaceholderDataSetByType
        (ReplacementType type, List<PlaceholderData> placeholderDatas)
    {
        return placeholderDatas.Where(d =>
        {
            if (d.ReplacementData != null)
            {
                return d.GetReplacementType() == type;
            }

            Debug.Log("Placeholder data not assigned");
            return false;
        }).ToList();
    }

    private void TryFillPlaceholders()
    {
        if (Placeholders.Count == 0)
        {
            Placeholders = transform.GetComponentsInChildren<Placeholder>()
                .Where(p => p.canBeCollectedRandomly)
                .ToList();

            TryFillPlaceholdersData();
        }
    }

    private void TryFillPlaceholdersData()
    {
        if (PlaceholderDatas.Count == 0 || PlaceholderDatas.Count != Placeholders.Count)
        {
            SetPlaceholderDataSet(Placeholders);
        }
    }

    private static List<PlaceholderData> SetPlaceholderDataSet(List<Placeholder> placeholders)
    {
        PlaceholderDatas = placeholders.Select(placeholder =>
        {
            placeholder.SetDataTransformValues();
            return placeholder.data;
        }).ToList();

        return PlaceholderDatas;
    }
}