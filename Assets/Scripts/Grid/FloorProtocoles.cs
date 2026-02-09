using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloorData
{
    public List<Transform> FloorSet;
    public Transform Root;
}

public class FloorProtocoles
{
    private List<Transform> floorSet = new();
    private Transform root;
    public Transform GetWorkingFloor() => floorSet[WorkingFloor];
    
    public int WorkingFloor { get; private set; }= 0;
    

    public void Setup(FloorData data)
    {
        floorSet = data.FloorSet;
        root = data.Root;
    }
    
    public int SwitchWorkingFloor(int floorIndex)
    {
        if (floorIndex >= floorSet.Count)
        {
            Debug.LogError("Floor index out of bounds");
            return 0;
        }
        
        WorkingFloor = floorIndex;
        return WorkingFloor;
    }

    public void IncreaseFloorSet()
    {
        var newFloor = new GameObject("Floor " + floorSet.Count);
        newFloor.transform.SetParent(root);
        
        floorSet.Add(newFloor.transform);
        WorkingFloor = floorSet.Count - 1;
    }

    public void ClearWorkingFloor()
    {
        var root = floorSet[WorkingFloor];            
        if (root.childCount == 0) return;

        for (int i = root.childCount - 1; i >= 0; i--)
        {
            var child = root.GetChild(i).gameObject;
            Object.Destroy(child);
            //Undo.DestroyObjectImmediate(child);
        }
    }

    public void DeleteLastFloor()
    {
        if (floorSet.Count == 1) return;
        
        if(WorkingFloor == floorSet.Count - 1)
            WorkingFloor = 0;

        floorSet.RemoveAt(floorSet.Count - 1);
    }
    
}
