using System;
using UnityEditor;
using UnityEngine;

public class MatrixMakerStructureTypeAdjacency : Editor
{
    private static Vector2 tableScroll;

    private static int Idx(int row, int col, int matrixSize) => row * matrixSize + col;
    private static int previousTypesLength;
    private static bool[] cachedMatrix = null;

    public static void DisposeStructureTypes(StructureType[] cityDataStructureTypes, ref bool[] matrix)
    {
        int structureTypesLength = cityDataStructureTypes.Length;
        EnsureMatrixSize(ref matrix, structureTypesLength);
        //SetMatrixDataOnChange(ref matrix, structureTypesLength);
        EnsureSymmetric(structureTypesLength, matrix);
        
        GUILayout.Label("Structure Types Adjacency Disposition", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Sets adjacency between structure types",
            MessageType.None);
        EditorGUILayout.Space();

        tableScroll = EditorGUILayout.BeginScrollView(tableScroll);
        DrawCompatibilityMatrix(structureTypesLength, cityDataStructureTypes, matrix);
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Clear All", GUILayout.Width(70), GUILayout.Height(16)))
        {
            for (int i = 0; i < matrix.Length; i++)
                matrix[i] = false;
        }

        if (GUILayout.Button("Select All", GUILayout.Width(70), GUILayout.Height(16)))
        {
            for (int i = 0; i < matrix.Length; i++)
                matrix[i] = true;
        }

        if (GUILayout.Button("Diagonal Only (Same Type)", GUILayout.Width(170), GUILayout.Height(16)))
        {
            for (int i = 0; i < matrix.Length; i++)
                matrix[i] = false;
            for (int i = 0; i < structureTypesLength; i++)
                matrix[Idx(i, i, structureTypesLength)] = true;
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(4);
    }

    private static void EnsureMatrixSize(ref bool[] matrix, int n)
    {
        int needed = n * n;
        if (matrix != null && matrix.Length == needed) return;

        bool[] next = new bool[needed];

        if (matrix != null && matrix.Length > 0)
        {
            int oldN = Mathf.RoundToInt(Mathf.Sqrt(matrix.Length));
            if (oldN * oldN == matrix.Length)          // bozuk veri değilse
            {
                int min = Mathf.Min(oldN, n);
                for (int r = 0; r < min; r++)
                    Array.Copy(matrix, r * oldN, next, r * n, min);
            }
        }

        matrix = next;
    }
    
    private static void SetMatrixDataOnChange(ref bool[] matrix, int structureTypesLength, bool value = true)
    {
        if (matrix == null || previousTypesLength != structureTypesLength)
        {
            if (matrix == null)
                Debug.LogWarning("Matrix data array is null");
            
            if(previousTypesLength != 0)
                matrix = RestoreMatrixFromCache(structureTypesLength);
            
            previousTypesLength = structureTypesLength;
        }
        
        CacheMatrix(matrix);
    }

    private static bool[] RestoreMatrixFromCache(int newCount)
    {
        bool[] restored = new bool[newCount * newCount];
        int oldCount = previousTypesLength;

        int min = Mathf.Min(oldCount, newCount);
        for (int r = 0; r < min; r++)
        for (int c = 0; c < min; c++)
            restored[r * newCount + c] = cachedMatrix[r * oldCount + c];

        return restored;
    }

    private static void CacheMatrix(bool[] matrix)
    {
        cachedMatrix = new bool[matrix.Length];

        for (var i = 0; i < matrix.Length; i++)
        {
            cachedMatrix[i] = matrix[i];
        }
    }
    

    private static void EnsureSymmetric(int count, bool[] matrix)
    {
        if (matrix == null || matrix.Length < count * count) return;

        for (int row = 0; row < count; row++)
        for (int col = row + 1; col < count; col++)
        {
            bool value = matrix[Idx(row, col, count)] || matrix[Idx(col, row, count)];
            matrix[Idx(row, col, count)] = value;
            matrix[Idx(col, row, count)] = value;
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

    private static void DrawCompatibilityMatrix(int count, StructureType[] structureTypes, bool[] matrix)
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
                bool current = matrix[Idx(row, col, count)];
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
                    matrix[Idx(row, col, count)] = newVal;
                    matrix[Idx(col, row, count)] = newVal;
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