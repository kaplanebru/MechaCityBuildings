using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DispositionDB", menuName = "CityBuilder/DispositionDB")]
public class DispositionDb : ScriptableObject
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

    public void AddDisposition(DispositionData data)
    {
        Dataset.Add(data);
        RefreshNames();
    }
    public void RemoveDisposition(DispositionData data)
    {
        Dataset.Remove(data);
        RefreshNames();
    }
    
    public bool IsNameTaken(string dispositionName)
    {
        return Dataset.Any(d => d.Name == dispositionName);
    }


    private void RefreshNames()
    {
        Names = new string[Dataset.Count];
        for (int i = 0; i < Names.Length; i++)
        {
            Names[i] = Dataset[i].Name;
        }
    }
}