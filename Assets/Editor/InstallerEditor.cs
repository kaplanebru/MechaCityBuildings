using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Installer), true)]
[CanEditMultipleObjects]
public class InstallerEditor : Editor
{
    protected Installer t;
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space(8);

        if (GUILayout.Button("Initiate Pools"))
        {
            CacheTarget();
            t.InitiatePools();
            
            EditorUtility.SetDirty(t);
        }

        if (GUILayout.Button("Release To Pool")) //TODO
        {
            CacheTarget();
            //InstallerEditorHelper.ReleaseItemsToPool(t);
        }

        if (GUILayout.Button("Refresh Pool With New Object"))
        {
            CacheTarget();
            InstallerEditorHelper.RefreshPools(t);
        }
    }

    private void CacheTarget()
    {
        if (t == null)
            t = target as Installer;
    }


}