#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class InstallerEditorHelper
{
    public static void DeleteAllChildrenInEditor(Transform parent)
    {
        if (parent == null) return;
        if (parent.childCount == 0) return;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            var child = parent.GetChild(i).gameObject;
            if (child.transform == parent) continue;
            Undo.DestroyObjectImmediate(child);
        }

        EditorUtility.SetDirty(parent);
    }

    public static void RefreshPools(Installer t)
    {
        DeleteAllChildrenInEditor(t.transform);

        foreach (var pool in t.pools)
        {
            DeleteAllChildrenInEditor(pool.transform);
            pool.ClearPool();
        }
    }

    public static void ReleaseItemsToPool(Installer t)
    {
        /*int floorCount = t.placementDatabase.GetPlacementFloorCount();
        for (int i = 0; i < floorCount; i++)
        {
            t.ReleaseItemsToPool(i);
        }*/
    }

    public static void SetSceneDirty(Installer t)
    {
        EditorUtility.SetDirty(t);
        if (!Application.isPlaying)
            EditorSceneManager.MarkSceneDirty(t.gameObject.scene);
    }
    
    /*private void ReloadSavedInstallment(Dictionary<StructureType, List<PlacementData>> categorizedPlacements)
    {
        ReleaseAll();
        
        //t.InstallStructures tek başına yeterli olur
        foreach (var categorizedBuilding in categorizedPlacements)
        {
            
            
            
            if (t.TryGet(categorizedBuilding.Key, out Installer installer))
            {
                InstallerEditorHelper.InstallSavedData(installer, 
                    categorizedBuilding.Value.ToArray());
            }
            else
            {
                Debug.LogWarning("Replacer not found: " + categorizedBuilding.Key);
            }
        }
        Debug.Log("Replace saved attempt");
    }*/
    
    public static void InstallSavedData(Installer t, PlacementData[] placeholderDataSet)
    {
        /*Undo.RecordObject(t, "ReplaceGiven All Saved Data");
        t.InstallFromAllPools(placeholderDataSet);
        SetSceneDirty(t);*/
    }
}
#endif