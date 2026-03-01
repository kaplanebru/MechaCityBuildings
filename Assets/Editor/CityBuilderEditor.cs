using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CityBuilder))]
public class CityBuilderEditor : Editor
{
    //User pref buraya eklenebilir
    protected CityBuilder t;
    private int floorIndex;
    
    private void OnEnable()
    {
        CacheTarget();
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        CacheTarget();
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        var e = Event.current;

        if (!t.OnPaintingState) return;

        // ✅ sadece Layout'ta kontrolü kap
        if (e.type == EventType.Layout)
            PaintDetector.CaptureSceneViewControl();

        // ✅ Event'i parametre olarak geçir
        t.units.painter.ExecutePainting(e);

        // Debug için (opsiyonel)
        if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
            SceneView.RepaintAll();
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space(8);

        if (GUILayout.Button("Recalculate Grid (If Needed)"))
        {
            CacheTarget();
            t.units.gridSystem.Recalculate();
        }

        EditorGUILayout.Space(8);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Start Painting On Floor"))
            {
                CacheTarget();
                t.units.gridSystem.Recalculate(); //TODO: if needed
                t.units.floorManagement.Setup(); //temp
                t.OnPaintingState = true;
            }

            //TODO: add null check: if no painting - return
            if (GUILayout.Button("Construct Buildings On Paint"))
            {
                CacheTarget();
                t.OnPaintingState = false;
                t.units.builder.ConstructBuildingsOnCells();
                GridMasker.ResetSelectedCells(t.units.gridSystem.overlayPainter, t.units.gridSystem.gridData);

            }
        }


        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Increase Floor"))
            {
                CacheTarget();
                t.units.floorManagement.IncreaseFloor();
            }

            if (GUILayout.Button("Delete Last Floor"))
            {
                CacheTarget();
                t.units.floorManagement.DeleteLastFloor();
            }
        }
        
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Switch Active Floor To"))
            {
                CacheTarget();
                t.units.floorManagement.SwitchActiveFloor(floorIndex);
            }

            floorIndex = EditorGUILayout.IntField(floorIndex); //todo enum field
        }

        EditorGUILayout.Space(8);


        EditorGUILayout.Space(8);
        DrawDefaultInspector();
    }

    private void CacheTarget()
    {
        if (t == null)
            t = target as CityBuilder;
    }
    
}