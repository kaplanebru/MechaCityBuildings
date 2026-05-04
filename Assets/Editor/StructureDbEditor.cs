using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StructureDatabase))]
public class StructureDbEditor : Editor
{
    private StructureDatabase t;
    private int cachedCount;
    private HashSet<StructureData> cachedData = new ();

    private void OnEnable()
    {
        RestoreCache();
        cachedCount = t.datas?.Count ?? 0;

        Undo.postprocessModifications += OnPostprocessModifications;
    }
    
    private UndoPropertyModification[] OnPostprocessModifications(UndoPropertyModification[] modifications)
    {
        RestoreCache();
        if (t.datas == null) return modifications;

        int currentCount = t.datas.Count;
        if (currentCount != cachedCount)
        {
            RegenerateEnum();
            cachedCount = currentCount;
        }
        /*if (currentCount > cachedCount)
        {
            foreach (var data in t.datas)
            {
                if (!cachedData.Contains(data))
                {
                    cachedData.Add(data);
                    //var newElement = t.datas[currentCount - 1];
                    OnElementAdded(data);
                    cachedCount = currentCount;
                    break;
                }
            }
        }
        else if(currentCount < cachedCount)
        {
            foreach (var data in cachedData)
            {
                if (!t.datas.Contains(data))
                {
                    cachedData.Remove(data);
                    OnElementRemoved(data);
                    cachedCount = currentCount;
                    break;
                }
            }
        }*/

        return modifications;
    }
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();
        if (GUILayout.Button("Apply Changes"))
        {
            RegenerateEnum();
            cachedCount = t.datas?.Count ?? 0;
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
    
    private void OnDisable()
    {
        Undo.postprocessModifications -= OnPostprocessModifications;
    }
    
}