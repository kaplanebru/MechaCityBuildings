using System;
using System.Collections.Generic;
using UnityEngine;

//TODO: Aslında grid size ve plane aynı olmalı. ya da grid 1 birimi değişir. ama plane ile eşleşse iyi olur

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public sealed class OverlayPainter : MonoBehaviour
{
    [Header("Visual")]
    [Tooltip("Small vertical offset above the ground to avoid z-fighting.")]
    [SerializeField] private float overlayHeightOffset = 0.01f;

    [Tooltip("Alpha for painted cells. 0 = invisible, 1 = fully visible.")]
    [Range(0f, 1f)]
    [SerializeField] private float paintedAlpha = 0.65f;

    [Tooltip("Alpha for unpainted cells. Set to 0 for invisible.")]
    [Range(0f, 1f)]
    [SerializeField] private float unpaintedAlpha = 0f;

    [Header("Optional")]
    [Tooltip("If true, rebuilds the whole mesh when you call RebuildAll().")]
    [SerializeField] private bool allowFullRebuild = true;

    // Mesh + arrays.
    private Mesh overlayMesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Color32[] colors;
    private float _height;

    // Grid dimensions.
    private int gridWidthInCells;
    private int gridHeightInCells;

    private bool hasAnyColorChanges;

    public bool isInitialized;
    
    public void SetPainterHeight(FloorData floorData)
    {
        _height = overlayHeightOffset + floorData.FloorGroundHeight;
        transform.position = new Vector3(transform.position.x, floorData.FloorGroundHeight, transform.position.z);
    }
    
    public void CreateOverlayMeshIfNeeded(GridData gridData)
    {
        if (isInitialized)
            return;
        
        gridWidthInCells = gridData.AdaptiveGridSize.x;
        gridHeightInCells = gridData.AdaptiveGridSize.y;

        if (gridWidthInCells <= 0 || gridHeightInCells <= 0)
            throw new InvalidOperationException("OverlayGridPainter: gridData.GridSize must be positive.");

        // Place overlay at grid origin in WORLD space.
        // Vertices are LOCAL.
        transform.position = gridData.OriginWorldTransform.position;
        transform.rotation = gridData.OriginWorldTransform.rotation;
        var scale = gridData.OriginWorldTransform.localScale;
        transform.localScale = new Vector3(scale.x, scale.y, scale.x);//Vector3.one;
        //unit size ile orantılı gitmeli sanırım

        int cellCount = gridWidthInCells * gridHeightInCells;

        vertices = new Vector3[cellCount * 4];
        triangles = new int[cellCount * 6];
        colors = new Color32[cellCount * 4];

        BuildLocalGeometry(vertices, triangles, gridData);
        FillAllCellAlphas(unpaintedAlpha);

        overlayMesh = new Mesh();
        overlayMesh.name = "OverlayGridMesh";
        overlayMesh.indexFormat = (vertices.Length > 65000)
            ? UnityEngine.Rendering.IndexFormat.UInt32
            : UnityEngine.Rendering.IndexFormat.UInt16;

        overlayMesh.vertices = vertices;
        overlayMesh.triangles = triangles;
        overlayMesh.colors32 = colors;
        overlayMesh.RecalculateBounds();

        GetComponent<MeshFilter>().sharedMesh = overlayMesh;

        isInitialized = true;
    }

 
    
    public void SetCellPainted(int xIndex, int yIndex, bool painted, GridData gridData)
    {
        CreateOverlayMeshIfNeeded(gridData);

        if (!IsInsideGrid(xIndex, yIndex))
            return;
        float targetAlpha = painted ? paintedAlpha : unpaintedAlpha;
        SetCellVertexAlphaImmediate(xIndex, yIndex, targetAlpha);

        hasAnyColorChanges = true;
    }

  

    private void LateUpdate()
    {
        if (!isInitialized)
            return;

        if (!hasAnyColorChanges)
            return;

        PushColorsToMesh();
        hasAnyColorChanges = false;
    }

    // ========================================================================
    // Geometry
    // ========================================================================

    private void BuildLocalGeometry(Vector3[] verticesArray, int[] trianglesArray, GridData gridData)
    {
        float cellSize = gridData.CellSize;

        // LOCAL offset above ground.
        float y = _height;//overlayHeightOffset;

        int vertexBaseIndex = 0;
        int triangleBaseIndex = 0;

        for (int yIndex = 0; yIndex < gridHeightInCells; yIndex++)
        {
            for (int xIndex = 0; xIndex < gridWidthInCells; xIndex++)
            {
                // LOCAL coordinates (origin is this GameObject).
                float x0 = xIndex * cellSize;
                float z0 = yIndex * cellSize;

                float x1 = x0 + cellSize;
                float z1 = z0 + cellSize;

                // Quad vertices (clockwise):
                verticesArray[vertexBaseIndex + 0] = new Vector3(x0, y, z0);
                verticesArray[vertexBaseIndex + 1] = new Vector3(x0, y, z1);
                verticesArray[vertexBaseIndex + 2] = new Vector3(x1, y, z1);
                verticesArray[vertexBaseIndex + 3] = new Vector3(x1, y, z0);

                // Two triangles: (0,1,2) and (0,2,3)
                trianglesArray[triangleBaseIndex + 0] = vertexBaseIndex + 0;
                trianglesArray[triangleBaseIndex + 1] = vertexBaseIndex + 1;
                trianglesArray[triangleBaseIndex + 2] = vertexBaseIndex + 2;

                trianglesArray[triangleBaseIndex + 3] = vertexBaseIndex + 0;
                trianglesArray[triangleBaseIndex + 4] = vertexBaseIndex + 2;
                trianglesArray[triangleBaseIndex + 5] = vertexBaseIndex + 3;

                vertexBaseIndex += 4;
                triangleBaseIndex += 6;
            }
        }
    }

    // ========================================================================
    // Colors
    // ========================================================================

    private void FillAllCellAlphas(float alpha)
    {
        byte alphaByte = FloatAlphaToByte(alpha);

        // White with variable alpha. Material can tint.
        Color32 baseColor = new Color32(255, 255, 255, alphaByte);

        for (int index = 0; index < colors.Length; index++)
        {
            colors[index] = baseColor;
        }
    }

    private void SetCellVertexAlphaImmediate(int xIndex, int yIndex, float alpha)
    {
        int cellLinearIndex = ToCellLinearIndex(xIndex, yIndex);
        int vertexBaseIndex = cellLinearIndex * 4;

        byte alphaByte = FloatAlphaToByte(alpha);

        colors[vertexBaseIndex + 0].a = alphaByte;
        colors[vertexBaseIndex + 1].a = alphaByte;
        colors[vertexBaseIndex + 2].a = alphaByte;
        colors[vertexBaseIndex + 3].a = alphaByte;
    }

    private void PushColorsToMesh()
    {
        // Full upload of vertex colors.
        overlayMesh.colors32 = colors;
    }

    // ========================================================================
    // Helpers
    // ========================================================================

    private bool IsInsideGrid(int xIndex, int yIndex)
    {
        return xIndex >= 0 && xIndex < gridWidthInCells && yIndex >= 0 && yIndex < gridHeightInCells;
    }

    private int ToCellLinearIndex(int xIndex, int yIndex)
    {
        return yIndex * gridWidthInCells + xIndex;
    }

    private byte FloatAlphaToByte(float alpha)
    {
        alpha = Mathf.Clamp01(alpha);
        return (byte)Mathf.RoundToInt(alpha * 255f);
    }
    
    /// <summary>
    /// Call after changing GridData (GridSize, CellSize, OriginWorld).
    /// This rebuilds geometry and resets colors.
    /// </summary>
    public void RebuildAll(GridData gridData)
    {
        if (!allowFullRebuild)
            return;

        isInitialized = false;

        // Reset mesh reference.
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
            meshFilter.sharedMesh = null;

        overlayMesh = null;
        vertices = null;
        triangles = null;
        colors = null;

        CreateOverlayMeshIfNeeded(gridData);
    }

    /// <summary>
    /// Apply a whole selection mask to the overlay.
    /// Useful after loading or after a big operation.
    /// </summary>
    public void ApplySelectionMask(bool[,] selectedCells,GridData gridData)
    {
        CreateOverlayMeshIfNeeded(gridData);

        if (selectedCells == null)
            throw new ArgumentNullException(nameof(selectedCells));

        int width = selectedCells.GetLength(0);
        int height = selectedCells.GetLength(1);

        if (width != gridWidthInCells || height != gridHeightInCells)
            throw new InvalidOperationException("ApplySelectionMask: selectedCells dimensions do not match gridData.GridSize.");

        for (int yIndex = 0; yIndex < gridHeightInCells; yIndex++)
        {
            for (int xIndex = 0; xIndex < gridWidthInCells; xIndex++)
            {
                float targetAlpha = selectedCells[xIndex, yIndex] ? paintedAlpha : unpaintedAlpha;
                SetCellVertexAlphaImmediate(xIndex, yIndex, targetAlpha);
            }
        }

        PushColorsToMesh();
    }
    
    public void SetCellsPainted(List<Vector2Int> touchedCells, bool painted, GridData gridData)
    {
        CreateOverlayMeshIfNeeded(gridData);

        if (touchedCells == null || touchedCells.Count == 0)
            return;

        float targetAlpha = painted ? paintedAlpha : unpaintedAlpha;

        for (int index = 0; index < touchedCells.Count; index++)
        {
            Vector2Int cell = touchedCells[index];

            if (!IsInsideGrid(cell.x, cell.y))
                continue;

            SetCellVertexAlphaImmediate(cell.x, cell.y, targetAlpha);
        }

        hasAnyColorChanges = true;
    }
}

