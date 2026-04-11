using UnityEditor;
using UnityEngine;

public class MatrixMakerStructureTypeAdjacency : Editor
{
    private static Vector2 tableScroll;

    private static int Idx(int row, int col, int adjacencySize) => row * adjacencySize + col;

    // --- Tablo çizimi ---
    public static void DisposeStructureTypes(StructureType[] selectedStructureTypes, bool[] adjacency)
    {
        int compatibilitySize = selectedStructureTypes.Length;

        EditorGUILayout.HelpBox("Set possible adjacency between structure types",
            MessageType.None);
        GUILayout.Label("Structure Types Adjacency Disposition", EditorStyles.boldLabel);
        EditorGUILayout.Space();


        tableScroll = EditorGUILayout.BeginScrollView(tableScroll);
        DrawCompatibilityMatrix(compatibilitySize, selectedStructureTypes, adjacency);
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

        /*if (GUILayout.Button("Çaprazı İşaretle (Aynı Tip)", GUILayout.Height(24)))
        {
            for (int i = 0; i < compatibilitySize; i++)
                compatibility[Idx(i, i, compatibilitySize)] = true;
        }*/

        EditorGUILayout.EndHorizontal();
    }

    private static void DrawCompatibilityMatrix(int count, StructureType[] selectedStructureTypes, bool[] adjacency)
    {
        int adjacencySize = selectedStructureTypes.Length;
        float labelW = 110f;
        float cellW  = 34f;
        float cellH  = 28f;

        // ── Başlık satırı ──────────────────────────────────────────────
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(labelW + 4);

        for (int col = 0; col < count; col++)
        {
            string colLabel   = selectedStructureTypes[col].ToString();
            string shortLabel = colLabel.Length > 5 ? colLabel.Substring(0, 4) + "…" : colLabel;
            GUILayout.Label(new GUIContent(shortLabel, colLabel),
                new GUIStyle(EditorStyles.centeredGreyMiniLabel) { wordWrap = false },
                GUILayout.Width(cellW));
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

            for (int col = 0; col < count; col++)
            {
                bool current  = adjacency[Idx(row, col, adjacencySize)];
                Rect cellRect = GUILayoutUtility.GetRect(cellW, cellH, GUILayout.Width(cellW));

                if (Event.current.type == EventType.Repaint)
                {
                    Color cellBg = current
                        ? new Color(0.2f,  0.55f, 0.25f, 0.45f)
                        : new Color(0.55f, 0.15f, 0.15f, 0.30f);
                    EditorGUI.DrawRect(cellRect, cellBg);
                }

                Rect toggleRect = new Rect(
                    cellRect.x + (cellRect.width  - 16f) / 2f,
                    cellRect.y + (cellRect.height - 16f) / 2f,
                    16f, 16f);

                EditorGUI.BeginChangeCheck();
                bool newVal = EditorGUI.Toggle(toggleRect, current);
                if (EditorGUI.EndChangeCheck())
                {
                    adjacency[Idx(row, col, adjacencySize)] = newVal;
                    // Simetrik mod: compatibility[Idx(col, row)] = newVal;
                }
            }

            EditorGUILayout.EndHorizontal();

            Rect sep = GUILayoutUtility.GetRect(1f, 1f);
            EditorGUI.DrawRect(sep, new Color(0.5f, 0.5f, 0.5f, 0.12f));
        }
    }
}