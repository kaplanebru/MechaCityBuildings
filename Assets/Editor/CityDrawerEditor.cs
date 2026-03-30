using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CityDrawer))]
[ExecuteInEditMode]
public class CityDrawerEditor : Editor
{
    protected CityDrawer t;
    private int floorIndex;
    public UserStates userState = UserStates.Empty;
    public Action<HashSet<CellWorldData>, int> OnCellsReady;


    private void OnEnable()
    {
        CacheTargetIfNeeded();
        SceneView.duringSceneGui += OnSceneGUI;
        EditorApplication.quitting += OnEditorQuit;
    }

    private void OnDisable()
    {
        CacheTargetIfNeeded();
        SceneView.duringSceneGui -= OnSceneGUI;
        EditorApplication.quitting -= OnEditorQuit;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space(8);

        if (GUILayout.Button("Debug"))
        {
            FloorManagement.DebugFM(t.units.floorDatabase);
            userState = UserStates.Drawing;
        }

        EditorGUILayout.Space(8);

        UserPrefEditorHelper.SetGridPreferencesFields(t.units.gridSystem.gridData,
            RecalculateGrid,
            t.UpdateAverageBuildingHeight,
            CacheTargetIfNeeded);

        UserPrefEditorHelper.SetBrushPreferences(t.units.paintData);
        EditorGUILayout.Space(2);


        if (GUILayout.Button("Recalculate Grid (On Map Update)"))
        {
            CacheTargetIfNeeded();
            RecalculateGrid();
        }

        EditorGUILayout.Space(8);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Painting State"))
            {
                CacheTargetIfNeeded();
                t.units.gridSystem.ReloadGrid(); //TODO: overlay de reload olmalı
                FloorManagement.HardRestore(t.units.floorDatabase); //temp
                userState = UserStates.Drawing;
            }

            //TODO: add null check: if no painting - return
            if (GUILayout.Button("Construct Buildings On Paint"))
            {
                CacheTargetIfNeeded();
                userState = UserStates.Construction;
                t.ConstructBuildingsRequest();
            }
        }


        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Increase Floor"))
            {
                CacheTargetIfNeeded();

                FloorManagement.IncreaseFloor(t.units.floorDatabase);
                userState = UserStates.Drawing;
            }

            if (GUILayout.Button("Delete Last Floor"))
            {
                CacheTargetIfNeeded();

                //todo: clear last floor
                FloorManagement.DeleteLastFloor(t.units.floorDatabase);
          

                userState = UserStates.Drawing;
            }

            if (GUILayout.Button("Clear Active Floor"))
            {
                ClearActiveFloor();
                userState = UserStates.Drawing;
            }
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Switch Active Floor To"))
            {
                CacheTargetIfNeeded();
                FloorManagement.SwitchActiveFloor(floorIndex, t.units.floorDatabase);
                userState = UserStates.Drawing;
            }

            floorIndex = EditorGUILayout.IntField(floorIndex); //todo enum field
        }

        EditorGUILayout.Space(8);


        EditorGUILayout.Space(8);
        DrawDefaultInspector();
    }

    private void ClearActiveFloor()
    {
        CacheTargetIfNeeded();
        FloorManagement.ClearActiveFloor(t.units.floorDatabase);

        t.units.gridSystem.ReloadGrid();
        GridMasker.ResetSelectedCells(t.units.gridSystem.overlayPainter, t.units.gridSystem.gridData);
    }

    private void RecalculateGrid()
    {
        t.units.gridSystem.RecalculateGrid();
        userState = UserStates.Drawing;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        var e = Event.current;

        if (userState != UserStates.Drawing) return;

        // ✅ sadece Layout'ta kontrolü kap
        if (e.type == EventType.Layout)
            PaintDetector.CaptureSceneViewControl();

        // ✅ Event'i parametre olarak geçir
        t.ExecutePainting(e);

        // Debug için (opsiyonel)
        if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
            SceneView.RepaintAll();
    }

    private void CacheTargetIfNeeded()
    {
        if (t == null)
            t = target as CityDrawer;
    }

    private void OnEditorQuit()
    {
        CacheTargetIfNeeded();
        userState = UserStates.Empty;
    }
}