using System;
using UnityEngine;
using UnityEditor;

public static class UserPrefEditorHelper
{
    public static void SetGridPreferencesFields(GridData gridData, Action gridReloadCallback, Action floorReloadCallback, Action cacheCallback)
    {
        if (gridData == null)
        {
            Debug.LogError("grid data is null");
            return;
        }

        //EditorGUI.BeginChangeCheck();
        using (new EditorGUILayout.HorizontalScope())
        {
            gridData.BuildingCellSize = EditorGUILayout.IntField(
                "Building Cell Size",
                gridData.BuildingCellSize);

            if (GUILayout.Button("Apply"))
            {
                cacheCallback();
                Undo.RecordObject(gridData, "Modify Grid Preferences");
                EditorUtility.SetDirty(gridData);
                gridReloadCallback();
            }
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            gridData.AverageBuildingHeight = EditorGUILayout.IntField(
                "Average Building Height",
                gridData.AverageBuildingHeight);

            if (GUILayout.Button("Apply"))
            {
                cacheCallback();
                Undo.RecordObject(gridData, "Modify Height");
                EditorUtility.SetDirty(gridData);
                floorReloadCallback();
            }
        }

        /*if (EditorGUI.EndChangeCheck())
        {
        }*/

        GUILayout.Space(10);
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