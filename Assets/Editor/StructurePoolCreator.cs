using UnityEditor;
using UnityEngine;

public class StructurePoolCreator : EditorWindow
{
    [MenuItem("Window/Tools/CityBuilder/Structure Pool Creator")]
    private static void Open() => GetWindow<StructurePoolCreator>("Structure Pools");

    [SerializeField] private Structure structurePrefab;
    [SerializeField] private int poolSize = 100;
    [SerializeField] private StructureType structureType;

    private void OnGUI()
    {
        structurePrefab = (Structure)EditorGUILayout.ObjectField("Prefab", structurePrefab, typeof(Structure), false);
        poolSize = Mathf.Max(1, EditorGUILayout.IntField("Pool Size", poolSize));
        structureType = (StructureType)EditorGUILayout.EnumPopup("Structure Type", structureType);
        //EditorGUILayout.LabelField("Structure Type", structurePrefab != null ? structurePrefab.type.ToString() : structureType.ToString());

        using (new EditorGUI.DisabledScope(structurePrefab == null))
            if (GUILayout.Button("Create Pool"))
                Create();
    }

    private void Create()
    {
        var go = new GameObject($"{structurePrefab.name} Pool");
        Undo.RegisterCreatedObjectUndo(go, "Create Structure Pool");
        Undo.AddComponent<StructurePool>(go).poolData = new PoolData
        {
            Prefab = structurePrefab,
            PoolSize = poolSize,
            StructureType = structureType//structurePrefab.type
        };
        Selection.activeGameObject = go;
    }
}