using System;
using UnityEditor;
using UnityEngine;

public class MatrixMakerStructureTypeAdjacency : Editor
{
    private static Vector2 tableScroll;

    private static int Idx(int row, int col, int matrixSize) => row * matrixSize + col;

    public static void DisposeStructureTypes(StructureType[] cityDataStructureTypes, ref bool[] matrix)
    {
        int structureTypesLength = cityDataStructureTypes.Length;
        EnsureMatrixSize(ref matrix, structureTypesLength);
        EnsureSymmetric(structureTypesLength, matrix);
        
        GUILayout.Label("Structure Types Adjacency Disposition", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Sets adjacency between structure types",
            MessageType.None);
        EditorGUILayout.Space();

        tableScroll = EditorGUILayout.BeginScrollView(tableScroll);
        MatrixDrawingHelper.DrawCompatibilityMatrix(structureTypesLength, cityDataStructureTypes, matrix);
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

    private static void EnsureMatrixSize(ref bool[] matrix, int length)
    {
        int needed = length * length;
        if (matrix != null && matrix.Length == needed) return;

        bool[] nextMatrix = new bool[needed];

        if (matrix != null && matrix.Length > 0)
        {
            int oldLength = Mathf.RoundToInt(Mathf.Sqrt(matrix.Length));
            if (oldLength * oldLength == matrix.Length)          // bozuk veri değilse
            {
                int min = Mathf.Min(oldLength, length);
                for (int r = 0; r < min; r++)
                    Array.Copy(matrix, r * oldLength, nextMatrix, r * length, min);
            }
        }

        matrix = nextMatrix;
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

   
}