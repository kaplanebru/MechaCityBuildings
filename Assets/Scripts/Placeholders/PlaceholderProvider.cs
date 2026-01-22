using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlaceholderProvider : MonoBehaviour
{
    private static List<Placeholder> Placeholders { get; set; } = new();
    private static List<PlaceholderData> PlaceholderDatas { get; set; } = new();

    private void OnEnable()
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
        return placeholderDatas.Where(d => d.Type == type).ToList();
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

    private static void TryFillPlaceholdersData()
    {
        if (PlaceholderDatas.Count == 0)
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