using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    Mesh hexMesh;
    // Temporary buffers only used during triangulation
    [NonSerialized] List<Vector3> vertices;
    [NonSerialized] List<int> triangles;
    [NonSerialized] List<Color> cellWeights;
    [NonSerialized] List<Vector2> uvs;
    [NonSerialized] List<Vector3> cellIndices;

    MeshCollider meshCollider;

    public bool useCollider;
    public bool useUVCoordinates;
    public bool useCellData;

    void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        if(useCollider)
            meshCollider = gameObject.AddComponent<MeshCollider>();
        hexMesh.name = "Hex Mesh";
    }

    public void Clear()
    {
        hexMesh.Clear();
        vertices = ListPool<Vector3>.Get();
        if(useCellData)
        {
            cellWeights = ListPool<Color>.Get();
            cellIndices = ListPool<Vector3>.Get();
        }
        if(useUVCoordinates)
            uvs = ListPool<Vector2>.Get();
        triangles = ListPool<int>.Get();
    }

    public void Apply()
    {
        hexMesh.SetVertices(vertices);
        ListPool<Vector3>.Add(vertices);
        if(useCellData)
        {
            hexMesh.SetColors(cellWeights);
            ListPool<Color>.Add(cellWeights);
            hexMesh.SetUVs(2, cellIndices);
            ListPool<Vector3>.Add(cellIndices);
        }
        if(useUVCoordinates)
        {
            hexMesh.SetUVs(0, uvs);
            ListPool<Vector2>.Add(uvs);
        }
        hexMesh.SetTriangles(triangles, 0);
        ListPool<int>.Add(triangles);
        hexMesh.RecalculateNormals();
        if(useCollider)
            meshCollider.sharedMesh = hexMesh;
    }

    public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    public void AddTriangleCellData(Vector3 indices, Color w1, Color w2, Color w3)
    {
        cellIndices.Add(indices);
        cellIndices.Add(indices);
        cellIndices.Add(indices);
        cellWeights.Add(w1);
        cellWeights.Add(w2);
        cellWeights.Add(w3);
    }

    public void AddTriangleCellData(Vector3 indices, Color weights)
    {
        AddTriangleCellData(indices, weights, weights, weights);
    }

    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }

    public void AddQuadCellData(Vector3 indices, Color w1, Color w2, Color w3, Color w4)
    {
        cellIndices.Add(indices);
        cellIndices.Add(indices);
        cellIndices.Add(indices);
        cellIndices.Add(indices);
        cellWeights.Add(w1);
        cellWeights.Add(w2);
        cellWeights.Add(w3);
        cellWeights.Add(w4);
    }

    public void AddQuadCellData(Vector3 indices, Color w1, Color w2)
    {
        AddQuadCellData(indices, w1, w1, w2, w2);
    }

    public void AddQuadCellData(Vector3 indices, Color weights)
    {
        AddQuadCellData(indices, weights, weights, weights, weights);
    }

    public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        uvs.Add(uv1);
        uvs.Add(uv2);
        uvs.Add(uv3);
    }

    public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4)
    {
        uvs.Add(uv1);
        uvs.Add(uv2);
        uvs.Add(uv3);
        uvs.Add(uv4);
    }

    public void AddQuadUV(float uMin, float uMax, float vMin, float vMax)
    {
        uvs.Add(new Vector2(uMin, vMin));
        uvs.Add(new Vector2(uMax, vMin));
        uvs.Add(new Vector2(uMin, vMax));
        uvs.Add(new Vector2(uMax, vMax));
    }

    public void AddQuadTerrainBiomes(Vector3 biomes)
    {
        cellIndices.Add(biomes);
        cellIndices.Add(biomes);
        cellIndices.Add(biomes);
        cellIndices.Add(biomes);
    }
}