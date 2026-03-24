using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Arrangement", menuName = "CityBuilder/New Arrangement")]
public class SavedArrangements : ScriptableObject
{
    public List<ArrangementData> Dataset = new();
    public string[] Names;

    public bool TryGetDataByName(string selectedName, out ArrangementData data)
    {
        data = null;
        if (!Names.Contains(selectedName)) return false;

        data = Dataset.FirstOrDefault(d => d.Name == selectedName);
        return data != null;
    }

    public void RemoveArrangement(ArrangementData data)
    {
        Dataset.Remove(data);
        RefreshNames();
    }

    private void RefreshNames()
    {
        Names = new string[Dataset.Count];
        for (int i = 0; i < Names.Length; i++)
        {
            Names[i] = Dataset[i].Name;
        }
    }

    public void AddArrangement(string arrangementName)
    {
        var arrangement = new ArrangementData(arrangementName, null);
        //TODO: not null

        Dataset.Add(arrangement);
        RefreshNames();
    }
}