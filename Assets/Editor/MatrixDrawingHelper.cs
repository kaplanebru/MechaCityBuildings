using UnityEditor;
using UnityEngine;

public class MatrixDrawingHelper
{
    private static int Idx(int row, int col, int matrixSize) => row * matrixSize + col;

     private static string StackVertical(string structureName)
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < structureName.Length; i++)
        {
            sb.Append(structureName[i]);
            sb.Append('\n');
        }
        return sb.ToString();
    }

    public static void DrawCompatibilityMatrix(int count, StructureType[] structureTypes, bool[] matrix)
    {
        float labelW = 60f;
        float cellW = 34f;
        float cellH = 28f;

        const int maxHeaderChars = 14;

        GUIStyle headerStyle = new GUIStyle(EditorStyles.miniLabel)
        {
            alignment = TextAnchor.LowerCenter,
            wordWrap = false,
            padding = new RectOffset(0, 0, 0, 0)
        };

        float lineH = headerStyle.lineHeight;
        int longest = 1;
        for (int i = 0; i < count; i++)
            longest = Mathf.Max(longest,
                Mathf.Min(structureTypes[i].ToString().Length, maxHeaderChars));

        float headerH = longest * lineH + 10f;

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
            string rowLabel = fullName;
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
