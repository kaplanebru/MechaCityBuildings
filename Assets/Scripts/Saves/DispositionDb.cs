using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Arrangement", menuName = "CityBuilder/New Arrangement")]
public class SavedDispositionDb : ScriptableObject
{
    public List<DispositionData> Dataset = new();
    public string[] Names;

    public bool TryGetDataByName(string selectedName, out DispositionData data)
    {
        data = null;
        if (!Names.Contains(selectedName)) return false;

        data = Dataset.FirstOrDefault(d => d.Name == selectedName);
        return data != null;
    }

    public void RemoveArrangement(DispositionData data)
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
        var arrangement = new DispositionData(arrangementName, null);
        //TODO: not null

        Dataset.Add(arrangement);
        RefreshNames();
    }
}