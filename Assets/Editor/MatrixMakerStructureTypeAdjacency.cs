using UnityEditor;
using UnityEngine;

public class MatrixMakerStructureTypeAdjacency : Editor
{
    private static Vector2 tableScroll;

    private static int Idx(int row, int col, int adjacencySize) => row * adjacencySize + col;
    private static int previousTypesLength;
    private static bool[] cachedAdjacency = null;

    public static void DisposeStructureTypes(StructureType[] cityDataStructureTypes, bool[] adjacency)
    {
        int structureTypesLength = cityDataStructureTypes.Length;
        SetAdjacencyDataOnChange(adjacency, structureTypesLength);

        EnsureSymmetric(structureTypesLength, adjacency);


        GUILayout.Label("Structure Types Adjacency Disposition", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Sets adjacency between structure types",
            MessageType.None);
        EditorGUILayout.Space();

        tableScroll = EditorGUILayout.BeginScrollView(tableScroll);
        DrawCompatibilityMatrix(structureTypesLength, cityDataStructureTypes, adjacency);
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Clear All", GUILayout.Width(70), GUILayout.Height(16)))
        {
            for (int i = 0; i < adjacency.Length; i++)
                adjacency[i] = false;
        }

        if (GUILayout.Button("Select All", GUILayout.Width(70), GUILayout.Height(16)))
        {
            for (int i = 0; i < adjacency.Length; i++)
                adjacency[i] = true;
        }

        if (GUILayout.Button("Diagonal Only (Same Type)", GUILayout.Width(170), GUILayout.Height(16)))
        {
            for (int i = 0; i < adjacency.Length; i++)
                adjacency[i] = false;
            for (int i = 0; i < structureTypesLength; i++)
                adjacency[Idx(i, i, structureTypesLength)] = true;
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(4);
    }

    private static void SetAdjacencyDataOnChange(bool[] adjacencyData, int structureTypesLength, bool value = true)
    {
        if (adjacencyData == null ||
            previousTypesLength != structureTypesLength) // or on type change even the numbers are same
        {
            if (adjacencyData == null)
                Debug.LogWarning("Adjacency data array is null");

            if (previousTypesLength == 0)
            {
                MakeTriviaOnEmptyCache(adjacencyData);
            }

            if (previousTypesLength != structureTypesLength)
                Debug.Log("previous type: " + previousTypesLength + " " + "current: " + structureTypesLength +
                          " adjacency array size mismatch");

            System.Array.Resize(ref adjacencyData, structureTypesLength * structureTypesLength);

            int minTypeCount = cachedAdjacency.Length < adjacencyData.Length
                ? cachedAdjacency.Length
                : adjacencyData.Length;
            for (int i = 0; i < minTypeCount; i++)
            {
                adjacencyData[i] = cachedAdjacency[i];
            }

            previousTypesLength = structureTypesLength;
            CacheAdjacency(adjacencyData);
        }
    }

    private static void CacheAdjacency(bool[] adjacency)
    {
        foreach (var item in adjacency)
        {
            Debug.Log("new adj " + item);
        }
        
        foreach (var item in cachedAdjacency)
        {
            Debug.Log("prew adj " + item);
        }

        
        if (adjacency.Length != cachedAdjacency.Length)
            cachedAdjacency = new bool[adjacency.Length];

        for (var i = 0; i < adjacency.Length; i++)
        {
            cachedAdjacency[i] = adjacency[i];
        }
    }

    private static void MakeTriviaOnEmptyCache(bool[] adjacency)
    {
        cachedAdjacency = new bool[adjacency.Length];
        for (var i = 0; i < adjacency.Length; i++)
            cachedAdjacency[i] = adjacency[i]; //true;
    }

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

    private static string GetRowLabel(string structureName)
    {
        // "S1_1x2" -> "1x2 S1"
        int underscoreIdx = structureName.IndexOf('_');
        if (underscoreIdx < 0) return structureName;

        string gridPart = structureName.Substring(underscoreIdx + 1); // 1x2
        string namePart = structureName.Substring(0, underscoreIdx); // S1

        return "(" + gridPart + ")" + "  " + namePart;
    }

    private static string StackVertical(string structureName)
    {
        // "S1_1x2" -> "1\nx\n2\n\nS1"
        int underscoreIdx = structureName.IndexOf('_');
        if (underscoreIdx < 0) return structureName;

        string gridPart = structureName.Substring(underscoreIdx + 1); // 1x2
        string namePart = structureName.Substring(0, underscoreIdx); // S1

        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < gridPart.Length; i++)
        {
            sb.Append(gridPart[i]);
            sb.Append('\n');
        }

        sb.Append('\n'); // boşluk satırı
        sb.Append(namePart);

        return sb.ToString();
    }

    private static void DrawCompatibilityMatrix(int count, StructureType[] structureTypes, bool[] adjacency)
    {
        float labelW = 60f;
        float cellW = 34f;
        float cellH = 28f;

        const int maxHeaderChars = 14;

        GUIStyle headerStyle = new GUIStyle(EditorStyles.miniLabel)
        {
            alignment = TextAnchor.LowerCenter,
            wordWrap = false,
            padding = new RectOffset(0, 0, 0, 2)
        };

        float lineH = headerStyle.lineHeight;
        int longest = 1;
        for (int i = 0; i < count; i++)
            longest = Mathf.Max(longest,
                Mathf.Min(structureTypes[i].ToString().Length, maxHeaderChars));

        float headerH = longest * lineH + 4f;

        EditorGUILayout.BeginHorizontal(GUILayout.Height(headerH));
        GUILayout.Space(labelW + 4);

        for (int slot = 0; slot < count; slot++)
        {
            int col = count - 1 - slot;
            string colLabel = structureTypes[col].ToString();
            string clipped = colLabel.Length > maxHeaderChars
                ? colLabel.Substring(0, maxHeaderChars - 1) + "…"
                : colLabel;

            GUILayout.Label(new GUIContent(StackVertical(clipped), colLabel), headerStyle,
                GUILayout.Width(cellW), GUILayout.Height(headerH));
        }

        EditorGUILayout.EndHorizontal();

        GUIStyle rowLabelStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 11,
            alignment = TextAnchor.MiddleLeft,
            wordWrap = false
        };

        for (int row = 0; row < count; row++)
        {
            string fullName = structureTypes[row].ToString();
            string rowLabel = GetRowLabel(fullName);
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

            for (int slot = 0; slot <= count - 1 - row; slot++)
            {
                int col = count - 1 - slot;
                bool current = adjacency[Idx(row, col, count)];
                Rect cellRect = GUILayoutUtility.GetRect(cellW, cellH, GUILayout.Width(cellW));

                if (Event.current.type == EventType.Repaint)
                {
                    Color cellBg = current
                        ? new Color(0.2f, 0.55f, 0.25f, 0.45f)
                        : new Color(0.55f, 0.15f, 0.15f, 0.30f);
                    EditorGUI.DrawRect(cellRect, cellBg);
                }

                GUI.Label(cellRect, new GUIContent(string.Empty,
                    $"{structureTypes[row]} ↔ {structureTypes[col]}"));

                Rect toggleRect = new Rect(
                    cellRect.x + (cellRect.width - 16f) / 2f,
                    cellRect.y + (cellRect.height - 16f) / 2f,
                    16f, 16f);

                EditorGUI.BeginChangeCheck();
                bool newVal = EditorGUI.Toggle(toggleRect, current);
                if (EditorGUI.EndChangeCheck())
                {
                    adjacency[Idx(row, col, count)] = newVal;
                    adjacency[Idx(col, row, count)] = newVal;
                }
            }

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