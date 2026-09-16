using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Compilation;
using UnityEngine;

public static class StructureTypeEditorHelper
{
    const string EnumName   = "StructureType";
    const string EnumPath   = "Assets/Scripts/GeneratedEnums/StructureType.cs";
    const string PendingKey = "StructureEnum.Pending";

    const string ToolDataRoot  = "Assets/ToolData";
    const string StructuresPath = ToolDataRoot + "/StructureDatas";

    // Pending kaydı 10 dakikadan eskiyse at — derleme hatası yüzünden asılı kalmış demektir.
    const double PendingTimeoutMinutes = 10;

    [Serializable]
    class Pending
    {
        public string dbGuid;
        public string quadGuid;
        public string safeName;
        public long   utcTicks;
    }

    // ---------------------------------------------------------------- FAZ 1

    public static void AddDataAndRegenerateEnum(
        StructureDatabase database, string rawName, QuadSample quadSample)
    {
        if (database == null)
        {
            Debug.LogError("Database null.");
            return;
        }

        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Play mode'dayken enum üretilemez. Önce play'den çık.");
            return;
        }

        var safeName = Sanitize(rawName);
        if (safeName == null)
        {
            Debug.LogError($"'{rawName}' geçerli bir enum ismine çevrilemedi.");
            return;
        }

        // Mevcut enum girdilerini derlenmiş tipten oku. Sayısal değerleri
        // koruyacağız ki serialize edilmiş referanslar kaymasın.
        var entries = Enum.GetValues(typeof(StructureType))
            .Cast<StructureType>()
            .Select(v => (name: v.ToString(), value: (int)v))
            .ToList();

        if (entries.Any(e => string.Equals(e.name, safeName, StringComparison.Ordinal)))
        {
            Debug.LogError($"'{safeName}' zaten enum'da var.");
            return;
        }

        if (database.datas.Where(d => d != null)
                          .Any(d => string.Equals(Sanitize(d.Name), safeName, StringComparison.Ordinal)))
        {
            Debug.LogError($"'{safeName}' zaten bir StructureData tarafından kullanılıyor.");
            return;
        }

        var nextValue = entries.Count == 0 ? 0 : entries.Max(e => e.value) + 1;
        entries.Add((safeName, nextValue));

        GenerateEnumFile(EnumName, entries, EnumPath);

        var pending = new Pending
        {
            dbGuid   = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(database)),
            quadGuid = quadSample == null
                       ? string.Empty
                       : AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(quadSample)),
            safeName = safeName,
            utcTicks = DateTime.UtcNow.Ticks
        };

        if (string.IsNullOrEmpty(pending.dbGuid))
        {
            Debug.LogError("Database bir asset değil — GUID alınamadı.");
            return;
        }

        SessionState.SetString(PendingKey, JsonUtility.ToJson(pending));

        AssetDatabase.ImportAsset(EnumPath, ImportAssetOptions.ForceUpdate);
        CompilationPipeline.RequestScriptCompilation();
    }

    // ---------------------------------------------------------------- FAZ 2
    // Unity çağırır: derleme + domain reload bittikten sonra.

    [DidReloadScripts]
    static void OnScriptsReloaded()
    {
        var json = SessionState.GetString(PendingKey, "");
        if (string.IsNullOrEmpty(json)) return;

        SessionState.EraseString(PendingKey);
        EditorApplication.delayCall += () => ApplyPending(json);
    }

    static void ApplyPending(string json)
    {
        Pending pending;
        try { pending = JsonUtility.FromJson<Pending>(json); }
        catch (Exception e) { Debug.LogError($"Pending kaydı okunamadı: {e.Message}"); return; }

        if (pending == null || string.IsNullOrEmpty(pending.safeName)) return;

        var age = DateTime.UtcNow - new DateTime(pending.utcTicks, DateTimeKind.Utc);
        if (age.TotalMinutes > PendingTimeoutMinutes)
        {
            Debug.LogWarning($"'{pending.safeName}' için bekleyen kayıt çok eski ({age.TotalMinutes:F0} dk), atlandı.");
            return;
        }

        if (!Enum.TryParse<StructureType>(pending.safeName, out var type))
        {
            Debug.LogError($"'{pending.safeName}' enum'da yok — enum dosyası derlenmemiş olabilir.");
            return;
        }

        var db = AssetDatabase.LoadAssetAtPath<StructureDatabase>(
            AssetDatabase.GUIDToAssetPath(pending.dbGuid));
        if (db == null)
        {
            Debug.LogError($"Database bulunamadı (guid: {pending.dbGuid}). Enum güncellendi ama data oluşturulmadı.");
            return;
        }

        QuadSample quad = null;
        if (!string.IsNullOrEmpty(pending.quadGuid))
        {
            quad = AssetDatabase.LoadAssetAtPath<QuadSample>(
                AssetDatabase.GUIDToAssetPath(pending.quadGuid));
            if (quad == null)
                Debug.LogWarning($"QuadSample bulunamadı (guid: {pending.quadGuid}), boş bırakıldı.");
        }

        EnsureFolder(StructuresPath);

        var data = ScriptableObject.CreateInstance<StructureData>();
        data.Name       = pending.safeName;
        data.Type       = type;
        data.QuadSample = quad;

        var assetPath = AssetDatabase.GenerateUniqueAssetPath(
            $"{StructuresPath}/{pending.safeName}_StructureData.asset");
        AssetDatabase.CreateAsset(data, assetPath);

        db.datas.Add(data);

        EditorUtility.SetDirty(data);
        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();

        Debug.Log($"{EnumName}.{pending.safeName} ve {assetPath} oluşturuldu.", data);
        EditorGUIUtility.PingObject(data);
    }

    // ---------------------------------------------------------------- yardımcılar

    static void GenerateEnumFile(string enumName, List<(string name, int value)> entries, string path)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated> StructureTypeEditorHelper tarafından üretildi. Elle düzenleme.");
        sb.AppendLine($"public enum {enumName}");
        sb.AppendLine("{");
        foreach (var e in entries.OrderBy(e => e.value))
            sb.AppendLine($"    {e.name} = {e.value},");
        sb.AppendLine("}");

        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(path, sb.ToString());
        // Refresh burada değil — çağıran taraf ImportAsset + RequestScriptCompilation yapıyor.
    }

    static string Sanitize(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;

        var s = Regex.Replace(raw.Trim(), @"[^a-zA-Z0-9_]", "_");
        s = Regex.Replace(s, "_{2,}", "_").Trim('_');

        if (s.Length == 0) return null;
        if (char.IsDigit(s[0])) s = "_" + s;
        return s;
    }

    static void EnsureFolder(string folder)
    {
        folder = folder.Replace('\\', '/').TrimEnd('/');
        if (folder == "Assets" || AssetDatabase.IsValidFolder(folder)) return;

        var parent = Path.GetDirectoryName(folder)?.Replace('\\', '/');
        var leaf   = Path.GetFileName(folder);
        if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(leaf)) return;

        EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, leaf);
    }
}