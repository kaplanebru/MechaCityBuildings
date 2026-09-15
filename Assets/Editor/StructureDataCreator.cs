using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class StructureDataCreator : EditorWindow
{
    //TODO: S1, Sn is vague : on the matrix add info about quadsize nexto the name
    [MenuItem("Window/Tools/Structure Data And Quad Sample Creator")]
    private static void Open() => GetWindow<StructureDataCreator>("Structure Data And Quad Sample Creator");

    private const string ToolDataRoot = "Assets/ToolData";
    private const string StructuresPath = ToolDataRoot + "/StructureDatas";
    private const string QuadsPath = ToolDataRoot + "/Quads";

    [SerializeField] private StructureType structureType;
    [SerializeField] private QuadSample quadSample;
    [SerializeField] private Vector2Int quadDimensions = new Vector2Int(0, 0);

    private StructureDatabase _cachedDb;

    private StructureDatabase Db
    {
        get
        {
            if (_cachedDb != null) return _cachedDb;

            string[] guids = AssetDatabase.FindAssets("t:StructureDatabase", new[] { ToolDataRoot });

            if (guids.Length == 0)
            {
                Debug.LogError($"No StructureDatabase asset found under {ToolDataRoot}.");
                return null;
            }

            if (guids.Length > 1)
                Debug.LogWarning(
                    $"Found {guids.Length} StructureDatabase assets under {ToolDataRoot}; using the first.");

            _cachedDb = AssetDatabase.LoadAssetAtPath<StructureDatabase>(AssetDatabase.GUIDToAssetPath(guids[0]));
            return _cachedDb;
        }
    }

    private void OnGUI()
    {
        StructureDatabase db = Db;

        using (new EditorGUI.DisabledScope(true))
            EditorGUILayout.ObjectField("Database", db, typeof(StructureDatabase), false);

        if (db == null)
        {
            EditorGUILayout.HelpBox($"No StructureDatabase found under {ToolDataRoot}.", MessageType.Error);
            return;
        }
        
        EditorGUILayout.LabelField("To Create Structure Data", EditorStyles.whiteBoldLabel);
        EditorGUILayout.Space(8);

        structureType = (StructureType)EditorGUILayout.EnumPopup("Structure Type", structureType);
        quadSample = (QuadSample)EditorGUILayout.ObjectField("Quad Sample", quadSample, typeof(QuadSample), false);

        using (new EditorGUI.DisabledScope(quadSample == null))
        {
            if (GUILayout.Button("Apply"))
                Apply();
        }

        EditorGUILayout.Space(16);
        EditorGUILayout.LabelField("To Create New Quad Sample If Needed", EditorStyles.whiteBoldLabel);
        quadDimensions = EditorGUILayout.Vector2IntField("Quad Width Height", quadDimensions);

        using (new EditorGUI.DisabledScope(quadDimensions.x <= 0 || quadDimensions.y <= 0))
        {
            if (GUILayout.Button("Create New Quad Sample"))
                CreateQuadSample();
        }
    }

    private void Apply()
    {
        if (quadSample == null)
        {
            Debug.LogError("Assign a Quad Sample first.");
            return;
        }

        if (Db.HasDataByType(structureType))
        {
            Debug.LogWarning($"Database already has data for {structureType}.\nPlease try another structure type.");
            return;
        }

        EnsureFolder(StructuresPath);

        Vector2Int size = quadSample.data.WidthHeight;

        StructureData structureData = CreateInstance<StructureData>();
        structureData.Type = structureType;
        structureData.QuadSample = quadSample;

        // Quad size baked into the name, so "S1 / Sn" reads as e.g. "Wall_3x2_StructureData".
        string path = AssetDatabase.GenerateUniqueAssetPath(
            $"{StructuresPath}/{structureType}_{size.x}x{size.y}_StructureData.asset");

        AssetDatabase.CreateAsset(structureData, path);

        Db.AddData(structureData);
        EditorUtility.SetDirty(Db); // otherwise the db change is lost on reload
        AssetDatabase.SaveAssets();

        EditorGUIUtility.PingObject(structureData);
        Debug.Log($"Created structure data: {path}");
    }


    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;

        string parent = Path.GetDirectoryName(path).Replace('\\', '/');
        EnsureFolder(parent); // "Assets" is always valid, so this terminates
        AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
    }

    private void CreateQuadSample()
    {
        EnsureFolder(QuadsPath);

        QuadSample sample = CreateInstance<QuadSample>();
        sample.data = new QuadData { WidthHeight = quadDimensions };
        sample.Generate();

        string path = AssetDatabase.GenerateUniqueAssetPath(
            $"{QuadsPath}/{quadDimensions.x}x{quadDimensions.y}_QuadSample.asset");

        AssetDatabase.CreateAsset(sample, path);
        AssetDatabase.SaveAssets();

        quadSample = sample;
        EditorGUIUtility.PingObject(sample);
        Debug.Log($"Created quad sample: {path}");
    }
}