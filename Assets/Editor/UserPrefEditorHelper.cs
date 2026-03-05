using System;
using UnityEngine;
using UnityEditor;

public static class UserPrefEditorHelper
{
    public static void SetGridPreferencesFields(GridData gridData, Action gridReloadCallback, Action cacheCallback)
    {
        if (gridData == null)
        {
            Debug.LogError("grid data is null");
            return;
        }

        //EditorGUI.BeginChangeCheck();
        using (new EditorGUILayout.HorizontalScope())
        {
            int cellSize = EditorGUILayout.IntField(
                "Building Cell Size",
                gridData.BuildingCellSize);

            if (GUILayout.Button("Apply"))
            {
                cacheCallback();
                Undo.RecordObject(gridData, "Modify Grid Preferences");

                gridData.BuildingCellSize = cellSize;

                EditorUtility.SetDirty(gridData);
                gridReloadCallback();
            }
        }

        /*if (EditorGUI.EndChangeCheck())
        {
        }*/

        GUILayout.Space(10);
    }

    public static void SetFloorPreferencesFields(FloorDatabase floorDB,
        Action cacheCallback)
    {
        if (floorDB == null)
        {
            Debug.LogError("floor database is null");
            return;
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            cacheCallback();

            using (new EditorGUILayout.HorizontalScope())
            {
                int averageBuildingHeight = EditorGUILayout.IntField(
                    "Average Building Height",
                    floorDB.AverageBuildingHeight);

                if (GUILayout.Button("Apply"))
                {
                    cacheCallback();
                    Undo.RecordObject(floorDB, "Modify Height");
                
                    floorDB.AverageBuildingHeight = averageBuildingHeight;
                
                    EditorUtility.SetDirty(floorDB);
                }
            }
        }


        CellItem dummy = (CellItem)EditorGUILayout.ObjectField(
            "Dummy",
            floorDB.Dummy,
            typeof(CellItem),
            false);
    }

    /* private static UserPreferences LoadOrCreatePreferences() //TODO: later
     {
         string path = "Assets/Editor/UserPreferences.asset";

         var prefs = AssetDatabase.LoadAssetAtPath<UserPreferences>(path);

         if (prefs == null)
         {
             prefs = ScriptableObject.CreateInstance<UserPreferences>();
             AssetDatabase.CreateAsset(prefs, path);
             AssetDatabase.SaveAssets();
         }

         return prefs;
     }*/
}