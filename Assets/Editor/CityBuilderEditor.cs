using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CityBuilder))]
public class CityBuilderEditor : Editor
{
    //User pref buraya eklenebilir
    protected CityBuilder t;
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

        if (GUILayout.Button("Debug"))
        {
            t.units.floorManagement.DebugFM();
            userState = UserStates.Drawing;
        }
        
        EditorGUILayout.Space(8);
        
        UserPrefEditorHelper.SetGridPreferencesFields(t.units.gridSystem.gridData, RecalculateGrid, CacheTargetIfNeeded );
        
        EditorGUILayout.Space(8);
        
        UserPrefEditorHelper.SetFloorPreferencesFields(t.units.floorManagement.db, CacheTargetIfNeeded);

        if (GUILayout.Button("Recalculate Grid (On Map Update)"))
        {
            CacheTargetIfNeeded();
            RecalculateGrid();
        }

        EditorGUILayout.Space(8);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Start Painting On Floor"))
            {
                CacheTargetIfNeeded();
                t.units.gridSystem.ReloadGrid(); //TODO: if needed
                t.units.floorManagement.HardRestore(); //temp
                userState = UserStates.Drawing;
            }

            //TODO: add null check: if no painting - return
            if (GUILayout.Button("Construct Buildings On Paint"))
            {
                CacheTargetIfNeeded();
                userState = UserStates.Construction;
                t.units.builder.ConstructBuildingsOnCells(t.units.gridSystem.cellRecorderCache);
                GridMasker.ResetSelectedCells(t.units.gridSystem.overlayPainter, t.units.gridSystem.gridData);

            }
        }


        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Increase Floor"))
            {
                CacheTargetIfNeeded();
                
                t.units.floorManagement.IncreaseFloor();
                userState = UserStates.Drawing;
            }

            if (GUILayout.Button("Delete Last Floor"))
            {
                CacheTargetIfNeeded();
                t.units.floorManagement.DeleteLastFloor();
                userState = UserStates.Drawing;
            }

            if (GUILayout.Button("Clear Active Floor"))
            {
                CacheTargetIfNeeded();
                t.units.floorManagement.ClearActiveFloor();
                userState = UserStates.Drawing;
            }
        }
        
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Switch Active Floor To"))
            {
                CacheTargetIfNeeded();
                t.units.floorManagement.SwitchActiveFloor(floorIndex);
                userState = UserStates.Drawing;
            }

            floorIndex = EditorGUILayout.IntField(floorIndex); //todo enum field
        }

        EditorGUILayout.Space(8);


        EditorGUILayout.Space(8);
        DrawDefaultInspector();
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
        t.units.painter.ExecutePainting(e);

        // Debug için (opsiyonel)
        if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
            SceneView.RepaintAll();
    }

    private void CacheTargetIfNeeded()
    {
        if (t == null)
            t = target as CityBuilder;
    }
    
    private void OnEditorQuit()
    {
        CacheTargetIfNeeded();
        userState = UserStates.Empty;
    }
    
}