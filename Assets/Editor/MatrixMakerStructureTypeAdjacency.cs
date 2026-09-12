
using UnityEditor;
using UnityEngine;
 
public class MatrixMakerStructureTypeAdjacency : Editor
{
    private static Vector2 tableScroll;
 
    private static int Idx(int row, int col, int adjacencySize) => row * adjacencySize + col;
 
    // --- Tablo çizimi ---
    public static void DisposeStructureTypes(StructureType[] selectedStructureTypes, bool[] adjacency)
    {
        int count = selectedStructureTypes.Length;
 
        // Eski (asimetrik) verileri tek seferde hizala.
        EnsureSymmetric(count, adjacency);
 
        EditorGUILayout.HelpBox("Set possible adjacency between structure types",
            MessageType.None);
        GUILayout.Label("Structure Types Adjacency Disposition", EditorStyles.boldLabel);
        EditorGUILayout.Space();
 
        tableScroll = EditorGUILayout.BeginScrollView(tableScroll);
        DrawCompatibilityMatrix(count, selectedStructureTypes, adjacency);
        EditorGUILayout.EndScrollView();
 
        EditorGUILayout.Space();
 
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear All", GUILayout.Height(16)))
        {
            for (int i = 0; i < adjacency.Length; i++)
                adjacency[i] = false;
        }
 
        if (GUILayout.Button("Select All", GUILayout.Height(16)))
        {
            for (int i = 0; i < adjacency.Length; i++)
                adjacency[i] = true;
        }
 
        if (GUILayout.Button("Diagonal Only (Same Type)", GUILayout.Height(16)))
        {
            for (int i = 0; i < adjacency.Length; i++)
                adjacency[i] = false;
            for (int i = 0; i < count; i++)
                adjacency[Idx(i, i, count)] = true;
        }
 
        EditorGUILayout.EndHorizontal();
    }
 
    /// <summary>
    /// adjacency dizisi hâlâ n*n olarak saklanır ama anlamı simetriktir:
    /// UI her çifti yalnızca bir kez gösterir, her yazma işlemi aynanır.
    /// Bu metot eski/asimetrik verileri OR'layarak tutarlı hâle getirir.
    /// </summary>
    private static void EnsureSymmetric(int count, bool[] adjacency)
    {
        if (adjacency == null || adjacency.Length < count * count) return;
 
        for (int row = 0; row < count; row++)
        for (int col = row + 1; col < count; col++)
        {
            bool value = adjacency[Idx(row, col, count)] || adjacency[Idx(col, row, count)];
            adjacency[Idx(row, col, count)] = value;
            adjacency[Idx(col, row, count)] = value;
        }
    }
 
    /// <summary>
    /// "Wall_Top" -> "W\na\nl\nl\n|\nT\no\np" : harfleri yukarıdan aşağıya dizer.
    /// Yatay karakterler (_ - –) dikeye döndükleri için "|" ile değiştirilir.
    /// </summary>
    private static string StackVertical(string stringName)
    {
        var sb = new System.Text.StringBuilder(stringName.Length * 2);
        
        
        for (int i = 3; i < stringName.Length; i++)
        {
            if (i > 0) sb.Append('\n');
 
            char c = stringName[i];
            //if (c == '_' || c == '-' || c == '–')
                //c = ':'; //|
 
            sb.Append(c);
        }

        sb.Append('\n');
        sb.Append(":");
        sb.Append('\n');

        for (int i = 0; i < 2; i++)
        {
            sb.Append(stringName[i]);

        }
        
        return sb.ToString();
    }
 
    private static void DrawCompatibilityMatrix(int count, StructureType[] selectedStructureTypes, bool[] adjacency)
    {
        float labelW = 110f;
        float cellW  = 34f;
        float cellH  = 28f;
 
        // ── Başlık satırı (ters sırada: son tip solda), harfler dikey ──
        // Sütunlar tersten dizildiği için satır 0 tam dolu başlar,
        // her satır sağdan bir hücre kaybeder; başlıklar yine de hizalı kalır.
        const int maxHeaderChars = 14;
 
        GUIStyle headerStyle = new GUIStyle(EditorStyles.miniLabel)
        {
            alignment = TextAnchor.LowerCenter,
            wordWrap  = false,
            padding   = new RectOffset(0, 0, 0, 2)
        };
 
        float lineH   = headerStyle.lineHeight;
        int   longest = 1;
        for (int i = 0; i < count; i++)
            longest = Mathf.Max(longest,
                Mathf.Min(selectedStructureTypes[i].ToString().Length, maxHeaderChars));
 
        float headerH = longest * lineH + 4f;
 
        EditorGUILayout.BeginHorizontal(GUILayout.Height(headerH));
        GUILayout.Space(labelW + 4);
 
        for (int slot = 0; slot < count; slot++)
        {
            int    col      = count - 1 - slot;
            string colLabel = selectedStructureTypes[col].ToString();
            string clipped  = colLabel.Length > maxHeaderChars
                ? colLabel.Substring(0, maxHeaderChars - 1) + "…"
                : colLabel;
 
            GUILayout.Label(new GUIContent(StackVertical(clipped), colLabel), headerStyle,
                GUILayout.Width(cellW), GUILayout.Height(headerH));
        }
 
        EditorGUILayout.EndHorizontal();
 
        // ── Veri satırları ─────────────────────────────────────────────
        GUIStyle rowLabelStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize  = 11,
            alignment = TextAnchor.MiddleLeft,
            wordWrap  = false
        };
 
        for (int row = 0; row < count; row++)
        {
            string rowLabel = selectedStructureTypes[row].ToString();
            string shortRow = rowLabel.Length > 13 ? rowLabel.Substring(0, 12) + "…" : rowLabel;
 
            Rect rowRect = EditorGUILayout.BeginHorizontal(GUILayout.Height(cellH));
            if (Event.current.type == EventType.Repaint)
            {
                Color bg = row % 2 == 0
                    ? new Color(0.22f, 0.22f, 0.22f, 0.3f)
                    : new Color(0.18f, 0.18f, 0.18f, 0.3f);
                EditorGUI.DrawRect(rowRect, bg);
            }
 
            GUILayout.Label(new GUIContent(shortRow, rowLabel),
                rowLabelStyle, GUILayout.Width(labelW), GUILayout.Height(cellH));
 
            // Soldan sağa: col = count-1 ... row  (yani üst üçgen, ters sırada).
            // Satır 0 tam dolu, her satır bir hücre kısalır.
            // Köşegeni de dışlamak istersen: slot < count - 1 - row yap.
            for (int slot = 0; slot <= count - 1 - row; slot++)
            {
                int  col      = count - 1 - slot;
                bool current  = adjacency[Idx(row, col, count)];
                Rect cellRect = GUILayoutUtility.GetRect(cellW, cellH, GUILayout.Width(cellW));
 
                if (Event.current.type == EventType.Repaint)
                {
                    Color cellBg = current
                        ? new Color(0.2f,  0.55f, 0.25f, 0.45f)
                        : new Color(0.55f, 0.15f, 0.15f, 0.30f);
                    EditorGUI.DrawRect(cellRect, cellBg);
                }
 
                // Hangi çifte tıkladığını hover ile göster.
                GUI.Label(cellRect, new GUIContent(string.Empty,
                    $"{selectedStructureTypes[row]} ↔ {selectedStructureTypes[col]}"));
 
                Rect toggleRect = new Rect(
                    cellRect.x + (cellRect.width  - 16f) / 2f,
                    cellRect.y + (cellRect.height - 16f) / 2f,
                    16f, 16f);
 
                EditorGUI.BeginChangeCheck();
                bool newVal = EditorGUI.Toggle(toggleRect, current);
                if (EditorGUI.EndChangeCheck())
                {
                    // Simetrik yazma: (row,col) ve (col,row) daima eşit kalır.
                    adjacency[Idx(row, col, count)] = newVal;
                    adjacency[Idx(col, row, count)] = newVal;
                }
            }
 
            // Kalan sağ taraf: çizilmez, üçgen şekli belli olsun diye soluk bırakılır.
            for (int slot = count - row; slot < count; slot++)
            {
                Rect skipped = GUILayoutUtility.GetRect(cellW, cellH, GUILayout.Width(cellW));
                if (Event.current.type == EventType.Repaint)
                    EditorGUI.DrawRect(skipped, new Color(0f, 0f, 0f, 0.10f));
            }
 
            EditorGUILayout.EndHorizontal();
 
            Rect sep = GUILayoutUtility.GetRect(1f, 1f);
            EditorGUI.DrawRect(sep, new Color(0.5f, 0.5f, 0.5f, 0.12f));
        }
    }
}
 


