using UnityEngine;
using UnityEngine.UI;

public class HexGridChunk : MonoBehaviour
{
    HexCell[] cells;
    Canvas gridCanvas;

    public HexMesh terrain;
    public HexMesh roads;
    public HexFeatureManager features;

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
        features.Clear();
        for(int i = 0; i < cells.Length; ++i)
            Triangulate(cells[i]);
        terrain.Apply();
        roads.Apply();
        features.Apply();
    }

    void Triangulate(HexCell cell)
    {
        for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; ++dir)
            Triangulate(cell, dir);
        if(cell.HasFeature)
            features.AddFeature(cell);
    }

    void Triangulate(HexCell cell, HexDirection dir)
    {
        Vector3 center = cell.transform.localPosition;
        Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(dir);
        Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(dir);

        terrain.AddTriangle(center, v1, v2);
        terrain.AddTriangleColor(cell.Color);

        TriangulateConnection(cell, v1, v2, dir);
    }

    void TriangulateConnection(HexCell cell, Vector3 v1, Vector3 v2, HexDirection dir)
    {
        HexCell neighbor = cell.GetNeighbor(dir);
        if(neighbor == null)
            return;

        Vector3 bridge = HexMetrics.GetBridge(dir);
        Vector3 v3 = v1 + bridge;
        Vector3 v4 = v2 + bridge;
        v3.y = v4.y = neighbor.Elevation * HexMetrics.ElevationStep;

        if(dir <= HexDirection.SE)
        {
            // Edges connections
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
        }

        TriangulateRoadSystem(cell, dir, v1, v2, v3, v4);
    }

    void TriangulateRoadSystem( HexCell cell, HexDirection dir, 
                                Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        if(cell.HasRoads)
        {
            Vector3 center = cell.transform.localPosition;

            Vector3 middleLeft = Vector3.Lerp(center, v1, 0.5f);
            Vector3 middleRight = Vector3.Lerp(center, v2, 0.5f);

            TriangulateRoad(center, middleLeft, middleRight, v1, v2, cell.HasRoadThroughEdge(dir));

            if(cell.HasRoadThroughEdge(dir))
            {
                HexCell neighbor = cell.GetNeighbor(dir);
                Vector3 v5 = Vector3.Lerp(v1, v2, 0.5f);
                Vector3 v6 = Vector3.Lerp(v3, v4, 0.5f);
                v6.y = neighbor.Elevation * HexMetrics.ElevationStep;

                v1 = Vector3.Lerp(v1, v5, 0.5f);
                v2 = Vector3.Lerp(v5, v2, 0.5f);

                v3 = Vector3.Lerp(v3, v6, 0.5f);
                v4 = Vector3.Lerp(v6, v4, 0.5f);

                TriangulateRoadSegment(v1, v5, v2, v3, v6, v4);
            }
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

    void TriangulateRoad(   Vector3 center, Vector3 middleLeft, Vector3 middleRight,
                            Vector3 upLeft, Vector3 upRight, bool hasRoadThroughCellEdge)
    {
        if(hasRoadThroughCellEdge)
        {
            Vector3 middleCenter = Vector3.Lerp(middleLeft, middleRight, 0.5f);
            Vector3 cornerMiddleLeft = Vector3.Lerp(upLeft, upRight, 0.25f);
            Vector3 cornerMiddle = Vector3.Lerp(upLeft, upRight, 0.5f);
            Vector3 cornerMiddleRight = Vector3.Lerp(upLeft, upRight, 0.75f);

            TriangulateRoadSegment( middleLeft, middleCenter, middleRight, 
                                    cornerMiddleLeft, cornerMiddle, cornerMiddleRight);

            roads.AddTriangle(center, middleLeft, middleCenter);
            roads.AddTriangle(center, middleCenter, middleRight);
            roads.AddTriangleUV(new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(1f, 0f));
            roads.AddTriangleUV(new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f));    
        }
        // Roads edges
        else
        {
            roads.AddTriangle(center, middleLeft, middleRight);
            roads.AddTriangleUV(new Vector3(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f));
        }
    }
}
