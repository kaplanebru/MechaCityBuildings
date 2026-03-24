using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class CityBuilder : MonoBehaviour
{
    public Randomizer randomizer;
    public Installer installer;
    public PlacementDatabase placementDatabase;
    [SerializeField] private CityDrawer cityDrawer;

    private void OnEnable()
    {
        cityDrawer.OnCellsReady += SetPlacementsAndInstallSingleFloor;
    }

    private void OnDisable()
    {
        cityDrawer.OnCellsReady -= SetPlacementsAndInstallSingleFloor;
    }

    private void SetPlacementsAndInstallSingleFloor(HashSet<CellWorldData> cellWorldDatas, int floorIndex)
    {
#if UNITY_EDITOR
        Undo.RecordObject(placementDatabase,"Installment From Cell");
        Undo.RecordObject(installer, "Installment From Cell");

        randomizer.SetPlacementsOnFloor(cellWorldDatas, floorIndex);
        randomizer.MixAndApplyPlacements(floorIndex);
        installer.InstallStructures(floorIndex);

        EditorUtility.SetDirty(placementDatabase);
        EditorUtility.SetDirty(installer);

        //if (!Application.isPlaying)
            //UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
    }

    public void RandomizeAndInstallTotalZone()
    {
        int floorCount = placementDatabase.GetPlacementFloorCount();

#if UNITY_EDITOR
        
        Undo.RecordObject(placementDatabase,"Installment From Cell");
        Undo.RecordObject(installer, "Installment From Cell");
        
        for (int i = 0; i < floorCount; i++)
        {
            randomizer.MixAndApplyPlacements(i);
            installer.InstallStructures(i);
        }
        
        EditorUtility.SetDirty(placementDatabase);
        EditorUtility.SetDirty(installer);
#endif
    }

    public void InstallGivenArrangement()
    {
        //placement data with placement floors
        //do we also need floor data (maybe later)
        
    }
    
}