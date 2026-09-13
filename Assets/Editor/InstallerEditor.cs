#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(Installer))]
public class InstallerEditor : Editor
{
    private Installer t;
    [SerializeField] private Structure structurePrefab;
    [SerializeField] private int poolSize = 100;
    [SerializeField] private StructureType structureType;

    public override void OnInspectorGUI()
    {
        GUILayout.Label("Structure Pool Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space(8);

        CreatePool();

        if (GUILayout.Button("Initiate Pools"))
        {
            CacheTarget();
            t.InitiatePools();

            EditorUtility.SetDirty(t);
        }

        if (GUILayout.Button("Reset Everything and POOLS"))
        {
            CacheTarget();
            //ResetEverything(); //todo
            RefreshPools();
        }

        EditorGUILayout.Space(8);
    }

    private void CreatePool()
    {
        structurePrefab = (Structure)EditorGUILayout.ObjectField("Prefab", structurePrefab, typeof(Structure), false);
        poolSize = Mathf.Max(1, EditorGUILayout.IntField("Pool Size", poolSize));
        structureType = (StructureType)EditorGUILayout.EnumPopup("Structure Type", structureType);
        //EditorGUILayout.LabelField("Structure Type", structurePrefab != null ? structurePrefab.type.ToString() : structureType.ToString());
        //using (new EditorGUI.DisabledScope(structurePrefab == null))
        if (GUILayout.Button("Create Pool")) //,GUILayout.Width(400)
        {
            CacheTarget();
            Create();
        }

        EditorGUILayout.Space(8);
        DeletePool();

        EditorGUILayout.Space(8);
        DrawDefaultInspector();
    }

    private void Create()
    {
        if (structurePrefab == null)
        {
            Debug.LogError("No Prefab assigned!");
            return;
        }

        var go = new GameObject($"{structurePrefab.name} Pool");
        go.transform.SetParent(t.root);
        Undo.RegisterCreatedObjectUndo(go, "Create Structure Pool");
        var newPool = Undo.AddComponent<StructurePool>(go);
        newPool.poolData = new PoolData
        {
            PoolSize = poolSize,
            StructureType = structureType,
            Prefab = structurePrefab,
        };
        Selection.activeGameObject = go;

        t.AddNewPool(newPool);
    }

    private int selectedPoolIndex; // = -1;

    private void DeletePool()
    {
        //TODO: MUST DELETE FLOOR RESIDENTS AS WELL? ON SCENE ELEMENTS CANT BE RELEASED WHEN DELETED. WHEN RELEASING IF WE PUT A DELETE IF NO POOL OPTION IT MIGHT WORK 
        CacheTarget();
        using (new EditorGUILayout.HorizontalScope())
        {
            selectedPoolIndex = EditorGUILayout.Popup("Pools of", selectedPoolIndex,
                t.pools.ConvertAll(p => p.name).ToArray()); //p.poolData.Prefab.name

            if (GUILayout.Button("Delete"))
            {
                if (selectedPoolIndex >= 0 && selectedPoolIndex < t.pools.Count)
                {
                    var pool = t.pools[selectedPoolIndex];
                    t.pools.RemoveAt(selectedPoolIndex);
                    Undo.DestroyObjectImmediate(pool.gameObject);
                    
                    selectedPoolIndex--;
                }
            }
        }
    }

    private void DeleteAllChildrenInEditor(Transform parent)
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

    public void RefreshPools()
    {
        foreach (var pool in t.pools)
        {
            DeleteAllChildrenInEditor(pool.transform);
            pool.ClearPool();
        }

        //DeleteAllChildrenInEditor(t.transform);
        //t.pools = null;
    }

    private void CacheTarget()
    {
        if (t == null)
            t = (Installer)target; // Works for subclasses too
    }


    public static void SetSceneDirty(Installer t)
    {
        EditorUtility.SetDirty(t);
        if (!Application.isPlaying)
            EditorSceneManager.MarkSceneDirty(t.gameObject.scene);
    }
}
#endif