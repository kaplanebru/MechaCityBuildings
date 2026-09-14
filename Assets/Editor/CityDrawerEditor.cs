using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CityDrawer))]
//[ExecuteInEditMode]
public class CityDrawerEditor : Editor
{
    protected CityDrawer t;
    private int floorIndex;
    public UserStates userState = UserStates.Empty;

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

        UserPrefEditorHelper.SetGridPreferencesFields(t.units.gridSystem.gridData,
            RecalculateGrid,
            t.UpdateAverageStructureHeight,
            CacheTargetIfNeeded);

        UserPrefEditorHelper.SetBrushPreferences(t.units.paintData);
        EditorGUILayout.Space(2);


        if (GUILayout.Button("Recalculate Grid (On Map Update)"))
        {
            CacheTargetIfNeeded();
            RecalculateGrid();
        }

        EditorGUILayout.Space(4);

        GUI.backgroundColor = Color.yellow;
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Painting State", GUILayout.Height(25)))
            {
                CacheTargetIfNeeded();
                t.units.gridSystem.ReloadGrid(); //TODO: overlay de reload olmalı
                FloorManagement.HardRestore(t.units.floorDatabase); //temp
                userState = UserStates.Drawing;
            }

            GUI.backgroundColor = Color.cyan;
            //TODO: add null check: if no painting - return
            if (GUILayout.Button("CONSTRUCT Buildings On Paint", GUILayout.Height(25)))
            {
                CacheTargetIfNeeded();
                userState = UserStates.Construction;
                t.ConstructionRequest();
            }

            GUI.backgroundColor = Color.white;
        }

        EditorGUILayout.Space(8);
        GUILayout.Label("Floor Settings", EditorStyles.boldLabel);

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
            CacheTargetIfNeeded();
            EditorGUILayout.LabelField("Active Floor: "+ t.units.floorDatabase.ActiveFloorIndex, EditorStyles.whiteLargeLabel);
           
            
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

        /*if (GUILayout.Button("Debug"))
        {
            FloorManagement.DebugFM(t.units.floorDatabase);
            userState = UserStates.Drawing;
        }

        EditorGUILayout.Space(8);*/
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