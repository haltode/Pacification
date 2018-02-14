using UnityEngine;
using UnityEngine.UI;

public class HexGridChunk : MonoBehaviour
{
    HexCell[] cells;
    Canvas gridCanvas;

    public HexMesh terrain;    
    public HexMesh roads;

    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();

        cells = new HexCell[HexMetrics.ChunkSizeX * HexMetrics.ChunkSizeZ];
        ShowUI(false);
    }

    public void AddCell(int index, HexCell cell)
    {
        cells[index] = cell;
        cell.chunk = this;
        cell.transform.SetParent(transform, false);
        cell.uiRect.SetParent(gridCanvas.transform, false);
    }

    public void Refresh()
    {
        enabled = true;
    }

    public void ShowUI(bool visible)
    {
        gridCanvas.gameObject.SetActive(visible);
    }

    void LateUpdate()
    {
        Triangulate();
        enabled = false;
    }

    public void Triangulate()
    {
        terrain.Clear();
        roads.Clear();
        for(int i = 0; i < cells.Length; ++i)
            Triangulate(cells[i]);
        terrain.Apply();
        roads.Apply();
    }

    void Triangulate(HexCell cell)
    {
        for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; ++dir)
            Triangulate(cell, dir);
    }

    void Triangulate(HexCell cell, HexDirection dir)
    {
        Vector3 center = cell.transform.localPosition;
        Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(dir);
        Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(dir);

        terrain.AddTriangle(center, v1, v2);
        terrain.AddTriangleColor(cell.Color);

        if(dir <= HexDirection.SE)
            TriangulateConnection(cell, v1, v2, dir);
    }

    void TriangulateConnection(HexCell cell, Vector3 v1, Vector3 v2, HexDirection dir)
    {
        HexCell neighbor = cell.GetNeighbor(dir);
        if(neighbor == null)
            return;

        // Edges connections
        Vector3 bridge = HexMetrics.GetBridge(dir);
        Vector3 v3 = v1 + bridge;
        Vector3 v4 = v2 + bridge;
        v3.y = v4.y = neighbor.Elevation * HexMetrics.ElevationStep;

        terrain.AddQuad(v1, v2, v3, v4);
        terrain.AddQuadColor(cell.Color, neighbor.Color);

        // Corners connections
        HexCell nextNeighbor = cell.GetNeighbor(dir.Next());
        if(dir <= HexDirection.E && nextNeighbor != null)
        {
            Vector3 v5 = v2 + HexMetrics.GetBridge(dir.Next());
            v5.y = nextNeighbor.Elevation * HexMetrics.ElevationStep;
            
            terrain.AddTriangle(v2, v4, v5);
            terrain.AddTriangleColor(cell.Color, neighbor.Color, nextNeighbor.Color);
        }

        // Road
        if(cell.HasRoadThroughEdge(dir))
        {
            Vector3 v5 = Vector3.Lerp(v1, v2, 0.5f);
            Vector3 v6 = Vector3.Lerp(v3, v4, 0.5f);
            v6.y = neighbor.Elevation * HexMetrics.ElevationStep;

            v1 = Vector3.Lerp(v1, v5, 0.5f);
            v2 = Vector3.Lerp(v5, v2, 0.5f);

            v3 = Vector3.Lerp(v3, v6, 0.5f);
            v4 = Vector3.Lerp(v6, v4, 0.5f);

            // Debug.Log("v1 " + v1);
            // Debug.Log("v2 " + v2);
            // Debug.Log("v3 " + v3);
            // Debug.Log("v4 " + v4);
            // Debug.Log("v5 " + v5);
            // Debug.Log("v6 " + v6);
            //TriangulateRoadSegment(v1, v3, v5, v2, v4, v6);
            //TriangulateRoadSegment(v1, v5, v3, v2, v6, v4); // works but not the correct way
            //TriangulateRoadSegment(v3, v5, v1, v4, v6, v2);
            TriangulateRoadSegment(v1, v5, v2, v3, v6, v4);
        }
    }

    void TriangulateRoadSegment(Vector3 v1, Vector3 v2, Vector3 v3,
                                Vector3 v4, Vector3 v5, Vector3 v6)
    {
        roads.AddQuad(v1, v2, v4, v5);
        roads.AddQuad(v2, v3, v5, v6);
        roads.AddQuadUV(0f, 1f, 0f, 0f);
        roads.AddQuadUV(1f, 0f, 0f, 0f);
    }
}
