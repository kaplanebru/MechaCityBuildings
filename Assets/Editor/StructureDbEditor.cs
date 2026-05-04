using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StructureDatabase))]
public class StructureDbEditor : Editor
{
    private StructureDatabase t;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();
        if (GUILayout.Button("Apply Changes"))
        {
            RestoreCache();
            RegenerateEnum();
        }
    }

    private void RestoreStructureDatasByEnums()
    {
        var values = Enum.GetValues(typeof(StructureType));
        for (var i = 0; i < t.datas.Count; i++)
        {
            var data = t.datas[i];
            data.Type = (StructureType)values.GetValue(i);
        }
    }
    
    private void RegenerateEnum()
    {
        var enumValues = t.datas
            .Where(d => d != null)
            .Select(d => d.Name) // ya da d.type, d.structureType vs.
            .ToList();
        enumValues.Add("Undefined");

        GenerateEnumFile("StructureType", enumValues, "Assets/Scripts/GeneratedEnums/StructureType.cs");
        AssetDatabase.Refresh();
        EditorApplication.delayCall += RestoreStructureDatasByEnums;
        Debug.Log($"StructureType enum regenerated: {string.Join(", ", enumValues)}");
    }

    private void GenerateEnumFile(string enumName, List<string> values, string path)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("// Bu dosya otomatik üretilmiştir, düzenleme!");
        sb.AppendLine($"public enum {enumName}");
        sb.AppendLine("{");
    
        foreach (var value in values)
        {
            // boşluk ve özel karakter temizle
            var safeName = System.Text.RegularExpressions.Regex.Replace(value, @"[^a-zA-Z0-9_]", "_");
            sb.AppendLine($"    {safeName},");
        }
    
        sb.AppendLine("}");

        // Klasör yoksa oluştur
        var directory = System.IO.Path.GetDirectoryName(path);
        if (!System.IO.Directory.Exists(directory))
            System.IO.Directory.CreateDirectory(directory);

        System.IO.File.WriteAllText(path, sb.ToString());
        AssetDatabase.Refresh(); // Unity'ye "yeniden derle" dedirt
    }

    private void RestoreCache()
    {
        if (t == null)
            t = target as StructureDatabase;
    }
}